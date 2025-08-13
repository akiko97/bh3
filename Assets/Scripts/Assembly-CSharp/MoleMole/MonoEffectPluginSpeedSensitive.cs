using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginSpeedSensitive : MonoBehaviour
	{
		[Header("Target Particle Systems")]
		public ParticleSystem[] targetParticleSystems;

		[Header("Max Speed")]
		public float maxSpeed = 1f;

		[SerializeField]
		private float[] _origRateMaxes;

		[SerializeField]
		private Vector3 _lastPos;

		private void Awake()
		{
			_origRateMaxes = new float[targetParticleSystems.Length];
			_lastPos = base.transform.position;
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = targetParticleSystems[i].emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				_origRateMaxes[i] = rate.constantMax;
				rate.curveScalar = 0f;
				emission.rate = rate;
			}
		}

		private void Update()
		{
			float num = Mathf.Min(1f, Vector3.Distance(base.transform.position, _lastPos) / (Time.deltaTime * maxSpeed));
			for (int i = 0; i < targetParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = targetParticleSystems[i].emission;
				ParticleSystem.MinMaxCurve rate = emission.rate;
				rate.constantMax = _origRateMaxes[i] * num;
				emission.rate = rate;
			}
			_lastPos = base.transform.position;
		}
	}
}
