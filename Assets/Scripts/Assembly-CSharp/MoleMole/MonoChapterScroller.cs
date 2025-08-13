using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(ScrollRect))]
	public class MonoChapterScroller : MonoBehaviour, IEventSystemHandler, IEndDragHandler, IBeginDragHandler
	{
		public enum Movement
		{
			Horizontal = 0,
			Vertical = 1
		}

		public enum State
		{
			Init = 0,
			Idle = 1,
			Drag = 2,
			Lerp = 3,
			ClickLerp = 4,
			Swipe = 5
		}

		public enum InitState
		{
			EnableGird = 0,
			Init = 1
		}

		public Movement moveType;

		public RectTransform content;

		public float stopLerpSpeedThreshold;

		private ScrollRect _scroller;

		private Dictionary<Transform, float> _diatanceMap;

		private List<Transform> _childSort;

		private Dictionary<Transform, int> _childIndexMap;

		private int _centerIndex;

		private int _childNum;

		private GridLayoutGroup _gridLayout;

		public float lerpSpeed = 5f;

		private Action<int> _changeSelectCallBack;

		public State state;

		private InitState _initState;

		public void Init(int initCenterIndex, int childNum, Action<int> changeSelectCallback = null)
		{
			_childNum = childNum;
			_centerIndex = initCenterIndex;
			state = State.Init;
			_initState = InitState.EnableGird;
			_changeSelectCallBack = changeSelectCallback;
		}

		public void ClickToChangeCenter(Transform child)
		{
			if (_childIndexMap == null)
			{
				InitChildIndexMap();
			}
			_centerIndex = _childIndexMap[child];
			state = State.ClickLerp;
		}

		public bool IsCenter(Transform child)
		{
			if (child == null || _childIndexMap == null)
			{
				return false;
			}
			return _childIndexMap[child] == _centerIndex;
		}

		private void Awake()
		{
			InitScroller();
		}

		private void Init()
		{
			state = State.Init;
			InitGrid();
			InitChildren();
			InitTransform();
			InitCenter();
		}

		private void InitScroller()
		{
			_scroller = GetComponent<ScrollRect>();
			_scroller.onValueChanged.AddListener(OnValueChanged);
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
			_gridLayout = content.GetComponent<GridLayoutGroup>();
			_gridLayout.enabled = false;
		}

		private void InitTransform()
		{
			if (moveType == Movement.Horizontal)
			{
				float size = (float)(_gridLayout.padding.left + _gridLayout.padding.right) + _gridLayout.cellSize.x * (float)_childNum + _gridLayout.spacing.x * (float)(_childNum - 1);
				content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
			else
			{
				float size2 = (float)(_gridLayout.padding.top + _gridLayout.padding.bottom) + _gridLayout.cellSize.y * (float)_childNum + _gridLayout.spacing.y * (float)(_childNum - 1);
				content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size2);
			}
		}

		private void InitChildren()
		{
			_diatanceMap = new Dictionary<Transform, float>();
			_childSort = new List<Transform>();
			_childIndexMap = new Dictionary<Transform, int>();
			int num = 0;
			foreach (Transform item in content)
			{
				_childIndexMap.Add(item, num);
				RectTransform component = item.GetComponent<RectTransform>();
				component.anchoredPosition = IndexToPosition(num, component.pivot);
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _gridLayout.cellSize.x);
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _gridLayout.cellSize.y);
				component.anchorMin = new Vector2(0f, 1f);
				component.anchorMax = new Vector2(0f, 1f);
				num++;
			}
		}

		private void InitChildIndexMap()
		{
			if (_childIndexMap == null)
			{
				_childIndexMap = new Dictionary<Transform, int>();
			}
			else
			{
				_childIndexMap.Clear();
			}
			int num = 0;
			foreach (Transform item in content)
			{
				_childIndexMap.Add(item, num);
				num++;
			}
		}

		private void InitCenter()
		{
			float num = CalculateCenterNormalizedPos(_centerIndex);
			if (moveType == Movement.Horizontal)
			{
				_scroller.horizontalNormalizedPosition = num;
			}
			else
			{
				_scroller.verticalNormalizedPosition = num;
			}
			UpdateChildren();
		}

		private Vector2 IndexToPosition(int index, Vector2 pivot)
		{
			Vector2 vector = ((moveType != Movement.Horizontal) ? new Vector2(0f, 0f - ((float)_gridLayout.padding.top + (_gridLayout.cellSize.y + _gridLayout.spacing.y) * (float)index)) : new Vector2((float)_gridLayout.padding.left + (_gridLayout.cellSize.x + _gridLayout.spacing.x) * (float)index, 0f));
			return vector + new Vector2(pivot.x * _gridLayout.cellSize.x, (pivot.y - 1f) * _gridLayout.cellSize.y);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			state = State.Drag;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			state = State.Swipe;
		}

		private void Update()
		{
			switch (state)
			{
			case State.Init:
				switch (_initState)
				{
				case InitState.EnableGird:
					_gridLayout = content.GetComponent<GridLayoutGroup>();
					_gridLayout.enabled = true;
					_initState = InitState.Init;
					break;
				case InitState.Init:
					Init();
					state = State.Idle;
					break;
				}
				break;
			case State.Lerp:
			case State.ClickLerp:
			{
				float horizontalNormalizedPosition = _scroller.horizontalNormalizedPosition;
				float num = CalculateCenterNormalizedPos(_centerIndex);
				if (moveType == Movement.Horizontal)
				{
					float num2 = Mathf.Lerp(_scroller.horizontalNormalizedPosition, num, Time.deltaTime * lerpSpeed);
					_scroller.horizontalNormalizedPosition = num2;
					horizontalNormalizedPosition = num2;
				}
				else
				{
					float num2 = Mathf.Lerp(_scroller.verticalNormalizedPosition, num, Time.deltaTime * lerpSpeed);
					_scroller.verticalNormalizedPosition = num2;
					horizontalNormalizedPosition = num2;
				}
				if (Mathf.Abs(num - horizontalNormalizedPosition) <= 0.001f)
				{
					if (_changeSelectCallBack != null)
					{
						_changeSelectCallBack(_centerIndex);
					}
					state = State.Idle;
				}
				break;
			}
			case State.Idle:
			case State.Drag:
				break;
			}
		}

		private float CalculateCenterNormalizedPos(int index)
		{
			return Mathf.Clamp((float)index / ((float)_childNum - 1f), 0f, 1f);
		}

		private float CaculateNormalizedPositionDelta(float absDelat)
		{
			float result = absDelat / content.rect.width;
			if (moveType == Movement.Vertical)
			{
				result = absDelat / content.rect.height;
			}
			return result;
		}

		public void OnValueChanged(Vector2 normalizedPos)
		{
			float f = ((moveType != Movement.Horizontal) ? _scroller.velocity.y : _scroller.velocity.x);
			if (state == State.Swipe && Mathf.Abs(f) < stopLerpSpeedThreshold)
			{
				state = State.Lerp;
			}
			UpdateChildren();
		}

		private void UpdateChildren()
		{
			if (_childNum <= 1)
			{
				return;
			}
			_diatanceMap.Clear();
			foreach (Transform item in content)
			{
				if (!_childIndexMap.ContainsKey(item))
				{
					InitChildIndexMap();
				}
			}
			foreach (Transform item2 in content)
			{
				_diatanceMap.Add(item2, CalcualteCenterDistance(item2));
				SetChildView(item2, _diatanceMap[item2]);
			}
			_childSort.Clear();
			_childSort.AddRange(_diatanceMap.Keys);
			_childSort.Sort((Transform lobj, Transform robj) => Mathf.FloorToInt(_diatanceMap[lobj] - _diatanceMap[robj]));
			for (int num = 0; num < _childSort.Count; num++)
			{
				int siblingIndex = _childSort.Count - 1 - num;
				_childSort[num].SetSiblingIndex(siblingIndex);
			}
		}

		private float CalcualteCenterDistance(Transform child)
		{
			float num = 0f;
			float num2 = CalculateCenterNormalizedPos(_childIndexMap[child]);
			if (moveType == Movement.Horizontal)
			{
				return Mathf.Abs(_scroller.horizontalNormalizedPosition - num2);
			}
			return Mathf.Abs(_scroller.verticalNormalizedPosition - num2);
		}

		private void SetChildView(Transform child, float distance)
		{
			float num = 1f / ((float)_childNum - 1f) / 2f;
			child.localScale = Vector3.one * Mathf.Clamp(1f - 0.08f / num * distance, 0f, 1f);
			if (distance <= num && state != State.ClickLerp)
			{
				_centerIndex = _childIndexMap[child];
			}
			MonoActivityEntryButton component = child.GetComponent<MonoActivityEntryButton>();
			MonoChapterButton component2 = child.GetComponent<MonoChapterButton>();
			if (component != null)
			{
				bool flag = distance <= num;
				if (component.selected != flag && flag && state != State.Init && state != State.Idle)
				{
					Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide");
				}
				component.UpdateView(distance <= num);
			}
			else if (component2 != null)
			{
				bool flag2 = distance <= num;
				if (component2.selected != flag2 && flag2 && state != State.Init && state != State.Idle)
				{
					Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide");
				}
				component2.UpdateView(distance <= num);
			}
		}
	}
}
