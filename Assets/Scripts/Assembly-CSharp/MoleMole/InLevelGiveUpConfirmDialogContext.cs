using System;
using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class InLevelGiveUpConfirmDialogContext : BaseDialogContext
	{
		private BaseDialogContext _fromDialogContext;

		private Action _giveUpCallBack;

		public InLevelGiveUpConfirmDialogContext(BaseDialogContext fromDialogContext, Action giveUpCallBack = null)
		{
			config = new ContextPattern
			{
				contextName = "InLevelGiveUpConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/InLevelGiveUpConfirmDialog"
			};
			_fromDialogContext = fromDialogContext;
			_giveUpCallBack = giveUpCallBack;
		}

		protected override bool SetupView()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			_fromDialogContext.view.SetActive(false);
			if ((int)Singleton<LevelScoreManager>.Instance.LevelType == 4)
			{
				base.view.transform.Find("Dialog/Content/InfoPanel/Hint").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessGiveUp");
			}
			else
			{
				base.view.transform.Find("Dialog/Content/InfoPanel/Hint").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_InLevelGiveUpHint");
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/Think").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), OnGiveUpBtnClick);
		}

		private void OnBGBtnClick()
		{
			_fromDialogContext.view.SetActive(true);
			Destroy();
		}

		private void OnGiveUpBtnClick()
		{
			Destroy();
			if (_giveUpCallBack != null)
			{
				_giveUpCallBack();
			}
		}
	}
}
