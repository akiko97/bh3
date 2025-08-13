using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonHoldChargeMixin : BaseAbilityAvatarSkillButtonHoldChargeMixin
	{
		private AvatarSkillButtonHoldChargeAnimatorMixin config;

		private EntityTimer _chargeTimer;

		public AbilityAvatarSkillButtonHoldChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonHoldChargeAnimatorMixin)config;
			_chargeTimer = new EntityTimer();
			_chargeTimeRatio = instancedAbility.Evaluate(this.config.ChargeTimeRatio);
		}

		public override void OnAdded()
		{
			base.OnAdded();
			_chargeTimer.Reset(false);
		}

		protected override void OnBeforeToInLoop()
		{
			_chargeTimer.timespan = config.ChargeLoopDurations[_loopIx] * _chargeTimeRatio;
			_chargeTimer.Reset(true);
		}

		protected override void OnInLoopToAfter()
		{
			_chargeTimer.Reset(false);
		}

		protected override void UpdateInLoop()
		{
			_chargeTimer.Core(actor.entity.GetProperty("Entity_AttackSpeed") + 1f);
		}

		protected override bool ShouldMoveToNextLoop()
		{
			return _chargeTimer.isTimeUp;
		}

		protected override void OnMoveingToNextLoop(bool endLoop)
		{
			if (endLoop)
			{
				_chargeTimer.Reset(false);
				return;
			}
			_chargeTimer.timespan = config.ChargeLoopDurations[_loopIx] * _chargeTimeRatio;
			_chargeTimer.Reset(true);
		}
	}
}
