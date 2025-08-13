using System.Collections.Generic;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginTrailStatic : MonoEffectPluginTrail
	{
		private StaticTrail _staticTrail;

		public bool generateDefaultDuration = true;

		public float appearDuration = 1f;

		public AnimationCurve appearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float vanishDuration = 1f;

		public AnimationCurve vanishCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private float _timer;

		[Header("Set this for entity overriding the first material of this smooth trail.")]
		public string MaterialOverrideKey;

		[Header("Set this to add overlay effect onto the moving trail anchor.")]
		public string EffectOverlayKey;

		public float TimeScale = 1f;

		public StaticTrail staticTrail
		{
			get
			{
				return _staticTrail;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_staticTrail = TrailRendererTransform.GetComponent<StaticTrail>();
			_staticTrail.Init(FramePosList);
			if (generateDefaultDuration)
			{
				appearDuration = (float)FramePosList.Length * (1f / 60f) * 1.2f;
				vanishDuration = appearDuration;
			}
		}

		public override void Setup()
		{
			base.Setup();
			AniAnchorTransform.localPosition = Vector3.zero;
			TrailRendererTransform.localPosition = AniAnchorTransform.localPosition;
			_timer = 0f;
			_staticTrail.ResetAnimation(appearDuration, appearCurve, vanishDuration, vanishCurve);
			SetAniAnchor();
		}

		public void HandleEffectOverride(MonoEffectOverride effectOverride)
		{
			if (!string.IsNullOrEmpty(MaterialOverrideKey) && effectOverride.materialOverrides.ContainsKey(MaterialOverrideKey))
			{
				if (_staticTrail.TrailData.TrailMaterials[0].name.EndsWith("(Instance)"))
				{
					if (Application.isEditor)
					{
						Object.DestroyImmediate(_staticTrail.TrailData.TrailMaterials[0]);
					}
					else
					{
						Object.Destroy(_staticTrail.TrailData.TrailMaterials[0]);
					}
				}
				_staticTrail.TrailData.TrailMaterials[0] = new Material(effectOverride.materialOverrides[MaterialOverrideKey]);
				_staticTrail.TrailData.TrailMaterials[0].name += "(Instance)";
				string val = effectOverride.materialOverrides[MaterialOverrideKey].GetTag("Distortion", false);
				_staticTrail.TrailData.TrailMaterials[0].SetOverrideTag("Distortion", val);
				_staticTrail.ResetAnimation(appearDuration, appearCurve, vanishDuration, vanishCurve);
			}
			if (!string.IsNullOrEmpty(EffectOverlayKey) && effectOverride.effectOverlays.ContainsKey(EffectOverlayKey))
			{
				List<MonoEffect> effects;
				Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(effectOverride.effectOverlays[EffectOverlayKey], AniAnchorTransform.position, AniAnchorTransform.forward, AniAnchorTransform.localScale, _effect.owner, out effects);
				for (int i = 0; i < effects.Count; i++)
				{
					effects[i].transform.SetParent(AniAnchorTransform, true);
				}
			}
		}

		protected override void Update()
		{
			float num = Time.deltaTime * _effect.TimeScale * TimeScale;
			_timer += num;
			_staticTrail.PlayAnimation(num);
			SetAniAnchor();
		}

		private void SetAniAnchor()
		{
			AniAnchorTransform.position = _staticTrail.GetPointAlongTrail(_timer / appearDuration);
		}

		public override bool IsToBeRemove()
		{
			return TrailRendererTransform == null || !_staticTrail.IsActive;
		}

		public override void SetDestroy()
		{
		}
	}
}
