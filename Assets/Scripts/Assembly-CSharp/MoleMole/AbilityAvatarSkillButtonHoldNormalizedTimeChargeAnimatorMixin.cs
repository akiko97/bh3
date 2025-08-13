using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin : BaseAbilityAvatarSkillButtonHoldChargeMixin
	{
		private AvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin config;

		private int _thersholdIndex;

		public AbilityAvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSkillButtonHoldNormalizedTimeChargeAnimatorMixin)config;
		}

		public override void OnAdded()
		{
			base.OnAdded();
		}

		protected override void OnBeforeToInLoop()
		{
			_thersholdIndex = 0;
		}

		protected override void UpdateInLoop()
		{
			if (config.ChargeEndNormalizeTimeThershold == null)
			{
				return;
			}
			float currentNormalizedTime = entity.GetCurrentNormalizedTime();
			float[] array = config.ChargeEndNormalizeTimeThershold[_loopIx];
			for (int i = _thersholdIndex; i < array.Length; i++)
			{
				if (currentNormalizedTime < array[i])
				{
					if (_thersholdIndex != i)
					{
						_thersholdIndex = i;
					}
					break;
				}
			}
		}

		protected override void OnInLoopToAfter()
		{
			if (config.ChargeEndNormalizeTimeThershold != null)
			{
				int num = _loopIx;
				if (_loopIx == _loopCount)
				{
					num--;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.ChargeEndNormalizeTimeActions[num][_thersholdIndex], instancedAbility, instancedModifier, actor, null);
			}
		}

		protected override void OnMoveingToNextLoop(bool endLoop)
		{
		}

		protected override bool ShouldMoveToNextLoop()
		{
			return entity.GetCurrentNormalizedTime() > config.ChargeLoopNormalizeTimeEnds[_loopIx];
		}
	}
}
