using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterFlashBombMixin : BaseAbilityMixin
	{
		private enum State
		{
			Idle = 0,
			Running = 1
		}

		private MonsterFlashBombMixin config;

		private float _delayTimer;

		private Vector3 _position;

		private State _state;

		private BaseMonoMonster _monster;

		public AbilityMonsterFlashBombMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterFlashBombMixin)config;
			_monster = (BaseMonoMonster)entity;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_delayTimer = config.DelayTime;
			_position = entity.XZPosition;
			_state = State.Running;
			FireMixinEffect(config.TriggerEffect, entity);
		}

		public override void Core()
		{
			base.Core();
			if (_state != State.Idle)
			{
				_delayTimer -= entity.TimeScale * Time.deltaTime;
				if (_delayTimer <= 0f)
				{
					DoFlashExplode();
					_state = State.Idle;
				}
			}
		}

		private void DoFlashExplode()
		{
			bool flag = false;
			foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				if (!(allAvatar == null) && allAvatar.IsActive() && Vector3.Angle(_position - allAvatar.XZPosition, allAvatar.transform.forward) <= config.Angle)
				{
					BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(allAvatar.GetRuntimeID());
					string[] modifierNames = config.ModifierNames;
					foreach (string modifierName in modifierNames)
					{
						baseAbilityActor.abilityPlugin.ApplyModifier(instancedAbility, modifierName);
					}
					if (Singleton<AvatarManager>.Instance.IsLocalAvatar(allAvatar.GetRuntimeID()))
					{
						flag = true;
						actor.abilityPlugin.HandleActionTargetDispatch(config.SuccessActions, instancedAbility, instancedModifier, null, null);
					}
				}
			}
			if (!flag)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.FailActions, instancedAbility, instancedModifier, null, null);
			}
		}
	}
}
