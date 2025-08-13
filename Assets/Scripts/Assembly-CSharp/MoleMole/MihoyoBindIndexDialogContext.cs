using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MihoyoBindIndexDialogContext : BaseDialogContext
	{
		public MihoyoBindIndexDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "MihoyoBindIndexDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoBindIndexDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/BindAccountBtn").GetComponent<Button>(), OnBindAccountBtnCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/RegisterBtn").GetComponent<Button>(), OnRegisterBtnCallBack);
		}

		protected override bool SetupView()
		{
			return false;
		}

		public void OnBindAccountBtnCallBack()
		{
			Close();
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoBindAccountDialogContext());
		}

		public void OnRegisterBtnCallBack()
		{
			Close();
			Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoRegisterDialogContext());
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}
	}
}
