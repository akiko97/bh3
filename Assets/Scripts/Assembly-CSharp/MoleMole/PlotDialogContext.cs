using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class PlotDialogContext : BaseDialogContext
	{
		public PlotDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "PlotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/PlotDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("SkipButton").GetComponent<Button>(), Destroy);
			BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), ButtonClickTest);
		}

		protected override bool SetupView()
		{
			base.view.transform.gameObject.SetActive(true);
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		public void ButtonClickTest()
		{
		}
	}
}
