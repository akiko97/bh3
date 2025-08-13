using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class ProtectedShieldAnimation : MonoBehaviour
	{
		public Vector3 hitPosition;

		public float hitAnmStartDuration = 0.2f;

		public float hitAnmEndDuration = 0.2f;

		public float _hitAnmTimer;

		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		private void Awake()
		{
			_renderer = GetComponent<Renderer>();
			_mpb = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_mpb);
		}

		private void Update()
		{
			UpdateHitAnimation();
			_renderer.SetPropertyBlock(_mpb);
		}

		public void PlayHitAnimation()
		{
		}

		private void UpdateHitAnimation()
		{
			if (!(_hitAnmTimer > hitAnmStartDuration + hitAnmEndDuration))
			{
				_hitAnmTimer += Time.deltaTime;
				_mpb.SetVector("_HitPosition", hitPosition);
				if (_hitAnmTimer > hitAnmStartDuration)
				{
					float value = Mathf.Clamp01((_hitAnmTimer - hitAnmStartDuration) / hitAnmEndDuration);
					_mpb.SetFloat("_HitAnmStartTime", 1f);
					_mpb.SetFloat("_HitAnmEndTime", value);
				}
				else
				{
					float value2 = Mathf.Clamp01(_hitAnmTimer / hitAnmStartDuration);
					_mpb.SetFloat("_HitAnmStartTime", value2);
					_mpb.SetFloat("_HitAnmEndTime", 0f);
				}
			}
		}
	}
}
