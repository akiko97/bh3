using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAnimatorParamLerpMixin : BaseAbilityMixin
	{
		private AnimatorParamLerpMixin config;

		private bool _isInLerping;

		private float _lerpStartValue;

		private float _lerpEndValue;

		public AbilityAnimatorParamLerpMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AnimatorParamLerpMixin)config;
			_isInLerping = false;
		}

		public override void OnAdded()
		{
			BaseMonoAnimatorEntity obj = actor.entity as BaseMonoAnimatorEntity;
			obj.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(obj.onCurrentSkillIDChanged, new Action<string, string>(OnSkillIDChanged));
		}

		public override void OnRemoved()
		{
			BaseMonoAnimatorEntity obj = actor.entity as BaseMonoAnimatorEntity;
			obj.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(obj.onCurrentSkillIDChanged, new Action<string, string>(OnSkillIDChanged));
		}

		private void OnSkillIDChanged(string skillOld, string skillNew)
		{
			if (_isInLerping)
			{
				_isInLerping = false;
			}
		}

		public override void Core()
		{
			if (_isInLerping)
			{
				float currentNormalizedTime = (actor.entity as BaseMonoAnimatorEntity).GetCurrentNormalizedTime();
				float num = Mathf.Clamp(currentNormalizedTime, config.LerpStartNormalizedTime, config.LerpEndNormalizedTime);
				float t = (num - config.LerpStartNormalizedTime) / (config.LerpEndNormalizedTime - config.LerpStartNormalizedTime);
				float animatorParam = Mathf.Lerp(_lerpStartValue, _lerpEndValue, t);
				SetAnimatorParam(animatorParam);
			}
		}

		private void SetAnimatorParam(float value)
		{
			(actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(config.AnimatorParamName, value);
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (config.LerpStartValue > 0f)
			{
				_lerpStartValue = config.LerpStartValue;
			}
			else
			{
				_lerpStartValue = (actor.entity as BaseMonoAnimatorEntity).GetLocomotionFloat(config.AnimatorParamName);
			}
			_lerpEndValue = config.LerpEndValue;
			_isInLerping = true;
		}
	}
}
