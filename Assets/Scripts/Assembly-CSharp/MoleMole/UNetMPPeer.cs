using UnityEngine;
using UnityEngine.Networking;

namespace MoleMole
{
	public class UNetMPPeer : MPPeer
	{
		private enum ClientConnectState
		{
			Idle = 0,
			JustConnectedWaitingForSetClientID = 1,
			ConnectedAndGotClientID = 2
		}

		private const int MAX_CONNECTION_COUNT = 5;

		private const int INVALID_CONNECTION_ID = -1;

		private const int SIMULATOR_MAX_TIME_OUT = 1500;

		private const int SIMULATOR_MIN_TIME_OUT = 1000;

		private const int SIMULATOR_AVG_TIME_OUT = 1250;

		private const int PEER_USE_CHANNEL = 0;

		private const int PEER_CHANNEL_COUNT = 2;

		private PeerState _state;

		private int _peerID;

		private ClientConnectState _clientConnectState;

		private bool _isDelaySimulator;

		private ConnectionSimulatorConfig _simulatorConfig;

		private ConnectionConfig _connConfig;

		private GlobalConfig _globalConfig;

		private byte[] _selfBuffer;

		private int _hostID;

		private int _selfConnID;

		private HostTopology _ht;

		private int _recvConnID;

		private int _recvChannelID;

		private int _recvDataSize;

		private byte[] _recvBuffer;

		private byte _recvErr;

		private int[] _peerMap;

		private int _serverConnectedPeerCount;

		private int _totalPeerCount;

		public override PeerState state
		{
			get
			{
				return _state;
			}
		}

		public override int peerID
		{
			get
			{
				return _peerID;
			}
		}

		public override int totalPeerCount
		{
			get
			{
				return _totalPeerCount;
			}
		}

		public override int channelSequenceCapacity
		{
			get
			{
				return 32;
			}
		}

		private bool isServer
		{
			get
			{
				return _peerID == 1;
			}
		}

		public override int reliableChannel
		{
			get
			{
				return 0;
			}
		}

		public override int stateUpdateChannel
		{
			get
			{
				return 1;
			}
		}

		public UNetMPPeer(bool delaySimulator = false)
		{
			_state = PeerState.Unitialized;
			_globalConfig = new GlobalConfig();
			_globalConfig.ReactorModel = ReactorModel.FixRateReactor;
			_globalConfig.ThreadAwakeTimeout = 22u;
			_globalConfig.MaxPacketSize = 1000;
			ConnectionConfig connectionConfig = new ConnectionConfig
			{
				IsAcksLong = true,
				PacketSize = 800,
				MaxCombinedReliableMessageCount = 8,
				MaxCombinedReliableMessageSize = 64,
				DisconnectTimeout = 10000u,
				PingTimeout = 10000u
			};
			for (int i = 0; i < channelSequenceCapacity; i++)
			{
				connectionConfig.AddChannel(QosType.ReliableSequenced);
				connectionConfig.AddChannel(QosType.StateUpdate);
			}
			_connConfig = connectionConfig;
			_ht = new HostTopology(connectionConfig, 5);
			_ht.ReceivedMessagePoolSize = 1024;
			_ht.SentMessagePoolSize = 1024;
			_recvBuffer = new byte[connectionConfig.PacketSize + 128];
			_isDelaySimulator = delaySimulator;
			_simulatorConfig = new ConnectionSimulatorConfig(1000, 1250, 1000, 1250, 0f);
		}

		public override void Init()
		{
			NetworkTransport.Init(_globalConfig);
			_state = PeerState.Inited;
			_selfConnID = -1;
			_hostID = -1;
			_selfBuffer = new byte[16];
			_serverConnectedPeerCount = 0;
			_peerMap = new int[7];
			_totalPeerCount = 0;
			ResetServerData();
		}

		public override void Shutdown()
		{
			if (_state > PeerState.Inited)
			{
				NetworkTransport.RemoveHost(_hostID);
			}
			NetworkTransport.Shutdown();
			ResetServerData();
			_clientConnectState = ClientConnectState.Idle;
			_state = PeerState.Unitialized;
		}

		private void ResetServerData()
		{
			_serverConnectedPeerCount = 0;
			for (int i = 0; i < _peerMap.Length; i++)
			{
				_peerMap[i] = -1;
			}
		}

