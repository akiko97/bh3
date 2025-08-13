using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveNearThanDistance : BaseMove
	{
		public enum TargetType
		{
			AttackTarget = 0,
			LocalAvatar = 1
		}

		private const float FIX_DIR_HOLD_TIME = 0.4f;

		private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;

		private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;

		private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;

		public SharedFloat distance = 0f;

		public TargetType targetType;

		public bool useFixedDir;

		public float fixDirRatio = 1f;

		public bool useGetPath = true;

		private Vector3 _targetCornerByGetPath;

		public bool avoidObstacle;

		private bool _avoidingObstacle;

		private float _avoidObstacleTime;

		private BaseMonoAnimatorEntity _entity;

		private float _fixDirTimer;

		private Vector3 _lastFixedDir;

		public override void OnAwake()
		{
			base.OnAwake();
			_entity = GetComponent<BaseMonoAnimatorEntity>();
		}

		public override void OnStart()
		{
			base.OnStart();
			_avoidingObstacle = false;
			_avoidObstacleTime = 0f;
		}

		protected override TaskStatus OnMoveUpdate()
		{
			UpdateTargetDistance();
			if (CheckCollided())
			{
				return TaskStatus.Success;
			}
			if (CheckStuck())
			{
				return TaskStatus.Failure;
			}
			float num;
			float num2;
			if (targetType == TargetType.LocalAvatar)
			{
				DoFaceToLocalAvatar();
				num = GetLocalAvatarDistance();
			}
			else
			{
				if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
				{
					return TaskStatus.Success;
				}
				num = GetTargetDistance();
				if (GlobalVars.USE_GET_PATH_SWITCH && useGetPath)
				{
					Vector3 targetCorner = default(Vector3);
					if (!Singleton<DetourManager>.Instance.GetTargetPosition(_entity, _aiEntity.transform.position, _aiEntity.AttackTarget.transform.position, ref targetCorner))
					{
						return TaskStatus.Running;
					}
					Debug.DrawLine(_aiEntity.transform.position, targetCorner, Color.yellow, 0.1f);
					_targetCornerByGetPath = targetCorner;
					DoFaceToTargetMore(true);
					num2 = Vector3.Distance(_aiEntity.XZPosition, _targetCornerByGetPath);
				}
				else
				{
					DoFaceToTargetMore(false);
				}
			}
			num2 = num;
			if (num < distance.Value)
			{
				return TaskStatus.Success;
			}
			if (_aiEntity.GetProperty("AI_CanTeleport") > 0f)
			{
				TriggerEliteTeleport(num2 - distance.Value);
			}
			DoMoveForward();
			return TaskStatus.Running;
		}

		private void DoFaceToTargetMore(bool hasResetTargetPos)
		{
			if (_fixDirTimer > 0f)
			{
				_fixDirTimer -= Time.deltaTime * _aiEntity.TimeScale;
				_aiController.TrySteer(_lastFixedDir);
			}
			else
			{
				if (avoidObstacle && _avoidingObstacle && Time.time - _avoidObstacleTime <= 0.2f)
				{
					return;
				}
				Vector3 vector = ((!hasResetTargetPos) ? GetTargetDirection() : GetTargetCornerDirection());
				Vector3 vector2 = vector;
				if (useFixedDir && GetTargetDistance() > 2f)
				{
					List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
					for (int i = 1; i < allMonsters.Count; i++)
					{
						if (allMonsters[i] != _monster)
						{
							Vector3 to = allMonsters[i].XZPosition - _monster.XZPosition;
							float num = Vector3.Angle(_monster.FaceDirection, to);
							if (num < 90f && to.magnitude < 4f)
							{
								Vector3 vector3 = to.normalized / to.magnitude * (1f - num / 90f) * fixDirRatio;
								vector2 -= vector3;
							}
						}
					}
					vector2 = vector2.normalized;
					if (vector2 != vector)
					{
						_fixDirTimer = 0.4f;
						_lastFixedDir = vector2;
						vector = vector2;
					}
				}
				if (useFixedDir)
				{
				}
				if (avoidObstacle)
				{
					Vector3 avoidObstacleDir = GetAvoidObstacleDir(vector);
					_avoidingObstacle = vector != avoidObstacleDir;
					vector = avoidObstacleDir;
					_avoidObstacleTime = Time.time;
				}
				_aiController.TrySteer(vector);
			}
		}

		protected Vector3 GetTargetCornerDirection()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 targetCornerByGetPath = _targetCornerByGetPath;
			Vector3 result = targetCornerByGetPath - xZPosition;
			result.y = 0f;
			result.Normalize();
			return result;
		}
	}
}
