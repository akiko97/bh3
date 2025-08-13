using System;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class KianaWingAdjuster : MonoBehaviour
	{
		private static readonly string OPAQUENESS_SCALER_NAME = "_OpaquenessScalerWithoutHDR";

		private static readonly string EMISSION_SCALER_SCALER_NAME = "_EmissionScalerScalerWithoutHDR";

		public float opaquenessScalerWithoutHDR = 2f;

		public float emissionScalerScalerWithoutHDR = 0.22f;

		private float _oldOpaquenessScalerWithoutHDR = 1f;

		private float _oldEmissionScalerScalerWithoutHDR = 1f;

		private MaterialPropertyBlock _mpb;

		private Renderer _renderer;

		private void Start()
		{
			GraphicsSettingUtil.onPostFXChanged = (Action<bool>)Delegate.Combine(GraphicsSettingUtil.onPostFXChanged, new Action<bool>(SettingHDR));
			_renderer = GetComponent<Renderer>();
			_mpb = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_mpb);
			SettingHDR(false);
		}

		private void OnDestroy()
		{
			GraphicsSettingUtil.onPostFXChanged = (Action<bool>)Delegate.Remove(GraphicsSettingUtil.onPostFXChanged, new Action<bool>(SettingHDR));
		}

		private void SettingHDR(bool postFXEnable)
		{
			if (base.gameObject.activeSelf)
			{
				bool flag = false;
				PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
				if (postFX != null && postFX.enabled)
				{
					flag = postFX.SupportHDR;
				}
				if (!flag)
				{
					_mpb.SetFloat(OPAQUENESS_SCALER_NAME, opaquenessScalerWithoutHDR);
					_mpb.SetFloat(EMISSION_SCALER_SCALER_NAME, emissionScalerScalerWithoutHDR);
				}
				else
				{
					_mpb.SetFloat(OPAQUENESS_SCALER_NAME, _oldOpaquenessScalerWithoutHDR);
					_mpb.SetFloat(EMISSION_SCALER_SCALER_NAME, _oldEmissionScalerScalerWithoutHDR);
				}
				_renderer.SetPropertyBlock(_mpb);
			}
		}
	}
}