		public override void Core()
		{
			if (_state <= PeerState.Inited)
			{
				return;
			}
			NetworkEventType networkEventType = NetworkEventType.DataEvent;
			while (networkEventType != NetworkEventType.Nothing && _state > PeerState.Inited)
			{
				networkEventType = NetworkTransport.ReceiveFromHost(_hostID, out _recvConnID, out _recvChannelID, _recvBuffer, _recvBuffer.Length, out _recvDataSize, out _recvErr);
				if (_state == PeerState.ServerListening)
				{
					switch (networkEventType)
					{
					case NetworkEventType.ConnectEvent:
					{
						CheckNetError(_recvErr, networkEventType);
						_serverConnectedPeerCount++;
						byte b = (byte)(_peerID + _serverConnectedPeerCount);
						_peerMap[b] = _recvConnID;
						_selfBuffer[0] = PackHeader(b, 1);
						_selfBuffer[1] = b;
						SendTo(_recvConnID, 0, _selfBuffer, 2);
						if (onConnected != null)
						{
							onConnected(b);
						}
						break;
					}
					case NetworkEventType.DisconnectEvent:
					{
						bool flag = false;
						for (int i = 2; i <= 1 + _serverConnectedPeerCount; i++)
						{
							if (_peerMap[i] != -1 && _peerMap[i] == _recvConnID)
							{
								_peerMap[i] = -1;
								if (onDisconnected != null)
								{
									onDisconnected(i);
								}
								flag = true;
								break;
							}
						}
						break;
					}
					case NetworkEventType.DataEvent:
						ServerDispatchDataEvent(networkEventType);
						break;
					}
				}
				else if (_state == PeerState.ClientConnecting)
				{
					if (_clientConnectState == ClientConnectState.Idle)
					{
						switch (networkEventType)
						{
						case NetworkEventType.ConnectEvent:
							CheckNetError(_recvErr, networkEventType);
							_clientConnectState = ClientConnectState.JustConnectedWaitingForSetClientID;
							break;
						case NetworkEventType.DataEvent:
							Debug.LogError("not expecting data event before peer ID is set");
							break;
						}
					}
					else if (_clientConnectState == ClientConnectState.JustConnectedWaitingForSetClientID && networkEventType == NetworkEventType.DataEvent)
					{
						CheckNetError(_recvErr, networkEventType);
						int num = ParseMessageType(_recvBuffer);
						int num2 = ParsePeerID(_recvBuffer);
						_peerID = num2;
						_state = PeerState.ClientConnected;
						_clientConnectState = ClientConnectState.ConnectedAndGotClientID;
						if (onConnected != null)
						{
							onConnected(_selfConnID);
						}
					}
				}
				else if (_state == PeerState.ClientConnected)
				{
					switch (networkEventType)
					{
					case NetworkEventType.DisconnectEvent:
						if (onDisconnected != null)
						{
							onDisconnected(_selfConnID);
						}
						_state = PeerState.ClientDropped;
						break;
					case NetworkEventType.DataEvent:
					{
						int num3 = ParseMessageType(_recvBuffer);
						CheckNetError(_recvErr, networkEventType);
						if (num3 == 2)
						{
							_totalPeerCount = _recvBuffer[1];
							_state = PeerState.Established;
							if (onEstablished != null)
							{
								onEstablished();
							}
						}
						else if (onPacket != null)
						{
							onPacket(_recvBuffer, _recvDataSize - 1, 1, _recvChannelID % 2);
						}
						break;
					}
					}
				}
				else
				{
					if (_state != PeerState.Established)
					{
						continue;
					}
					if (isServer)
					{
						switch (networkEventType)
						{
						case NetworkEventType.DataEvent:
							ServerDispatchDataEvent(networkEventType);
							break;
						}
						continue;
					}
					switch (networkEventType)
					{
					case NetworkEventType.DataEvent:
						CheckNetError(_recvErr, networkEventType);
						if (onPacket != null)
						{
							onPacket(_recvBuffer, _recvDataSize - 1, 1, _recvChannelID % 2);
						}
						break;
					}
				}
			}
		}

		public override void Listen(int serverPort = 0, string ipAddress = null)
		{
			if (ipAddress != null && serverPort != 0)
			{
				_hostID = NetworkTransport.AddHost(_ht, serverPort, ipAddress);
			}
			else if (ipAddress == null && serverPort != 0)
			{
				_hostID = NetworkTransport.AddHost(_ht, serverPort);
			}
			else
			{
				_hostID = NetworkTransport.AddHost(_ht);
			}
			_peerID = 1;
			_state = PeerState.ServerListening;
		}

		public override void StopListen()
		{
			NetworkTransport.RemoveHost(_hostID);
			_peerID = 0;
			_state = PeerState.Inited;
			ResetServerData();
		}

