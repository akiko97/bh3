using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterSwordDodgeMixin : BaseAbilityMixin
	{
		private MonsterSwordDodgeMixin config;

		public AbilityMonsterSwordDodgeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterSwordDodgeMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				OnPostBeingHit((EvtBeingHit)evt);
			}
			return true;
		}

		private AttackData CreateNoDamageAttack()
		{
			AttackData attackData = new AttackData();
			attackData.damage = 0f;
			attackData.isInComboCount = false;
			attackData.attackerAniDamageRatio = 2f;
			attackData.aniDamageRatio = 2f;
			attackData.hitLevel = AttackResult.ActorHitLevel.Normal;
			attackData.hitEffect = AttackResult.AnimatorHitEffect.Normal;
			attackData.resolveStep = AttackData.AttackDataStep.AttackerResolved;
			return attackData;
		}

		private void OnPostBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.attackerClass == EntityClass.ShortSworder && evt.attackData.attackerAniDamageRatio < config.NoDodgeAttackRatio && evt.attackData.attackerNature != EntityNature.Psycho && (config.DodgeRatio > Random.value || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp))
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.DodgeActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.sourceID, actor.runtimeID, null, CreateNoDamageAttack()));
			}
		}
	}
}
