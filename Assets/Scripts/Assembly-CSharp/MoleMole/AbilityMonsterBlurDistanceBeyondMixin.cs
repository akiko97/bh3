using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterBlurDistanceBeyondMixin : BaseAbilityMixin
	{
		private MonsterBlurDistanceBeyondMixin config;

		private BaseAbilityActor _abilityActor;

		private bool _inDistance;

		public AbilityMonsterBlurDistanceBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterBlurDistanceBeyondMixin)config;
			_abilityActor = actor;
			_inDistance = true;
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
		}

		public override void Core()
		{
			bool flag = Vector3.Distance(Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition, _abilityActor.entity.XZPosition) < instancedAbility.Evaluate(config.Distance);
			if (flag != _inDistance)
			{
				ControlModifierByIsInDistance(flag);
			}
		}

		private void ControlModifierByIsInDistance(bool isInDistance)
		{
			_inDistance = isInDistance;
			if (_inDistance)
			{
				SetMonsterBlur(false);
				RemoveModifiers();
			}
			else
			{
				SetMonsterBlur(true);
				AddModifiers();
			}
		}

		private void AddModifiers()
		{
			if (config.ModifierNames != null && config.ModifierNames.Length > 0)
			{
				int i = 0;
				for (int num = config.ModifierNames.Length; i < num; i++)
				{
					_abilityActor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
		}

		private void RemoveModifiers()
		{
			if (config.ModifierNames != null && config.ModifierNames.Length > 0)
			{
				int i = 0;
				for (int num = config.ModifierNames.Length; i < num; i++)
				{
					_abilityActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
		}

		private void SetMonsterBlur(bool isBlur)
		{
			if (_abilityActor != null)
			{
				_abilityActor.entity.SetCountedIsGhost(isBlur);
				_abilityActor.entity.SetCountedDenySelect(isBlur);
			}
		}
	}
}
