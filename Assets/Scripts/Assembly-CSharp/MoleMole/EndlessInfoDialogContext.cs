using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class EndlessInfoDialogContext : BaseDialogContext
	{
		public EndlessInfoDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "EndlessInfoDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/EndlessInfoDialog"
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), Destroy);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Destroy);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/ScrollView/Viewport/Content/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessInfo");
			return false;
		}
	}
}
