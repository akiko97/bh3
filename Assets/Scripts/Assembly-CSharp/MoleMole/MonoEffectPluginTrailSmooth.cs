using System.Collections.Generic;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginTrailSmooth : MonoEffectPluginTrail
	{
		private SmoothTrail _smoothTrail;

		private float _timer;

		private int _controlPointIndex;

		private float _fixedDeltaTime = 1f / 60f;

		[Header("Set this for entity overriding the first material of this smooth trail.")]
		public string MaterialOverrideKey;

		[Header("Set this to add overlay effect onto the moving trail anchor.")]
		public string EffectOverlayKey;

		protected override void Awake()
		{
			base.Awake();
			_smoothTrail = TrailRendererTransform.GetComponent<SmoothTrail>();
		}

		public override void Setup()
		{
			base.Setup();
			_smoothTrail.Emit = true;
			_timer = 0f;
			_controlPointIndex = -1;
			AniAnchorTransform.localPosition = FramePosList[0];
			TrailRendererTransform.localPosition = AniAnchorTransform.localPosition;
			if (_smoothTrail != null)
			{
				_smoothTrail.ClearSystem(true);
			}
		}

		public void HandleEffectOverride(MonoEffectOverride effectOverride)
		{
			if (!string.IsNullOrEmpty(MaterialOverrideKey) && effectOverride.materialOverrides.ContainsKey(MaterialOverrideKey))
			{
				_smoothTrail.TrailData.TrailMaterials[0] = effectOverride.materialOverrides[MaterialOverrideKey];
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
			float num = Time.timeScale * _effect.TimeScale;
			_timer += Time.deltaTime * num;
			Vector3 position = Vector3.zero;
			_smoothTrail.TimeScale = _effect.TimeScale;
			if (GetPointPosition(ref position))
			{
				AniAnchorTransform.localPosition = position;
				TrailRendererTransform.localPosition = AniAnchorTransform.localPosition;
			}
		}

		private bool GetPointPosition(ref Vector3 position)
		{
			if (_controlPointIndex < FramePosList.Length)
			{
				if (_controlPointIndex == -1)
				{
					_controlPointIndex++;
					position = FramePosList[_controlPointIndex];
					return true;
				}
				if (_timer > _fixedDeltaTime)
				{
					_controlPointIndex++;
					_timer -= _fixedDeltaTime;
					if (_controlPointIndex < FramePosList.Length)
					{
						position = FramePosList[_controlPointIndex];
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool IsToBeRemove()
		{
			return TrailRendererTransform == null || (_smoothTrail.NumSegments() == 0 && _timer != 0f);
		}

		public override void SetDestroy()
		{
			_controlPointIndex = FramePosList.Length;
		}
	}
}
