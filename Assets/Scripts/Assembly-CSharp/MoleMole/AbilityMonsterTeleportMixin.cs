using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterTeleportMixin : BaseAbilityMixin
	{
		private MonsterTeleportMixin config;

		private float _baselineDistance;

		private float _teleportInterval;

		private bool _isTeleporting;

		private EntityTimer _teleportTimer;

		private Vector3 _teleportOffset;

		private BaseMonoMonster _monster;

		private RaycastHit _teleportHit;

		public AbilityMonsterTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterTeleportMixin)config;
			_baselineDistance = instancedAbility.Evaluate(this.config.BaselineDistance);
			_monster = (BaseMonoMonster)entity;
			_isTeleporting = false;
			_teleportInterval = this.config.TeleportInverval;
			_teleportTimer = new EntityTimer(_teleportInterval);
			_teleportTimer.Reset(false);
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			float num = 1f;
			if (evt.abilityArgument != null)
			{
				num = (float)evt.abilityArgument;
			}
			float signedDistance = num * _baselineDistance;
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.IdleOrMovement))
			{
				Teleport(signedDistance);
			}
		}

		private void Teleport(float signedDistance)
		{
			float num = Mathf.Sign(signedDistance);
			float num2 = Mathf.Abs(signedDistance);
			Vector3 xZPosition = _monster.XZPosition;
			xZPosition.y = 1.1f;
			Vector3 vector = ((!(_monster.AttackTarget != null)) ? _monster.FaceDirection : (_monster.AttackTarget.XZPosition - _monster.XZPosition));
			if (!config.towardsTarget)
			{
				vector = -vector;
			}
			vector *= num;
			if (Physics.Raycast(xZPosition, vector, out _teleportHit, num2, (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
			{
				num2 = _teleportHit.distance;
			}
			float createCollisionRadius = _monster.config.CommonArguments.CreateCollisionRadius;
			FireMixinEffect(config.TeleportFromEffect, _monster);
			_isTeleporting = true;
			_teleportTimer.Reset(true);
			float num3 = num2 - createCollisionRadius;
			_teleportOffset = num3 * vector.normalized;
		}

		private void EndTeleport(Vector3 offset)
		{
			_isTeleporting = false;
			_monster.transform.position += offset;
			FireMixinEffect(config.TeleportToEffect, _monster);
			_teleportOffset = Vector3.zero;
		}

		public override void Core()
		{
			base.Core();
			if (_isTeleporting && _teleportTimer.isActive)
			{
				_teleportTimer.Core(1f);
				if (_teleportTimer.isTimeUp)
				{
					_teleportTimer.Reset(false);
					EndTeleport(_teleportOffset);
				}
			}
		}
	}
}
