using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSliderGroup : MonoBehaviour
	{
		public float minValue;

		public float value;

		public float maxValue = 1f;

		private MonoMaskSlider[] _sliders;

		public Sprite healthyHPSprite;

		public Sprite unhealthyHPSprite;

		private LocalAvatarHealthMode _healthyMode;

		private float _perRatio;

		private int _segmentNum;

		public void Init()
		{
			_sliders = GetComponentsInChildren<MonoMaskSlider>();
			_segmentNum = _sliders.Length;
			_healthyMode = LocalAvatarHealthMode.Healthy;
			_perRatio = 1f / (float)_segmentNum;
			Material material = _sliders[0].GetComponentInChildren<ImageForSmoothMask>().material;
			for (int i = 0; i < _segmentNum; i++)
			{
				_sliders[i].maxValue = 1f;
				_sliders[i].GetComponent<Image>().material = material;
				_sliders[i].GetComponentInChildren<ImageForSmoothMask>().material = material;
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
			float num = (this.value - this.minValue) / (this.maxValue - this.minValue);
			int num2 = Mathf.FloorToInt(num / _perRatio);
			float num3 = num / _perRatio - (float)num2;
			for (int i = 0; i < _segmentNum; i++)
			{
				if (i < num2)
				{
					_sliders[i].UpdateValue(1f, 1f, 0f);
				}
				else if (i == num2)
				{
					_sliders[i].UpdateValue(num3, 1f, 0f);
				}
				else
				{
					_sliders[i].UpdateValue(0f, 1f, 0f);
				}
			}
		}

		public void SetupInDanageView(LocalAvatarHealthMode mode)
		{
			if (mode != _healthyMode)
			{
				_healthyMode = mode;
				Sprite sprite = ((_healthyMode != LocalAvatarHealthMode.Healthy) ? unhealthyHPSprite : healthyHPSprite);
				MonoMaskSlider[] sliders = _sliders;
				foreach (MonoMaskSlider monoMaskSlider in sliders)
				{
					monoMaskSlider.transform.Find("Slider/Fill").GetComponent<Image>().sprite = sprite;
				}
			}
		}

		private void OnDestroy()
		{
			_sliders = null;
			healthyHPSprite = null;
			unhealthyHPSprite = null;
		}
	}
}
