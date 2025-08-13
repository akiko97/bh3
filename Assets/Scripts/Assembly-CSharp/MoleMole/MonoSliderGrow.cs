using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Slider))]
	public class MonoSliderGrow : MonoBehaviour
	{
		private const float SPEED = 1f;

		public bool play;

		public float valueBefore;

		public float maxValueBefore;

		public float valueAfter;

		public float maxValueAfter;

		public int growTimes;

		private float _ratioBefore;

		private float _ratioAfter;

		private Slider _slider;

		private int _count;

		private void Awake()
		{
			play = false;
			_slider = GetComponent<Slider>();
		}

		private void Update()
		{
			if (!play)
			{
				return;
			}
			float num = _slider.value + 1f * Time.deltaTime;
			if (_count == growTimes && num >= _ratioAfter)
			{
				play = false;
				_slider.maxValue = maxValueAfter;
				_slider.value = valueAfter;
				return;
			}
			if (num >= 1f)
			{
				_count++;
				num = 0f;
			}
			_slider.value = num;
		}

		public void Play(float valBefore, float maxBefore, float valAfter, float maxValAfter, int growTimes)
		{
			play = true;
			valueBefore = valBefore;
			maxValueBefore = maxBefore;
			valueAfter = valAfter;
			maxValueAfter = maxValAfter;
			this.growTimes = growTimes;
			_ratioBefore = valueBefore / maxValueBefore;
			_ratioAfter = valueAfter / maxValueAfter;
			_slider.maxValue = 1f;
			_slider.value = _ratioBefore;
			_count = 0;
		}
	}
}
