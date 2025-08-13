using System;
using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class AgreementDialogContext : BaseSequenceDialogContext
	{
		public Action<bool> buttonCallBack;

		public AgreementDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "AgreementDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/UserAgreementDialog",
				cacheType = ViewCacheType.DontCache
			};
		}

		protected override bool SetupView()
		{
			string text = CommonUtils.LoadTextFileToString("Data/Agreement");
			base.view.transform.Find("Dialog/Content/Scroll View/Viewport/Content/DescText").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>().interactable = false;
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/Toggle").GetComponent<Toggle>(), OnAgreeToggleCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>(), OnAgreeButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/NotAgreeBtn").GetComponent<Button>(), OnNotAgreeButtonCallBack);
		}

		public void OnAgreeToggleCallBack(bool check)
		{
			base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>().interactable = check;
		}

		public void OnAgreeButtonCallBack()
		{
			if (buttonCallBack != null)
			{
				buttonCallBack(true);
			}
			Destroy();
		}

		public void OnNotAgreeButtonCallBack()
		{
			if (buttonCallBack != null)
			{
				buttonCallBack(false);
			}
			Destroy();
		}
	}
}
