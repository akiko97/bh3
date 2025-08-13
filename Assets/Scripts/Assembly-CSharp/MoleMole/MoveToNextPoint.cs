using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveToNextPoint : BaseMove
	{
		private const float FIX_DIR_HOLD_TIME = 0.4f;

		private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;

		private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;

		private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;

		public SharedVector3 TargetPosition;

		public SharedString SpawnPointName;

		public float distance = 1f;

		public bool useFixedDir;

		public float fixDirRatio = 1f;

		public bool useGetPath = true;

		private Vector3 _targetCornerByGetPath;

		private BaseMonoAnimatorEntity _entity;

		private MonoStageEnv _stageEnv;

		private int _spawnPointIndex = -1;

		private int _lastSpawnPointIndex = -1;

		private HashSet<int> _passedSpawnPoints = new HashSet<int>();

		private bool _useRandomSpawnPoint;

		private uint _targetRuntimeID;

		private uint _lastTargetRuntimeID;

		private HashSet<uint> _passedRuntimeIDs = new HashSet<uint>();

		public bool avoidObstacle = true;

		private bool _avoidingObstacle;

		private float _avoidObstacleTime;

		private float _fixDirTimer;

		private Vector3 _lastFixedDir;

		public override void OnAwake()
		{
			failMoveTime = 0f;
			base.OnAwake();
			_entity = GetComponent<BaseMonoAnimatorEntity>();
			_stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
		}

		public override void OnStart()
		{
			base.OnStart();
			_avoidingObstacle = false;
			_avoidObstacleTime = 0f;
			TargetPosition.Value = GetTargetPosition();
		}

		protected override TaskStatus OnMoveUpdate()
		{
			if (MonsterExists())
			{
				return TaskStatus.Failure;
			}
			if (_useRandomSpawnPoint)
			{
				Vector3 targetPosition = GetTargetPosition(false);
				if (targetPosition != _aiEntity.transform.position)
				{
					TargetPosition.Value = targetPosition;
				}
			}
			Debug.DrawLine(_aiEntity.transform.position, TargetPosition.Value, Color.blue, 0.1f);
			float currentSpawnDistance = GetCurrentSpawnDistance();
			if (currentSpawnDistance < distance)
			{
				if (_spawnPointIndex != -1 && !_passedSpawnPoints.Contains(_spawnPointIndex))
				{
					_passedSpawnPoints.Add(_spawnPointIndex);
				}
				if (_targetRuntimeID != 0 && !_passedRuntimeIDs.Contains(_targetRuntimeID))
				{
					_passedRuntimeIDs.Add(_targetRuntimeID);
				}
				_lastSpawnPointIndex = _spawnPointIndex;
				_lastTargetRuntimeID = _targetRuntimeID;
				return TaskStatus.Success;
			}
			if (CheckStuck())
			{
				_lastSpawnPointIndex = _spawnPointIndex;
				_lastTargetRuntimeID = _targetRuntimeID;
				return TaskStatus.Failure;
			}
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

		private Vector3 GetTargetPosition(bool useRandom = true)
		{
			_spawnPointIndex = -1;
			_targetRuntimeID = 0u;
			_useRandomSpawnPoint = false;
			MonoSpawnPoint hintSpawnPoint = GetHintSpawnPoint();
			if (hintSpawnPoint != null)
			{
				return hintSpawnPoint.transform.position;
			}
			BaseActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<BaseActor>(6);
			for (int i = 0; i < actorByCategory.Length; i++)
			{
				if (actorByCategory[i].IsActive() && actorByCategory[i] is StageExitFieldActor && actorByCategory[i].runtimeID != _lastTargetRuntimeID && !_passedRuntimeIDs.Contains(actorByCategory[i].runtimeID))
				{
					_targetRuntimeID = actorByCategory[i].runtimeID;
					return actorByCategory[i].gameObject.transform.position;
				}
			}
			List<BaseMonoDynamicObject> allNavigationArrows = Singleton<DynamicObjectManager>.Instance.GetAllNavigationArrows();
			for (int j = 0; j < allNavigationArrows.Count; j++)
			{
				if (allNavigationArrows[j].IsActive() && allNavigationArrows[j].GetRuntimeID() != _lastTargetRuntimeID && !_passedRuntimeIDs.Contains(allNavigationArrows[j].GetRuntimeID()))
				{
					Vector3 result = allNavigationArrows[j].XZPosition - allNavigationArrows[j].transform.forward * 100f;
					RaycastHit hitInfo;
					if (Physics.Raycast(allNavigationArrows[j].XZPosition + 0.5f * Vector3.up, -allNavigationArrows[j].transform.forward, out hitInfo, 100f, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
					{
						result = allNavigationArrows[j].XZPosition - allNavigationArrows[j].transform.forward * hitInfo.distance;
					}
					_targetRuntimeID = allNavigationArrows[j].GetRuntimeID();
					return result;
				}
			}
			if (useRandom)
			{
				int num = 0;
				do
				{
					_spawnPointIndex = Random.Range(0, _stageEnv.spawnPoints.Length);
				}
				while (num++ < 10 && _stageEnv.spawnPoints.Length > 1 && _passedSpawnPoints.Contains(_spawnPointIndex));
				if (_stageEnv.spawnPoints[_spawnPointIndex] != null)
				{
					_useRandomSpawnPoint = true;
					return _stageEnv.spawnPoints[_spawnPointIndex].transform.position;
				}
			}
			return _aiEntity.transform.position;
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

		private bool MonsterExists()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				if (allMonsters[i].IsActive() && !allMonsters[i].denySelect)
				{
					return true;
				}
			}
			return false;
		}

		private MonoSpawnPoint GetHintSpawnPoint()
		{
			MonoSpawnPoint spawnPoint = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.GetSpawnPoint();
			if (spawnPoint != null)
			{
				_spawnPointIndex = _stageEnv.GetNamedSpawnPointIx(spawnPoint.name);
				if (_spawnPointIndex >= 0 && _spawnPointIndex != _lastSpawnPointIndex && !_passedSpawnPoints.Contains(_spawnPointIndex))
				{
					return spawnPoint;
				}
			}
			return null;
		}
	}
}
