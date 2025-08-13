using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarLimitSkillByStaminaMixin : BaseAbilityMixin
	{
		private AvatarLimitSkillByStaminaMixin config;

		private float _stamina;

		private bool _maskSkill;

		private bool _isSkilling;

		protected string _currentSkillID;

		private AvatarActor _avatarActor;

		private BaseMonoAnimatorEntity _animatorEntity;

		private DisplayValue<float> _staminaDisplay;

		public AbilityAvatarLimitSkillByStaminaMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarLimitSkillByStaminaMixin)config;
			_animatorEntity = (BaseMonoAnimatorEntity)entity;
			_avatarActor = (AvatarActor)actor;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_stamina = config.StaminaMax;
		}

		public override void OnAdded()
		{
			BaseMonoAnimatorEntity animatorEntity = _animatorEntity;
			animatorEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(animatorEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			if (config.ShowStaminaBar)
			{
				_staminaDisplay = actor.abilityPlugin.CreateOrGetDisplayFloat("Stamina", 0f, 1f, 1f);
			}
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (config.SkillID == to)
			{
				DelegateUtils.UpdateField(ref _stamina, _stamina - config.SkillHeatCost, UpdateStaminaDisplayValue);
				_isSkilling = true;
			}
			if (config.SkillID == from)
			{
				_isSkilling = false;
			}
		}

		public override void Core()
		{
			base.Core();
			if (_stamina < config.StaminaMax && !_isSkilling)
			{
				DelegateUtils.UpdateField(ref _stamina, Mathf.Min(_stamina + entity.TimeScale * Time.deltaTime * config.ResumeSpeed, config.StaminaMax), UpdateStaminaDisplayValue);
			}
			if (_stamina < config.SkillHeatCost && !_maskSkill)
			{
				_avatarActor.entity.MaskTrigger(config.MaskTriggerID);
				_maskSkill = true;
			}
			if (_stamina >= config.SkillHeatCost && _maskSkill)
			{
				_avatarActor.entity.UnmaskTrigger(config.MaskTriggerID);
				_maskSkill = false;
			}
		}

		private void UpdateStaminaDisplayValue(float fromStamina, float toStamina)
		{
			if (_staminaDisplay != null)
			{
				_staminaDisplay.Pub(Mathf.Clamp01(toStamina / config.StaminaMax));
			}
		}
	}
}
