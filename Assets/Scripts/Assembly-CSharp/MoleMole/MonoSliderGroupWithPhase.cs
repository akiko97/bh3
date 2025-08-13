using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSliderGroupWithPhase : MonoBehaviour
	{
		private const int MAX_PHASE = 4;

		public float minValue;

		public float value;

		public float maxValue = 1f;

		public int maxPhase = 1;

		public Sprite[] spriteList;

		private MonoMaskSlider[] _sliders;

		private float _perPhaseRatio;

		private float _perSegmentRatio;

		private int _segmentNum;

		private int _currentPhase;

		public void Init()
		{
			_sliders = GetComponentsInChildren<MonoMaskSlider>();
			_segmentNum = _sliders.Length;
			_perPhaseRatio = 1f / (float)maxPhase;
			_perSegmentRatio = 1f / (float)_segmentNum;
			Material material = _sliders[0].GetComponentInChildren<ImageForSmoothMask>().material;
			for (int i = 0; i < _segmentNum; i++)
			{
				_sliders[i].maxValue = 1f;
				_sliders[i].GetComponent<Image>().material = material;
				_sliders[i].GetComponentInChildren<ImageForSmoothMask>().material = material;
			}
			_currentPhase = maxPhase;
			SetupPhaseView();
		}

		public void UpdateMaxPhase(int newMaxPhase)
		{
			if (maxPhase != newMaxPhase)
			{
				maxPhase = newMaxPhase;
				_perPhaseRatio = 1f / (float)maxPhase;
				UpdateValue(value, maxValue, minValue);
			}
		}

		public void UpdateValue(float value, float maxValue, float minValue = 0f)
		{
			if (_sliders == null)
			{
				Init();
			}
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.value = ((!(value > maxValue)) ? value : maxValue);
			float num = ((maxValue != 0f) ? ((this.value - this.minValue) / (this.maxValue - this.minValue)) : 1f);
			int num2 = Mathf.CeilToInt(num / _perPhaseRatio);
			if (_currentPhase != num2)
			{
				_currentPhase = num2;
				SetupPhaseView();
			}
			float num3 = num / _perPhaseRatio - (float)Mathf.Max(0, _currentPhase - 1);
			int num4 = Mathf.FloorToInt(num3 / _perSegmentRatio);
			float num5 = num3 / _perSegmentRatio - (float)num4;
			for (int i = 0; i < _segmentNum; i++)
			{
				if (i < num4)
				{
					_sliders[i].UpdateValue(1f, 1f, 0f);
				}
				else if (i == num4)
				{
					_sliders[i].UpdateValue(num5, 1f, 0f);
				}
				else
				{
					_sliders[i].UpdateValue(0f, 1f, 0f);
				}
			}
		}

		private void SetupPhaseView()
		{
			MonoMaskSlider[] sliders = _sliders;
			foreach (MonoMaskSlider monoMaskSlider in sliders)
			{
				int num = Mathf.Max(0, _currentPhase - 1);
				monoMaskSlider.transform.GetComponent<Image>().sprite = spriteList[num];
				monoMaskSlider.transform.Find("Slider/Fill").GetComponent<Image>().sprite = spriteList[_currentPhase];
			}
		}

		private void OnDestroy()
		{
			_sliders = null;
			spriteList = null;
		}
	}
}