		public override void ServerReady()
		{
			_totalPeerCount = 1 + _serverConnectedPeerCount;
			for (int i = 2; i <= 1 + _serverConnectedPeerCount; i++)
			{
				_selfBuffer[0] = PackHeader((byte)i, 2);
				_selfBuffer[1] = (byte)_totalPeerCount;
				SendTo(_peerMap[i], 0, _selfBuffer, 2);
			}
			_state = PeerState.Established;
			if (onEstablished != null)
			{
				onEstablished();
			}
		}

		public override void Connect(string ipAddress, int serverPort)
		{
			byte error;
			if (_isDelaySimulator)
			{
				_hostID = NetworkTransport.AddHostWithSimulator(_ht, 1000, 1500);
				_selfConnID = NetworkTransport.ConnectWithSimulator(_hostID, ipAddress, serverPort, 0, out error, _simulatorConfig);
			}
			else
			{
				_hostID = NetworkTransport.AddHost(_ht);
				_selfConnID = NetworkTransport.Connect(_hostID, ipAddress, serverPort, 0, out error);
			}
			if (error != 0)
			{
				throw new UnityException("bad connection : " + (NetworkError)error);
			}
			_state = PeerState.ClientConnecting;
			_clientConnectState = ClientConnectState.Idle;
		}

		private void ServerDispatchDataEvent(NetworkEventType netEvent)
		{
			CheckNetError(_recvErr, netEvent);
			int num = ParsePeerID(_recvBuffer);
			if (num == 7)
			{
				if (onPacket != null)
				{
					onPacket(_recvBuffer, _recvDataSize - 1, 1, _recvChannelID % 2);
				}
				for (int i = 2; i <= 1 + _serverConnectedPeerCount; i++)
				{
					int num2 = _peerMap[i];
					if (num2 != -1 && num2 != _recvConnID)
					{
						SendTo(num2, _recvChannelID, _recvBuffer, _recvDataSize);
					}
				}
			}
			else if (num == _peerID)
			{
				if (onPacket != null)
				{
					onPacket(_recvBuffer, _recvDataSize - 1, 1, _recvChannelID % 2);
				}
			}
			else
			{
				int num3 = _peerMap[num];
				if (num3 != -1)
				{
					SendTo(num3, _recvChannelID, _recvBuffer, _recvDataSize);
				}
			}
		}

		private void SendTo(int connID, int channel, byte[] data, int len)
		{
			byte error;
			NetworkTransport.Send(_hostID, connID, channel, data, len, out error);
			CheckNetError(error);
		}

		public override void SendByChannel(byte[] data, int len, int channel, int channelSequence)
		{
			int num = ParsePeerID(data);
			if (num == _peerID)
			{
				if (onPacket != null)
				{
					onPacket(data, len - 1, 1, channel);
				}
				return;
			}
			int channel2 = channelSequence * 2 + channel;
			if (isServer)
			{
				if (num == 7)
				{
					for (int i = 2; i <= 1 + _serverConnectedPeerCount; i++)
					{
						int num2 = _peerMap[i];
						if (num2 != -1)
						{
							SendTo(num2, channel2, data, len);
						}
					}
				}
				else
				{
					int num3 = _peerMap[num];
					if (num3 != -1)
					{
						SendTo(num3, channel2, data, len);
					}
				}
			}
			else
			{
				SendTo(_selfConnID, channel2, data, len);
			}
		}

		private void CheckNetError(byte err, NetworkEventType evt = NetworkEventType.Nothing)
		{
			if (err != 0)
			{
				string text = string.Format("net error: {0} ", (NetworkError)err);
				if (evt != NetworkEventType.Nothing)
				{
					text = text + " evt: " + evt;
				}
				throw new UnityException(text);
			}
		}

		public override void OnGUI()
		{
			byte error;
			GUILayout.Label(string.Format("state: {0}, isServer: {1}, peerID: {2}, peerCnt: {3}, trans recv: {4,5}, trans send {5,5}", _state, isServer, _peerID, _totalPeerCount, (state != PeerState.Established) ? "<>" : NetworkTransport.GetPacketReceivedRate(_hostID, _selfConnID, out error).ToString(), (state != PeerState.Established) ? "<>" : NetworkTransport.GetPacketSentRate(_hostID, _selfConnID, out error).ToString()));
		}

		public override void GetPeerStats(out ulong sendTotal, out ulong recvTotal, out ulong sendCount, out ulong recvCount)
		{
			sendTotal = 0uL;
			recvTotal = 0uL;
			sendCount = 0uL;
			recvCount = 0uL;
		}

		public override void GetPeerStats2(out string stat2)
		{
			if (isServer)
			{
				stat2 = "<server>";
			}
			else
			{
				byte error;
				stat2 = string.Format("rtt: {0,5:0000.00}", NetworkTransport.GetCurrentRtt(_hostID, _selfConnID, out error));
			}
		}
	}
}
