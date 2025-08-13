using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginFakeWorldFollow : BaseMonoEffectPlugin
	{
		[Header("Main particle, should be like the rotating ones but is set to loop.")]
		public ParticleSystem MainParticle;

		[Header("An array of particle systems that are the same.")]
		public ParticleSystem[] TailParticles;

		public float PerParticleDuration = 0.5f;

		private Vector3[] _holdPositions;

		private Quaternion[] _holdRotations;

		private int _curIx;

		private float _timer;

		protected override void Awake()
		{
			base.Awake();
			_holdPositions = new Vector3[TailParticles.Length];
			_holdRotations = new Quaternion[TailParticles.Length];
		}

		public override void Setup()
		{
			base.Setup();
			for (int i = 0; i < TailParticles.Length; i++)
			{
				TailParticles[i].Clear();
				TailParticles[i].Stop();
			}
			_curIx = -1;
			_timer = PerParticleDuration;
		}

		private void FireTailAtIndex(int ix)
		{
			TailParticles[ix].Clear();
			_holdPositions[ix] = MainParticle.transform.position;
			_holdRotations[ix] = MainParticle.transform.rotation;
			TailParticles[ix].Emit(1);
		}

		private void LateUpdate()
		{
			_timer += Time.deltaTime * _effect.TimeScale;
			if (_timer > PerParticleDuration)
			{
				_curIx = (_curIx + 1) % TailParticles.Length;
				FireTailAtIndex(_curIx);
				_timer = 0f;
			}
			for (int i = 0; i < TailParticles.Length; i++)
			{
				TailParticles[i].transform.position = _holdPositions[i];
				TailParticles[i].transform.rotation = _holdRotations[i];
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}
	}
}
