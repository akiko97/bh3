using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarModifyPropertyByComboMixin : BaseAbilityMixin
	{
		private AvatarModifyPropertyByCombo config;

		private float _perComboDelta;

		private int _propertyIx;

		public AbilityAvatarModifyPropertyByComboMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarModifyPropertyByCombo)config;
			if (this.config.MaxValueCombo == null)
			{
				_perComboDelta = instancedAbility.Evaluate(this.config.PerComboDelta);
			}
			else
			{
				_perComboDelta = instancedAbility.Evaluate(this.config.MaxValue) / (float)instancedAbility.Evaluate(this.config.MaxValueCombo);
			}
		}

		public override void OnAdded()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackSpeedByCombo));
			_propertyIx = actor.PushProperty(config.Property, 0f);
			UpdateAttackSpeedByCombo(0, Singleton<LevelManager>.Instance.levelActor.levelCombo);
		}

		public override void OnRemoved()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackSpeedByCombo));
			actor.PopProperty(config.Property, _propertyIx);
		}

		private void UpdateAttackSpeedByCombo(int from, int to)
		{
			float value = Mathf.Clamp(instancedAbility.Evaluate(config.Initial) + (float)to * _perComboDelta, instancedAbility.Evaluate(config.MinValue), instancedAbility.Evaluate(config.MaxValue));
			actor.SetPropertyByStackIndex(config.Property, _propertyIx, value);
		}
	}
}
