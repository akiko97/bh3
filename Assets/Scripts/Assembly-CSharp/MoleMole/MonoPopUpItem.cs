using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoPopUpItem : MonoBehaviour
	{
		public delegate void ClickCallBack(string name, int index);

		public string itemName;

		public int itemIndex;

		private ClickCallBack _clickCallBack;

		public void SetupView(string name, int index, ClickCallBack callBack = null)
		{
			itemName = name;
			itemIndex = index;
			_clickCallBack = callBack;
			base.transform.Find("Text").GetComponent<Text>().text = itemName;
		}

		public void OnClick()
		{
			if (_clickCallBack != null)
			{
				_clickCallBack(itemName, itemIndex);
			}
		}
	}
}
