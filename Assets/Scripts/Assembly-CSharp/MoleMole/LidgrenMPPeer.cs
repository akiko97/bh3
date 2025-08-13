using System;
using System.Net;
using Lidgren.Network;
using UnityEngine;

namespace MoleMole
{
	public class LidgrenMPPeer : MPPeer
	{
		private enum ClientConnectState
		{
			Idle = 0,
			JustConnectedWaitingForSetClientID = 1,
			ConnectedAndGotClientID = 2
		}

		private const string APP_ID = "ng";

		private const int BUFFER_SIZE = 1024;

		private const int PEER_USE_SEQUENCE_ID = 0;

		private PeerState _state;

		private int _peerID;

		private int _totalPeerCount;

		private NetPeer _lidgrenPeer;

		private NetClient _lidgrenClient;

		private NetServer _lidgrenServer;

		private NetConnection[] _peerMap;

		private int _serverConnectedPeerCount;

		public override int peerID
		{
			get
			{
				return _peerID;
			}
		}

		public override int reliableChannel
		{
			get
			{
				return 67;
			}
		}

		public override int stateUpdateChannel
		{
			get
			{
				return 2;
			}
		}

		public override PeerState state
		{
			get
			{
				return _state;
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

		public LidgrenMPPeer()
		{
			_state = PeerState.Unitialized;
		}

		public override void Init()
		{
			_state = PeerState.Inited;
			_peerMap = new NetConnection[7];
			_serverConnectedPeerCount = 0;
		}

		public override void Listen(int serverPort = 0, string ipAddress = null)
		{
			NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration("ng");
			netPeerConfiguration.AutoFlushSendQueue = true;
			netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			if (serverPort != 0)
			{
				netPeerConfiguration.Port = serverPort;
			}
			if (ipAddress != null)
			{
				netPeerConfiguration.LocalAddress = IPAddress.Parse(ipAddress);
			}
			else if (ipAddress == null)
			{
				netPeerConfiguration.LocalAddress = IPAddress.Parse("0.0.0.0");
			}
			_lidgrenServer = new NetServer(netPeerConfiguration);
			_lidgrenPeer = _lidgrenServer;
			_lidgrenServer.Start();
			_peerID = 1;
			_state = PeerState.ServerListening;
		}

		public override void StopListen()
		{
			_lidgrenServer.Shutdown("what?");
			_peerID = 0;
			_state = PeerState.Inited;
			ResetServerData();
		}

		private void ResetServerData()
		{
			_serverConnectedPeerCount = 0;
			for (int i = 0; i < _peerMap.Length; i++)
			{
				_peerMap[i] = null;
			}
		}

		public override void ServerReady()
		{
			_totalPeerCount = 1 + _serverConnectedPeerCount;
			byte[] array = new byte[64];
			array[0] = PackHeader(7, 2);
			array[1] = (byte)_totalPeerCount;
			SendByChannel(array, array.Length, reliableChannel, 0);
			_state = PeerState.Established;
			if (onEstablished != null)
			{
				onEstablished();
			}
		}

		public override void Connect(string ipAddress, int serverPort)
		{
			NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration("ng");
			netPeerConfiguration.AutoFlushSendQueue = true;
			_lidgrenClient = new NetClient(netPeerConfiguration);
			_lidgrenPeer = _lidgrenClient;
			_lidgrenClient.Start();
			NetOutgoingMessage netOutgoingMessage = _lidgrenClient.CreateMessage(8);
			netOutgoingMessage.Write("ng");
			_lidgrenClient.Connect(ipAddress, serverPort, netOutgoingMessage);
			_state = PeerState.ClientConnecting;
		}

		public override void Shutdown()
		{
			if (_state > PeerState.Inited)
			{
				_lidgrenPeer.Shutdown("BYE");
			}
			ResetServerData();
			_state = PeerState.Unitialized;
		}

		public override void Core()
		{
			if (_state <= PeerState.Inited)
			{
				return;
			}
			NetIncomingMessage netIncomingMessage;
			while ((netIncomingMessage = _lidgrenPeer.ReadMessage()) != null)
			{
				NetIncomingMessageType messageType = netIncomingMessage.MessageType;
				if (messageType == NetIncomingMessageType.VerboseDebugMessage || messageType == NetIncomingMessageType.DebugMessage || messageType == NetIncomingMessageType.WarningMessage || messageType == NetIncomingMessageType.ErrorMessage)
				{
					Debug.LogError(netIncomingMessage.ReadString());
				}
				if (_state == PeerState.ServerListening)
				{
					switch (netIncomingMessage.MessageType)
					{
					case NetIncomingMessageType.ConnectionApproval:
					{
						string text = netIncomingMessage.ReadString();
						if (text == "ng")
						{
							byte b = 0;
							for (int k = 2; k < _peerMap.Length; k++)
							{
								if (_peerMap[k] == null)
								{
									b = (byte)k;
									break;
								}
							}
							_peerMap[b] = netIncomingMessage.SenderConnection;
							NetOutgoingMessage netOutgoingMessage = _lidgrenPeer.CreateMessage();
							netOutgoingMessage.Data[0] = PackHeader(b, 1);
							netOutgoingMessage.Data[1] = b;
							netOutgoingMessage.LengthBytes = 2;
							netIncomingMessage.SenderConnection.Approve(netOutgoingMessage);
						}
						else
						{
							netIncomingMessage.SenderConnection.Deny();
						}
						break;
					}
					case NetIncomingMessageType.StatusChanged:
					{
						NetConnectionStatus netConnectionStatus = (NetConnectionStatus)netIncomingMessage.ReadByte();
						string message = netIncomingMessage.ReadString();
						Debug.Log(message);
						switch (netConnectionStatus)
						{
						case NetConnectionStatus.Disconnected:
						{
							bool flag = false;
							for (int j = 2; j < _peerMap.Length; j++)
							{
								if (_peerMap[j].RemoteUniqueIdentifier == netIncomingMessage.SenderConnection.RemoteUniqueIdentifier)
								{
									flag = true;
									_peerMap[j] = null;
									_serverConnectedPeerCount--;
									break;
								}
							}
							break;
						}
						case NetConnectionStatus.Connected:
						{
							int connID = 0;
							for (int i = 2; i < _peerMap.Length; i++)
							{
								if (_peerMap[i].RemoteUniqueIdentifier == netIncomingMessage.SenderConnection.RemoteUniqueIdentifier)
								{
									connID = i;
									break;
								}
							}
							_serverConnectedPeerCount++;
							if (onConnected != null)
							{
								onConnected(connID);
							}
							break;
						}
						}
						break;
					}
					case NetIncomingMessageType.Data:
						ServerDispatchDataEvent(netIncomingMessage);
						break;
					}
				}
				else if (_state == PeerState.ClientConnecting)
				{
					if (netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged)
					{
						NetConnectionStatus netConnectionStatus2 = (NetConnectionStatus)netIncomingMessage.ReadByte();
						string arg = netIncomingMessage.ReadString();
						Debug.Log(string.Format("{0}, {1}", netConnectionStatus2, arg));
						if (netConnectionStatus2 == NetConnectionStatus.Connected)
						{
							NetIncomingMessage remoteHailMessage = netIncomingMessage.SenderConnection.RemoteHailMessage;
							int num = ParsePeerID(remoteHailMessage.Data);
							int num2 = ParseMessageType(remoteHailMessage.Data);
							int num3 = remoteHailMessage.Data[1];
							_peerID = num;
							_state = PeerState.ClientConnected;
							if (onConnected != null)
							{
								onConnected(_peerID);
							}
						}
					}
					else if (netIncomingMessage.MessageType != NetIncomingMessageType.Data)
					{
					}
				}
				else if (_state == PeerState.ClientConnected)
				{
					switch (netIncomingMessage.MessageType)
					{
					case NetIncomingMessageType.StatusChanged:
					{
						NetConnectionStatus netConnectionStatus3 = (NetConnectionStatus)netIncomingMessage.ReadByte();
						string message2 = netIncomingMessage.ReadString();
						Debug.Log(message2);
						if (netConnectionStatus3 == NetConnectionStatus.Disconnected)
						{
							if (onDisconnected != null)
							{
								onDisconnected(_peerID);
							}
							_state = PeerState.ClientDropped;
						}
						break;
					}
					case NetIncomingMessageType.Data:
					{
						int num4 = ParseMessageType(netIncomingMessage.Data);
						if (num4 == 2)
						{
							_totalPeerCount = netIncomingMessage.Data[1];
							_state = PeerState.Established;
							if (onEstablished != null)
							{
								onEstablished();
							}
						}
						else if (onPacket != null)
						{
							onPacket(netIncomingMessage.Data, netIncomingMessage.LengthBytes - 1, 1, (int)netIncomingMessage.DeliveryMethod);
						}
						break;
					}
					}
				}
				else if (_state == PeerState.Established)
				{
					if (isServer)
					{
						messageType = netIncomingMessage.MessageType;
						if (messageType != NetIncomingMessageType.StatusChanged && messageType == NetIncomingMessageType.Data)
						{
							ServerDispatchDataEvent(netIncomingMessage);
						}
					}
					else
					{
						messageType = netIncomingMessage.MessageType;
						if (messageType != NetIncomingMessageType.StatusChanged && messageType == NetIncomingMessageType.Data && onPacket != null)
						{
							onPacket(netIncomingMessage.Data, netIncomingMessage.LengthBytes - 1, 1, (int)netIncomingMessage.DeliveryMethod);
						}
					}
				}
				_lidgrenPeer.Recycle(netIncomingMessage);
			}
		}

		private void ServerDispatchDataEvent(NetIncomingMessage recvMsg)
		{
			byte[] data = recvMsg.Data;
			int num = ParsePeerID(data);
			if (num == 7)
			{
				if (onPacket != null)
				{
					onPacket(data, recvMsg.LengthBytes - 1, 1, (int)recvMsg.DeliveryMethod);
				}
				for (int i = 2; i < _peerMap.Length; i++)
				{
					NetConnection netConnection = _peerMap[i];
					if (netConnection != null && netConnection.RemoteUniqueIdentifier != recvMsg.SenderConnection.RemoteUniqueIdentifier)
					{
						SendTo(netConnection, recvMsg.DeliveryMethod, data, recvMsg.LengthBytes, recvMsg.SequenceChannel);
					}
				}
			}
			else if (num == _peerID)
			{
				if (onPacket != null)
				{
					onPacket(data, recvMsg.LengthBytes - 1, 1, (int)recvMsg.DeliveryMethod);
				}
			}
			else
			{
				NetConnection netConnection2 = _peerMap[num];
				if (netConnection2 != null)
				{
					SendTo(netConnection2, recvMsg.DeliveryMethod, data, recvMsg.LengthBytes, recvMsg.SequenceChannel);
				}
			}
		}

		private void SendTo(NetConnection conn, NetDeliveryMethod channel, byte[] data, int len, int channelSequence)
		{
			NetOutgoingMessage netOutgoingMessage = _lidgrenPeer.CreateMessage(data.Length);
			Buffer.BlockCopy(data, 0, netOutgoingMessage.Data, 0, len);
			netOutgoingMessage.LengthBytes = len;
			NetSendResult netSendResult = _lidgrenPeer.SendMessage(netOutgoingMessage, conn, channel, channelSequence);
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
			}
			else if (isServer)
			{
				if (num == 7)
				{
					for (int i = 2; i < _peerMap.Length; i++)
					{
						NetConnection netConnection = _peerMap[i];
						if (netConnection != null)
						{
							SendTo(netConnection, (NetDeliveryMethod)channel, data, len, channelSequence);
						}
					}
				}
				else
				{
					NetConnection netConnection2 = _peerMap[num];
					if (netConnection2 != null)
					{
						SendTo(netConnection2, (NetDeliveryMethod)channel, data, len, channelSequence);
					}
				}
			}
			else
			{
				SendTo(_lidgrenClient.ServerConnection, (NetDeliveryMethod)channel, data, len, channelSequence);
			}
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
				stat2 = string.Format("{0,5:0000.00}", _lidgrenClient.ServerConnection.AverageRoundtripTime * 1000f);
			}
		}

		public override void OnGUI()
		{
			if (_state > PeerState.Inited)
			{
				GUILayout.Label(string.Format("state: {0}, isServer: {1}, peerID: {2}, peerCnt: {3}, peerUID: {4}", _state, isServer, _peerID, _totalPeerCount, _lidgrenPeer.UniqueIdentifier));
			}
		}
	}
}
