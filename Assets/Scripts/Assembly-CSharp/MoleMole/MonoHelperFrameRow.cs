using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoHelperFrameRow : MonoBehaviour
	{
		private FriendBriefDataItem _friendData;

		private bool _isFrozen;

		private DateTime _nextAvaliableTime;

		private Action<FriendBriefDataItem> _onFrameClick;

		private Action<FriendBriefDataItem> _onIconClick;

		public void SetupView(FriendBriefDataItem friendData, bool selected, Action<FriendBriefDataItem> onFrameClick = null, Action<FriendBriefDataItem> onIconClick = null)
		{
			_friendData = friendData;
			bool flag = Singleton<FriendModule>.Instance.IsMyFriend(_friendData.uid);
			_isFrozen = Singleton<FriendModule>.Instance.isHelperFrozen(friendData.uid);
			if (_isFrozen)
			{
				_nextAvaliableTime = Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(friendData.uid);
			}
			base.transform.Find("BG/Select").gameObject.SetActive(selected);
			base.transform.Find("BG/Normal").gameObject.SetActive(!selected && !_isFrozen);
			base.transform.Find("Grey").gameObject.SetActive(_isFrozen);
			base.transform.Find("PlayerName").GetComponent<Text>().text = friendData.nickName;
			base.transform.Find("FriendMark/Friend").gameObject.SetActive(flag);
			base.transform.Find("FriendMark/Strange").gameObject.SetActive(!flag);
			base.transform.Find("AvatarStar").GetComponent<MonoAvatarStar>().SetupView(friendData.avatarStar);
			base.transform.Find("Lv").GetComponent<Text>().text = "LV." + friendData.level;
			base.transform.Find("SelectMark").gameObject.SetActive(selected);
			base.transform.Find("PlayerIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_friendData.AvatarIconPath);
			int addFP = GetAddFP(flag);
			base.transform.Find("FriendPoint/Value").GetComponent<Text>().text = addFP.ToString();
			base.transform.Find("Btns/FrameBtn").gameObject.SetActive(!_isFrozen);
			base.transform.Find("FriendPoint").gameObject.SetActive(!_isFrozen);
			base.transform.Find("RemainTimer").gameObject.SetActive(_isFrozen);
			if (_isFrozen)
			{
				base.transform.Find("RemainTimer/Time").GetComponent<MonoRemainTimer>().SetTargetTime(_nextAvaliableTime, null, OnFrozenTimeOutCallBack);
			}
			_onFrameClick = onFrameClick;
			_onIconClick = onIconClick;
		}

		public void OnFrameClick()
		{
			if (_onFrameClick != null)
			{
				_onFrameClick(_friendData);
			}
		}

		public void OnIconClick()
		{
			if (_onIconClick != null)
			{
				_onIconClick(_friendData);
			}
		}

		public int GetAddFP(bool isFriend)
		{
			return (!isFriend) ? MiscData.Config.BasicConfig.StrangeAddFriendPoint : MiscData.Config.BasicConfig.FriendAddFriendPoint;
		}

		private void OnFrozenTimeOutCallBack()
		{
			_isFrozen = false;
			base.transform.Find("BG/Select").gameObject.SetActive(false);
			base.transform.Find("BG/Normal").gameObject.SetActive(true);
			base.transform.Find("Grey").gameObject.SetActive(false);
			base.transform.Find("Btns/FrameBtn").gameObject.SetActive(!_isFrozen);
			base.transform.Find("FriendPoint").gameObject.SetActive(!_isFrozen);
			base.transform.Find("RemainTimer").gameObject.SetActive(_isFrozen);
		}
	}
}
