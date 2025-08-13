using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoFriendChatInfo : MonoBehaviour
	{
		private FriendBriefDataItem _friendBriefData;

		private ChangeTalkingFriend _onChangeTalkingFriendClick;

		public bool hasNewMessage;

		public void SetupView(FriendBriefDataItem friendBriefData, bool hasNewMessage, ChangeTalkingFriend onChangeTalkingFriend = null)
		{
			_friendBriefData = friendBriefData;
			_onChangeTalkingFriendClick = onChangeTalkingFriend;
			this.hasNewMessage = hasNewMessage;
			base.transform.Find("FriendBtn/Image").gameObject.SetActive(hasNewMessage);
		}

		public FriendBriefDataItem GetFriendData()
		{
			return _friendBriefData;
		}

		public void SetNewMessageTipShow(bool show)
		{
			base.transform.Find("FriendBtn/Image").gameObject.SetActive(show);
			hasNewMessage = show;
		}

		public void RefreshNickName()
		{
			Text component = base.transform.Find("FriendBtn/Name").GetComponent<Text>();
			if (component != null && component.text != _friendBriefData.nickName)
			{
				component.text = _friendBriefData.nickName;
			}
		}

		public void OnChangeTalkingFriendClick()
		{
			_onChangeTalkingFriendClick(_friendBriefData);
		}
	}
}
