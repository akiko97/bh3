using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoActScroller : MonoLevelScroller, IEventSystemHandler, IEndDragHandler
	{
		private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

		private const string MATERIAL_COLOR_PATH = "Material/ImageMonoColor";

		public void InitActs(int centIndex, int childNum, Action lerpEndCallBack = null, bool lerpAfterInit = true)
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
			if (lerpAfterInit)
			{
				state = MonoChapterScroller.State.ClickLerp;
				if (boundScroller != null)
				{
					boundScroller.driveByOutside = true;
					boundScroller.state = MonoChapterScroller.State.ClickLerp;
					boundScroller.centerIndex = centerIndex;
					driveByOutside = false;
				}
			}
			else
			{
				_scroller.verticalNormalizedPosition = 1f - (float)centIndex / (float)(_childNum - 1);
				OnEndLerp();
			}
		}

		private new void Update()
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
			if (state == MonoChapterScroller.State.Lerp || state == MonoChapterScroller.State.ClickLerp)
			{
				float num = CalculateCenterNormalizedPos(centerIndex);
				verticalNormalizedPosition = ((!driveByOutside) ? Mathf.Lerp(_scroller.verticalNormalizedPosition, num, Time.deltaTime * lerpSpeed) : _scroller.verticalNormalizedPosition);
				_scroller.verticalNormalizedPosition = verticalNormalizedPosition;
				if (Mathf.Abs(num - verticalNormalizedPosition) < stopLerpThreshold)
				{
					OnEndLerp();
				}
			}
			if (boundScroller != null && boundScroller.driveByOutside && !driveByOutside)
			{
				boundScroller.SetNormalizePositionY(_scroller.verticalNormalizedPosition);
			}
		}

		public override void OnEndLerp()
		{
			state = MonoChapterScroller.State.Idle;
			if (boundScroller != null)
			{
				boundScroller.driveByOutside = false;
				boundScroller.state = MonoChapterScroller.State.Idle;
				boundScroller.SetNormalizePositionY(_scroller.verticalNormalizedPosition);
				boundScroller.OnEndLerp();
			}
			if (_onLerpEndCallBack != null)
			{
				_onLerpEndCallBack();
			}
			if (onLerpEndCallBack != null)
			{
				onLerpEndCallBack();
			}
		}

		public override void UpdateContent()
		{
			for (int i = 0; i < _childNum; i++)
			{
				Transform transform = _indexToChildDict[i];
				if (!(transform == null))
				{
					float distance = CalcualteCenterDistance(i);
					SetUpChildView(_indexToChildDict[i], distance);
				}
			}
		}

		public override void SetUpChildView(Transform childTrans, float distance)
		{
			if (!(childTrans == null))
			{
				bool flag = _childIndexDict[childTrans] == centerIndex;
				MonoActButton component = childTrans.GetComponent<MonoActButton>();
				if (component != null && flag && !component.selected && state != MonoChapterScroller.State.Idle && state != MonoChapterScroller.State.Init)
				{
					Singleton<WwiseAudioManager>.Instance.Post("UI_Gen_Obj_Slide");
				}
				component.SetupStatus(flag);
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			state = MonoChapterScroller.State.Swipe;
			_scroller.velocity *= initSwipeSpeedRatio;
			if (boundScroller != null)
			{
				boundScroller.state = MonoChapterScroller.State.Swipe;
			}
		}
	}
}
