using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoScrollViewWithDynamicLoading : MonoBehaviour
	{
		private List<IScrollViewItem> _storageItemList;

		private int _createdStorageItemNum;

		private GridLayoutGroup _grid;

		private int _max_storage_item_num_shown;

		private Transform _contentTrans;

		private Vector2 _previousPoint;

		private RectTransform _maskRectTrans;

		private float _timer;

		private float _loadPeriod = 2f;

		private GridLayoutGroup.Constraint _gridConstraint;

		private CreateGameObjecDelegate CreateGameObjectAndSetupView;

		public void SetCreateDelegate(CreateGameObjecDelegate action)
		{
			CreateGameObjectAndSetupView = action;
		}

		private void Init()
		{
			_contentTrans = base.transform.Find("Content");
			_grid = _contentTrans.GetComponent<GridLayoutGroup>();
			_maskRectTrans = base.transform as RectTransform;
			_createdStorageItemNum = 0;
			_previousPoint = new Vector2(0f, 1f);
		}

		public void SetStorageItemList(List<IScrollViewItem> storageItemList, GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.FixedColumnCount)
		{
			Init();
			foreach (Transform contentTran in _contentTrans)
			{
				Object.Destroy(contentTran.gameObject);
			}
			Vector2 cellSize = _grid.cellSize;
			RectTransform rectTransform = base.transform as RectTransform;
			_grid.constraint = constraint;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			_gridConstraint = constraint;
			switch (constraint)
			{
			case GridLayoutGroup.Constraint.FixedColumnCount:
				num = rectTransform.rect.width;
				num3 = rectTransform.rect.height;
				num2 = cellSize.x + _grid.spacing.x;
				num4 = cellSize.y + _grid.spacing.y;
				break;
			case GridLayoutGroup.Constraint.FixedRowCount:
				num = rectTransform.rect.height;
				num3 = rectTransform.rect.width;
				num2 = cellSize.y + _grid.spacing.y;
				num4 = cellSize.x + _grid.spacing.x;
				break;
			}
			_grid.constraintCount = Mathf.FloorToInt(num / num2);
			_storageItemList = storageItemList;
			_createdStorageItemNum = 0;
			_max_storage_item_num_shown = Mathf.FloorToInt(num3 / num4 + 2f) * _grid.constraintCount;
			LoadStorageItemFromSpecifiedIndex(0);
		}

		private void LoadStorageItemFromSpecifiedIndex(int beginIndex)
		{
			int num = 0;
			int num2 = beginIndex;
			while (num2 < _storageItemList.Count && num < _max_storage_item_num_shown)
			{
				CreateGameObjectAndSetupView(_storageItemList[num2]).transform.SetParent(_contentTrans, false);
				num2++;
				num++;
				_createdStorageItemNum++;
			}
		}

		private void Update()
		{
			if (_storageItemList != null && _createdStorageItemNum < _storageItemList.Count)
			{
				_timer += Time.deltaTime;
				if (_timer > _loadPeriod)
				{
					LoadStorageItemFromSpecifiedIndex(_createdStorageItemNum);
					_timer = 0f;
				}
			}
		}

		public void OnScrollView(Vector2 point)
		{
			if (Mathf.Approximately(Vector2.Distance(_previousPoint, point), 0f))
			{
				_previousPoint = point;
				return;
			}
			if (_createdStorageItemNum < _storageItemList.Count)
			{
				if (_gridConstraint == GridLayoutGroup.Constraint.FixedColumnCount && point.y < 0.15f)
				{
					LoadStorageItemFromSpecifiedIndex(_createdStorageItemNum);
				}
				if (_gridConstraint == GridLayoutGroup.Constraint.FixedRowCount && point.x > 0.85f)
				{
					LoadStorageItemFromSpecifiedIndex(_createdStorageItemNum);
				}
			}
			Vector2 position = _maskRectTrans.TransformPoint(_maskRectTrans.rect.position);
			Vector2 size = _maskRectTrans.TransformVector(_maskRectTrans.rect.size);
			Rect rect = new Rect(position, size);
			for (int i = 0; i < _contentTrans.childCount; i++)
			{
				Transform child = _contentTrans.GetChild(i);
				RectTransform rectTransform = child as RectTransform;
				Vector2 position2 = rectTransform.TransformPoint(rectTransform.rect.position);
				Vector2 size2 = rectTransform.TransformVector(rectTransform.rect.size);
				Rect other = new Rect(position2, size2);
				bool active = rect.Overlaps(other, true);
				child.gameObject.SetActive(active);
			}
		}

		public bool isEmpty()
		{
			return _contentTrans == null || _contentTrans.childCount <= 0;
		}

		public void MaskItemsExceptSepcifyList(List<StorageDataItemBase> itemList)
		{
			foreach (Transform contentTran in _contentTrans)
			{
				if (!itemList.Contains(contentTran.GetComponent<MonoStorageItemIcon>()._data))
				{
					contentTran.GetComponent<Button>().interactable = false;
				}
			}
		}

		public void MakeAllItemsInteractable()
		{
			foreach (Transform contentTran in _contentTrans)
			{
				contentTran.GetComponent<Button>().interactable = true;
			}
		}
	}
}
