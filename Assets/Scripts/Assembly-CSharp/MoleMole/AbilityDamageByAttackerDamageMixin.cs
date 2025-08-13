using MoleMole.Config;

namespace MoleMole
{
	public class AbilityDamageByAttackerDamageMixin : BaseAbilityMixin
	{
		public DamageByAttackerDamageMixin config;

		private ConfigEntityAttackProperty _attackProperty = new ConfigEntityAttackProperty();

		public AbilityDamageByAttackerDamageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DamageByAttackerDamageMixin)config;
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return PostBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool PostBeingHit(EvtBeingHit evt)
		{
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			float addedDamageValue = evt.attackData.damage * instancedAbility.Evaluate(config.DamagePercentage);
			_attackProperty.AddedDamageValue = addedDamageValue;
			_attackProperty.DamagePercentage = 0f;
			_attackProperty.AniDamageRatio = 0f;
			_attackProperty.FrameHalt = 0;
			_attackProperty.HitType = AttackResult.ActorHitType.Ailment;
			_attackProperty.HitEffect = AttackResult.AnimatorHitEffect.Normal;
			_attackProperty.RetreatVelocity = 0f;
			_attackProperty.IsAnimEventAttack = true;
			_attackProperty.IsInComboCount = false;
			bool forceSkipAttackerResolve = !actor.IsActive();
			AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(actor, _attackProperty, null, null);
			AttackPattern.SendHitEvent(actor.runtimeID, evt.sourceID, null, null, attackData, forceSkipAttackerResolve);
			actor.abilityPlugin.HandleActionTargetDispatch(config.Actions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
			return true;
		}
	}
}
