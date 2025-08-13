using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityTriggerBullet : BaseActor
	{
		public MonoTriggerBullet triggerBullet;

		private BaseAbilityActor _owner;

		public override void Init(BaseMonoEntity entity)
		{
			triggerBullet = (MonoTriggerBullet)entity;
			runtimeID = triggerBullet.GetRuntimeID();
		}

		public void Setup(BaseAbilityActor owner, float speed, MixinTargetting targetting, bool ignoreTimeScale, float aliveDuration = -1f)
		{
			_owner = owner;
			triggerBullet.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(_owner.runtimeID, targetting));
			triggerBullet.speed = speed;
			triggerBullet.speedAdd = Vector3.zero;
			triggerBullet.AliveDuration = aliveDuration;
			triggerBullet.IgnoreTimeScale = ignoreTimeScale;
		}

		public void Kill()
		{
			triggerBullet.SetDied();
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID));
			triggerBullet.enabled = false;
		}
	}
}
