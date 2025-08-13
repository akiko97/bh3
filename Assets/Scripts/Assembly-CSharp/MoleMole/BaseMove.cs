using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public abstract class BaseMove : Action
	{
		protected IAIEntity _aiEntity;

		protected IAIController _aiController;

		public SharedString moveSpeedKey;

		public bool moveByAnim = true;

		public float moveSpeed = 1f;

		private float _speed;

		public LayerMask CollisionLayerMask;

		public SharedLayerMask CollidedLayerMask;

		public SharedVector3 CollidedAwayForward;

		protected bool _hasCollided;

		public bool stopOnSucceed = true;

		public bool stopOnFailure;

		public bool updateDistance;

		public SharedFloat sharedDistance;

		public float failMoveTime;

		private float _failMoveTimer;

		private bool _usefailMoveTime;

		public bool checkStuck;

		public float checkStuckTime = 1f;

		public float checkStuckDistance = 1f;

		private Vector3 _checkStuckLastPos;

		private float _checkStuckTime;

		protected BaseMonoMonster _monster;

		protected virtual float GetSteerRatio()
		{
			return (_aiEntity.GetProperty("Animator_MoveSpeedRatio") + 1f) * 1.5f;
		}

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoAvatar)
			{
				_aiEntity = (BaseMonoAvatar)component;
				moveSpeedKey.Value = "AvatarSpeed(FIXED)";
				_speed = 0f;
			}
			else if (component is BaseMonoMonster)
			{
				_aiEntity = (BaseMonoMonster)component;
				_speed = (component as BaseMonoMonster).GetOriginMoveSpeed(moveSpeedKey.Value);
			}
			_aiController = _aiEntity.GetActiveAIController();
			_monster = _aiEntity as BaseMonoMonster;
			if (failMoveTime <= 0f)
			{
				_usefailMoveTime = false;
				return;
			}
			_usefailMoveTime = true;
			_failMoveTimer = failMoveTime;
		}

		public sealed override TaskStatus OnUpdate()
		{
			if (_usefailMoveTime && _failMoveTimer > 0f)
			{
				_failMoveTimer -= Time.deltaTime;
				if (_failMoveTimer < 0f)
				{
					_failMoveTimer = failMoveTime;
					if (stopOnFailure)
					{
						DoStopMove();
					}
					return TaskStatus.Failure;
				}
			}
			TaskStatus taskStatus = OnMoveUpdate();
			if (taskStatus == TaskStatus.Success && stopOnSucceed)
			{
				DoStopMove();
			}
			else if (taskStatus == TaskStatus.Failure && stopOnFailure)
			{
				DoStopMove();
			}
			return taskStatus;
		}

		protected abstract TaskStatus OnMoveUpdate();

		protected virtual void UpdateTargetDistance()
		{
			if (updateDistance && !(_aiEntity.AttackTarget == null) && _aiEntity.AttackTarget.IsActive())
			{
				sharedDistance.SetValue(CalculateTargetDistance());
			}
		}

		protected float GetTargetDistance()
		{
			if (updateDistance)
			{
				return sharedDistance.Value;
			}
			return CalculateTargetDistance();
		}

		protected virtual float CalculateTargetDistance()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = _aiEntity.AttackTarget.XZPosition;
			return Vector3.Distance(xZPosition, xZPosition2);
		}

		protected float GetLocalAvatarDistance()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
			return Vector3.Distance(xZPosition, xZPosition2);
		}

		protected void DoMoveForward()
		{
			if (moveByAnim)
			{
				_aiController.TryMove(_speed);
				return;
			}
			Vector3 forward = _aiEntity.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			forward *= moveSpeed;
			((BaseMonoAnimatorEntity)_aiEntity).SetOverrideVelocity(forward);
			((BaseMonoAnimatorEntity)_aiEntity).SetNeedOverrideVelocity(true);
		}

		protected void DoMoveBack()
		{
			if (moveByAnim)
			{
				_aiController.TryMove(0f - _speed);
				return;
			}
			Vector3 overrideVelocity = -_aiEntity.transform.forward;
			overrideVelocity.y = 0f;
			overrideVelocity.Normalize();
			overrideVelocity *= moveSpeed;
			((BaseMonoAnimatorEntity)_aiEntity).SetOverrideVelocity(overrideVelocity);
			((BaseMonoAnimatorEntity)_aiEntity).SetNeedOverrideVelocity(true);
		}

		protected void DoMoveLeft()
		{
			if (moveByAnim)
			{
				_aiController.TryMoveHorizontal(0f - _speed);
				return;
			}
			Vector3 overrideVelocity = -_aiEntity.transform.right;
			overrideVelocity.y = 0f;
			overrideVelocity.Normalize();
			overrideVelocity *= moveSpeed;
			((BaseMonoAnimatorEntity)_aiEntity).SetOverrideVelocity(overrideVelocity);
			((BaseMonoAnimatorEntity)_aiEntity).SetNeedOverrideVelocity(true);
		}

		protected void DoMoveRight()
		{
			if (moveByAnim)
			{
				_aiController.TryMoveHorizontal(_speed);
				return;
			}
			Vector3 right = _aiEntity.transform.right;
			right.y = 0f;
			right.Normalize();
			right *= moveSpeed;
			((BaseMonoAnimatorEntity)_aiEntity).SetOverrideVelocity(right);
			((BaseMonoAnimatorEntity)_aiEntity).SetNeedOverrideVelocity(true);
		}

		protected void DoStopMove()
		{
			if (moveByAnim)
			{
				_aiController.TryStop();
				return;
			}
			((BaseMonoAnimatorEntity)_aiEntity).SetOverrideVelocity(Vector3.zero);
			((BaseMonoAnimatorEntity)_aiEntity).SetNeedOverrideVelocity(false);
		}

		protected virtual void DoFaceToTarget()
		{
			Vector3 targetDirection = GetTargetDirection();
			_aiController.TrySteer(targetDirection);
		}

		protected virtual void DoFaceToLocalAvatar()
		{
			Vector3 localAvatarDirection = GetLocalAvatarDirection();
			_aiController.TrySteer(localAvatarDirection);
		}

		protected Vector3 GetLocalAvatarDirection()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
			Vector3 result = xZPosition2 - xZPosition;
			result.y = 0f;
			result.Normalize();
			return result;
		}

		protected virtual Vector3 GetTargetDirection()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 xZPosition2 = _aiEntity.AttackTarget.XZPosition;
			Vector3 result = xZPosition2 - xZPosition;
			result.y = 0f;
			result.Normalize();
			return result;
		}

		public override void OnStart()
		{
			TryStartCollisionCheck();
			_checkStuckTime = 0f;
			_checkStuckLastPos = _aiEntity.XZPosition;
		}

		public override void OnEnd()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component != null && (int)CollisionLayerMask != 0)
			{
				component.ClearCheckForCollision();
			}
			if (!moveByAnim)
			{
				DoStopMove();
			}
		}

		protected void TryStartCollisionCheck()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component != null && (int)CollisionLayerMask != 0)
			{
				component.SetCheckForCollision(CollisionLayerMask, OnCollisionCallback);
			}
			_hasCollided = false;
		}

		protected bool CheckCollided()
		{
			return (int)CollisionLayerMask != 0 && _hasCollided;
		}

		protected void ResetCollided()
		{
			_hasCollided = false;
			OnEnd();
		}

		private void OnCollisionCallback(int layer, Vector3 awayForward)
		{
			_hasCollided = true;
			CollidedLayerMask.SetValue(1 << layer);
			CollidedAwayForward.SetValue(awayForward);
		}

		protected bool CheckAndSetHitWall()
		{
			return false;
		}

		protected void TriggerEliteTeleport(float toDistance)
		{
			if (!(Mathf.Abs(toDistance) < 0.5f))
			{
				string abilityName = "Elite_Teleport";
				EvtAbilityStart evtAbilityStart = new EvtAbilityStart(_monster.GetRuntimeID());
				evtAbilityStart.abilityName = abilityName;
				evtAbilityStart.abilityArgument = toDistance;
				Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			}
		}

		protected bool CheckStuck()
		{
			if (checkStuck)
			{
				_checkStuckTime += Time.deltaTime;
				if (_checkStuckTime > checkStuckTime)
				{
					float num = Vector3.SqrMagnitude(_checkStuckLastPos - _aiEntity.XZPosition);
					if (num < checkStuckDistance)
					{
						return true;
					}
					_checkStuckTime = 0f;
					_checkStuckLastPos = _aiEntity.XZPosition;
				}
			}
			return false;
		}

		protected Vector3 GetAvoidObstacleDir(Vector3 targetFaceDir)
		{
			int layerMask = (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER);
			if (Physics.Raycast(_aiEntity.transform.position, targetFaceDir, 1f, layerMask))
			{
				for (int i = 30; i <= 150; i += 30)
				{
					Vector3 vector = Quaternion.AngleAxis(i, Vector3.up) * targetFaceDir;
					Vector3 vector2 = Quaternion.AngleAxis(-i, Vector3.up) * targetFaceDir;
					bool flag = Physics.Raycast(_aiEntity.transform.position, vector, 2f, layerMask);
					bool flag2 = Physics.Raycast(_aiEntity.transform.position, vector2, 2f, layerMask);
					Debug.DrawRay(_aiEntity.transform.position, vector, Color.green, 0.1f);
					Debug.DrawRay(_aiEntity.transform.position, vector2, Color.green, 0.1f);
					if (!flag && !flag2)
					{
						targetFaceDir = ((Random.Range(0, 100) >= 50) ? vector2 : vector);
						break;
					}
					if (!flag && flag2)
					{
						targetFaceDir = vector;
						break;
					}
					if (flag && !flag2)
					{
						targetFaceDir = vector2;
						break;
					}
				}
			}
			return targetFaceDir;
		}
	}
}
