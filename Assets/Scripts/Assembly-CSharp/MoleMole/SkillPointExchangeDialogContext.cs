using System;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class SkillPointExchangeDialogContext : BaseDialogContext
	{
		private CanvasTimer _timer;

		private Text _nextRecoverTimeText;

		public SkillPointExchangeDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "StaminaExchangeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SkillPointExchangeDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			PlayerSkillPointExchangeInfo value = Singleton<PlayerModule>.Instance.playerData.skillPointExchangeCache.Value;
			_nextRecoverTimeText = base.view.transform.Find("Dialog/Content/SkillPtInfo/RecoverTimeText").GetComponent<Text>();
			SetupSkillPtInfo();
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_SkillPtExchange", value.usableTimes);
			base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = value.hcoinCost.ToString();
			base.view.transform.Find("Dialog/Content/Exchange/SkillPtNumText").GetComponent<Text>().text = value.skillPointGet.ToString();
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
			_timer.timeTriggerCallback = SetupSkillPtInfo;
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestSkillPointExchange();
			DestroyTimerAndSelf();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			DestroyTimerAndSelf();
		}

		public void Close()
		{
			DestroyTimerAndSelf();
		}

		private void SetupSkillPtInfo()
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			base.view.transform.Find("Dialog/Content/SkillPtInfo/NumText").GetComponent<Text>().text = playerData.skillPoint + "/" + playerData.skillPointLimit;
			if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull())
			{
				DateTime now = TimeUtil.Now;
				TimeSpan timeSpan = Singleton<PlayerModule>.Instance.playerData.nextSkillPtRecoverDatetime.Subtract(now);
				if (timeSpan.TotalSeconds > 0.0)
				{
					_nextRecoverTimeText.gameObject.SetActive(true);
					_nextRecoverTimeText.text = string.Format("( {0:D2} : {1:D2} )", timeSpan.Minutes, timeSpan.Seconds);
				}
			}
			else
			{
				_nextRecoverTimeText.gameObject.SetActive(false);
			}
		}

		private void DestroyTimerAndSelf()
		{
			if (_timer != null)
			{
				_timer.Destroy();
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
