namespace MoleMole
{
	public class MPAvatarActorPlugin : BaseMPAbilityActorPlugin
	{
		protected AvatarIdentity _identity;

		protected AvatarActor _actor;

		public MPAvatarActorPlugin(BaseActor actor)
		{
			_actor = (AvatarActor)actor;
		}

		public void SetupIdentity(AvatarIdentity identity)
		{
			_identity = identity;
			Setup(_actor, identity);
		}

		protected override bool OnRemoteReplicatedEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnRemoteBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnRemoteBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (evt.attackData.hitCollision == null)
			{
				_actor.AmendHitCollision(evt.attackData);
			}
			evt.attackData.resolveStep = AttackData.AttackDataStep.FinalResolved;
			float num = (float)_actor.HP - evt.resolvedDamage;
			if (num <= 0f)
			{
				num = 0f;
			}
			DelegateUtils.UpdateField(ref _actor.HP, num, num - (float)_actor.HP, _actor.onHPChanged);
			_actor.FireAttackDataEffects(evt.attackData);
			_actor.AbilityBeingHit(evt);
			_actor.BeingHit(evt.attackData, evt.beHitEffect, evt.sourceID);
			return true;
		}
	}
}
