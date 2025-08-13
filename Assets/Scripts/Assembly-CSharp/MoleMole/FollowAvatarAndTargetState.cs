using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class FollowAvatarAndTargetState : BaseFollowBaseState
	{
		private class MotionAngleInfo
		{
			public float targetsAngle;

			public float rotateAngle;

			public MotionAngleInfo()
			{
			}

			public MotionAngleInfo(float targetsAngle, float rotateAngle)
			{
				this.targetsAngle = targetsAngle;
				this.rotateAngle = rotateAngle;
			}
		}

		public enum FollowMode
		{
			TargetMode = 0,
			DirectionMode = 1
		}

		protected const float BIO_TARGET_MAX_ANGLE = 45f;

		private const float BIO_TARGET_MIN_ANGLE = 17f;

		private const int BIO_TARGET_CAMERA_MODE_DEFAULT = 0;

		private const int BIO_TARGET_CAMERA_MODE_ALWAYS_HORIZONTAL = 1;

		private const float BIO_TRIGGER_SOLVE_WALL_HIT_RATIO = 0.75f;

		protected const float BIO_TARGET_MIN_ANGLE_RATIO = 0.4f;

		protected const float BIO_TARGET_MAX_ANGLE_RATIO = 1.1f;

		private int _bioTargetCameraMode;

		protected float _disEntity2Target;

		protected Vector3 _dirCamera2Entity;

		protected float _disCamera2Entity;

		protected Vector3 _dirCamera2Target;

		protected float _disCamera2Target;

		protected float _entityCollisionRadius;

		protected float _targetCollisionRadius;

		protected float _targetPolar;

		protected float _targetForwardDelta;

		protected bool _hasSetTargetForwardDelta;

		protected List<float> _feasibleAngles = new List<float>(4);

		private List<MotionAngleInfo> _motionAngleInfos = new List<MotionAngleInfo>(4);

		[ShowInInspector]
		private FollowMode _mode;

		protected bool _isCameraSolveWallHit;

		private bool _isCurrentWallHit;

		private float _originAnchorPolar;

		private float _currentAnchorPolar;

		private float _disableLerpSumUpTime;

		private float _speedForOneCircle = 3000f;

		private float _speedSolvingWallHit;

		private float _accelerationSolvingWallHit = 7000f;

		private float _minSpeedSolvingWallHit = 200f;

		private float DELTA_TIME_PER_FRAME = 0.016f;

		public FollowAvatarAndTargetState(MainCameraFollowState followState)
			: base(followState)
		{
			_mode = FollowMode.TargetMode;
			maskedShortStates.Add(_owner.rotateToAvatarFacingState);
		}

		public override void Enter()
		{
			if (_owner.rotateToAvatarFacingState.active)
			{
				_owner.RemoveShortState();
			}
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelLerpRatioRecovering();
			}
		}

		public void SwitchMode(FollowMode mode)
		{
			_mode = mode;
		}

		private void CalculateTargetPolarModeHorizontal()
		{
			float num = Mathf.Acos(Vector3.Dot(_dirCamera2Entity, _dirCamera2Target) / (_disCamera2Entity * _disCamera2Target)) * 57.29578f;
			if (_disEntity2Target < _disCamera2Entity)
			{
				float num2 = Mathf.Acos(_disEntity2Target / 2f / _disCamera2Entity) * 57.29578f;
				float targetsAngle = 180f - 2f * num2;
				DoAdjToCorrectTargetsAngle(targetsAngle, false);
			}
			else if (num < 17f)
			{
				DoAdjToCorrectTargetsAngle(17f, false);
			}
			else if (num > 45f)
			{
				DoAdjToCorrectTargetsAngle(45f, false);
			}
		}

		private float FindMaxAdjAngle()
		{
			if (_disEntity2Target < _disCamera2Entity)
			{
				float num = Mathf.Asin(_disEntity2Target / (_owner.anchorRadius * _owner.cameraLocateRatio)) * 57.29578f;
				if (num < 0f)
				{
					Debug.LogError(string.Format("FindMaxAdjAngle not valid, accecptableMaxTargetsAngle{0:f}", num));
					Debug.Break();
				}
				return Mathf.Min(num, 45f);
			}
			return 45f;
		}

		protected float FindMinAdjAngle()
		{
			return Mathf.Asin((_entityCollisionRadius + 0.3f) * _owner.avatar.commonConfig.CommonArguments.CameraMinAngleRatio / _owner.anchorRadius * _owner.cameraLocateRatio) * 57.29578f;
		}

		private bool CheckNeedRotateToMaxAngle(float currentAngle)
		{
			float num = _disEntity2Target * _disEntity2Target + _disCamera2Target * _disCamera2Target - _disCamera2Entity * _disCamera2Entity;
			if (num >= 0f)
			{
				return false;
			}
			float num2 = Mathf.Atan(_targetCollisionRadius / (_owner.anchorRadius * _owner.cameraLocateRatio - Mathf.Sqrt(_disEntity2Target * _disEntity2Target - _targetCollisionRadius * _targetCollisionRadius))) * 57.29578f;
			return currentAngle <= num2;
		}

		protected void CalcTargetPoloarIngoreWallHitWhenHasAttackTarget()
		{
			float num = Mathf.Acos(Vector3.Dot(_dirCamera2Entity, _dirCamera2Target) / (_disCamera2Entity * _disCamera2Target)) * 57.29578f;
			if (_disEntity2Target < _disCamera2Entity)
			{
				if (CheckNeedRotateToMaxAngle(num))
				{
					DoAdjToCorrectTargetsAngle(FindMaxAdjAngle(), false);
					return;
				}
				float num2 = FindMinAdjAngle();
				if (num < num2 * 0.4f)
				{
					DoAdjToCorrectTargetsAngle(num2, false);
				}
				else
				{
					DoAdjToCorrectTargetsAngle(num, true);
				}
			}
			else
			{
				float num3 = FindMinAdjAngle();
				if (num < num3 * 0.4f)
				{
					DoAdjToCorrectTargetsAngle(num3, false);
				}
				else if (num > 49.5f)
				{
					DoAdjToCorrectTargetsAngle(45f, false);
				}
				else
				{
					DoAdjToCorrectTargetsAngle(num, false);
				}
			}
		}

		protected virtual void CalcTargetPolarIgnoreWallHit()
		{
			CalcTargetPoloarIngoreWallHitWhenHasAttackTarget();
		}

		public void CalcTargetPolarWhenWallHit()
		{
			float num = FindMaxAdjAngle();
			float num2 = FindMinAdjAngle();
			if (!(num < num2))
			{
				SolveWallHit(num, num2);
			}
		}

		private MotionAngleInfo GetMotionAngleInfo(float targetsAngle, bool forceTarget)
		{
			CalcRotateAngles(targetsAngle);
			_motionAngleInfos.Clear();
			for (int i = 0; i < _feasibleAngles.Count; i++)
			{
				_motionAngleInfos.Add(new MotionAngleInfo(targetsAngle, _feasibleAngles[i]));
			}
			if (_motionAngleInfos.Count > 0)
			{
				_motionAngleInfos.Sort((MotionAngleInfo a, MotionAngleInfo b) => Mathf.Abs(a.rotateAngle).CompareTo(Mathf.Abs(b.rotateAngle)));
				bool flag = false;
				MotionAngleInfo result = null;
				for (int num = 0; num < _motionAngleInfos.Count; num++)
				{
					float targetPolar = _owner.anchorPolar + _motionAngleInfos[num].rotateAngle;
					RaycastHit wallHitRet;
					if (!CheckWallHitByTargetPolar(targetPolar, out wallHitRet))
					{
						result = _motionAngleInfos[num];
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return result;
				}
				return (!forceTarget) ? null : _motionAngleInfos[0];
			}
			return null;
		}

		private void SolveWallHit(float maxAdjAngle, float minAdjAngle)
		{
			MotionAngleInfo motionAngleInfo;
			if (IsTargetBlockSightEntity())
			{
				motionAngleInfo = GetMotionAngleInfo(maxAdjAngle, false);
				if (motionAngleInfo == null)
				{
					motionAngleInfo = GetMotionAngleInfo(minAdjAngle, true);
				}
			}
			else
			{
				motionAngleInfo = GetMotionAngleInfo(minAdjAngle, false);
				if (motionAngleInfo == null)
				{
					motionAngleInfo = GetMotionAngleInfo(maxAdjAngle, true);
				}
			}
			if (motionAngleInfo != null)
			{
				_targetPolar = _owner.anchorPolar + motionAngleInfo.rotateAngle;
				_targetForwardDelta = 0f;
				_hasSetTargetForwardDelta = true;
				StartSolvingWallHit();
			}
		}

		private bool IsTargetBlockSightEntity()
		{
			float num = (_disCamera2Entity * _disCamera2Entity + _disEntity2Target * _disEntity2Target - _disCamera2Target * _disCamera2Target) / (2f * _disCamera2Entity * _disEntity2Target);
			if (num <= 0f)
			{
				return false;
			}
			float num2 = Mathf.Sqrt(_disEntity2Target * _disEntity2Target - _targetCollisionRadius * _targetCollisionRadius) / _disEntity2Target;
			return num > num2;
		}

		private void CalcRotateAngles(float targetsAngle)
		{
			float num = Mathf.Acos((_disCamera2Entity * _disCamera2Entity + _disEntity2Target * _disEntity2Target - _disCamera2Target * _disCamera2Target) / (2f * _disCamera2Entity * _disEntity2Target)) * 57.29578f;
			float num2 = Mathf.Asin(_disCamera2Entity / _disEntity2Target * Mathf.Sin(targetsAngle * ((float)Math.PI / 180f))) * 57.29578f;
			if (!float.IsNaN(targetsAngle) && !float.IsNaN(num2) && !float.IsNaN(num))
			{
				CalcFeasibleAngles(targetsAngle, num2, num, false);
			}
		}

		protected virtual void PreCalculate()
		{
			Vector3 vector = _owner.avatar.AttackTarget.XZPosition - _owner.avatar.XZPosition;
			vector.y = 0f;
			_disEntity2Target = vector.magnitude;
			_dirCamera2Entity = _owner.avatar.XZPosition - _owner.followData.cameraPosition;
			_dirCamera2Entity.y = 0f;
			_disCamera2Entity = _dirCamera2Entity.magnitude;
			_dirCamera2Target = _owner.avatar.AttackTarget.XZPosition - _owner.followData.cameraPosition;
			_dirCamera2Target.y = 0f;
			_disCamera2Target = _dirCamera2Target.magnitude;
			_entityCollisionRadius = GetColliderRadius(_owner.avatar);
			_targetCollisionRadius = GetColliderRadius(_owner.avatar.AttackTarget);
		}

		protected float GetColliderRadius(BaseMonoEntity entity)
		{
			CapsuleCollider componentInChildren = entity.GetComponentInChildren<CapsuleCollider>();
			if (!componentInChildren)
			{
				return 0.5f;
			}
			return componentInChildren.radius;
		}

		private void CalcFeasibleAngles(float targetsAngle, float asinNum, float acosNum, bool allowObtuseAngle)
		{
			float item = 180f - asinNum - targetsAngle - acosNum;
			float item2 = 0f - (180f - asinNum - targetsAngle + acosNum);
			_feasibleAngles.Add(item);
			_feasibleAngles.Add(item2);
			if (allowObtuseAngle)
			{
				float item3 = asinNum - targetsAngle - acosNum;
				float item4 = 0f - (asinNum - targetsAngle + acosNum);
				_feasibleAngles.Add(item3);
				_feasibleAngles.Add(item4);
			}
			if ((double)Vector3.Cross(_dirCamera2Entity, _dirCamera2Target).y < 0.0)
			{
				for (int i = 0; i < _feasibleAngles.Count; i++)
				{
					_feasibleAngles[i] = TruncateRotateAngle(_feasibleAngles[i]);
				}
			}
			else
			{
				for (int j = 0; j < _feasibleAngles.Count; j++)
				{
					_feasibleAngles[j] = TruncateRotateAngle(0f - _feasibleAngles[j]);
				}
			}
		}

		private float TruncateRotateAngle(float rotateAngle)
		{
			if (Mathf.Abs(rotateAngle) > 180f)
			{
				if (rotateAngle > 0f)
				{
					return rotateAngle - 360f;
				}
				return rotateAngle + 360f;
			}
			return rotateAngle;
		}

		protected void DoAdjToCorrectTargetsAngle(float targetsAngle, bool allowObtuseAngle)
		{
			float num = Mathf.Acos((_disCamera2Entity * _disCamera2Entity + _disEntity2Target * _disEntity2Target - _disCamera2Target * _disCamera2Target) / (2f * _disCamera2Entity * _disEntity2Target)) * 57.29578f;
			float num2 = Mathf.Asin(_disCamera2Entity / _disEntity2Target * Mathf.Sin(targetsAngle * ((float)Math.PI / 180f))) * 57.29578f;
			if (!float.IsNaN(targetsAngle) && !float.IsNaN(num2) && !float.IsNaN(num))
			{
				CalcFeasibleAngles(targetsAngle, num2, num, allowObtuseAngle);
			}
			if (_feasibleAngles.Count > 0)
			{
				float num3 = 0f;
				_feasibleAngles.Sort((float a, float b) => Mathf.Abs(a).CompareTo(Mathf.Abs(b)));
				num3 = _feasibleAngles[0];
				_targetPolar = _owner.anchorPolar + num3;
				if ((double)Vector3.Cross(_dirCamera2Entity, _dirCamera2Target).y < 0.0)
				{
					_targetForwardDelta = (0f - targetsAngle) / 2f;
				}
				else
				{
					_targetForwardDelta = targetsAngle / 2f;
				}
				_hasSetTargetForwardDelta = true;
			}
		}

		private void StartSolvingWallHit()
		{
			_isCameraSolveWallHit = true;
			if (Application.targetFrameRate == -1)
			{
				DELTA_TIME_PER_FRAME = 1f / 60f;
			}
			else
			{
				DELTA_TIME_PER_FRAME = 1f / (float)Application.targetFrameRate;
			}
			_owner.needLerpForwardThisFrame = false;
			_owner.needLerpPositionThisFrame = false;
			_originAnchorPolar = GetRealPolar();
			_currentAnchorPolar = _originAnchorPolar;
			_disableLerpSumUpTime = 0f;
			_owner.focusOnAvatar = true;
			_targetPolar = Miscs.NormalizedRotateAngle(_originAnchorPolar, _targetPolar);
			_speedSolvingWallHit = Miscs.AbsAngleDiff(_originAnchorPolar, _targetPolar) * _speedForOneCircle / 360f;
		}

		private float GetRealPolar()
		{
			Vector3 normalized = (_owner.cameraPosition - _owner.avatar.XZPosition).normalized;
			return Mathf.Atan2(normalized.z, normalized.x) * 57.29578f;
		}

		private void EndSolvingWallHit()
		{
			if (_isCameraSolveWallHit)
			{
				_isCameraSolveWallHit = false;
				_owner.needLerpForwardThisFrame = true;
				_owner.needLerpPositionThisFrame = true;
				_owner.focusOnAvatar = false;
				_disableLerpSumUpTime = 0f;
			}
		}

		private void ConvertToWorldSpace(float angle, out Vector3 followCenterPosition, out Vector3 cameraPosition)
		{
			Vector3 zero = Vector3.zero;
			zero.x = _owner.anchorRadius * Mathf.Cos(angle * ((float)Math.PI / 180f)) * Mathf.Cos(_owner.anchorElevation * ((float)Math.PI / 180f));
			zero.z = _owner.anchorRadius * Mathf.Sin(angle * ((float)Math.PI / 180f)) * Mathf.Cos(_owner.anchorElevation * ((float)Math.PI / 180f));
			zero.y = _owner.anchorRadius * Mathf.Sin(_owner.anchorElevation * ((float)Math.PI / 180f));
			followCenterPosition = _owner.avatar.XZPosition;
			followCenterPosition.y = _owner.followCenterY;
			cameraPosition = followCenterPosition + zero * _owner.cameraLocateRatio;
		}

		private void ConvertToWorldSpaceUseCurrentRadius(float angle, out Vector3 followCenterPosition, out Vector3 cameraPosition)
		{
			Vector3 zero = Vector3.zero;
			zero.x = _disCamera2Entity * Mathf.Cos(angle * ((float)Math.PI / 180f)) * Mathf.Cos(_owner.anchorElevation * ((float)Math.PI / 180f));
			zero.z = _disCamera2Entity * Mathf.Sin(angle * ((float)Math.PI / 180f)) * Mathf.Cos(_owner.anchorElevation * ((float)Math.PI / 180f));
			zero.y = _disCamera2Entity * Mathf.Sin(_owner.anchorElevation * ((float)Math.PI / 180f));
			followCenterPosition = _owner.avatar.XZPosition;
			followCenterPosition.y = _owner.followCenterY;
			cameraPosition = followCenterPosition + zero;
		}

		public void TryToSolveWallHit()
		{
			PreCalculate();
			CalcTargetPolarWhenWallHit();
		}

		public void ReplaceCamera()
		{
			PreCalculate();
			if (_disEntity2Target <= _entityCollisionRadius + _targetCollisionRadius - 0.1f)
			{
				_targetPolar = _owner.anchorPolar;
			}
			else if (_bioTargetCameraMode == 1)
			{
				CalculateTargetPolarModeHorizontal();
			}
			else
			{
				CalcTargetPolarIgnoreWallHit();
			}
		}

		private bool CheckWallHitByTargetPolar(float targetPolar, out RaycastHit wallHitRet)
		{
			Vector3 followCenterPosition;
			Vector3 cameraPosition;
			ConvertToWorldSpace(targetPolar, out followCenterPosition, out cameraPosition);
			RaycastHit wallHitRet2;
			bool result = CheckWallHit(cameraPosition, followCenterPosition, out wallHitRet2);
			wallHitRet = wallHitRet2;
			return result;
		}

		private bool CheckWallHit(Vector3 cameraPosition, Vector3 followCenterPosition, out RaycastHit wallHitRet)
		{
			float magnitude = (cameraPosition - followCenterPosition).magnitude;
			RaycastHit hitInfo;
			bool result = Physics.Raycast(followCenterPosition, cameraPosition - followCenterPosition, out hitInfo, magnitude, 1 << InLevelData.CAMERA_COLLIDER_LAYER);
			wallHitRet = hitInfo;
			return result;
		}

		protected void DoWhenSolveWallHit()
		{
			_owner.needLerpPositionThisFrame = false;
			_owner.needLerpForwardThisFrame = false;
			bool reachTargetPolar = false;
			float a = _speedSolvingWallHit - _accelerationSolvingWallHit * (_disableLerpSumUpTime + Time.deltaTime);
			a = Mathf.Max(a, _minSpeedSolvingWallHit);
			float delta = a * DELTA_TIME_PER_FRAME;
			float currentAnchorPolar = LerpAnchorPolarForMinMotion(_originAnchorPolar, _currentAnchorPolar, _targetPolar, delta, out reachTargetPolar);
			_owner.anchorPolar = (_currentAnchorPolar = currentAnchorPolar);
			if (reachTargetPolar)
			{
				EndSolvingWallHit();
			}
			else
			{
				_disableLerpSumUpTime += Time.deltaTime;
			}
		}

		protected void DoUpdateWhenHasAttackTarget()
		{
			if (!_isCameraSolveWallHit)
			{
				Vector3 xZPosition = _owner.avatar.XZPosition;
				xZPosition.y = _owner.followCenterY;
				RaycastHit wallHitRet;
				_isCurrentWallHit = CheckWallHit(_owner.followData.cameraPosition, xZPosition, out wallHitRet);
				if (_isCurrentWallHit)
				{
					float magnitude = (_owner.followData.cameraPosition - xZPosition).magnitude;
					float num = magnitude - wallHitRet.distance;
					if (num / magnitude > 0.75f)
					{
						TryToSolveWallHit();
					}
					else
					{
						ReplaceCamera();
					}
				}
				else
				{
					ReplaceCamera();
				}
				if (!_isCameraSolveWallHit)
				{
					float num2 = Miscs.AbsAngleDiff(_owner.anchorPolar, _targetPolar);
					if (!(num2 > 150f))
					{
						_owner.anchorPolar = _targetPolar;
					}
				}
				else
				{
					DoWhenSolveWallHit();
				}
			}
			else
			{
				DoWhenSolveWallHit();
			}
		}

		public override void Update()
		{
			_feasibleAngles.Clear();
			if (_mode == FollowMode.DirectionMode)
			{
				return;
			}
			if (_owner.avatar.AttackTarget == null)
			{
				_owner.TransitBaseState(_owner.followAvatarState);
				return;
			}
			_hasSetTargetForwardDelta = false;
			DoUpdateWhenHasAttackTarget();
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

		private float LerpAnchorPolarForMinMotion(float originAnchorPolar, float currentAnchorPolar, float targetAnchorPolar, float delta, out bool reachTargetPolar)
		{
			reachTargetPolar = false;
			float num = 0f;
			if (targetAnchorPolar >= originAnchorPolar)
			{
				num = currentAnchorPolar + delta;
				if (num >= targetAnchorPolar)
				{
					reachTargetPolar = true;
					num = targetAnchorPolar;
				}
			}
			else
			{
				num = currentAnchorPolar - delta;
				if (num <= targetAnchorPolar)
				{
					reachTargetPolar = true;
					num = targetAnchorPolar;
				}
			}
			return num;
		}

		public override void Exit()
		{
			EndSolvingWallHit();
			float to = Mathf.Atan2(0f - _owner.followData.cameraForward.z, 0f - _owner.followData.cameraForward.x) * 57.29578f;
			to = Miscs.NormalizedRotateAngle(_owner.anchorPolar, to);
			_owner.followAvatarState.SetEnteringLerpTarget(to);
			if (_mode == FollowMode.DirectionMode)
			{
				_mode = FollowMode.TargetMode;
				if (_owner.timedPullZState.active)
				{
					_owner.timedPullZState.ForceToExitState();
				}
			}
		}

		public override void ResetState()
		{
		}
	}
}
