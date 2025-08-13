using System;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class MonoClientPacketConsumer : MonoBehaviour
	{
		public enum Status
		{
			Normal = 0,
			WaitingConnect = 1,
			RepeatLogin = 2
		}

		private const float RECONNECT_INTERVAL = 5f;

		private MiClient _client;

		public string host;

		public ushort port;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		private GeneralDialogContext _errorDialogContext;

		private float _timer;

		private int _reconnectTimes;

		public uint lastServerPacketId;

		public uint clientPacketId;

		public bool netReachAlreadyInit;

		public NetworkReachability netReach;

		public Status _status;

		public void Init(MiClient client)
		{
			_client = client;
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		private void Start()
		{
			host = _client.Host;
			port = _client.Port;
			_loadingWheelDialogContext = new LoadingWheelWidgetContext();
			_status = Status.Normal;
		}

		private void Update()
		{
			NetPacketV1 netPacketV = _client.recv();
			if (netPacketV != null)
			{
				OnConnectNormal();
				Type typeByCmdID = Singleton<CommandMap>.Instance.GetTypeByCmdID(netPacketV.getCmdId());
				if (typeByCmdID != null)
				{
					DispatchPacket(netPacketV);
				}
			}
			else
			{
				if (!Singleton<NetworkManager>.Instance.alreadyLogin)
				{
					return;
				}
				NetworkReachability internetReachability = Application.internetReachability;
				if (!netReachAlreadyInit)
				{
					netReachAlreadyInit = true;
					netReach = internetReachability;
				}
				else if (netReach != internetReachability)
				{
					netReach = internetReachability;
					if (_client.isConnected())
					{
						_client.disconnect();
					}
				}
				if (_client.isConnected() || Singleton<MainUIManager>.Instance == null || Singleton<MainUIManager>.Instance.SceneCanvas == null)
				{
					return;
				}
				if (_status == Status.Normal)
				{
					_status = Status.WaitingConnect;
					Reconnect();
				}
				else if (_status == Status.WaitingConnect)
				{
					_timer += Time.deltaTime;
					if (_timer > 5f)
					{
						ShowLoadingWheel();
						Reconnect();
					}
				}
				else if (_status == Status.RepeatLogin)
				{
					TryShowErrorDialog();
				}
			}
		}

		private bool DispatchPacket(NetPacketV1 pkt)
		{
			if (pkt.getTime() != 0)
			{
				lastServerPacketId = pkt.getTime();
			}
			return Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.NetwrokPacket, pkt));
		}

		public void OnApplicationQuit()
		{
			_client.disconnect();
		}

		private void Reconnect()
		{
			_reconnectTimes++;
			_timer = 0f;
			_status = Status.WaitingConnect;
			Singleton<NetworkManager>.Instance.QuickLogin();
		}

		private void OnConnectNormal()
		{
			if (_status != Status.Normal)
			{
				_timer = 0f;
				_reconnectTimes = 0;
				_status = Status.Normal;
				if (ShouldShowLoadingWheelWhenDisconnect())
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnSocketConnect));
				}
			}
			if (_loadingWheelDialogContext != null && _loadingWheelDialogContext.view != null)
			{
				_loadingWheelDialogContext.Finish();
			}
		}

		private void ShowLoadingWheel()
		{
			if (ShouldShowLoadingWheelWhenDisconnect())
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnSocketDisconnect));
				Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
			}
		}

		private bool ShouldShowLoadingWheelWhenDisconnect()
		{
			return !(Singleton<MainUIManager>.Instance.SceneCanvas is MonoInLevelUICanvas) || MiscData.Config.BasicConfig.ShouldShowLoadingWheelWhenDisconnectInLevel;
		}

		public void SetRepeatLogin()
		{
			_status = Status.RepeatLogin;
		}

		public void TryShowErrorDialog()
		{
			if (_errorDialogContext == null || !(_errorDialogContext.view != null))
			{
				_errorDialogContext = new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Err_PlayerRepeatLogin"),
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						GeneralLogicManager.RestartGame();
					}
				};
				Singleton<MainUIManager>.Instance.ShowDialog(_errorDialogContext);
			}
		}
	}
}
