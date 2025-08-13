using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMaskSliderGrow : MonoBehaviour
	{
		public MonoMaskSlider maskSlider;

		public Text currentValueText;

		public Text MaxValueText;

		private float _value;

		private float speed = 0.5f;

		private bool play;

		private float valueBefore;

		private float maxValueBefore;

		private float valueAfter;

		private float maxValueAfter;

		private int growTimes;

		private float _ratioBefore;

		private float _ratioAfter;

		private int _count;

		private List<float> maxList;

		private Action<Transform> everyFullAction;

		private Action<Transform> overAction;

		private void Update()
		{
			if (!play)
			{
				return;
			}
			_value += speed;
			if (_count >= growTimes && _value >= _ratioAfter)
			{
				play = false;
				maskSlider.UpdateValue(_ratioAfter, 1f, 0f);
				if (currentValueText != null)
				{
					currentValueText.text = valueAfter.ToString();
				}
				if (MaxValueText != null)
				{
					float num = maxList[maxList.Count - 1];
					if (_count < maxList.Count)
					{
						num = maxList[_count - 1];
					}
					MaxValueText.text = num.ToString();
				}
				if (overAction != null)
				{
					overAction(base.transform);
				}
				return;
			}
			if (_value >= 1f)
			{
				if (everyFullAction != null)
				{
					everyFullAction(base.transform);
				}
				_count++;
				if (MaxValueText != null)
				{
					float num2 = maxList[maxList.Count - 1];
					if (_count < maxList.Count)
					{
						num2 = maxList[_count - 1];
					}
					MaxValueText.text = num2.ToString();
				}
				_value = 0f;
			}
			float num3 = maxList[_count - 1];
			if (currentValueText != null)
			{
				currentValueText.text = Mathf.FloorToInt(_value * num3).ToString();
			}
			maskSlider.UpdateValue(_value, 1f, 0f);
		}

		public void Play(float valBefore, float valAfter, List<float> maxList, Action<Transform> firstFullAction = null, Action<Transform> overAction = null)
		{
			everyFullAction = firstFullAction;
			this.overAction = overAction;
			this.maxList = maxList;
			growTimes = maxList.Count;
			valueAfter = valAfter;
			float num = maxList[0];
			float num2 = maxList[maxList.Count - 1];
			speed = ((num - valBefore) / num + valAfter / num2 + ((growTimes <= 2) ? 0f : ((float)(growTimes - 2)))) / (60f * maskSlider.sliderGrowTime);
			_ratioBefore = valBefore / num;
			_ratioAfter = valAfter / num2;
			_value = _ratioBefore;
			maskSlider.UpdateValue(_ratioBefore, 1f, 0f);
			if (currentValueText != null)
			{
				currentValueText.text = valBefore.ToString();
			}
			if (MaxValueText != null)
			{
				MaxValueText.text = num.ToString();
			}
			_count = 1;
			play = true;
		}
	}
}
