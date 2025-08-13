using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoCgIconButton : MonoBehaviour
	{
		public delegate void ClickCallBack(CgDataItem item);

		private ClickCallBack _clickCallBack;

		public CgDataItem _item;

		public void SetupView(CgDataItem item)
		{
			_item = item;
			bool flag = IsLocked();
			base.transform.Find("Lock").gameObject.SetActive(flag);
			base.transform.Find("Image").gameObject.SetActive(!flag);
			if (!string.IsNullOrEmpty(_item.cgIconPath) && !flag)
			{
				string prefabPath = string.Format("SpriteOutput/CGReplay/{0}", _item.cgIconPath);
				base.transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
			}
		}

		public void OnClick()
		{
			if (_clickCallBack != null && !IsLocked())
			{
				_clickCallBack(_item);
			}
		}

		public void SetClickCallback(ClickCallBack callback)
		{
			_clickCallBack = callback;
		}

		private bool IsLocked()
		{
			return !Singleton<CGModule>.Instance.IsCGFinished(_item.cgID);
		}
	}
}
