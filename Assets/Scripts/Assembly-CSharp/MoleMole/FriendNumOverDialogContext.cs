using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class FriendNumOverDialogContext : BaseSequenceDialogContext
	{
		public FriendNumOverDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "FriendNumOverDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/FriendNumOverDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), OnGoBtnClick);
		}

		protected override bool SetupView()
		{
			return false;
		}

		private void OnGoBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext());
			Destroy();
		}

		private void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		private void Close()
		{
			Destroy();
		}
	}
}
