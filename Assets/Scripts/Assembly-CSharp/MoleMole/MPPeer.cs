namespace MoleMole
{
	public abstract class MPPeer
	{
		public enum PeerState
		{
			Unitialized = 0,
			Inited = 1,
			ServerListening = 2,
			ClientConnecting = 3,
			ClientConnected = 4,
			ClientDropped = 5,
			Established = 6
		}

		public delegate void ReceiveHandler(byte[] buffer, int len, int offset, int channel);

		public delegate void ConnectedHandler(int connID);

		public delegate void DisconnectedHandler(int connID);

		public delegate void EstablishedHandler();

		public const int PEER_PACKET_RESERVE_BYTE_COUNT = 1;

		public const int PEER_ARR_COUNT = 7;

		public const byte INVALID_PEER_ID = 0;

		public const byte MASTER_PEER_ID = 1;

		public const byte SEND_TO_ALL_PEER_ID = 7;

		public const byte PACKET_PEER_ID_MASK = 224;

		public const byte PACKET_METATYPE_MASK = 31;

		public const byte PACKET_METATYPE_SET_CLIENT_PEER_ID = 1;

		public const byte PACKET_METATYPE_SERVER_FINALIZED = 2;

		public const byte PACKET_METATYPE_USER_MESSAGE = 31;

		public ReceiveHandler onPacket;

		public ConnectedHandler onConnected;

		public DisconnectedHandler onDisconnected;

		public EstablishedHandler onEstablished;

		public abstract PeerState state { get; }

		public abstract int peerID { get; }

		public abstract int totalPeerCount { get; }

		public abstract int reliableChannel { get; }

		public abstract int stateUpdateChannel { get; }

		public abstract int channelSequenceCapacity { get; }

		public int ParsePeerID(byte[] data)
		{
			return (data[0] & 0xE0) >> 5;
		}

		public int ParseMessageType(byte[] data)
		{
			return data[0] & 0x1F;
		}

		public byte PackHeader(byte peerID, byte metaType)
		{
			return (byte)((peerID << 5) | metaType);
		}

		public abstract void Init();

		public abstract void Shutdown();

		public abstract void Core();

		public abstract void Connect(string ipAddress, int serverPort);

		public abstract void SendByChannel(byte[] data, int len, int channel, int channelSequence);

		public abstract void Listen(int serverPort = 0, string ipAddress = null);

		public abstract void StopListen();

		public abstract void ServerReady();

		public virtual void OnGUI()
		{
		}

		public virtual bool CanSend()
		{
			return state == PeerState.Established || state == PeerState.ClientConnected || state == PeerState.ServerListening;
		}

		public abstract void GetPeerStats(out ulong sendTotal, out ulong recvTotal, out ulong sendCount, out ulong recvCount);

		public abstract void GetPeerStats2(out string stat2);
	}
}
