using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoBulletinTitleButton : MonoBehaviour
	{
		private enum MarkType
		{
			None = 0,
			Hot = 1,
			New = 2
		}

		private Bulletin _bulletinDataItem;

		private ShowBulletinById _onShowBulletinById;

		public void SetupView(Bulletin bulletinDataItem, bool isSelected, ShowBulletinById onShowBulletinById = null)
		{
			_bulletinDataItem = bulletinDataItem;
			_onShowBulletinById = onShowBulletinById;
			base.transform.Find("Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(_bulletinDataItem.title_button);
			base.transform.Find("NewImg").gameObject.SetActive(_bulletinDataItem.mark == 2);
			base.transform.Find("HotImg").gameObject.SetActive(_bulletinDataItem.mark == 1);
			base.transform.GetComponent<Button>().interactable = !isSelected;
		}

		public void OnShowBulletinById()
		{
			if (_onShowBulletinById != null)
			{
				_onShowBulletinById(_bulletinDataItem.id);
			}
		}
	}
}
