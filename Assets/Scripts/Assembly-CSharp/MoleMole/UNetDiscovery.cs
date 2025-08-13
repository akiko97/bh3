using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MoleMole
{
	public class UNetDiscovery
	{
		public delegate void ClientDiscoveredServerDelegate(string serverIP, int serverPort);

		private const int BROADCAST_PORT = 47777;

		private const string ANY_IP = "<ANY>";

		private int _key;

		private int _version;

		private int _subVersion;

		private int _interval;

		private HostTopology _ht;

		private int _serverHostID;

		private int _clientHostID;

		private int _recvHostID;

		protected int _recvConnID;

		protected int _recvChannelID;

		private int _recvDataSize;

		private byte[] _recvBuffer;

		private byte _recvErr;

		public ClientDiscoveredServerDelegate onClientDiscoveredServer;

		private bool _setupTeardownTransport;

		public bool isServerBroadcasting { get; private set; }

		public bool isClientBroadcasting { get; private set; }

		public UNetDiscovery(int key, int version, int subVersion, int interval, bool setupTeardownTransport = false)
		{
			_key = key;
			_version = version;
			_subVersion = subVersion;
			_interval = interval;
			_setupTeardownTransport = setupTeardownTransport;
			ConnectionConfig connectionConfig = new ConnectionConfig();
			connectionConfig.AddChannel(QosType.Unreliable);
			_ht = new HostTopology(connectionConfig, 1);
			_recvBuffer = new byte[1024];
		}

		public void Init()
		{
			if (_setupTeardownTransport)
			{
				NetworkTransport.Init();
			}
			_serverHostID = -1;
			_clientHostID = -1;
		}

		public void Shutdown()
		{
			if (isClientBroadcasting)
			{
				ClientStopListen();
			}
			if (isServerBroadcasting)
			{
				ServerStopBroadcast();
			}
			if (_setupTeardownTransport)
			{
				NetworkTransport.Shutdown();
			}
		}

		public void ServerStartBroadcast(int targetPort, string targetIp = null)
		{
			_serverHostID = NetworkTransport.AddHost(_ht, 0);
			CheckHostID(_serverHostID);
			byte[] array = ServerTupleToByte(targetIp, targetPort);
			byte error;
			bool flag = NetworkTransport.StartBroadcastDiscovery(_serverHostID, 47777, _key, _version, _subVersion, array, array.Length, _interval, out error);
			CheckNetError(error);
			isServerBroadcasting = true;
		}

		public void ServerStopBroadcast()
		{
			NetworkTransport.StopBroadcastDiscovery();
			NetworkTransport.RemoveHost(_serverHostID);
			_serverHostID = -1;
			isServerBroadcasting = false;
		}

		public void ClientStartListen()
		{
			_clientHostID = NetworkTransport.AddHost(_ht, 47777);
			CheckHostID(_clientHostID);
			byte error;
			NetworkTransport.SetBroadcastCredentials(_clientHostID, _key, _version, _subVersion, out error);
			CheckNetError(error);
			isClientBroadcasting = true;
		}

		public void ClientStopListen()
		{
			NetworkTransport.RemoveHost(_clientHostID);
			_clientHostID = -1;
			isClientBroadcasting = false;
		}

		private void CheckHostID(int hostID)
		{
			if (hostID < 0)
			{
				throw new UnityException("failed to add host: ");
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

		public void Core()
		{
			if (!isClientBroadcasting)
			{
				return;
			}
			NetworkEventType networkEventType = NetworkEventType.DataEvent;
			while (isClientBroadcasting && networkEventType != NetworkEventType.Nothing)
			{
				networkEventType = NetworkTransport.ReceiveFromHost(_clientHostID, out _recvConnID, out _recvChannelID, _recvBuffer, _recvBuffer.Length, out _recvDataSize, out _recvErr);
				if (networkEventType == NetworkEventType.BroadcastEvent)
				{
					NetworkTransport.GetBroadcastConnectionMessage(_clientHostID, _recvBuffer, _recvBuffer.Length, out _recvDataSize, out _recvErr);
					CheckNetError(_recvErr);
					string address;
					int port;
					NetworkTransport.GetBroadcastConnectionInfo(_clientHostID, out address, out port, out _recvErr);
					CheckNetError(_recvErr);
					string ip;
					int port2;
					ParseServerTuple(_recvBuffer, _recvDataSize, out ip, out port2);
					if (ip == "<ANY>")
					{
						ip = address;
					}
					if (onClientDiscoveredServer != null)
					{
						onClientDiscoveredServer(ip, port2);
					}
				}
			}
		}

		private byte[] ServerTupleToByte(string ip, int port)
		{
			if (ip == null)
			{
				ip = "<ANY>";
			}
			string s = string.Format("{0}_{1}", ip, port);
			return Encoding.ASCII.GetBytes(s);
		}

		private void ParseServerTuple(byte[] buffer, int len, out string ip, out int port)
		{
			string text = Encoding.ASCII.GetString(buffer, 0, len);
			string[] array = text.Split('_');
			ip = array[0];
			port = int.Parse(array[1]);
		}
	}
}
