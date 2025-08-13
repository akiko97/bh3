using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(ScrollRect))]
	public class MonoGridScroller : MonoBehaviour, IEventSystemHandler, IEndDragHandler
	{
		public enum Movement
		{
			Horizontal = 0,
			Vertical = 1
		}

		public delegate void OnChange(Transform trans, int index);

		public Transform itemPrefab;

		public RectTransform grid;

		public Movement moveType;

		public MonoScrollBarAutoHide scrollBarAutoHide;

		public MonoScrollBar scrollBar;

		public string scrollAudioPatternName = "UI_Gen_Obj_Slide";

		private HashSet<int> _transIndexSet = new HashSet<int>();

		private HashSet<int> _showIndexSet = new HashSet<int>();

		private Dictionary<int, RectTransform> _transDict = new Dictionary<int, RectTransform>();

		private OnChange _onChange;

		private int _itemCount;

		private int _transCount;

		private int _col;

		private int _row;

		private Rect _scrollerRect;

		private Vector2 _cellSize = Vector2.zero;

		private Vector2 _spacing = Vector2.zero;

		private ScrollRect _scroller;

		private Vector2 _padding = Vector2.zero;

		private float _bottom;

		private bool _isFirstRefresh;

		private int _preRefreshIndex;

		private bool _initialized;

		public Vector2 ItemSize
		{
			get
			{
				return _spacing + _cellSize;
			}
		}

		public void Init(OnChange onChange, int itemCount, Vector2? normalizedPosition = null)
		{
			if (_initialized)
			{
				_onChange = onChange;
				if (itemCount > _itemCount)
				{
					AddChildren(itemCount - _itemCount);
				}
				else if (itemCount < _itemCount)
				{
					RemoveChildren(_itemCount - itemCount);
				}
				InitTransform(normalizedPosition);
				RefreshCurrent();
				_itemCount = itemCount;
			}
			else
			{
				_onChange = onChange;
				_itemCount = itemCount;
				Clear();
				InitScroller();
				InitGrid();
				InitChildren();
				InitTransform(normalizedPosition);
			}
			InitScorllBar();
			_isFirstRefresh = true;
			_initialized = true;
			_preRefreshIndex = 0;
		}

		public void RefreshCurrent()
		{
			foreach (int item in _transIndexSet)
			{
				if (_onChange != null)
				{
					_onChange(_transDict[item], item);
				}
			}
		}

		public void RefreshCurrentByIndex(int index)
		{
			if (_transIndexSet.Contains(index) && _onChange != null)
			{
				_onChange(_transDict[index], index);
			}
		}

		public Vector2 GetNormalizedPosition()
		{
			return _scroller.normalizedPosition;
		}

		public void SetNormalizedPosition(Vector2 normalizedPosition)
		{
			_scroller.normalizedPosition = normalizedPosition;
		}

		public void ScrollToPrevPage()
		{
			float delta = ((moveType != Movement.Horizontal) ? _scrollerRect.height : (0f - _scrollerRect.width));
			DoScroll(delta);
		}

		public void ScrollToNextPage()
		{
			float delta = ((moveType != Movement.Horizontal) ? (0f - _scrollerRect.height) : _scrollerRect.width);
			DoScroll(delta);
		}

		public void ScrollToPreItem()
		{
			float delta = ((moveType != Movement.Horizontal) ? _cellSize.y : (0f - _cellSize.x));
			DoScroll(delta);
		}

		public void ScrollToNextItem()
		{
			float delta = ((moveType != Movement.Horizontal) ? (0f - _cellSize.y) : _cellSize.x);
			DoScroll(delta);
		}

		public void ScrollToBegin()
		{
			SetNormalizedPosition(new Vector2(0f, 1f));
		}

		public void ScrollToEnd()
		{
			SetNormalizedPosition(new Vector2(1f, 0f));
		}

		private void DoScroll(float delta)
		{
			Rect rect = _scroller.content.rect;
			if (moveType == Movement.Horizontal)
			{
				float num = rect.width - _scrollerRect.width;
				float num2 = _scroller.horizontalNormalizedPosition * num + delta;
				float horizontalNormalizedPosition = ((num != 0f) ? Mathf.Clamp(num2 / num, 0f, 1f) : 0f);
				_scroller.horizontalNormalizedPosition = horizontalNormalizedPosition;
			}
			else
			{
				float num3 = rect.height - _scrollerRect.height;
				float num4 = _scroller.verticalNormalizedPosition * num3 + delta;
				float verticalNormalizedPosition = ((num3 != 0f) ? Mathf.Clamp(num4 / num3, 0f, 1f) : 0f);
				_scroller.verticalNormalizedPosition = verticalNormalizedPosition;
			}
		}

		public void AddChildren(int addCount)
		{
			_itemCount += addCount;
			int transCount = _transCount;
			_transCount = CalculateTransCount();
			if (_transCount > transCount)
			{
				for (int i = transCount; i < _transCount; i++)
				{
					AddNewChildByIndex(i);
				}
			}
			RefreshIndexSet();
			InitTransform();
		}

		public void RemoveChildren(int removeCount)
		{
			_itemCount -= removeCount;
			int transCount = _transCount;
			int num = CalculateTransCount();
			removeCount = transCount - num;
			int num2 = _transIndexSet.Max();
			for (int num3 = num2; num3 > num2 - removeCount; num3--)
			{
				RemoveChildByIndex(num3);
			}
			RefreshIndexSet();
			InitTransform();
		}

		public Transform GetItemTransByIndex(int index)
		{
			if (_transDict.ContainsKey(index))
			{
				return _transDict[index];
			}
			return null;
		}

		public int GetMaxItemCountWithouScroll()
		{
			return _col * _row;
		}

		private void InitScroller()
		{
			_scroller = GetComponent<ScrollRect>();
			_scroller.onValueChanged.AddListener(OnValueChanged);
			_scrollerRect = _scroller.GetComponent<RectTransform>().rect;
			if (moveType == Movement.Horizontal)
			{
				_scroller.vertical = false;
				_scroller.horizontal = true;
			}
			else
			{
				_scroller.vertical = true;
				_scroller.horizontal = false;
			}
		}

		private void InitGrid()
		{
			GridLayoutGroup component = grid.GetComponent<GridLayoutGroup>();
			_cellSize = component.cellSize;
			_spacing = component.spacing;
			_padding.x = component.padding.left;
			_padding.y = -component.padding.top;
			_bottom = component.padding.bottom;
			grid.GetComponent<GridLayoutGroup>().enabled = false;
		}

		private void InitChildren()
		{
			LayoutElement component = _scroller.GetComponent<LayoutElement>();
			if (component != null)
			{
				_col = Mathf.CeilToInt(component.preferredWidth / (ItemSize.x * grid.localScale.x + _spacing.x));
				_row = Mathf.CeilToInt(component.preferredHeight / (ItemSize.y * grid.localScale.y + _spacing.y));
			}
			else
			{
				_col = Mathf.CeilToInt(_scrollerRect.width / (ItemSize.x * grid.localScale.x + _spacing.x));
				_row = Mathf.CeilToInt(_scrollerRect.height / (ItemSize.y * grid.localScale.y + _spacing.y));
			}
			if (moveType == Movement.Horizontal)
			{
				_row = grid.GetComponent<GridLayoutGroup>().constraintCount;
				_col += 2;
			}
			else
			{
				_col = grid.GetComponent<GridLayoutGroup>().constraintCount;
				_row += 2;
			}
			_transCount = CalculateTransCount();
			for (int i = 0; i < _transCount; i++)
			{
				AddNewChildByIndex(i);
			}
		}

		private void AddNewChildByIndex(int index)
		{
			Transform transform = grid.transform.AddChildFromPrefab(itemPrefab, index.ToString());
			InitChild(transform.GetComponent<RectTransform>(), index);
			_onChange(transform, index);
			_transIndexSet.Add(index);
			_transDict.Add(index, transform.GetComponent<RectTransform>());
		}

		private void RemoveChildByIndex(int i)
		{
			if (_transDict.ContainsKey(i))
			{
				Transform transform = _transDict[i];
				transform.gameObject.SetActive(false);
				Object.Destroy(transform.gameObject);
				_transCount--;
				_transIndexSet.Remove(i);
				_transDict.Remove(i);
			}
		}

		private int CalculateTransCount()
		{
			int num = _col * _row;
			if (num > _itemCount)
			{
				num = _itemCount;
			}
			return num;
		}

		private void InitChild(RectTransform rectTrans, int index)
		{
			rectTrans.anchorMax = new Vector2(0f, 1f);
			rectTrans.anchorMin = new Vector2(0f, 1f);
			rectTrans.sizeDelta = _cellSize;
			rectTrans.anchoredPosition = IndexToPosition(index, rectTrans.pivot);
		}

		private void InitTransform(Vector2? normalizedPosition = null)
		{
			if (moveType == Movement.Horizontal)
			{
				int num = Mathf.CeilToInt((float)_itemCount * 1f / (float)_row);
				grid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cellSize.x * (float)num + _spacing.x * (float)(num - 1) + Mathf.Abs(_padding.x));
			}
			else
			{
				int num2 = Mathf.CeilToInt((float)_itemCount * 1f / (float)_col);
				grid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _cellSize.y * (float)num2 + _spacing.y * (float)(num2 - 1) + Mathf.Abs(_padding.y) + _bottom);
			}
			if (normalizedPosition.HasValue)
			{
				_scroller.normalizedPosition = normalizedPosition.Value;
			}
			OnValueChanged(_scroller.normalizedPosition);
		}

		private void InitScorllBar()
		{
			if (scrollBar == null)
			{
				return;
			}
			LayoutElement component = _scroller.GetComponent<LayoutElement>();
			int num = 0;
			int num2 = 0;
			if (component != null)
			{
				num = Mathf.FloorToInt(component.preferredWidth / (ItemSize.x * grid.localScale.x + _spacing.x));
				num2 = Mathf.FloorToInt(component.preferredHeight / (ItemSize.y * grid.localScale.y + _spacing.y));
			}
			else
			{
				num = Mathf.FloorToInt(_scrollerRect.width / (ItemSize.x * grid.localScale.x + _spacing.x));
				num2 = Mathf.FloorToInt(_scrollerRect.height / (ItemSize.y * grid.localScale.y + _spacing.y));
			}
			bool visible = false;
			if (moveType == Movement.Vertical)
			{
				if (num2 < _itemCount / num)
				{
					visible = true;
				}
			}
			else if (num < _itemCount / num2)
			{
				visible = true;
			}
			scrollBar.SetVisible(visible);
		}

		private void Clear()
		{
			_transIndexSet.Clear();
			_transDict.Clear();
			_showIndexSet.Clear();
			if (grid != null)
			{
				grid.DestroyChildren();
			}
		}

		public void OnValueChanged(Vector2 normalizedPosition)
		{
			ProcessSlideAudio();
			if (_transCount == _itemCount)
			{
				return;
			}
			RefreshIndexSet();
			if (scrollBarAutoHide != null)
			{
				if (scrollBarAutoHide.hidebyDefault)
				{
					float velocity = ((moveType != Movement.Horizontal) ? _scroller.velocity.y : _scroller.velocity.x);
					scrollBarAutoHide.UpdateStatus(velocity);
				}
				else if (moveType == Movement.Horizontal)
				{
					scrollBarAutoHide.UpdateStatus(_itemCount > _col - 2);
				}
				else
				{
					scrollBarAutoHide.UpdateStatus(_itemCount > _row - 2);
				}
			}
		}

		private void RefreshIndexSet()
		{
			int num = 0;
			if (moveType == Movement.Horizontal)
			{
				float num2 = 0f - grid.GetComponent<RectTransform>().anchoredPosition.x;
				int num3 = (int)(num2 / ItemSize.x);
				num = num3 * _row;
			}
			else
			{
				float y = grid.GetComponent<RectTransform>().anchoredPosition.y;
				int num4 = (int)(y / ItemSize.y);
				num = num4 * _col;
			}
			SwapIndex(num);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (_itemCount > GetMaxItemCountWithouScroll() && scrollBarAutoHide != null && scrollBarAutoHide.hidebyDefault)
			{
				float velocity = ((moveType != Movement.Horizontal) ? _scroller.velocity.y : _scroller.velocity.x);
				scrollBarAutoHide.UpdateStatus(velocity);
			}
		}

		private void SwapIndex(int startIndex)
		{
			_showIndexSet.Clear();
			for (int i = 0; i < _transCount; i++)
			{
				int num = i + startIndex;
				if (num >= _itemCount)
				{
					num = startIndex - (num - _itemCount + 1);
				}
				else if (num < 0)
				{
					num += _transCount;
				}
				if (num < 0)
				{
					num = 0;
				}
				_showIndexSet.Add(num);
			}
			if (!_showIndexSet.SetEquals(_transIndexSet))
			{
				IEnumerator<int> enumerator = _showIndexSet.Except(_transIndexSet).GetEnumerator();
				IEnumerator<int> enumerator2 = _transIndexSet.Except(_showIndexSet).GetEnumerator();
				while (enumerator.MoveNext() && enumerator2.MoveNext())
				{
					ChangeToIndex(enumerator2.Current, enumerator.Current);
					_onChange(_transDict[enumerator.Current], enumerator.Current);
				}
				HashSet<int> transIndexSet = _transIndexSet;
				_transIndexSet = _showIndexSet;
				_showIndexSet = transIndexSet;
			}
		}

		private void ChangeToIndex(int from, int to)
		{
			if (_transDict.ContainsKey(from))
			{
				RectTransform rectTransform = _transDict[from];
				rectTransform.anchoredPosition = IndexToPosition(to, rectTransform.pivot);
				_transDict.Remove(from);
				_transDict.Add(to, rectTransform);
			}
		}

		private Vector2 IndexToPosition(int index, Vector2 pivot)
		{
			Vector2 vector = ((moveType != Movement.Horizontal) ? new Vector2(ItemSize.x * (float)(index % _col), (0f - ItemSize.y) * (float)(index / _col)) : new Vector2(ItemSize.x * (float)(index / _row), (0f - ItemSize.y) * (float)(index % _row)));
			return vector + _padding + new Vector2(pivot.x * _cellSize.x, (pivot.y - 1f) * _cellSize.y);
		}

		public Dictionary<int, RectTransform> GetItemDict()
		{
			return _transDict;
		}

		private void ProcessSlideAudio()
		{
			if (!string.IsNullOrEmpty(scrollAudioPatternName))
			{
				int num = 0;
				if (moveType == Movement.Horizontal)
				{
					float num2 = 0f - grid.GetComponent<RectTransform>().anchoredPosition.x;
					int num3 = (int)(num2 / ItemSize.x);
					num = num3;
				}
				else
				{
					float y = grid.GetComponent<RectTransform>().anchoredPosition.y;
					int num4 = (int)(y / ItemSize.y);
					num = num4;
				}
				if (!_isFirstRefresh && _preRefreshIndex != num)
				{
					Singleton<WwiseAudioManager>.Instance.Post(scrollAudioPatternName);
				}
				_isFirstRefresh = false;
				_preRefreshIndex = num;
			}
		}
	}
}
