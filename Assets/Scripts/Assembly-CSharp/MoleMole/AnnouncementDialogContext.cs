using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class AnnouncementDialogContext : BaseWidgetContext
	{
		private string _announcement;

		public AnnouncementDialogContext(string announcement)
		{
			config = new ContextPattern
			{
				contextName = "AnnouncementDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AnnouncementDialog"
			};
			uiType = UIType.SuspendBar;
			_announcement = announcement;
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Announcement").GetComponent<Text>().text = _announcement;
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/BG"), EventTriggerType.PointerClick, OnBGClick);
		}

		private void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}
	}
}
