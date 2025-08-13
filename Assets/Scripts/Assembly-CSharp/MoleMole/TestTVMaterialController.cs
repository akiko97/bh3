using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(RawImage))]
	public class TestTVMaterialController : MonoBehaviour
	{
		private RawImage _rawImage;

		private Material _material;

		public float distortionFrequency;

		public float distortionAmplitude;

		public float distortionAnimationSpeed;

		public float colorScatterStrength;

		public float noiseStrength;

		public float bloomFactor;

		public bool enable;

		private void Start()
		{
			_rawImage = GetComponent<RawImage>();
			_material = _rawImage.material;
			enable = false;
			distortionFrequency = _material.GetFloat("_DistortionFrequency");
			distortionAmplitude = _material.GetFloat("_DistortionAmplitude");
			distortionAnimationSpeed = _material.GetFloat("_DistortionAnmSpeed");
			colorScatterStrength = _material.GetFloat("_ColorScatterStrength");
			noiseStrength = _material.GetFloat("_NoiseStrength");
			bloomFactor = _material.GetFloat("_BloomFactor");
		}

		private void Update()
		{
			distortionAmplitude = Mathf.Clamp(distortionAmplitude, 0f, 1f);
			colorScatterStrength = Mathf.Clamp(colorScatterStrength, -1f, 1f);
			if (_material != null && enable)
			{
				_material.SetFloat("_DistortionFrequency", distortionFrequency);
				_material.SetFloat("_DistortionAmplitude", distortionAmplitude);
				_material.SetFloat("_DistortionAnmSpeed", distortionAnimationSpeed);
				_material.SetFloat("_ColorScatterStrength", colorScatterStrength);
				_material.SetFloat("_NoiseStrength", noiseStrength);
				_material.SetFloat("_BloomFactor", bloomFactor);
			}
		}
	}
}
