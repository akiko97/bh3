using System;
using System.Collections.Generic;
using FlatBuffers;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public abstract class BaseMPManager
	{
		private const int BUFFER_SIZE = 1024;

		private const int TOTAL_HEADER_BYTE = 6;

		protected MPPeer _peer;

		protected bool _isMaster;

		private int _peerID;

		private byte[] _sendBuffer;

		private byte[] _recvBuffer;

		private byte[] _tmpBuffer;

		private FlatBufferBuilder _defaultSendBuilder;

		private FlatBufferBuilder _instantiateBuilder;

		private ByteBuffer _recvByteBuffer;

		private SprotoPack _bitPacker;

		private Dictionary<uint, BaseMPIdentity> _identities;

		private List<BaseMPIdentity> _identitiesList;

		private BaseMPIdentity[] _identitiesBuffer = new BaseMPIdentity[64];

		private Dictionary<uint, int> _pendingInstantiates;

		private bool[] _channelOccupiedMap;

		private Dictionary<uint, int> _runtimeIDChannelMap;

		public int peerID
		{
			get
			{
				return _peerID;
			}
		}

		public bool isMaster
		{
			get
			{
				return _isMaster;
			}
		}

		public BaseMPManager()
		{
			_bitPacker = new SprotoPack();
			_sendBuffer = new byte[1024];
			_recvBuffer = new byte[1024];
			_tmpBuffer = new byte[1024];
			_recvByteBuffer = new ByteBuffer(_recvBuffer);
			_defaultSendBuilder = new FlatBufferBuilder(1024);
			_instantiateBuilder = new FlatBufferBuilder(32);
			_identities = new Dictionary<uint, BaseMPIdentity>();
			_identitiesList = new List<BaseMPIdentity>();
			_pendingInstantiates = new Dictionary<uint, int>();
			_peer = IdleMPPeer.IDLE_PEER;
			MPMappings.InitMPMappings();
		}

		public void SetupPeer(MPPeer peer, bool isMasterClient)
		{
			_peer = peer;
			_isMaster = isMasterClient;
			_peerID = peer.peerID;
			_peer.onPacket = OnPeerPacketCallback;
			InitChannelManagement();
		}

		public MPSendPacketContainer CreateSendPacket<T>() where T : Table
		{
			return CreateSendPacket<T>(_defaultSendBuilder);
		}

		public MPSendPacketContainer CreateSendPacket<T>(FlatBufferBuilder builder) where T : Table
		{
			return CreateSendPacket(typeof(T), builder);
		}

		public MPSendPacketContainer CreateSendPacket(Type type, FlatBufferBuilder builder)
		{
			return new MPSendPacketContainer
			{
				packetTypeID = MPMappings.MPPacketMapping.Get(type),
				builder = builder
			};
		}

		public void SendReliableToOthers(uint runtimeID, MPSendPacketContainer pc)
		{
			pc.runtimeID = runtimeID;
			SendByChannel(7, _peer.reliableChannel, pc);
		}

		public void SendReliableToPeer(uint runtimeID, int peerID, MPSendPacketContainer pc)
		{
			pc.runtimeID = runtimeID;
			SendByChannel(peerID, _peer.reliableChannel, pc);
		}

		public void SendStateUpdateToOthers(uint runtimeID, MPSendPacketContainer pc)
		{
			pc.runtimeID = runtimeID;
			SendByChannel(7, _peer.stateUpdateChannel, pc);
		}

		private void SendByChannel(int peerID, int channel, MPSendPacketContainer pc)
		{
			ByteBuffer dataBuffer = pc.builder.DataBuffer;
			int num = dataBuffer.Position - 6;
			dataBuffer.PutByte(num, (byte)pc.packetTypeID);
			dataBuffer.PutByte(num + 1, PackControlByte(_peerID));
			dataBuffer.PutUint(num + 2, pc.runtimeID);
			dataBuffer.Position = num;
			int num2 = dataBuffer.Length - dataBuffer.Position;
			Buffer.BlockCopy(dataBuffer.Data, dataBuffer.Position, _tmpBuffer, 0, num2);
			int num3 = _bitPacker.pack(_tmpBuffer, _bitPacker.RoundUpTo8(num2), _sendBuffer, 1);
			num2 = num3;
			pc.builder.Clear();
			_sendBuffer[0] = _peer.PackHeader((byte)peerID, 31);
			int channelSequence = _runtimeIDChannelMap[pc.runtimeID];
			_peer.SendByChannel(_sendBuffer, num2 + 1, channel, channelSequence);
			pc.state = MPSendContainerState.Sent;
		}

		private void OnPeerPacketCallback(byte[] buffer, int len, int offset, int channel)
		{
			Buffer.BlockCopy(buffer, offset, _tmpBuffer, 0, len);
			_bitPacker.unpack(_tmpBuffer, len, _recvBuffer, 0);
			_recvByteBuffer.Reset();
			int num = _recvByteBuffer.Get(0);
			int controlByte = _recvByteBuffer.Get(1);
			int fromPeerID = ParseControlByteSenderPeerID(controlByte);
			uint runtimeID = _recvByteBuffer.GetUint(2);
			Type type = MPMappings.MPPacketMapping.Get(num);
			Table table = MPMappings.cachedRecvPackets[num];
			_recvByteBuffer.Position = 6;
			table.ResetAndInitTo(_recvByteBuffer);
			MPRecvPacketContainer pc = new MPRecvPacketContainer
			{
				runtimeID = runtimeID,
				channel = channel,
				fromPeerID = fromPeerID,
				packet = table
			};
			DispatchPacket(pc);
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		protected virtual void DispatchPacket(MPRecvPacketContainer pc)
		{
			BaseMPIdentity value;
			if (pc.packet is Packet_Basic_Instantiate)
			{
				Packet_Basic_Instantiate packet_Basic_Instantiate = pc.As<Packet_Basic_Instantiate>();
				BindImportantFixedChannel(pc.runtimeID, packet_Basic_Instantiate.ChannelSequence);
				_pendingInstantiates.Add(pc.runtimeID, packet_Basic_Instantiate.PeerType);
			}
			else if (pc.packet is Packet_Basic_Destroy)
			{
				RemoveMPIdentity(pc.runtimeID);
			}
			else if (_pendingInstantiates.ContainsKey(pc.runtimeID))
			{
				Type type = MPMappings.MPPeerMapping.Get(_pendingInstantiates[pc.runtimeID]);
				CreateRemoteMPIdentity(type, pc.runtimeID, pc);
				_pendingInstantiates.Remove(pc.runtimeID);
			}
			else if (pc.channel == _peer.reliableChannel)
			{
				_identities[pc.runtimeID].OnReliablePacket(pc);
			}
			else if (pc.channel == _peer.stateUpdateChannel && _identities.TryGetValue(pc.runtimeID, out value))
			{
				value.OnStateUpdatePacket(pc);
			}
		}

		public virtual void Core()
		{
			_peer.Core();
			if (_identitiesList.Count > _identitiesBuffer.Length)
			{
				_identitiesBuffer = new BaseMPIdentity[_identitiesBuffer.Length * 2];
			}
			int count = _identitiesList.Count;
			_identitiesList.CopyTo(_identitiesBuffer);
			for (int i = 0; i < count; i++)
			{
				_identitiesBuffer[i].Core();
			}
		}

		public virtual void PostCore()
		{
		}

		public void Destroy()
		{
		}

		public T GetIdentity<T>(uint runtimeID) where T : BaseMPIdentity
		{
			return (T)_identities[runtimeID];
		}

		public T TryGetIdentity<T>(uint runtimeID) where T : BaseMPIdentity
		{
			BaseMPIdentity value;
			_identities.TryGetValue(runtimeID, out value);
			return value as T;
		}

		public BaseMPIdentity TryGetIdentity(uint runtimeID)
		{
			BaseMPIdentity value;
			_identities.TryGetValue(runtimeID, out value);
			return value;
		}

		public T InstantiateMPIdentity<T>(uint runtimeID, MPSendPacketContainer initPc) where T : BaseMPIdentity, new()
		{
			int num = AllocChannelSequenceForRuntimeID(runtimeID);
			MPSendPacketContainer pc = CreateSendPacket<Packet_Basic_Instantiate>(_instantiateBuilder);
			Packet_Basic_Instantiate.StartPacket_Basic_Instantiate(pc.builder);
			Packet_Basic_Instantiate.AddPeerType(pc.builder, (byte)MPMappings.MPPeerMapping.Get(typeof(T)));
			Packet_Basic_Instantiate.AddChannelSequence(pc.builder, (byte)num);
			pc.Finish(Packet_Basic_Instantiate.EndPacket_Basic_Instantiate(pc.builder));
			SendReliableToOthers(runtimeID, pc);
			SendReliableToOthers(runtimeID, initPc);
			return CreateMPIdentity<T>(runtimeID);
		}

		public void DestroyMPIdentity(uint runtimeID)
		{
			MPSendPacketContainer pc = CreateSendPacket<Packet_Basic_Destroy>();
			Packet_Basic_Destroy.StartPacket_Basic_Destroy(pc.builder);
			pc.Finish(Packet_Basic_Destroy.EndPacket_Basic_Destroy(pc.builder));
			SendReliableToOthers(runtimeID, pc);
			RemoveMPIdentity(runtimeID);
		}

		public void RegisterIdentity(uint runtimeID, int channelSequence, BaseMPIdentity identity)
		{
			BindImportantFixedChannel(runtimeID, channelSequence);
			InitializeIdentity(identity, runtimeID);
		}

		private BaseMPIdentity CreateRemoteMPIdentity(Type type, uint runtimeID, MPRecvPacketContainer pc)
		{
			object obj = Activator.CreateInstance(type);
			BaseMPIdentity baseMPIdentity = (BaseMPIdentity)obj;
			baseMPIdentity.PreInitReplicateRemote(pc);
			InitializeIdentity(baseMPIdentity, runtimeID);
			return baseMPIdentity;
		}

		private T CreateMPIdentity<T>(uint runtimeID) where T : BaseMPIdentity, new()
		{
			T val = new T();
			InitializeIdentity(val, runtimeID);
			return val;
		}

		private void InitializeIdentity(BaseMPIdentity identity, uint runtimeID)
		{
			identity.runtimeID = runtimeID;
			identity.isOwner = identity.GetPeerID() == _peer.peerID;
			_identities.Add(runtimeID, identity);
			_identitiesList.Add(identity);
			identity.Init();
			if (identity.isAuthority)
			{
				identity.OnAuthorityStart();
			}
			else
			{
				identity.OnRemoteStart();
			}
		}

		private void RemoveMPIdentity(uint runtimeID)
		{
			RemoveMPIdentity(_identities[runtimeID]);
		}

		private void RemoveMPIdentity(BaseMPIdentity identity)
		{
			ReleaseChannelSequenceForRuntimeID(identity.runtimeID);
			identity.OnRemoval();
			bool flag = _identitiesList.Remove(identity);
			flag &= _identities.Remove(identity.runtimeID);
		}

		private byte PackControlByte(int selfPeerID)
		{
			return (byte)((byte)peerID << 5);
		}

		private int ParseControlByteSenderPeerID(int controlByte)
		{
			return (controlByte & 0xE0) >> 5;
		}

		private void InitChannelManagement()
		{
			_runtimeIDChannelMap = new Dictionary<uint, int>();
			_channelOccupiedMap = new bool[_peer.channelSequenceCapacity];
			_channelOccupiedMap[_peer.channelSequenceCapacity - 1] = true;
		}

		public void BindImportantFixedChannel(uint runtimeID, int channel)
		{
			_channelOccupiedMap[channel] = true;
			_runtimeIDChannelMap[runtimeID] = channel;
		}

		private int AllocChannelSequenceForRuntimeID(uint runtimeID)
		{
			if (_runtimeIDChannelMap.ContainsKey(runtimeID))
			{
				return _runtimeIDChannelMap[runtimeID];
			}
			int i;
			for (i = 0; i < _channelOccupiedMap.Length - 1 && _channelOccupiedMap[i]; i++)
			{
			}
			if (i != _peer.channelSequenceCapacity - 1)
			{
				_channelOccupiedMap[i] = true;
			}
			_runtimeIDChannelMap.Add(runtimeID, i);
			return i;
		}

		private void ReleaseChannelSequenceForRuntimeID(uint runtimeID)
		{
			int num = _runtimeIDChannelMap[runtimeID];
			if (num != _peer.channelSequenceCapacity - 1)
			{
				_channelOccupiedMap[num] = false;
			}
			_runtimeIDChannelMap.Remove(runtimeID);
		}

		public int GetAllocedChannelID(uint runtimeID)
		{
			return _runtimeIDChannelMap[runtimeID];
		}
	}
}
