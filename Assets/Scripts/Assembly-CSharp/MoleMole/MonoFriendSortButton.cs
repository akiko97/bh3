using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Button))]
	public class MonoFriendSortButton : MonoBehaviour
	{
		public FriendModule.FriendSortType sortTypeAsc;

		public FriendModule.FriendSortType sortTypeDesc;

		public FriendModule.FriendSortType sortTypeDefault;

		private string _currentTabKey;

		public void OnClick()
		{
			FriendModule.FriendSortType friendSortType = Singleton<FriendModule>.Instance.sortTypeMap[_currentTabKey];
			FriendModule.FriendSortType friendSortType2 = friendSortType;
			friendSortType2 = ((sortTypeAsc == friendSortType) ? sortTypeDesc : ((sortTypeDesc != friendSortType) ? sortTypeDefault : sortTypeAsc));
			Singleton<FriendModule>.Instance.sortTypeMap[_currentTabKey] = friendSortType2;
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetFriendSortType, friendSortType2));
		}

		public void SetupView(string currentTabKey)
		{
			_currentTabKey = currentTabKey;
			FriendModule.FriendSortType friendSortType = Singleton<FriendModule>.Instance.sortTypeMap[_currentTabKey];
			Image component = base.transform.GetComponent<Image>();
			bool flag = (component.enabled = sortTypeAsc == friendSortType || sortTypeDesc == friendSortType);
			component.color = ((!flag) ? Color.white : MiscData.GetColor("Yellow"));
			base.transform.Find("Text").GetComponent<Text>().color = ((!flag) ? Color.white : MiscData.GetColor("Black"));
			base.transform.Find("Order").gameObject.SetActive(flag);
			base.transform.Find("Order/UpImg").gameObject.SetActive(sortTypeAsc == friendSortType);
			base.transform.Find("Order/DownImg").gameObject.SetActive(sortTypeDesc == friendSortType);
		}
	}
}
