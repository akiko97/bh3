using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSPDisplayText : MonoBehaviour
	{
		private const float MIN_SP_BAR_EMIT_SCALER = 1f;

		private const float MAX_SP_BAR_EMIT_SCALER = 1.5f;

		private Material _spMaterial;

		private bool _isSPBrightAnimationPlaying;

		private float _emitScalerGrowSpeed = 0.2f;

		public void SetupView(float spBefore, float spAfter, float delta, bool showText = false)
		{
			Text component = base.transform.Find("DisplayText/Text").GetComponent<Text>();
			_spMaterial = base.transform.Find("Bar/MaskSlider/Slider/Fill").GetComponent<ImageForSmoothMask>().material;
			_isSPBrightAnimationPlaying = false;
			int num = UIUtil.FloorToIntCustom(delta);
			if (delta > 0f)
			{
				component.text = string.Format("+{0}", num);
				if (showText)
				{
					base.transform.Find("DisplayText").gameObject.SetActive(true);
					PlayDisplayAnimation();
				}
				else
				{
					base.transform.Find("DisplayText").gameObject.SetActive(false);
				}
				PlaySPAddAnimation();
			}
			if (delta < 0f)
			{
				component.text = string.Format("{0}", num);
			}
		}

		private float GetSPBarEmissionScaler()
		{
			return _spMaterial.GetFloat("_EmissionScaler");
		}

		private void SetSPBarEmissionScaler(float scaler)
		{
			if (!(_spMaterial == null))
			{
				float value = Mathf.Clamp(scaler, 1f, 1.5f);
				_spMaterial.SetFloat("_EmissionScaler", value);
			}
		}

		private void PlaySPAddAnimation()
		{
			_isSPBrightAnimationPlaying = true;
			SetSPBarEmissionScaler(1f);
		}

		public void Update()
		{
			if (_isSPBrightAnimationPlaying)
			{
				float sPBarEmissionScaler = GetSPBarEmissionScaler();
				if (sPBarEmissionScaler < 1.5f)
				{
					float sPBarEmissionScaler2 = Mathf.Clamp(sPBarEmissionScaler + _emitScalerGrowSpeed, 1f, 1.5f);
					SetSPBarEmissionScaler(sPBarEmissionScaler2);
				}
				else
				{
					_isSPBrightAnimationPlaying = false;
					SetSPBarEmissionScaler(1f);
				}
			}
		}

		private void PlayDisplayAnimation()
		{
			Animation component = base.transform.Find("DisplayText").GetComponent<Animation>();
			if (component != null)
			{
				if (component.isPlaying)
				{
					component.Rewind();
				}
				component.Play("DisplaySP", PlayMode.StopAll);
			}
		}
	}
}
