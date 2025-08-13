using System;
using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class GeneralConfirmDialogContext : BaseSequenceDialogContext
	{
		public enum ButtonType
		{
			SingleButton = 0,
			DoubleButton = 1
		}

		public string desc;

		public string okBtnText;

		public string cancelBtnText;

		public Action<bool> buttonCallBack;

		public Action destroyCallBack;

		public bool notDestroyAfterCallback;

		public ButtonType type;

		public GeneralConfirmDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "GeneralConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/GeneralConfirmDialog",
				cacheType = ViewCacheType.DontCache
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").GetComponent<Button>(), OnCancelButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK").GetComponent<Button>(), OnOKButtonCallBack);
		}

		protected override bool SetupView()
		{
			string text = ((!string.IsNullOrEmpty(okBtnText)) ? okBtnText : LocalizationGeneralLogic.GetText("Menu_OK"));
			string text2 = ((!string.IsNullOrEmpty(cancelBtnText)) ? cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel"));
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel/Text").GetComponent<Text>().text = text2;
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK/Text").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = desc;
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").gameObject.SetActive(type == ButtonType.DoubleButton);
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
