using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Move")]
	public class MoveByTime : BaseMove
	{
		public enum SpeedDirection
		{
			Back = 0,
			Forward = 1,
			Left = 2,
			Right = 3,
			WanderLR = 4
		}

		public enum MoveType
		{
			ConstantDirection = 0,
			RandomDirection = 1,
			AlwaysFaceTarget = 2,
			ChooseDirection = 3
		}

		public enum AngelType
		{
			Local = 0,
			TargetRelative = 1,
			World = 2
		}

		private const float WANDER_HIT_WALL_CHECK_TIME_INTERVAL = 1f;

		private const float CHECK_WALL_STEP = 12f;

		private const float CHECK_WALL_DISTANCE_CAP = 10f;

		private const float CHECK_WALL_CLIP_OFF = 0.5f;

		public MoveType moveType = MoveType.AlwaysFaceTarget;

		public SpeedDirection speedDirection;

		public float moveTime;

		public AngelType angelType;

		public SharedFloat angel = 0f;

		public float chooseAngel_1;

		public float chooseAngel_2;

		public float randAngelMin;

		public float randAngelMax;

		public float steerSpeedRatio = 1f;

		public bool instantSteer;

		public float wanderIntervalTime = 1f;

		private float _wanderTimer;

		private float _wanderHitWallTimer;

		private SpeedDirection _currentDirection;

		private float _timer;

		private Vector3 _toDirection;

		protected override float GetSteerRatio()
		{
			return base.GetSteerRatio() * steerSpeedRatio;
		}

		public override void OnStart()
		{
			base.OnStart();
			_timer = moveTime;
			Vector3 vector = _aiEntity.FaceDirection;
			switch (angelType)
			{
			case AngelType.Local:
				vector = _aiEntity.FaceDirection;
				break;
			case AngelType.TargetRelative:
				if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
				{
					_timer = -1f;
				}
				else
				{
					vector = GetTargetDirection();
				}
				break;
			case AngelType.World:
				vector = Vector3.forward;
				break;
			}
			if (moveType == MoveType.ConstantDirection)
			{
				SetDirectionFromAngle(angel.Value, vector);
			}
			else if (moveType == MoveType.RandomDirection)
			{
				SetRandomDirectionWithWallcheck(vector);
			}
			else if (moveType == MoveType.ChooseDirection)
			{
				if (Random.value > 0.5f)
				{
					angel = chooseAngel_1;
				}
				else
				{
					angel = chooseAngel_2;
				}
				SetDirectionFromAngle(angel.Value, vector);
			}
			if (speedDirection == SpeedDirection.WanderLR)
			{
				GetRandomWanDerDirectionLR();
				_wanderTimer = wanderIntervalTime;
				_wanderHitWallTimer = 1f;
			}
			else
			{
				_currentDirection = speedDirection;
			}
		}

		private void SetRandomDirectionWithWallcheck(Vector3 charDirection)
		{
			int num = Mathf.FloorToInt((randAngelMax - randAngelMin) / 12f);
			float num2 = -1f;
			Vector3 toDirection = Vector3.zero;
			float num3 = ((!(randAngelMax > randAngelMin)) ? randAngelMax : randAngelMin);
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = Quaternion.AngleAxis(num3 + (float)i * 12f, Vector3.up) * charDirection;
				float raycastDistance = CollisionDetectPattern.GetRaycastDistance(_aiEntity.RootNodePosition, vector, 10f, 0.5f, InLevelData.STAGE_COLLIDER_LAYER);
				if (raycastDistance > num2)
				{
					num2 = raycastDistance;
					toDirection = vector;
				}
			}
			if (num2 == 10f)
			{
				toDirection = Quaternion.AngleAxis(Random.Range(randAngelMin, randAngelMax), Vector3.up) * charDirection;
			}
			_toDirection = toDirection;
			_toDirection.y = 0f;
			_toDirection.Normalize();
		}

		private void SetDirectionFromAngle(float angle, Vector3 charDirection)
		{
			_toDirection = Quaternion.AngleAxis(angle, Vector3.up) * charDirection;
			_toDirection.y = 0f;
			_toDirection.Normalize();
		}

		protected override TaskStatus OnMoveUpdate()
		{
			UpdateTargetDistance();
			if (CheckCollided() && speedDirection != SpeedDirection.WanderLR)
			{
				return TaskStatus.Success;
			}
			if (_timer < 0f)
			{
				return TaskStatus.Success;
			}
			_timer -= Time.deltaTime * _aiEntity.TimeScale;
			if (speedDirection == SpeedDirection.WanderLR)
			{
				if (CheckCollided())
				{
					ReverseDirectionLR();
					_wanderHitWallTimer = 1f;
					_wanderTimer = wanderIntervalTime;
					ResetCollided();
				}
				if (_wanderHitWallTimer < 0f)
				{
					TryStartCollisionCheck();
					_wanderHitWallTimer = 1f;
				}
				else
				{
					_wanderHitWallTimer -= Time.deltaTime * _aiEntity.TimeScale;
				}
				if (_wanderTimer < 0f)
				{
					_wanderTimer = wanderIntervalTime;
					GetRandomWanDerDirectionLR();
				}
				_wanderTimer -= Time.deltaTime * _aiEntity.TimeScale;
			}
			if (moveType == MoveType.AlwaysFaceTarget)
			{
				if (_aiEntity.AttackTarget == null || !_aiEntity.AttackTarget.IsActive())
				{
					return TaskStatus.Success;
				}
				DoFaceToTarget();
			}
			else if (moveType == MoveType.ConstantDirection || moveType == MoveType.ChooseDirection || moveType == MoveType.RandomDirection)
			{
				DoFaceToCertainDirection();
			}
			DoMove();
			return TaskStatus.Running;
		}

		private void DoMove()
		{
			switch (_currentDirection)
			{
			case SpeedDirection.Back:
				DoMoveBack();
				break;
			case SpeedDirection.Forward:
				DoMoveForward();
				break;
			case SpeedDirection.Left:
				DoMoveLeft();
				break;
			case SpeedDirection.Right:
				DoMoveRight();
				break;
			}
		}

		private void DoFaceToCertainDirection()
		{
			if (instantSteer)
			{
				_aiController.TrySteerInstant(_toDirection);
			}
			else
			{
				_aiController.TrySteer(_toDirection, GetSteerRatio());
			}
		}

		protected override void DoFaceToTarget()
		{
			Vector3 targetDirection = GetTargetDirection();
			if (instantSteer)
			{
				_aiController.TrySteerInstant(targetDirection);
			}
			else
			{
				_aiController.TrySteer(targetDirection, GetSteerRatio());
			}
		}

		protected override void DoFaceToLocalAvatar()
		{
			Vector3 localAvatarDirection = GetLocalAvatarDirection();
			if (instantSteer)
			{
				_aiController.TrySteerInstant(localAvatarDirection);
			}
			else
			{
				_aiController.TrySteer(localAvatarDirection, GetSteerRatio());
			}
		}

		private void GetRandomWanDerDirectionLR()
		{
			if (Random.value > 0.5f)
			{
				_currentDirection = SpeedDirection.Left;
			}
			else
			{
				_currentDirection = SpeedDirection.Right;
			}
		}

		private void ReverseDirectionLR()
		{
			if (_currentDirection == SpeedDirection.Left)
			{
				_currentDirection = SpeedDirection.Right;
			}
			else if (_currentDirection == SpeedDirection.Right)
			{
				_currentDirection = SpeedDirection.Left;
			}
		}
	}
}
