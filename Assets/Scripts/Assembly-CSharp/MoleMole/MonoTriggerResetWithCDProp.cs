using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Collider))]
	public class MonoTriggerResetWithCDProp : MonoTriggerProp
	{
		public float ResetCD = 0.5f;

		private float _CDTimer;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			_CDTimer = ResetCD;
		}

		protected override void Update()
		{
			base.Update();
			_CDTimer -= Time.deltaTime;
			if (_CDTimer <= 0f)
			{
				_insideColliders.Clear();
				_triggerCollider.enabled = false;
				_triggerCollider.enabled = true;
				_CDTimer = ResetCD;
			}
		}
	}
}
