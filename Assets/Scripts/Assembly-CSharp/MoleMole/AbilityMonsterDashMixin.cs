using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterDashMixin : BaseAbilityMixin
	{
		private MonsterDashMixin config;

		private BaseMonoMonster _monster;

		private Vector3 _startPos;

		private Vector3 _targetPos;

		private float _timer;

		public AbilityMonsterDashMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterDashMixin)config;
			_monster = entity as BaseMonoMonster;
			_timer = -1f;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			StartDash();
		}

		private void StartDash()
		{
			float dashTime = config.DashTime;
			if (dashTime != 0f)
			{
				_timer = dashTime;
				BaseMonoEntity attackTarget = entity.GetAttackTarget();
				if (!(attackTarget == null))
				{
					_startPos = entity.transform.position;
					float num = Vector3.Distance(_startPos, attackTarget.transform.position) - config.TargetDistance;
					_targetPos = _startPos + entity.transform.forward * num;
					Vector3 overrideVelocity = (_targetPos - _startPos) / dashTime;
					_monster.SetNeedOverrideVelocity(true);
					_monster.SetOverrideVelocity(overrideVelocity);
					_monster.PushHighspeedMovement();
					Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(actor.runtimeID));
					Debug.DrawLine(_startPos, _targetPos, Color.yellow, 2f);
				}
			}
		}

		private void EndDash()
		{
			_monster.SetNeedOverrideVelocity(false);
			_monster.SetOverrideVelocity(Vector3.zero);
			_monster.PopHighspeedMovement();
			_timer = -1f;
		}

		public override void Core()
		{
			if (_timer <= 0f)
			{
				return;
			}
			if (!CheckAllowFollow())
			{
				EndDash();
				return;
			}
			_timer -= Time.deltaTime * entity.TimeScale;
			if (_timer <= 0f)
			{
				EndDash();
			}
			else if (Vector3.Distance(entity.transform.position, _startPos) >= Vector3.Distance(_startPos, _targetPos))
			{
				EndDash();
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
	}
}
