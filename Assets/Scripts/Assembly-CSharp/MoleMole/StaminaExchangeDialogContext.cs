using System;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class StaminaExchangeDialogContext : BaseDialogContext
	{
		private CanvasTimer _timer;

		private Text _nextRecoverTimeText;

		private string _descText;

		public StaminaExchangeDialogContext(string desc)
		{
			config = new ContextPattern
			{
				contextName = "StaminaExchangeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/StaminaExchangeDialog"
			};
			_descText = desc;
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
			PlayerStaminaExchangeInfo value = Singleton<PlayerModule>.Instance.playerData.staminaExchangeCache.Value;
			_nextRecoverTimeText = base.view.transform.Find("Dialog/Content/StaminaInfo/RecoverTimeText").GetComponent<Text>();
			SetupStaminaInfo();
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_descText, value.usableTimes);
			base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = value.hcoinCost.ToString();
			base.view.transform.Find("Dialog/Content/Exchange/StaminaNumText").GetComponent<Text>().text = value.staminaGet.ToString();
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
			_timer.timeTriggerCallback = SetupStaminaInfo;
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestStaminaExchange();
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

		private void SetupStaminaInfo()
		{
			PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
			base.view.transform.Find("Dialog/Content/StaminaInfo/NumText").GetComponent<Text>().text = playerData.stamina + "/" + playerData.MaxStamina;
			if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull())
			{
				DateTime now = TimeUtil.Now;
				TimeSpan timeSpan = Singleton<PlayerModule>.Instance.playerData.nextStaminaRecoverDatetime.Subtract(now);
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
