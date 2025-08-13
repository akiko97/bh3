using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAttachModifierToAreaDetectionMixin : BaseAbilityMixin
	{
		private AttachModifierToAreaDetectionMixin config;

		private float _delayTimer;

		private bool _hasEntity;

		private BaseMonoEntity _bufferEntity;

		public AbilityAttachModifierToAreaDetectionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AttachModifierToAreaDetectionMixin)config;
		}

		public override void OnAdded()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Combine(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnActiveChanged));
			_delayTimer = config.Delay;
		}

		public override void OnRemoved()
		{
			BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
			baseMonoAbilityEntity.onActiveChanged = (Action<bool>)Delegate.Remove(baseMonoAbilityEntity.onActiveChanged, new Action<bool>(OnActiveChanged));
			RemoveModifiers();
		}

		public override void Core()
		{
			if (!entity.IsActive())
			{
				return;
			}
			_delayTimer -= Time.deltaTime * entity.TimeScale;
			if (_delayTimer > 0f)
			{
				return;
			}
			if (_hasEntity)
			{
				if (_bufferEntity == null || Vector3.Distance(actor.entity.XZPosition, _bufferEntity.XZPosition) > instancedAbility.Evaluate(config.Radius))
				{
					_bufferEntity = null;
					if (!FindEntity())
					{
						_hasEntity = false;
						if (!config.IsInvert)
						{
							RemoveModifiers();
						}
						else
						{
							ApplyModifiers();
						}
					}
				}
			}
			else if (FindEntity())
			{
				_hasEntity = true;
				if (!config.IsInvert)
				{
					ApplyModifiers();
				}
				else
				{
					RemoveModifiers();
				}
			}
			_delayTimer = config.Delay;
		}

		private void OnActiveChanged(bool active)
		{
			if (!active)
			{
				_hasEntity = false;
				_bufferEntity = null;
				RemoveModifiers();
			}
			else
			{
				_delayTimer = config.Delay;
			}
		}

		private bool FindEntity()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[i];
				if (Vector3.Distance(actor.entity.XZPosition, baseMonoMonster.XZPosition) <= instancedAbility.Evaluate(config.Radius))
				{
					_bufferEntity = baseMonoMonster;
					return true;
				}
			}
			return false;
		}

		private void ApplyModifiers()
		{
			for (int i = 0; i < config.ModifierNames.Length; i++)
			{
				string modifierName = config.ModifierNames[i];
				actor.abilityPlugin.ApplyModifier(instancedAbility, modifierName);
			}
		}

		private void RemoveModifiers()
		{
			for (int i = 0; i < config.ModifierNames.Length; i++)
			{
				string modifierName = config.ModifierNames[i];
				actor.abilityPlugin.TryRemoveModifier(instancedAbility, modifierName);
			}
		}
	}
}
