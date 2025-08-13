using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveToSpawnPoint : BaseMove
	{
		private const float FIX_DIR_HOLD_TIME = 0.4f;

		private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;

		private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;

		private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;

		public SharedVector3 TargetPosition;

		public string SpawnPointName;

		public float distance = 1f;

		public bool useFixedDir;

		public float fixDirRatio = 1f;

		public bool useGetPath = true;

		private Vector3 _targetCornerByGetPath;

		private BaseMonoAnimatorEntity _entity;

		private float _fixDirTimer;

		private Vector3 _lastFixedDir;

		public override void OnAwake()
		{
			base.OnAwake();
			_entity = GetComponent<BaseMonoAnimatorEntity>();
			TargetPosition.Value = GetSpawnPointPos(SpawnPointName);
		}

		protected override TaskStatus OnMoveUpdate()
		{
			float currentSpawnDistance = GetCurrentSpawnDistance();
			if (GlobalVars.USE_GET_PATH_SWITCH && useGetPath)
			{
				Vector3 targetCorner = default(Vector3);
				if (!Singleton<DetourManager>.Instance.GetTargetPosition(_entity, _aiEntity.transform.position, TargetPosition.Value, ref targetCorner))
				{
					return TaskStatus.Running;
				}
				Debug.DrawLine(_aiEntity.transform.position, targetCorner, Color.yellow, 0.1f);
				_targetCornerByGetPath = targetCorner;
				DoFaceToTargetMore(true);
			}
			else
			{
				DoFaceToTargetMore(false);
			}
			if (currentSpawnDistance < distance)
			{
				return TaskStatus.Success;
			}
			DoMoveForward();
			return TaskStatus.Running;
		}

		private float GetCurrentSpawnDistance()
		{
			return Miscs.DistancForVec3IgnoreY(_aiEntity.transform.position, TargetPosition.Value);
		}

		private void DoFaceToTargetMore(bool hasResetTargetPos)
		{
			if (_fixDirTimer > 0f)
			{
				_fixDirTimer -= Time.deltaTime * _aiEntity.TimeScale;
				_aiController.TrySteer(_lastFixedDir);
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
			_aiController.TrySteer(vector);
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

		private Vector3 GetSpawnPointPos(string spawnName)
		{
			MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
			int num = ((spawnName == null) ? Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length) : stageEnv.GetNamedSpawnPointIx(spawnName));
			return stageEnv.spawnPoints[num].transform.position;
		}

		private Vector3 GetTargetDir()
		{
			Vector3 xZPosition = _aiEntity.XZPosition;
			Vector3 value = TargetPosition.Value;
			Vector3 result = value - xZPosition;
			result.y = 0f;
			result.Normalize();
			return result;
		}
	}
}
