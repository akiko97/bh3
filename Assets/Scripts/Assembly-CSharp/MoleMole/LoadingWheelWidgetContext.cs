using System;
using MoleMole.Config;

namespace MoleMole
{
	public class LoadingWheelWidgetContext : BaseWidgetContext
	{
		private float MAX_WAIT_TIME = 5f;

		private CanvasTimer _timer;

		private bool _isSimpleWaitCmd;

		private ushort _waitCmdID;

		public Action timeOutCallBack;

		public bool ignoreMaxWaitTime;

		public LoadingWheelWidgetContext(ushort waitCmd, Action timeOutCallBack = null)
		{
			SetConfig();
			_isSimpleWaitCmd = true;
			_waitCmdID = waitCmd;
			this.timeOutCallBack = timeOutCallBack;
		}

		public LoadingWheelWidgetContext()
		{
			SetConfig();
			_isSimpleWaitCmd = false;
		}

		private void SetConfig()
		{
			config = new ContextPattern
			{
				contextName = "LoadingWheelDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/LoadingWheelDialog",
				cacheType = ViewCacheType.DontCache
			};
			uiType = UIType.MostFront;
		}

		public void Finish()
		{
			Destroy();
		}

		public void SetMaxWaitTime(float maxWaitTime)
		{
			if (maxWaitTime > 0f)
			{
				MAX_WAIT_TIME = maxWaitTime;
			}
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			if (!_isSimpleWaitCmd)
			{
				return false;
			}
			ushort cmdId = pkt.getCmdId();
			if (cmdId == _waitCmdID)
			{
				Finish();
			}
			return false;
		}

		protected override bool SetupView()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			if (!ignoreMaxWaitTime)
			{
				_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(MAX_WAIT_TIME, 0f);
				_timer.timeUpCallback = OnTimeOut;
			}
			return false;
		}

		private void OnTimeOut()
		{
			if (timeOutCallBack != null)
			{
				timeOutCallBack();
			}
			Destroy();
		}

		public override void Destroy()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			base.Destroy();
		}
	}
}
