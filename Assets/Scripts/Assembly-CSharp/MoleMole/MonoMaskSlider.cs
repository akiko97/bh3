using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[SerializeField]
	public class MonoMaskSlider : MonoBehaviour
	{
		public RectTransform maskRect;

		public RectTransform fillRect;

		public Slider.Direction dirction;

		public float sliderGrowTime = 0.2f;

		public float minValue;

		public float value;

		public float maxValue = 1f;

		public Action<float, float> onValueChanged;

		public void UpdateValue(float value, float maxValue, float minValue = 0f)
		{
			float ratio = GetRatio();
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.value = ((!(value > maxValue)) ? value : maxValue);
			Rect rect = maskRect.rect;
			float ratio2 = GetRatio();
			float num = 0f;
			int num2 = 1;
			switch (dirction)
			{
			case Slider.Direction.LeftToRight:
				num = rect.width;
				break;
			case Slider.Direction.RightToLeft:
				num = rect.width;
				num2 = -1;
				break;
			case Slider.Direction.BottomToTop:
				num = rect.height;
				break;
			case Slider.Direction.TopToBottom:
				num = rect.height;
				num2 = -1;
				break;
			}
			float num3 = (float)num2 * num * (1f - ratio2);
			maskRect.anchoredPosition = new Vector2(0f - num3, maskRect.anchoredPosition.y);
			fillRect.anchoredPosition = new Vector2(num3, fillRect.anchoredPosition.y);
			if (onValueChanged != null)
			{
				onValueChanged(ratio, ratio2);
			}
		}

		private float GetRatio()
		{
			if (maxValue == minValue)
			{
				return 1f;
			}
			return (value - minValue) / (maxValue - minValue);
		}
	}
}
