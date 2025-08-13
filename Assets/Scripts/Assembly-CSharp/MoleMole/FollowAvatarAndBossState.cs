using UnityEngine;

namespace MoleMole
{
	public class FollowAvatarAndBossState : FollowAvatarAndTargetState
	{
		private const int LPF_POSITIONS_COUNT = 60;

		private BaseMonoEntity _bossTarget;

		private Vector3[] _followPositions;

		public bool needLPF = true;

		public BaseMonoEntity bossTarget
		{
			get
			{
				return _bossTarget;
			}
			set
			{
				_bossTarget = value;
				if (needLPF)
				{
					for (int i = 0; i < _followPositions.Length; i++)
					{
						_followPositions[i] = _bossTarget.XZPosition;
					}
				}
			}
		}

		public FollowAvatarAndBossState(MainCameraFollowState followState)
			: base(followState)
		{
			_followPositions = new Vector3[60];
		}

		protected override void PreCalculate()
		{
			BaseMonoEntity attackTarget = _owner.avatar.AttackTarget;
			BaseMonoEntity entity;
			Vector3 vector;
			if (attackTarget != null)
			{
				if (attackTarget is MonoBodyPartEntity && !((MonoBodyPartEntity)attackTarget).IsCameraTargetable)
				{
					entity = bossTarget;
					vector = GetAverageBossTargetPosition();
				}
				else
				{
					entity = attackTarget;
					vector = attackTarget.XZPosition;
				}
			}
			else
			{
				entity = bossTarget;
				vector = GetAverageBossTargetPosition();
			}
			Vector3 vector2 = vector - _owner.avatar.XZPosition;
			vector2.y = 0f;
			_disEntity2Target = vector2.magnitude;
			_dirCamera2Entity = _owner.avatar.XZPosition - _owner.followData.cameraPosition;
			_dirCamera2Entity.y = 0f;
			_disCamera2Entity = _dirCamera2Entity.magnitude;
			_dirCamera2Target = vector - _owner.followData.cameraPosition;
			_dirCamera2Target.y = 0f;
			_disCamera2Target = _dirCamera2Target.magnitude;
			_entityCollisionRadius = GetColliderRadius(_owner.avatar);
			_targetCollisionRadius = GetColliderRadius(entity);
		}

		private void CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget()
		{
			float num = Mathf.Acos(Vector3.Dot(_dirCamera2Entity, _dirCamera2Target) / (_disCamera2Entity * _disCamera2Target)) * 57.29578f;
			if (_disEntity2Target < _disCamera2Entity)
			{
				float num2 = _disEntity2Target * _disEntity2Target + _disCamera2Entity * _disCamera2Entity - _disCamera2Target * _disCamera2Target;
				if (num2 > 0f)
				{
					float targetsAngle = Mathf.Atan2(_disEntity2Target, _disCamera2Entity) * 57.29578f;
					DoAdjToCorrectTargetsAngle(targetsAngle, false);
				}
				else
				{
					DoAdjToCorrectTargetsAngle(num, true);
				}
			}
			else if (num > 45f)
			{
				DoAdjToCorrectTargetsAngle(45f, false);
			}
			else
			{
				DoAdjToCorrectTargetsAngle(num, false);
			}
		}

		protected override void CalcTargetPolarIgnoreWallHit()
		{
			BaseMonoEntity attackTarget = _owner.avatar.AttackTarget;
			if (attackTarget != null)
			{
				if (attackTarget is MonoBodyPartEntity && !((MonoBodyPartEntity)attackTarget).IsCameraTargetable)
				{
					CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget();
				}
				else
				{
					CalcTargetPoloarIngoreWallHitWhenHasAttackTarget();
				}
			}
			else
			{
				CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget();
			}
		}

		private void DoUpdateWhenHasNoAttackTarget()
		{
			if (_isCameraSolveWallHit)
			{
				DoWhenSolveWallHit();
				return;
			}
			ReplaceCamera();
			_owner.anchorPolar = _targetPolar;
		}

		public override void Update()
		{
			_feasibleAngles.Clear();
			if (bossTarget == null || !bossTarget.IsActive())
			{
				_owner.TransitBaseState(_owner.followAvatarState, true);
				return;
			}
			SampleBossTargetPosition();
			_hasSetTargetForwardDelta = false;
			_owner.posLerpRatio = 1f;
			BaseMonoEntity attackTarget = _owner.avatar.AttackTarget;
			if (attackTarget != null)
			{
				if (attackTarget is MonoBodyPartEntity && !((MonoBodyPartEntity)attackTarget).IsCameraTargetable)
				{
					DoUpdateWhenHasNoAttackTarget();
				}
				else
				{
					DoUpdateWhenHasAttackTarget();
				}
			}
			else
			{
				DoUpdateWhenHasNoAttackTarget();
			}
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelForwardDeltaAngleRecover();
			}
			if (_hasSetTargetForwardDelta)
			{
				if (_targetForwardDelta * _owner.forwardDeltaAngle > 0f)
				{
					_owner.forwardDeltaAngle = Mathf.Lerp(_owner.forwardDeltaAngle, _targetForwardDelta, Time.deltaTime * 5f);
				}
				else
				{
					_owner.forwardDeltaAngle = _targetForwardDelta;
				}
			}
		}

		private void SampleBossTargetPosition()
		{
			if (needLPF)
			{
				for (int num = _followPositions.Length - 1; num > 0; num--)
				{
					_followPositions[num] = _followPositions[num - 1];
				}
				_followPositions[0] = bossTarget.XZPosition;
			}
		}

		private Vector3 GetAverageBossTargetPosition()
		{
			if (needLPF)
			{
				Vector3 zero = Vector3.zero;
				for (int i = 0; i < _followPositions.Length; i++)
				{
					zero += _followPositions[i];
				}
				Vector3 vector = zero / _followPositions.Length;
				Debug.DrawLine(vector, vector + Vector3.up);
				return vector;
			}
			return bossTarget.XZPosition;
		}
	}
}
