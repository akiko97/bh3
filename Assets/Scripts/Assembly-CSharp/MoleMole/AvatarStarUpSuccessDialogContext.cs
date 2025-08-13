using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarStarUpSuccessDialogContext : BaseDialogContext
	{
		private AvatarDataItem avatarData;

		public AvatarStarUpSuccessDialogContext(AvatarDataItem avatarData)
		{
			config = new ContextPattern
			{
				contextName = "ChangeNicknameDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarStarUpSuccessDialog",
				ignoreNotify = true
			};
			this.avatarData = avatarData;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			AvatarStarMetaData avatarStarMetaDataByKey = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarData.avatarID, avatarData.star - 1);
			AvatarStarMetaData avatarStarMetaDataByKey2 = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarData.avatarID, avatarData.star);
			base.view.transform.Find("Dialog/Content/HP/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.hpAdd.ToString();
			base.view.transform.Find("Dialog/Content/HP/RatioAfterNumText").GetComponent<Text>().text = avatarStarMetaDataByKey2.hpAdd.ToString();
			base.view.transform.Find("Dialog/Content/HP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.hpBase - avatarStarMetaDataByKey.hpBase + (avatarStarMetaDataByKey2.hpAdd - avatarStarMetaDataByKey.hpAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/SP/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.spAdd.ToString();
			base.view.transform.Find("Dialog/Content/SP/RatioAfterNumText").GetComponent<Text>().text = avatarStarMetaDataByKey2.spAdd.ToString();
			base.view.transform.Find("Dialog/Content/SP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.spBase - avatarStarMetaDataByKey.spBase + (avatarStarMetaDataByKey2.spAdd - avatarStarMetaDataByKey.spAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/ATK/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.atkAdd.ToString();
			base.view.transform.Find("Dialog/Content/ATK/RatioAfterNumText").GetComponent<Text>().text = avatarStarMetaDataByKey2.atkAdd.ToString();
			base.view.transform.Find("Dialog/Content/ATK/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.atkBase - avatarStarMetaDataByKey.atkBase + (avatarStarMetaDataByKey2.atkAdd - avatarStarMetaDataByKey.atkAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/DEF/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.dfsAdd.ToString();
			base.view.transform.Find("Dialog/Content/DEF/RatioAfterNumText").GetComponent<Text>().text = avatarStarMetaDataByKey2.dfsAdd.ToString();
			base.view.transform.Find("Dialog/Content/DEF/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.dfsBase - avatarStarMetaDataByKey.dfsBase + (avatarStarMetaDataByKey2.dfsAdd - avatarStarMetaDataByKey.dfsAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/CRT/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.crtAdd.ToString();
			base.view.transform.Find("Dialog/Content/CRT/RatioAfterNumText").GetComponent<Text>().text = avatarStarMetaDataByKey2.crtAdd.ToString();
			base.view.transform.Find("Dialog/Content/CRT/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.crtBase - avatarStarMetaDataByKey.crtBase + (avatarStarMetaDataByKey2.crtAdd - avatarStarMetaDataByKey.crtAdd) * (float)avatarData.level);
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void Close()
		{
			Destroy();
		}
	}
}
