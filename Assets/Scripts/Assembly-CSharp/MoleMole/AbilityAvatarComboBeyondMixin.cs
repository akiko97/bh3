using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarComboBeyondMixin : BaseAbilityMixin
	{
		private AvatarComboBeyondMixin config;

		private AvatarActor _avatarActor;

		public AbilityAvatarComboBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarComboBeyondMixin)config;
			_avatarActor = actor as AvatarActor;
		}

		public override void OnAdded()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackByCombo));
		}

		public override void OnRemoved()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackByCombo));
		}

		private int EvaluateComboStep(int combo)
		{
			int num = 0;
			for (int i = 0; i < config.ComboSteps.Length && combo >= instancedAbility.Evaluate(config.ComboSteps[i]); i++)
			{
				num++;
			}
			return num;
		}

		private void ApplyStepModifier(int step)
		{
			if (step != 0)
			{
				int num = step - 1;
				_avatarActor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierNames[num]);
			}
		}

		private void RemoveStepModifier(int step)
		{
			if (step != 0)
			{
				int num = step - 1;
				string modifierName = config.ModifierNames[num];
				_avatarActor.abilityPlugin.TryRemoveModifier(instancedAbility, modifierName);
			}
		}

		private void UpdateAttackByCombo(int from, int to)
		{
			int num = EvaluateComboStep(from);
			int num2 = EvaluateComboStep(to);
			if (num != num2)
			{
				RemoveStepModifier(num);
				ApplyStepModifier(num2);
			}
		}
	}
}
