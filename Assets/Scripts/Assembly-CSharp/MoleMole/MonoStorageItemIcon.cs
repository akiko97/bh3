using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStorageItemIcon : MonoBehaviour
	{
		public StorageDataItemBase _data;

		private int _loadPosition;

		private Type _type;

		private bool _setupAlready;

		private Color _originColor;

		private StorageItemAction _storageItemProcess;

		private void Start()
		{
			if (!_setupAlready)
			{
				ClearContent();
			}
		}

		public void SetupView(StorageDataItemBase data, Transform parentTrans, StorageItemAction action, int loadPosition, Type type, bool interactable = true)
		{
			_originColor = base.transform.GetComponent<Image>().color;
			_data = data;
			_storageItemProcess = action;
			_type = type;
			_loadPosition = loadPosition;
			base.transform.SetParent(parentTrans, false);
			if (_data == null)
			{
				RealClearContent();
				return;
			}
			base.transform.Find("Image").gameObject.SetActive(true);
			GameObject gameObject = Miscs.LoadResource<GameObject>(data.GetIconPath());
			base.transform.Find("Image").GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			if (_data is MaterialDataItem)
			{
				base.transform.Find("LevelAndCost").gameObject.SetActive(false);
				base.transform.Find("Number").gameObject.SetActive(true);
				base.transform.Find("Number/Number").GetComponent<Text>().text = _data.number.ToString();
			}
			else
			{
				base.transform.Find("LevelAndCost").gameObject.SetActive(true);
				base.transform.Find("Number").gameObject.SetActive(false);
				base.transform.Find("LevelAndCost/LevelNumber").GetComponent<Text>().text = _data.level.ToString();
				base.transform.Find("LevelAndCost/CostNumber").GetComponent<Text>().text = _data.GetCost().ToString();
			}
			base.transform.GetComponent<Button>().interactable = interactable;
			_setupAlready = true;
		}

		private void RealClearContent()
		{
			base.transform.Find("Image").gameObject.SetActive(false);
			base.transform.Find("LevelAndCost").gameObject.SetActive(false);
			base.transform.Find("Number").gameObject.SetActive(false);
			ResetColor();
		}

		public void ClearContent()
		{
			if (_data != null)
			{
				RealClearContent();
				_data = null;
			}
		}

		public void ResetColor()
		{
			base.transform.GetComponent<Image>().color = _originColor;
		}

		public void ProcessLoadedItem()
		{
			if (_data.avatarID != -1 && Singleton<AvatarModule>.Instance.GetAvatarByID(_data.avatarID) != null)
			{
				base.transform.GetComponent<Image>().color = Color.red;
			}
		}

		public void ProcessSelectedItem()
		{
			base.transform.GetComponent<Image>().color = Color.green;
		}

		public bool ProcessEvoMaterial()
		{
			if (_data == null)
			{
				RealClearContent();
				return true;
			}
			MaterialDataItem materialDataItem = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(_data.GetType(), _data.ID) as MaterialDataItem;
			if (materialDataItem == null || materialDataItem.number < _data.number)
			{
				base.transform.GetComponent<Image>().color = Color.gray;
				base.transform.Find("Image").GetComponent<Image>().color = Color.gray;
				return false;
			}
			return true;
		}

		public void ProcessEquipmentCost(int avatarCostLeft)
		{
			if (avatarCostLeft < _data.GetCost())
			{
				base.transform.GetComponent<Image>().color = Color.gray;
				base.transform.Find("Image").GetComponent<Image>().color = Color.gray;
				base.transform.GetComponent<Button>().interactable = false;
			}
		}

		public void SetupViewWithExtraInfo(StorageDataItemBase data, Transform parentTrans, StorageItemAction action, int loadPosition, Type type, bool isAlreadyLoaded, bool interactable)
		{
			SetupView(data, parentTrans, action, loadPosition, type, interactable);
			ProcessLoadedItem();
		}

		public void OnClick()
		{
			if (_storageItemProcess != null)
			{
				_storageItemProcess(_data, base.transform, _loadPosition, _type);
			}
		}
	}
}
