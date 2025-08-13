using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoFriendInfo : MonoBehaviour
	{
		private FriendBriefDataItem _friendBriefData;

		private AvatarDataItem _friendAvatarData;

		private FriendOverviewPageContext.FriendTab _friendTab;

		private RequestCallBack _onRequestBtnClick;

		private AcceptCallBack _onAcceptBtnClick;

		private RejectCallBack _onRejectBtnClick;

		private DetailCallBack _onDetailBtnClick;

		public void SetupView(FriendBriefDataItem friendBriefData, FriendOverviewPageContext.FriendTab friendTab, RequestCallBack onRequest = null, AcceptCallBack onAccept = null, RejectCallBack onReject = null, DetailCallBack onDetailBtnClick = null)
		{
			_friendBriefData = friendBriefData;
			_friendTab = friendTab;
			_onRequestBtnClick = onRequest;
			_onAcceptBtnClick = onAccept;
			_onRejectBtnClick = onReject;
			_onDetailBtnClick = onDetailBtnClick;
			_friendAvatarData = new AvatarDataItem(friendBriefData.showAvatarID);
			base.transform.Find("AvatarImage/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_friendAvatarData.IconPath);
			base.transform.Find("AvatarImage/BGColor").GetComponent<Image>().sprite = GetBGSprite();
			base.transform.Find("Nickname").GetComponent<Text>().text = friendBriefData.nickName;
			base.transform.Find("Lv/Num").GetComponent<Text>().text = friendBriefData.level.ToString();
			base.transform.Find("AvatarInfo/Combat/Num").GetComponent<Text>().text = friendBriefData.avatarCombat.ToString();
			base.transform.Find("AvatarInfo/AvatarStar/Star").GetComponent<MonoAvatarStar>().SetupView(friendBriefData.avatarStar);
			base.transform.Find("AvatarImage/NewMark").gameObject.SetActive(friendTab == FriendOverviewPageContext.FriendTab.FriendListTab && !Singleton<FriendModule>.Instance.IsOldFriend(friendBriefData.uid));
			bool flag = _friendTab == FriendOverviewPageContext.FriendTab.AddFriendTab;
			bool flag2 = _friendTab == FriendOverviewPageContext.FriendTab.RequestListTab;
			bool flag3 = _friendTab == FriendOverviewPageContext.FriendTab.FriendListTab;
			bool active = flag || flag2 || flag3;
			base.transform.Find("ActionBtns/TalkBtn").gameObject.SetActive(false);
			base.transform.Find("ActionBtns/AddFriendBtn").gameObject.SetActive(false);
			base.transform.Find("ActionBtns/ReplyBtns").gameObject.SetActive(false);
			base.transform.Find("ActionBtns/TalkBtn").gameObject.SetActive(flag3);
			base.transform.Find("ActionBtns/AddFriendBtn").gameObject.SetActive(flag);
			base.transform.Find("ActionBtns/ReplyBtns").gameObject.SetActive(flag2);
			base.transform.Find("ActionBtns").gameObject.SetActive(active);
		}

		public void OnRequestBtnClick()
		{
			if (_onRequestBtnClick != null)
			{
				_onRequestBtnClick(_friendBriefData);
			}
		}

		public void OnAcceptBtnClick()
		{
			if (_onAcceptBtnClick != null)
			{
				_onAcceptBtnClick(_friendBriefData);
			}
		}

		public void OnRejectBtnClick()
		{
			if (_onRejectBtnClick != null)
			{
				_onRejectBtnClick(_friendBriefData);
			}
		}

		public void OnDetailBtnClick()
		{
			if (_onDetailBtnClick != null)
			{
				_onDetailBtnClick(_friendBriefData);
			}
		}

		public void OnTalkBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext(_friendBriefData.uid));
		}

		public int GetFriendUID()
		{
			return _friendBriefData.uid;
		}

		private Sprite GetBGSprite()
		{
			switch ((EntityNature)_friendAvatarData.Attribute)
			{
			case EntityNature.Mechanic:
				return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_jixie);
			case EntityNature.Biology:
				return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_shengwu);
			case EntityNature.Psycho:
				return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_yineng);
			default:
				return null;
			}
		}
	}
}
