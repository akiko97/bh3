using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class SearchedFriendDetailDialogContext : BaseDialogContext
	{
		public readonly FriendDetailDataItem playerInfo;

		public readonly bool hideActionBtns;

		public SearchedFriendDetailDialogContext(FriendDetailDataItem playerInfo, bool hideActionBtns = false)
		{
			config = new ContextPattern
			{
				contextName = "SearchedFriendDetailDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/SearchedFriendInfoShowDialog",
				ignoreNotify = true
			};
			this.playerInfo = playerInfo;
			this.hideActionBtns = hideActionBtns;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").GetComponent<Button>(), OnAddFriendBtnCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/CheckInfoBtn").GetComponent<Button>(), OnDetailBtnClick);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			bool flag = Singleton<FriendModule>.Instance.IsMyFriend(playerInfo.uid);
			base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").gameObject.SetActive(!flag);
			base.view.transform.Find("Dialog/Content/IsFriendMark").gameObject.SetActive(flag);
			base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").gameObject.SetActive(!Singleton<FriendModule>.Instance.IsMyFriend(playerInfo.uid));
			base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(playerInfo.leaderAvatar.IconPath);
			base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = playerInfo.nickName;
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = playerInfo.Desc;
			return false;
		}

		public void OnAddFriendBtnCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestAddFriend(playerInfo.uid);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", Singleton<FriendModule>.Instance.TryGetPlayerNickName(playerInfo.uid))));
			Close();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}

		private void OnDetailBtnClick()
		{
			RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(playerInfo);
			Singleton<MainUIManager>.Instance.ShowPage(context);
			Close();
		}
	}
}
