using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class ClearStorageHintDialog : BaseDialogContext
	{
		private const float TIMER_SPAN = 1.5f;

		public ClearStorageHintDialog(float timerSpan = 1.5f)
		{
			config = new ContextPattern
			{
				contextName = "ClearStorageHintDialog",
				viewPrefabPath = "UI/Menus/Dialog/ClearStorageHintDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Destroy);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/ReturnBtn").GetComponent<Button>(), Destroy);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/ClearBtn").GetComponent<Button>(), OnClearBtnClick);
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		private void OnClearBtnClick()
		{
			Destroy();
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext());
		}
	}
}
