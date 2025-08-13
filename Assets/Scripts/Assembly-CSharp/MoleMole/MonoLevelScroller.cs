using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(ScrollRect))]
	public class MonoLevelScroller : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
	{
		private const string LEVEL_LINES_ANI_STR = "LevelLinesFadeIn";

		public MonoLevelScroller boundScroller;

		public MonoChapterScroller.State state;

		protected ScrollRect _scroller;

		protected GridLayoutGroup _grid;

		protected Transform _contentTrans;

		protected int _childNum;

		protected Dictionary<Transform, int> _childIndexDict;

		protected Dictionary<int, Transform> _indexToChildDict;

		protected float _selectedOffset;

		public bool driveByOutside;

		public int centerIndex;

		protected bool _finishInit;

		public Action onLerpEndCallBack;

		public float stopSwipeSpeedThreshold = 0.1f;

		public MonoChapterScroller.Movement moveType = MonoChapterScroller.Movement.Vertical;

		public float initSwipeSpeedRatio = 1f;

		public float lerpSpeed = 5f;

		public float stopLerpThreshold = 0.001f;

		protected Action _onLerpEndCallBack;

		private int _originalCenterIndex;

		protected float _dragDelta;

		private void Awake()
		{
			_scroller = GetComponent<ScrollRect>();
			_scroller.onValueChanged.AddListener(OnValueChanged);
		}

		public void InitLevelPanels(int centIndex, int childNum, Action lerpEndCallBack = null, bool lerpAfterInit = true)
		{
			_contentTrans = base.transform.Find("Content");
			_grid = _contentTrans.GetComponent<GridLayoutGroup>();
			_childNum = childNum;
			driveByOutside = false;
			centerIndex = centIndex;
			_finishInit = true;
			_onLerpEndCallBack = lerpEndCallBack;
			_dragDelta = 0f;
			Setup();
			if (!lerpAfterInit)
			{
				_scroller.verticalNormalizedPosition = 1f - (float)centIndex / (float)(_childNum - 1);
				OnEndLerp();
			}
		}

		public void Setup()
		{
			_childIndexDict = new Dictionary<Transform, int>();
			_indexToChildDict = new Dictionary<int, Transform>();
			int num = 0;
			foreach (Transform contentTran in _contentTrans)
			{
				if (contentTran.GetComponent<MonoItemStatus>().isValid)
				{
					_childIndexDict.Add(contentTran, num);
					_indexToChildDict.Add(num, contentTran);
					num++;
				}
			}
			_selectedOffset = 1f / ((float)_childNum - 1f) / 2f;
			UpdateContent();
			state = MonoChapterScroller.State.Lerp;
		}

		public void Update()
		{
			if (!_finishInit)
			{
				return;
			}
			if (_indexToChildDict.ContainsValue(null))
			{
				Setup();
			}
			float verticalNormalizedPosition = _scroller.verticalNormalizedPosition;
			if (state == MonoChapterScroller.State.Swipe)
			{
				float f = ((moveType != MonoChapterScroller.Movement.Horizontal) ? _scroller.velocity.y : _scroller.velocity.x);
				if (Mathf.Abs(f) < stopSwipeSpeedThreshold)
				{
					state = MonoChapterScroller.State.Lerp;
				}
			}
			if (state == MonoChapterScroller.State.Lerp || state == MonoChapterScroller.State.ClickLerp)
			{
				float num = CalculateCenterNormalizedPos(centerIndex);
				verticalNormalizedPosition = ((!driveByOutside) ? Mathf.Lerp(_scroller.verticalNormalizedPosition, num, Time.deltaTime * 5f) : _scroller.verticalNormalizedPosition);
				_scroller.verticalNormalizedPosition = verticalNormalizedPosition;
				if (Mathf.Approximately(num, verticalNormalizedPosition))
				{
					OnEndLerp();
				}
			}
			if (boundScroller != null && boundScroller.driveByOutside && !driveByOutside)
			{
				boundScroller.SetNormalizePositionY(_scroller.verticalNormalizedPosition);
			}
		}

		public virtual void OnValueChanged(Vector2 normalizedPos)
		{
			float f = ((moveType != MonoChapterScroller.Movement.Horizontal) ? _scroller.velocity.y : _scroller.velocity.x);
			if (state == MonoChapterScroller.State.Swipe && Mathf.Abs(f) < stopSwipeSpeedThreshold)
			{
				state = MonoChapterScroller.State.Lerp;
			}
			for (int i = 0; i < _childNum; i++)
			{
				float num = CalcualteCenterDistance(i);
				SetUpChildView(_indexToChildDict[i], num);
				if (num <= _selectedOffset && state != MonoChapterScroller.State.ClickLerp)
				{
					centerIndex = i;
				}
			}
		}

		public void ClickToChangeCenter(Transform child)
		{
			centerIndex = _childIndexDict[child];
			state = MonoChapterScroller.State.ClickLerp;
			if (boundScroller != null)
			{
				boundScroller.driveByOutside = true;
				boundScroller.state = MonoChapterScroller.State.ClickLerp;
				boundScroller.centerIndex = centerIndex;
				driveByOutside = false;
			}
		}

		public virtual void SetUpChildView(Transform childTrans, float distance)
		{
		}

		protected float CalculateCenterNormalizedPos(int index)
		{
			return 1f - (float)index / ((float)_childNum - 1f);
		}

		protected float CaculateNormalizedPositionDelta(float absDelat)
		{
			float result = absDelat / _contentTrans.GetComponent<RectTransform>().rect.width;
			if (moveType == MonoChapterScroller.Movement.Vertical)
			{
				result = absDelat / _contentTrans.GetComponent<RectTransform>().rect.height;
			}
			return result;
		}

		protected float CalcualteCenterDistance(int index)
		{
			float num = 0f;
			float num2 = CalculateCenterNormalizedPos(index);
			return Mathf.Abs(_scroller.verticalNormalizedPosition - num2);
		}

		public virtual void OnEndLerp()
		{
			state = MonoChapterScroller.State.Idle;
			if (boundScroller != null)
			{
				boundScroller.driveByOutside = false;
			}
			if (_onLerpEndCallBack != null)
			{
				_onLerpEndCallBack();
			}
			_scroller.velocity = Vector2.zero;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			state = MonoChapterScroller.State.Drag;
			driveByOutside = false;
			if (boundScroller != null)
			{
				boundScroller.driveByOutside = true;
				boundScroller.state = MonoChapterScroller.State.Drag;
			}
			_originalCenterIndex = centerIndex;
			_dragDelta = 0f;
			if (moveType == MonoChapterScroller.Movement.Horizontal)
			{
				_dragDelta += eventData.delta.x;
			}
			else
			{
				_dragDelta += eventData.delta.y;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (moveType == MonoChapterScroller.Movement.Horizontal)
			{
				_dragDelta += eventData.delta.x;
			}
			else
			{
				_dragDelta += eventData.delta.y;
			}
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (moveType == MonoChapterScroller.Movement.Horizontal)
			{
				_dragDelta += eventData.delta.x;
			}
			else
			{
				_dragDelta += eventData.delta.y;
			}
			int num = _originalCenterIndex;
			if (moveType == MonoChapterScroller.Movement.Horizontal)
			{
				num = ((!(_dragDelta > 0f)) ? (num + 1) : (num - 1));
			}
			if (moveType == MonoChapterScroller.Movement.Vertical)
			{
				num = ((!(_dragDelta > 0f)) ? (num - 1) : (num + 1));
			}
			num = Mathf.Clamp(num, 0, _childNum - 1);
			ClickToChangeCenter(_indexToChildDict[num]);
		}

		public void SetNormalizedPosition(Vector2 normalizedPosition)
		{
			_scroller.normalizedPosition = normalizedPosition;
		}

		public void SetNormalizePositionY(float positionY)
		{
			Vector2 normalizedPosition = new Vector2(_scroller.normalizedPosition.x, positionY);
			_scroller.normalizedPosition = normalizedPosition;
		}

		public virtual void UpdateContent()
		{
		}

		public void ShowItemByIndex(int index)
		{
			if (index < _childNum)
			{
				SetNormalizePositionY(1f - (float)index / (float)(_childNum - 1));
			}
		}

		public Transform GetCenterTransform()
		{
			return _indexToChildDict[centerIndex];
		}
	}
}
