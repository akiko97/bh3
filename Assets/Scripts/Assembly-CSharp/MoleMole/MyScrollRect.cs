using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MyScrollRect : ScrollRect
	{
		private SlideWithScrollOneByOne _slideWithScroll;

		private int _childCount;

		private float _step;

		protected override void Start()
		{
			base.Start();
			_slideWithScroll = base.transform.GetComponent<SlideWithScrollOneByOne>();
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			_slideWithScroll.SetTargetPosition(GetScrollPosition(base.horizontalNormalizedPosition));
		}

		private float GetScrollPosition(float position)
		{
			_childCount = base.transform.Find("Content").childCount;
			_step = 1f / (float)(_childCount - 1);
			int num = Mathf.FloorToInt(position / _step);
			if (num < 0)
			{
				num = 0;
			}
			if (num > _childCount)
			{
				num = _childCount;
			}
			float num2 = ((!(position - (float)num * _step >= _step / 2f)) ? ((float)num * _step) : ((float)(num + 1) * _step));
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return num2;
		}
	}
}
