using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoCanvas : MonoBehaviour
	{
		private List<CanvasTimer> _canvasTimers = new List<CanvasTimer>();

		private List<CanvasTimer> _newTimersDuringUpdate = new List<CanvasTimer>();

		private GMTalkDialogContext _GMTalkDialogContext;

		private GeneralConfirmDialogContext _quitGameDialogContext;

		public virtual void Start()
		{
			GameObject gameObject = GameObject.Find("EventSystem");
			if (gameObject != null)
			{
			}
			string sceneName = "Loading";
			if (this is MonoMainCanvas)
			{
				sceneName = "MainMenuWithSpaceship";
			}
			Action callBackWhenSceneLoaded = Singleton<MainUIManager>.Instance.GetCallBackWhenSceneLoaded(sceneName);
			if (callBackWhenSceneLoaded != null)
			{
				callBackWhenSceneLoaded();
			}
			Singleton<MainUIManager>.Instance.ResetSceneLoadedCallBack(sceneName);
		}

		public virtual void Update()
		{
			RemoveAllTimeUpTimer();
			_canvasTimers.AddRange(_newTimersDuringUpdate);
			_newTimersDuringUpdate.Clear();
			for (int i = 0; i < _canvasTimers.Count; i++)
			{
				CanvasTimer canvasTimer = _canvasTimers[i];
				canvasTimer.Core();
			}
			if (GlobalVars.DEBUG_FEATURE_ON)
			{
				UpdateForGMTalk();
			}
			UpdateEscapeListener();
		}

		public virtual void OnDestroy()
		{
		}

		public virtual void PlayVideo(CgDataItem cgDataItem)
		{
		}

		private void RemoveAllTimeUpTimer()
		{
			_canvasTimers.RemoveAll((CanvasTimer x) => x.IsTimeUp);
		}

		public CanvasTimer CreateTimer(float m_timespan, float m_triggerCD = 0f)
		{
			CanvasTimer canvasTimer = new CanvasTimer();
			canvasTimer.timespan = m_timespan;
			canvasTimer.triggerCD = m_triggerCD;
			CanvasTimer canvasTimer2 = canvasTimer;
			_newTimersDuringUpdate.Add(canvasTimer2);
			return canvasTimer2;
		}

		public CanvasTimer CreateInfiniteTimer(float m_triggerCD = 0f)
		{
			CanvasTimer canvasTimer = new CanvasTimer();
			canvasTimer.infiniteTimeSpan = true;
			canvasTimer.triggerCD = m_triggerCD;
			CanvasTimer canvasTimer2 = canvasTimer;
			_newTimersDuringUpdate.Add(canvasTimer2);
			return canvasTimer2;
		}

		private void UpdateForGMTalk()
		{
			if (Input.touchCount == 4)
			{
				if (_GMTalkDialogContext == null)
				{
					_GMTalkDialogContext = new GMTalkDialogContext();
				}
				if (_GMTalkDialogContext.view == null)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(_GMTalkDialogContext);
				}
			}
		}

		private void UpdateEscapeListener()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Singleton<AccountManager>.Instance.manager.ShowExitUI();
			}
		}

		public virtual GameObject GetSpaceShipObj()
		{
			return null;
		}

		public virtual void ClearAllWidgetContext()
		{
		}

		public void ShowQuitGameDialog()
		{
			if (_quitGameDialogContext == null)
			{
				_quitGameDialogContext = new GeneralConfirmDialogContext
				{
					type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_EscQuitGame"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							Singleton<AccountManager>.Instance.manager.DoExit();
						}
						else
						{
							Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnQuitGameDialogDestroy));
						}
					}
				};
			}
			if (_quitGameDialogContext.view == null)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(_quitGameDialogContext);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnQuitGameDialogShow));
			}
		}
	}
}
