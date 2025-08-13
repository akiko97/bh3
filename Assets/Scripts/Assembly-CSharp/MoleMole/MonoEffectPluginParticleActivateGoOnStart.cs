using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginParticleActivateGoOnStart : BaseMonoEffectPlugin
	{
		[Header("Target Game Objects that needs to be set active on Start()")]
		public GameObject[] targetGOs;

		private bool _hasStarted;

		public override void Setup()
		{
			if (_hasStarted)
			{
				for (int i = 0; i < targetGOs.Length; i++)
				{
					targetGOs[i].gameObject.SetActive(true);
				}
			}
		}

		protected override void Awake()
		{
			for (int i = 0; i < targetGOs.Length; i++)
			{
				targetGOs[i].gameObject.SetActive(false);
			}
		}

		private void Start()
		{
			for (int i = 0; i < targetGOs.Length; i++)
			{
				targetGOs[i].gameObject.SetActive(true);
			}
			_hasStarted = true;
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}

		private void OnDisable()
		{
			for (int i = 0; i < targetGOs.Length; i++)
			{
				targetGOs[i].gameObject.SetActive(false);
			}
		}
	}
}
