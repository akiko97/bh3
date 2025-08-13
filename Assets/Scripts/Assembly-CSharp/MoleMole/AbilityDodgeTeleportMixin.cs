using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityDodgeTeleportMixin : BaseAbilityMixin
	{
		private const float SCREEN_EDGE_RATIO = 0.2f;

		private DodgeTeleportMixin config;

		protected float _cdTimer;

		private float _timer;

		private Vector3 _speed;

		private bool _fadingOut;

		public AbilityDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DodgeTeleportMixin)config;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				OnPostBeingHit((EvtBeingHit)evt);
			}
			return true;
		}

		protected virtual void OnPostBeingHit(EvtBeingHit evtHit)
		{
			if (config.CanHitTrigger && evtHit.attackData.hitType == AttackResult.ActorHitType.Ranged && CheckAllowTeleport())
			{
				IgnoreHitDamage(evtHit);
				Teleport();
				ClearTargetAttackTarget(evtHit.sourceID);
			}
			if (_timer > 0f)
			{
				IgnoreHitDamage(evtHit);
			}
		}

		protected virtual void ClearTargetAttackTarget(uint sourceID)
		{
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (CheckAllowTeleport())
			{
				Teleport();
			}
		}

		private void Teleport()
		{
			actor.abilityPlugin.HandleActionTargetDispatch(config.TeleportActions, instancedAbility, instancedModifier, null, null);
			Vector3 teleportPosition = GetTeleportPosition(config.DirectionMode, instancedAbility.Evaluate(config.Angle), config.Distance);
			TeleportTo(teleportPosition, config.TeleportTime);
			if (instancedAbility.Evaluate(config.CDTime) > 0f)
			{
				_cdTimer = instancedAbility.Evaluate(config.CDTime);
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(actor.runtimeID));
		}

		protected void IgnoreHitDamage(EvtBeingHit evtHit)
		{
			if (evtHit != null)
			{
				evtHit.attackData.Reject(AttackResult.RejectType.RejectAll);
			}
		}

		public override void Core()
		{
			float num = Time.deltaTime * entity.TimeScale;
			if (_cdTimer > 0f)
			{
				_cdTimer -= num;
			}
			if (!(_timer > 0f))
			{
				return;
			}
			_timer -= num;
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = entity as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity != null)
			{
				if (_timer <= 0f)
				{
					baseMonoAnimatorEntity.SetHasAdditiveVelocity(false);
					baseMonoAnimatorEntity.SetAdditiveVelocity(Vector3.zero);
					baseMonoAnimatorEntity.PopHighspeedMovement();
				}
				if (config.NeedFade)
				{
					ProcessFade(baseMonoAnimatorEntity);
				}
			}
		}

		private void ProcessFade(BaseMonoAnimatorEntity animatorEntity)
		{
			if (_fadingOut && _timer < 0.03f)
			{
				_fadingOut = false;
				animatorEntity.SetTrigger("DodgeFadeIn");
			}
		}

		protected Vector3 GetTeleportPosition(TeleportDirectionMode mode, float angle, float distance)
		{
			switch (mode)
			{
			case TeleportDirectionMode.UseAngle:
			case TeleportDirectionMode.CameraCenter:
			case TeleportDirectionMode.UseSteerAngle:
			{
				Vector3 normalized = GetDirection(config.DirectionMode, instancedAbility.Evaluate(config.Angle));
				return entity.transform.position + normalized * distance;
			}
			case TeleportDirectionMode.FromTarget:
			{
				BaseMonoEntity attackTarget = entity.GetAttackTarget();
				if (attackTarget == null)
				{
					return entity.transform.position;
				}
				Vector3 normalized = (entity.transform.position - attackTarget.transform.position).normalized;
				return attackTarget.transform.position + normalized * distance;
			}
			case TeleportDirectionMode.SpawnPoint:
				return GetSpawnPointPos(config.SpawnPoint);
			case TeleportDirectionMode.SpawnPointByDistance:
				return GetSpawnPointPos(config.Distance);
			case TeleportDirectionMode.SpawnPointByDistanceFromTarget:
				return GetSpawnPointPos(config.Distance, true);
			default:
				return entity.transform.position;
			}
		}

		protected virtual Vector3 GetDirection(TeleportDirectionMode mode, float angle)
		{
			Vector3 vector = Vector3.forward;
			switch (mode)
			{
			case TeleportDirectionMode.UseAngle:
				vector = Quaternion.Euler(0f, angle, 0f) * entity.transform.forward;
				break;
			case TeleportDirectionMode.CameraCenter:
			{
				bool flag = false;
				float x = Camera.main.WorldToScreenPoint(entity.transform.position).x;
				if (x < (float)Screen.width * 0.2f || x > (float)Screen.width * 0.8f)
				{
					flag = true;
				}
				if (flag)
				{
					Vector3 xZPosition = entity.XZPosition;
					Vector3 position = Camera.main.transform.position;
					position = new Vector3(position.x, 0f, position.z);
					Vector3 forward = Camera.main.transform.forward;
					forward = new Vector3(forward.x, 0f, forward.z);
					Vector3 vector2 = position - xZPosition;
					vector = ((!(Vector3.Cross(vector2, forward).y > 0f)) ? (Quaternion.Euler(0f, 270f, 0f) * vector2) : (Quaternion.Euler(0f, 90f, 0f) * vector2));
				}
				else
				{
					vector = ((!(Random.value > 0.5f)) ? (Quaternion.Euler(0f, 270f, 0f) * entity.transform.forward) : (Quaternion.Euler(0f, 90f, 0f) * entity.transform.forward));
				}
				break;
			}
			}
			return vector.normalized;
		}

		private Vector3 GetSpawnPointPos(string spawnName)
		{
			MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
			int namedSpawnPointIx = stageEnv.GetNamedSpawnPointIx(spawnName);
			return stageEnv.spawnPoints[namedSpawnPointIx].transform.position;
		}

		private Vector3 GetSpawnPointPos(float distance, bool fromTarget = false)
		{
			Vector3 position = entity.transform.position;
			if (fromTarget)
			{
				BaseMonoEntity attackTarget = entity.GetAttackTarget();
				if (attackTarget != null)
				{
					position = attackTarget.transform.position;
				}
			}
			MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
			int num = -1;
			float num2 = 100f;
			for (int i = 0; i < Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length; i++)
			{
				Vector3 position2 = stageEnv.spawnPoints[i].transform.position;
				float num3 = Mathf.Abs(Vector3.Distance(position2, position) - distance);
				if (num3 < num2)
				{
					num = i;
					num2 = num3;
				}
			}
			return stageEnv.spawnPoints[num].transform.position;
		}

		protected void TeleportTo(Vector3 position, float time)
		{
			_timer = time;
			_speed = (position - entity.gameObject.transform.position) / _timer;
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = entity as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity != null)
			{
				if (config.NeedFade)
				{
					baseMonoAnimatorEntity.SetTrigger("DodgeFadeOut");
					_fadingOut = true;
				}
				baseMonoAnimatorEntity.SetHasAdditiveVelocity(true);
				baseMonoAnimatorEntity.SetAdditiveVelocity(_speed);
				baseMonoAnimatorEntity.PushHighspeedMovement();
			}
		}

		protected bool CheckAllowTeleport()
		{
			if (_cdTimer > 0f)
			{
				return false;
			}
			if (actor == null)
			{
				return false;
			}
			string currentSkillID = actor.entity.CurrentSkillID;
			if (string.IsNullOrEmpty(currentSkillID))
			{
				return false;
			}
			if (config.TeleportSkillIDs == null)
			{
				return false;
			}
			if (actor.abilityState.ContainsState(AbilityState.Paralyze) || actor.abilityState.ContainsState(AbilityState.Stun))
			{
				return false;
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, actor, null))
			{
				return false;
			}
			int i = 0;
			for (int num = config.TeleportSkillIDs.Length; i < num; i++)
			{
				if (currentSkillID == config.TeleportSkillIDs[i])
				{
					return true;
				}
			}
			return false;
		}
	}
}
