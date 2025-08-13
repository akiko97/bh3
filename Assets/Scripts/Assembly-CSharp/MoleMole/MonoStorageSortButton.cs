using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Button))]
	public class MonoStorageSortButton : MonoBehaviour
	{
		public StorageModule.StorageSortType sortTypeAsc;

		public StorageModule.StorageSortType sortTypeDesc;

		public StorageModule.StorageSortType sortTypeDefault;

		private string _currentTabKey;

		public void OnClick()
		{
			StorageModule.StorageSortType storageSortType = Singleton<StorageModule>.Instance.sortTypeMap[_currentTabKey];
			StorageModule.StorageSortType storageSortType2 = storageSortType;
			storageSortType2 = ((sortTypeAsc == storageSortType) ? sortTypeDesc : ((sortTypeDesc != storageSortType) ? sortTypeDefault : sortTypeAsc));
			Singleton<StorageModule>.Instance.sortTypeMap[_currentTabKey] = storageSortType2;
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetStorageSortType, storageSortType2));
		}

		public void SetupView(string currentTabKey)
		{
			_currentTabKey = currentTabKey;
			StorageModule.StorageSortType storageSortType = Singleton<StorageModule>.Instance.sortTypeMap[_currentTabKey];
			Image component = base.transform.GetComponent<Image>();
			bool flag = (component.enabled = sortTypeAsc == storageSortType || sortTypeDesc == storageSortType);
			component.color = ((!flag) ? Color.white : MiscData.GetColor("Yellow"));
			base.transform.Find("Text").GetComponent<Text>().color = ((!flag) ? Color.white : MiscData.GetColor("Black"));
			base.transform.Find("Order").gameObject.SetActive(flag);
			base.transform.Find("Order/UpImg").gameObject.SetActive(sortTypeAsc == storageSortType);
			base.transform.Find("Order/DownImg").gameObject.SetActive(sortTypeDesc == storageSortType);
		}
	}
}
