using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class ResourceDetailDialogContext : BaseDialogContext
	{
		public readonly RewardUIData resourceData;

		public ResourceDetailDialogContext(RewardUIData resourceData)
		{
			config = new ContextPattern
			{
				contextName = "ResourceDetailDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ItemDetailDialog",
				ignoreNotify = true
			};
			this.resourceData = resourceData;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(false);
			SetupRarityView();
			base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = resourceData.GetImageSprite();
			Transform transform = base.view.transform.Find("Dialog/Content/Star/EquipStar");
			transform.gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(resourceData.nameTextID);
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = GetAllDesc();
			base.view.transform.Find("Dialog/Content/Num").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/Num/Text").GetComponent<Text>().text = resourceData.value.ToString();
			base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(false);
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}

		private void SetupRarityView()
		{
			base.view.transform.Find("Dialog/Content/Icon").GetComponent<Image>().color = ((resourceData.rewardType != ResourceType.Hcoin) ? MiscData.GetColor("ResourceBlue") : MiscData.GetColor("ResourcePurple"));
		}

		private string GetAllDesc()
		{
			return UIUtil.ProcessStrWithNewLine(LocalizationGeneralLogic.GetText(resourceData.descTextID));
		}
	}
}
