using System;
using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class GeneralDialogContext : BaseSequenceDialogContext
	{
		public enum ButtonType
		{
			SingleButton = 0,
			DoubleButton = 1
		}

		public string title;

		public string desc;

		public string okBtnText;

		public string cancelBtnText;

		public Action<bool> buttonCallBack;

		public Action destroyCallBack;

		public bool notDestroyAfterTouchBG;

		public bool notDestroyAfterCallback;

		public bool hideCloseBtn;

		public ButtonType type;

		public GeneralDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "GeneralDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/GeneralDialog",
				cacheType = ViewCacheType.DontCache
			};
		}

		protected override void BindViewCallbacks()
		{
			if (type == ButtonType.SingleButton)
			{
				BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), OnOKButtonCallBack);
			}
			else
			{
				BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), OnCancelButtonCallBack);
				BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnOKButtonCallBack);
			}
			if (!notDestroyAfterTouchBG)
			{
				BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			}
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			if (type == ButtonType.SingleButton)
			{
				string text = ((!string.IsNullOrEmpty(okBtnText)) ? okBtnText : LocalizationGeneralLogic.GetText("Menu_OK"));
				base.view.transform.Find("Dialog/Content/SingleButton/Btn/Text").GetComponent<Text>().text = text;
				base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/DoubleButton").gameObject.SetActive(false);
			}
			else if (type == ButtonType.DoubleButton)
			{
				string text2 = ((!string.IsNullOrEmpty(okBtnText)) ? okBtnText : LocalizationGeneralLogic.GetText("Menu_OK"));
				string text3 = ((!string.IsNullOrEmpty(cancelBtnText)) ? cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel"));
				base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn/Text").GetComponent<Text>().text = text3;
				base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn/Text").GetComponent<Text>().text = text2;
				base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(false);
				base.view.transform.Find("Dialog/Content/DoubleButton").gameObject.SetActive(true);
			}
			base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = title;
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = desc;
			base.view.transform.Find("Dialog/CloseBtn").gameObject.SetActive(!hideCloseBtn);
			return false;
		}

		public void OnOKButtonCallBack()
		{
			OnButtonClick(true);
		}

		public void OnCancelButtonCallBack()
		{
			OnButtonClick(false);
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void Close()
		{
			Destroy();
			if (destroyCallBack != null)
			{
				destroyCallBack();
			}
		}

		private void OnButtonClick(bool isOk)
		{
			if (!notDestroyAfterCallback)
			{
				Destroy();
			}
			if (buttonCallBack != null)
			{
				buttonCallBack(isOk);
			}
		}
	}
}
