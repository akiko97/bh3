using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MoleMole
{
	public class MonoDebugMP : MonoBehaviour
	{
		private enum State
		{
			Idle = 0,
			DiscoveryClient = 1,
			DiscoveryServer = 2,
			StandByClient = 3,
			Connected = 4
		}

		private const int DEBUG_MP_BOX_WIDTH = 380;

		private const int DEBUG_MP_BOX_HEIGHT = 120;

		private const float STAT_WINDOW = 0.5f;

		[Header("Which net lib to use")]
		public MPPeerType peerType;

		[Header("Server Listen Port")]
		public int gameServerPort = 53712;

		[Header("Discovery Key")]
		public int DiscoveryKey = 2333;

		[Header("Discovery Version")]
		public int DiscoveryVersion = 1;

		[Header("Discovery Sub Version")]
		public int DiscoverySubVersion = 1;

		[Header("Server wait for n player to start")]
		public int WaitForPlayerCount = 2;

		[Header("uNet simulator, doesn't really work very well really")]
		public bool isDelaySimulator;

		private State _state;

		public Action<MPPeer> onPeerReady;

		private MPPeer _peer;

		private UNetDiscovery _discovery;

		private int _connectedPeerCount;

		private GUIStyle _style;

		private ulong _lastSendTotal;

		private ulong _lastRecvTotal;

		private float _lastSendRate;

		private float _lastRecvRate;

		private ulong _lastSendCount;

		private ulong _lastRecvCount;

		private ulong _lastSendCountWindowed;

		private ulong _lastRecvCountWindowed;

		private float _statTimer;

		private void Awake()
		{
			_style = new GUIStyle();
			_style.margin = new RectOffset();
			_style.padding = new RectOffset();
			_style.normal.textColor = Color.magenta;
			_style.hover.textColor = Color.magenta;
			_style.focused.textColor = Color.magenta;
			_style.active.textColor = Color.magenta;
			_style.onNormal.textColor = Color.magenta;
			_style.onHover.textColor = Color.magenta;
			_style.onFocused.textColor = Color.magenta;
			_style.onActive.textColor = Color.magenta;
			Texture2D texture2D = new Texture2D(4, 4);
			Color[] array = new Color[16];
			Color black = Color.black;
			black.a = 0.5f;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = black;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			_style.normal.background = texture2D;
			if (peerType == MPPeerType.uNet)
			{
				_discovery = new UNetDiscovery(DiscoveryKey, DiscoveryVersion, DiscoverySubVersion, 1000);
				_peer = new UNetMPPeer(isDelaySimulator);
			}
			else if (peerType == MPPeerType.Lidgren)
			{
				_discovery = new UNetDiscovery(DiscoveryKey, DiscoveryVersion, DiscoverySubVersion, 1000, true);
				_peer = new LidgrenMPPeer();
			}
			_peer.Init();
			_discovery.Init();
			_peer.onConnected = OnClientConnected;
			_discovery.onClientDiscoveredServer = OnClientFoundServer;
			_peer.onEstablished = OnEstablished;
			_connectedPeerCount = 0;
		}

		private void OnDestroy()
		{
			_discovery.Shutdown();
			_peer.Shutdown();
			UnityEngine.Object.Destroy(_style.normal.background);
		}

		private void OnEstablished()
		{
			_state = State.Connected;
			InitPlotting();
			if (onPeerReady != null)
			{
				onPeerReady(_peer);
			}
		}

		private void OnClientConnected(int peerID)
		{
			if (_state == State.DiscoveryServer)
			{
				_connectedPeerCount++;
				if (_connectedPeerCount + 1 == WaitForPlayerCount)
				{
					if (_discovery.isServerBroadcasting)
					{
						_discovery.Shutdown();
					}
					_peer.ServerReady();
				}
			}
			else if (_state == State.DiscoveryClient)
			{
				_state = State.StandByClient;
			}
		}

		private string MapToIPV4Literal(string serverIP)
		{
			IPAddress address;
			IPAddress.TryParse(serverIP, out address);
			if (address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				byte[] addressBytes = address.GetAddressBytes();
				return string.Format("{0}.{1}.{2}.{3}", addressBytes[12], addressBytes[13], addressBytes[14], addressBytes[15]);
			}
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				return serverIP;
			}
			return null;
		}

		private void OnClientFoundServer(string serverIP, int serverPort)
		{
			if (_state == State.DiscoveryClient)
			{
				if (peerType == MPPeerType.Lidgren)
				{
					serverIP = MapToIPV4Literal(serverIP);
				}
				_peer.Connect(serverIP, serverPort);
				if (_discovery.isClientBroadcasting)
				{
					_discovery.ClientStopListen();
				}
			}
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(Screen.width - 380, 0f, 380f, 120f), _style);
			_peer.OnGUI();
			if (_state == State.Idle)
			{
				if (GUILayout.Button("Start Discovery Server", GUILayout.Height(30f)) && !_discovery.isServerBroadcasting)
				{
					_peer.Listen(gameServerPort);
					_discovery.ServerStartBroadcast(gameServerPort);
					_state = State.DiscoveryServer;
				}
				if (GUILayout.Button("Start Discovery Client", GUILayout.Height(30f)) && !_discovery.isClientBroadcasting)
				{
					_discovery.ClientStartListen();
					_state = State.DiscoveryClient;
				}
			}
			else if (_state == State.DiscoveryServer)
			{
				GUILayout.Label("Discovery Server Broadcasting on port: " + gameServerPort);
				if (GUILayout.Button("Stop", GUILayout.Height(30f)) && _discovery.isServerBroadcasting)
				{
					_discovery.ServerStopBroadcast();
					_peer.StopListen();
					_state = State.Idle;
				}
			}
			else if (_state == State.DiscoveryClient)
			{
				GUILayout.Label("Discovery Client Listenting on port: " + gameServerPort);
				if (GUILayout.Button("Stop", GUILayout.Height(30f)))
				{
					_discovery.ClientStopListen();
					_state = State.Idle;
				}
			}
			else if (_state == State.StandByClient)
			{
				GUILayout.Label("client standby, waiting for start.");
			}
			else if (_state == State.Connected)
			{
				DrawAndCalcStats();
			}
			GUILayout.EndArea();
		}

		private void InitPlotting()
		{
			PlotManager.Instance.PlotCreate("SendByteRate", 0f, 150f, Color.red, Vector2.zero);
			PlotManager.Instance.PlotCreate("RecvByteRate", Color.green, "SendByteRate");
		}

		private float AvgWithLast(float lhs, float rhs)
		{
			return (lhs + rhs) / 2f;
		}

		private void DrawAndCalcStats()
		{
			ulong sendTotal;
			ulong recvTotal;
			ulong sendCount;
			ulong recvCount;
			_peer.GetPeerStats(out sendTotal, out recvTotal, out sendCount, out recvCount);
			GUILayout.Label(string.Format("s(kb):{0,10}, r(kb):{1,10}, s(cnt):{2,10}, r(cnt):{3,10}", sendTotal / 1000, recvTotal / 1000, sendCount, recvCount), _style);
			GUILayout.Label(string.Format("s(kb/m):{0,5:00.00},r(kb/m):{1,5:00.00},s(cnt/m):{2,5}, r(cnt/m):{3,5}", _lastSendRate, _lastRecvRate, _lastSendCountWindowed, _lastRecvCountWindowed), _style);
			string stat;
			_peer.GetPeerStats2(out stat);
			GUILayout.Label(stat);
			_statTimer += Time.unscaledDeltaTime;
			if (_statTimer > 0.5f)
			{
				float num = sendTotal - _lastSendTotal;
				float num2 = recvTotal - _lastRecvTotal;
				float num3 = sendCount - _lastSendCount;
				float num4 = recvCount - _lastRecvCount;
				_lastSendRate = AvgWithLast(_lastSendRate, num / _statTimer * 0.060000002f);
				_lastRecvRate = AvgWithLast(_lastRecvRate, num2 / _statTimer * 0.060000002f);
				_lastSendCountWindowed = (ulong)AvgWithLast(_lastSendCountWindowed, num3 / _statTimer * 60f);
				_lastRecvCountWindowed = (ulong)AvgWithLast(_lastRecvCountWindowed, num4 / _statTimer * 60f);
				PlotManager.Instance.PlotAdd("SendByteRate", _lastSendRate);
				PlotManager.Instance.PlotAdd("RecvByteRate", _lastRecvRate);
				_statTimer = 0f;
				_lastSendTotal = sendTotal;
				_lastRecvTotal = recvTotal;
				_lastSendCount = sendCount;
				_lastRecvCount = recvCount;
			}
		}

		private void Update()
		{
			_discovery.Core();
			_peer.Core();
		}
	}
}
