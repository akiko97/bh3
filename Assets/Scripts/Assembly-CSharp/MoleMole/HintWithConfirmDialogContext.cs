using System;
using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class HintWithConfirmDialogContext : BaseSequenceDialogContext
	{
		public enum ButtonType
		{
			SingleButton = 0,
			DoubleButton = 1
		}

		private string _title;

		private string _desc;

		private string _okBtnText;

		private string _cancelBtnText;

		private Action _singleButtonCallBack;

		private Action<bool> _doubleButtonCallBack;

		private ButtonType _type;

		public HintWithConfirmDialogContext(string desc, string okBtnText, Action buttonCallBack, string title)
		{
			config = new ContextPattern
			{
				contextName = "HintWithConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/HintWithConfirmDialog"
			};
			_title = title;
			_desc = desc;
			_okBtnText = okBtnText;
			_singleButtonCallBack = buttonCallBack;
			_type = ButtonType.SingleButton;
		}

		public HintWithConfirmDialogContext(string desc, string okBtnText, string cancelBtnText, Action<bool> buttonCallBack, string title)
		{
			config = new ContextPattern
			{
				contextName = "HintWithConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/HintWithConfirmDialog"
			};
			_title = title;
			_desc = desc;
			_okBtnText = okBtnText;
			_cancelBtnText = cancelBtnText;
			_doubleButtonCallBack = buttonCallBack;
			_type = ButtonType.DoubleButton;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").GetComponent<Button>(), OnCancelButtonCallBack);
		}

		protected override bool SetupView()
		{
			string text = ((!string.IsNullOrEmpty(_okBtnText)) ? _okBtnText : LocalizationGeneralLogic.GetText("Menu_OK"));
			string text2 = ((!string.IsNullOrEmpty(_cancelBtnText)) ? _cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel"));
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK/Text").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel/Text").GetComponent<Text>().text = text2;
			base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = _title;
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = _desc;
			base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").gameObject.SetActive(_type == ButtonType.DoubleButton);
			return false;
		}

		public void OnOKButtonCallBack()
		{
			if (_type == ButtonType.SingleButton)
			{
				Destroy();
				if (_singleButtonCallBack != null)
				{
					_singleButtonCallBack();
				}
			}
			else
			{
				OnDoubleButtonClick(true);
			}
		}

		public void OnCancelButtonCallBack()
		{
			OnDoubleButtonClick(false);
		}

		private void OnDoubleButtonClick(bool isOk)
		{
			Destroy();
			if (_doubleButtonCallBack != null)
			{
				_doubleButtonCallBack(isOk);
			}
		}
	}
}
