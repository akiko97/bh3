using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarWeaponOverHeatMixin : BaseAbilityMixin
	{
		private AvatarWeaponOverHeatMixin config;

		private float _heat;

		private bool _isOverHeat;

		private bool _isAttacking;

		protected string _currentSkillID;

		private float _coolSpeed;

		private AvatarActor _avatarActor;

		private float _currentHeatAddSpeed;

		private BaseMonoAnimatorEntity _animatorEntity;

		private DisplayValue<float> _overheatValueDisplay;

		private DisplayValue<float> _isOverheatDisplay;

		public AbilityAvatarWeaponOverHeatMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarWeaponOverHeatMixin)config;
			_animatorEntity = (BaseMonoAnimatorEntity)entity;
			_avatarActor = (AvatarActor)actor;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_heat = 0f;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(actor.runtimeID);
			BaseMonoAnimatorEntity animatorEntity = _animatorEntity;
			animatorEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(animatorEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			_isOverheatDisplay = actor.abilityPlugin.CreateOrGetDisplayFloat("IsOverheat", 0f, 1f, (!_isOverHeat) ? 0f : 1f);
			_overheatValueDisplay = actor.abilityPlugin.CreateOrGetDisplayFloat("OverheatRatio", 0f, 1f, _heat / config.OverHeatMax);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackStart>(actor.runtimeID);
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtAttackStart)
			{
				return OnSkillStart((EvtAttackStart)evt);
			}
			return false;
		}

		private bool OnSkillStart(EvtAttackStart evt)
		{
			if (config.SkillIDs != null)
			{
				for (int i = 0; i < config.SkillIDs.Length; i++)
				{
					if (config.SkillIDs[i] == evt.skillID && !_animatorEntity.ContainAnimEventPredicate(config.IgnorePredicate))
					{
						DelegateUtils.UpdateField(ref _heat, _heat + config.SkillHeatAdds[i], UpdateOverheatDisplayValue);
						_coolSpeed = 0f;
						break;
					}
				}
			}
			return true;
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (config.ContinuousSkillIDs != null)
			{
				for (int i = 0; i < config.ContinuousSkillIDs.Length; i++)
				{
					if (from == config.ContinuousSkillIDs[i])
					{
						_currentHeatAddSpeed = 0f;
						_currentSkillID = null;
						_isAttacking = false;
					}
					if (to == config.ContinuousSkillIDs[i])
					{
						_currentHeatAddSpeed = config.ContinuousHeatAddSpeed[i];
						_currentSkillID = config.ContinuousSkillIDs[i];
						_isAttacking = true;
					}
				}
			}
			if (config.NoCoolSkillIDs == null)
			{
				return;
			}
			for (int j = 0; j < config.NoCoolSkillIDs.Length; j++)
			{
				if (from == config.NoCoolSkillIDs[j])
				{
					_isAttacking = false;
				}
				if (to == config.NoCoolSkillIDs[j])
				{
					_isAttacking = true;
				}
			}
		}

		public override void Core()
		{
			base.Core();
			if (_currentHeatAddSpeed != 0f)
			{
				DelegateUtils.UpdateField(ref _heat, _heat + _currentHeatAddSpeed * entity.TimeScale * Time.deltaTime * instancedAbility.Evaluate(config.ContinuousHeatSpeedRatio), UpdateOverheatDisplayValue);
				_coolSpeed = 0f;
			}
			if (_heat > 0f && _animatorEntity.ContainAnimEventPredicate(config.IgnorePredicate))
			{
				DelegateUtils.UpdateField(ref _heat, 0f, UpdateOverheatDisplayValue);
			}
			if (_heat > config.OverHeatMax && !_isOverHeat)
			{
				_isOverHeat = true;
				_isOverheatDisplay.Pub(1f);
				actor.abilityPlugin.HandleActionTargetDispatch(config.OverHeatActions, instancedAbility, instancedModifier, actor, null);
				_avatarActor.SetMuteSkill(config.OverHeatButtonSkillID, true);
				Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(actor.runtimeID).IsLockDirection = false;
			}
			if (_isOverHeat)
			{
				if (_heat > 0f)
				{
					if (_coolSpeed < config.OverHeatCoolSpeed)
					{
						_coolSpeed += Time.deltaTime * config.OverHeatCoolSpeed / config.ToMaxCoolSpeedTime;
					}
					DelegateUtils.UpdateField(ref _heat, _heat - entity.TimeScale * Time.deltaTime * _coolSpeed, UpdateOverheatDisplayValue);
				}
				else
				{
					_isOverHeat = false;
					_isOverheatDisplay.Pub(0f);
					actor.abilityPlugin.HandleActionTargetDispatch(config.CoolDownActions, instancedAbility, instancedModifier, actor, null);
					_avatarActor.SetMuteSkill(config.OverHeatButtonSkillID, false);
				}
			}
			else if (_heat > 0f && !_isAttacking)
			{
				if (_coolSpeed < config.CoolSpeed)
				{
					_coolSpeed += Time.deltaTime * config.CoolSpeed / config.ToMaxCoolSpeedTime;
				}
				DelegateUtils.UpdateField(ref _heat, _heat - entity.TimeScale * Time.deltaTime * _coolSpeed, UpdateOverheatDisplayValue);
			}
		}

		private void UpdateOverheatDisplayValue(float fromHeat, float toHeat)
		{
			_overheatValueDisplay.Pub(Mathf.Clamp01(toHeat / config.OverHeatMax));
		}
	}
}
