using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoApplicationBehaviour : MonoBehaviour
	{
		private DateTime _timeEnterBG = TimeUtil.Now;

		private ApplicationManager _applicationManager;

		public void Init(ApplicationManager applicationManager)
		{
			_applicationManager = applicationManager;
		}

		private void OnApplicationPause(bool paused)
		{
			if (Singleton<LevelManager>.Instance != null)
			{
				Singleton<LevelManager>.Instance.SetPause(paused);
			}
			if (paused)
			{
				_timeEnterBG = TimeUtil.Now;
				return;
			}
			bool flag = false;
			if (MiscData.Config != null && MiscData.Config.BasicConfig.IsRestartWhenGameResume && (TimeUtil.Now - _timeEnterBG).TotalSeconds > (double)MiscData.Config.BasicConfig.RestartGameTimeSpanSeconds && Singleton<NetworkManager>.Instance != null && Singleton<NetworkManager>.Instance.alreadyLogin)
			{
				flag = true;
				GeneralLogicManager.RestartGame();
			}
			if (!flag && Singleton<PlayerModule>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.DetectCheat();
			}
			if (!flag)
			{
				Singleton<AccountManager>.Instance.manager.ShowPausePage();
				Singleton<AccountManager>.Instance.manager.ShowToolBar();
			}
		}

		private void Update()
		{
			GraphicsUtils.RebindAllRenderTexturesToCamera();
			if (Singleton<QAManager>.Instance != null)
			{
				Singleton<QAManager>.Instance.UpdateSendMessageToSever();
			}
			_applicationManager.ManualCoroutinesMoveNext();
			_applicationManager.ClearFinishCoroutines();
		}
	}
}
