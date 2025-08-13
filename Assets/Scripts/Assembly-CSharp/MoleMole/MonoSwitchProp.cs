using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoSwitchProp : MonoHitableProp
	{
		[Header("Alive Transform")]
		public Transform AliveTransform;

		[Header("Killed Transform")]
		public Transform KilledTransform;

		private bool _isActive;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			_isActive = true;
			AliveTransform.gameObject.SetActive(true);
			KilledTransform.gameObject.SetActive(false);
			Singleton<PropObjectManager>.Instance.RegisterDestroyOnStageChange(runtimeID);
		}

		public override void SetDied(KillEffect killEffect)
		{
			_isActive = false;
			hitbox.enabled = false;
			SetCountedDenySelect(true, true);
			AliveTransform.gameObject.SetActive(false);
			KilledTransform.gameObject.SetActive(true);
			if (config.PropArguments.OnKillEffectPattern != null)
			{
				FireEffect(config.PropArguments.OnKillEffectPattern);
			}
		}

		public override bool IsActive()
		{
			return _isActive;
		}
	}
}
