using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterForceFollowMixin : BaseAbilityMixin
	{
		private MonsterForceFollowMixin config;

		private bool _isFollow;

		private bool _isApproching;

		private bool _isKeepingAway;

		private BaseMonoMonster _monster;

		private BaseMonoEntity _targetEntity;

		public AbilityMonsterForceFollowMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterForceFollowMixin)config;
			_monster = entity as BaseMonoMonster;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			StartFollow();
		}

		private void StartFollow()
		{
			_targetEntity = entity.GetAttackTarget();
			Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(actor.runtimeID));
			_isFollow = true;
			_isApproching = false;
			_isKeepingAway = false;
		}

		private void EndFollow()
		{
			_monster.SetHasAdditiveVelocity(false);
			_monster.SetAdditiveVelocity(Vector3.zero);
			_isFollow = false;
			_isApproching = false;
			_isKeepingAway = false;
		}

		public override void Core()
		{
			if (!_isFollow)
			{
				return;
			}
			if (!CheckAllowFollow())
			{
				EndFollow();
				return;
			}
			if (!CheckShouldFollow())
			{
				_monster.SetHasAdditiveVelocity(false);
				_monster.SetAdditiveVelocity(Vector3.zero);
				return;
			}
			float num = Time.deltaTime * entity.TimeScale;
			if (num != 0f)
			{
				Vector3 normalized = (entity.transform.position - _targetEntity.transform.position).normalized;
				Vector3 vector = _targetEntity.transform.position + normalized * config.TargetDistance;
				Vector3 additiveVelocity = (vector - entity.transform.position) / num;
				if (additiveVelocity.magnitude > config.FollowSpeed)
				{
					additiveVelocity = additiveVelocity.normalized * config.FollowSpeed;
				}
				additiveVelocity.y = 0f;
				_monster.SetHasAdditiveVelocity(true);
				_monster.SetAdditiveVelocity(additiveVelocity);
			}
		}

		private bool CheckAllowFollow()
		{
			MonsterActor monsterActor = actor as MonsterActor;
			if (monsterActor == null)
			{
				return false;
			}
			string currentSkillID = monsterActor.monster.CurrentSkillID;
			if (string.IsNullOrEmpty(currentSkillID))
			{
				return false;
			}
			if (config.SkillIDs == null)
			{
				return false;
			}
			float currentNormalizedTime = _monster.GetCurrentNormalizedTime();
			if (currentNormalizedTime > config.NormalizeTimeEnd)
			{
				return false;
			}
			if (actor.abilityState.ContainsState(AbilityState.Paralyze) || actor.abilityState.ContainsState(AbilityState.Stun))
			{
				return false;
			}
			int i = 0;
			for (int num = config.SkillIDs.Length; i < num; i++)
			{
				if (currentSkillID == config.SkillIDs[i])
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckShouldFollow()
		{
			if (_targetEntity == null)
			{
				return false;
			}
			float num = Vector3.Distance(entity.transform.position, _targetEntity.transform.position);
			if (num > ((!_isApproching) ? config.MaxDistance : config.TargetDistance))
			{
				_isApproching = true;
				_isKeepingAway = false;
				return true;
			}
			if (num < ((!_isKeepingAway) ? config.MinDistance : config.TargetDistance))
			{
				_isApproching = false;
				_isKeepingAway = true;
				return true;
			}
			return false;
		}
	}
}
