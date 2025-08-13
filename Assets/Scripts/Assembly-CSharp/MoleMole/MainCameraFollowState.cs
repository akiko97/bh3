using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	public class MainCameraFollowState : BaseMainCameraState
	{
		public class FollowData
		{
			public Vector3 anchorPosition;

			public Vector3 cameraPosition;

			public Vector3 cameraForwardNoForwardDelta;

			public Vector3 cameraForward;

			public Vector3 followCenterPosition;
		}

		public enum RangeState
		{
			Near = 0,
			Far = 1,
			Furter = 2,
			High = 3,
			Higher = 4
		}

		public enum EnterPolarMode
		{
			NearestPointOnSphere = 0,
			AlongAvatarFacing = 1,
			AlongTargetPolar = 2
		}

		private const int LPF_POSITIONS_COUNT = 3;

		public Vector3 followCenterXZPosition;

		public float followCenterY;

		public float anchorRadius = 6f;

		public float anchorPolar;

		public float anchorElevation;

		public float forwardDeltaAngle;

		public bool isCameraLocateRatioUserDefined;

		public float cameraLocateRatio = 0.535f;

		public FollowData followData;

		public bool needLerpPositionThisFrame;

		public bool needLerpForwardThisFrame;

		public bool lerpPositionOvershootLastFrame;

		public bool lerpForwardOvershootLastFrame;

		public bool needSmoothFollowCenterThisFrame;

		public float posLerpRatio = 1f;

		public float forwardLerpRatio = 1f;

		private Vector3[] _lastFollowCenters;

		public bool needLPF;

		private EnterPolarMode _enterPolarMode;

		private float _enterPolarArgument;

		public BaseMonoAvatar avatar;

		public bool focusOnAvatar;

		private RaycastHit _wallHit;

		public FollowAvatarPoleState followAvatarState;

		public FollowAvatarAndTargetState followAvatarAndTargetState;

		public FollowAvatarAndBossState followAvatarAndBossState;

		public FollowAvatarAndCrowdState followAvatarAndCrowdState;

		public FollowAvatarControlledRotate followAvatarControlledRotate;

		public FollowRotateToAvatarFacing rotateToAvatarFacingState;

		public FollowStandBy standByState;

		public FollowLookAtPosition lookAtPositionState;

		public FollowRangeTransit rangeTransitState;

		public FollowTimedPullZ timedPullZState;

		public FollowSuddenChange suddenChangeState;

		public FollowSuddenRecover suddenRecoverState;

		public FollowSlowMotionKill slowMotionKillState;

		public FollowRecovering recoverState;

		private BaseFollowBaseState _baseState;

		public BaseFollowBaseState lastBaseState;

		private BaseFollowShortState _shortState;

		private BaseFollowBaseState _nextBaseState;

		private float _stableModeCDTimer;

		public MonoMainCamera mainCamera
		{
			get
			{
				return _owner;
			}
		}

		public MainCameraFollowState(MonoMainCamera camera)
			: base(camera)
		{
			lerpDirectionalLight = true;
			followData = new FollowData();
			rotateToAvatarFacingState = new FollowRotateToAvatarFacing(this);
			standByState = new FollowStandBy(this);
			lookAtPositionState = new FollowLookAtPosition(this);
			rangeTransitState = new FollowRangeTransit(this);
			timedPullZState = new FollowTimedPullZ(this);
			suddenChangeState = new FollowSuddenChange(this);
			suddenRecoverState = new FollowSuddenRecover(this);
			slowMotionKillState = new FollowSlowMotionKill(this);
			followAvatarState = new FollowAvatarPoleState(this);
			followAvatarAndTargetState = new FollowAvatarAndTargetState(this);
			followAvatarAndBossState = new FollowAvatarAndBossState(this);
			followAvatarAndCrowdState = new FollowAvatarAndCrowdState(this);
			followAvatarControlledRotate = new FollowAvatarControlledRotate(this);
			recoverState = new FollowRecovering(this);
			anchorRadius = 6f;
			followCenterY = 1.2f;
			anchorElevation = MainCameraData.CAMERA_DEFAULT_ELEVATION_DEGREE;
			forwardDeltaAngle = 0f;
			cameraFOV = mainCamera.originalFOV;
			recoverState.SetupRecoverRadius(anchorRadius);
			recoverState.SetupRecoverCenterY(followCenterY);
			recoverState.SetupRecoverElevation(anchorElevation);
			recoverState.SetupRecoverForwardDelta(forwardDeltaAngle);
			recoverState.SetupRecoverLerpPosRatio(1f);
			recoverState.SetupRecoverLerpForwardRatio(1f);
			recoverState.SetupRecoverFOV(cameraFOV);
		}

		public override void Enter()
		{
			followCenterXZPosition = avatar.XZPosition;
			lastBaseState = _baseState;
			_baseState = GetTargetBaseState(false, avatar.AttackTarget);
			_baseState.SetActive(true);
			_baseState.Enter();
			lastBaseState = null;
			_lastFollowCenters = new Vector3[3];
			if (needLPF)
			{
				for (int i = 0; i < _lastFollowCenters.Length; i++)
				{
					_lastFollowCenters[i] = followCenterXZPosition;
				}
			}
			if (_enterPolarMode == EnterPolarMode.AlongAvatarFacing)
			{
				Vector3 faceDirection = avatar.FaceDirection;
				anchorPolar = Mathf.Atan2(0f - faceDirection.z, 0f - faceDirection.x) * 57.29578f;
			}
			else if (_enterPolarMode == EnterPolarMode.NearestPointOnSphere)
			{
				Vector3 normalized = (_owner.transform.position - followCenterXZPosition).normalized;
				anchorPolar = Mathf.Atan2(normalized.z, normalized.x) * 57.29578f;
			}
			else if (_enterPolarMode == EnterPolarMode.AlongTargetPolar)
			{
				anchorPolar = _enterPolarArgument;
			}
			ConvertToFollowData();
			cameraPosition = followData.cameraPosition;
			cameraForward = followData.cameraForward;
			cameraFOV = _owner.originalFOV;
			_stableModeCDTimer = 0f;
		}

		public void SetEnterPolarMode(EnterPolarMode mode, float polar = 0f)
		{
			_enterPolarMode = mode;
			_enterPolarArgument = polar;
		}

		public override void Exit()
		{
			_baseState.Exit();
			_baseState.SetActive(false);
			TryRemoveShortState();
			followAvatarState.ResetState();
			followAvatarAndTargetState.ResetState();
			followAvatarAndBossState.ResetState();
			followAvatarAndCrowdState.ResetState();
			followAvatarControlledRotate.ResetState();
			recoverState.RecoverImmediately();
		}

		public void TryToTransitToOtherBaseState(bool checkHasAnyControl, BaseMonoEntity attackTarget = null)
		{
			BaseFollowBaseState targetBaseState = GetTargetBaseState(checkHasAnyControl, attackTarget);
			if (targetBaseState != null)
			{
				TransitBaseState(targetBaseState);
			}
		}

		private BaseFollowBaseState GetTargetBaseState(bool checkHasAnyControl, BaseMonoEntity attackTarget = null)
		{
			bool flag = false;
			if (lastBaseState == followAvatarAndBossState)
			{
				BaseMonoEntity bossTarget = ((FollowAvatarAndBossState)lastBaseState).bossTarget;
				if (bossTarget != null && bossTarget.IsActive())
				{
					followAvatarAndBossState.bossTarget = bossTarget;
					return followAvatarAndBossState;
				}
				flag = true;
			}
			else
			{
				if (lastBaseState == followAvatarAndCrowdState)
				{
					return followAvatarAndCrowdState;
				}
				flag = true;
			}
			if (flag)
			{
				if (attackTarget != null && attackTarget.IsActive())
				{
					return followAvatarAndTargetState;
				}
				if (avatar.IsLockDirection)
				{
					followAvatarAndTargetState.SwitchMode(FollowAvatarAndTargetState.FollowMode.DirectionMode);
				}
				if (!checkHasAnyControl)
				{
					return followAvatarState;
				}
				if (avatar.GetActiveControlData().hasAnyControl)
				{
					return followAvatarState;
				}
			}
			return null;
		}

		private void ConvertToFollowData()
		{
			Vector3 vector = new Vector3
			{
				x = anchorRadius * Mathf.Cos(anchorPolar * ((float)Math.PI / 180f)) * Mathf.Cos(anchorElevation * ((float)Math.PI / 180f)),
				z = anchorRadius * Mathf.Sin(anchorPolar * ((float)Math.PI / 180f)) * Mathf.Cos(anchorElevation * ((float)Math.PI / 180f)),
				y = anchorRadius * Mathf.Sin(anchorElevation * ((float)Math.PI / 180f))
			};
			followData.followCenterPosition = followCenterXZPosition;
			followData.followCenterPosition.y = followCenterY;
			followData.anchorPosition = followData.followCenterPosition + vector;
			followData.cameraPosition = followData.followCenterPosition + vector * cameraLocateRatio;
			followData.cameraForward = followData.followCenterPosition - followData.cameraPosition;
			followData.cameraForward.Normalize();
			followData.cameraForwardNoForwardDelta = followData.cameraForward;
			followData.cameraForward = Quaternion.AngleAxis(forwardDeltaAngle, Vector3.up) * followData.cameraForward;
			followData.cameraForward.Normalize();
		}

		public override void Update()
		{
			if (needLPF)
			{
				for (int num = _lastFollowCenters.Length - 1; num > 0; num--)
				{
					_lastFollowCenters[num] = _lastFollowCenters[num - 1];
				}
				_lastFollowCenters[0] = followCenterXZPosition;
			}
			Vector3 vector = followCenterXZPosition;
			followCenterXZPosition = avatar.XZPosition;
			Vector3 vector2 = vector - followCenterXZPosition;
			needLerpPositionThisFrame = true;
			needLerpForwardThisFrame = true;
			needSmoothFollowCenterThisFrame = false;
			SubStateTransitionUpdate();
			if (recoverState.active)
			{
				recoverState.Update();
			}
			if (_nextBaseState != null)
			{
				_baseState.Exit();
				_baseState.SetActive(false);
				lastBaseState = _baseState;
				_baseState = _nextBaseState;
				_baseState.Enter();
				_baseState.SetActive(true);
				_nextBaseState = null;
				if (_shortState != null && _baseState.maskedShortStates.Contains(_shortState))
				{
					RemoveShortState();
				}
			}
			if (_shortState != null)
			{
				_shortState.Update();
				bool flag = false;
				if (_shortState != null)
				{
					flag = _shortState.isSkippingBaseState;
				}
				if (!flag || _baseState.cannotBeSkipped)
				{
					_baseState.Update();
				}
				if (_shortState != null)
				{
					_shortState.PostUpdate();
				}
			}
			else
			{
				_baseState.Update();
			}
			if (needLPF && needSmoothFollowCenterThisFrame)
			{
				Vector3 vector3 = followCenterXZPosition;
				for (int i = 0; i < _lastFollowCenters.Length; i++)
				{
					vector3 += _lastFollowCenters[i];
				}
				followCenterXZPosition = vector3 / (_lastFollowCenters.Length + 1);
				UnityEngine.Debug.DrawLine(followCenterXZPosition, followCenterXZPosition + Vector3.up);
			}
			ConvertToFollowData();
			Vector3 vector4 = followData.cameraPosition;
			Vector3 vector5 = followData.cameraForward;
			float magnitude = (vector4 - followData.followCenterPosition).magnitude;
			if (Physics.Raycast(followData.followCenterPosition, -followData.cameraForwardNoForwardDelta, out _wallHit, magnitude, 1 << InLevelData.CAMERA_COLLIDER_LAYER))
			{
				float num2 = magnitude - _wallHit.distance;
				vector4 = Vector3.Lerp(_wallHit.point, vector4, 0.1f);
				vector4.y += num2 * 0.1f;
				_owner.cameraComponent.nearClipPlane = Mathf.Lerp(_owner.originalNearClip, 0.01f, num2 / magnitude);
				Vector3 axis = Vector3.Cross(Vector3.up, cameraForward);
				vector5 = Quaternion.AngleAxis(0.05f * num2 * 57.29578f, axis) * vector5;
				vector5.Normalize();
				cameraShakeRatio = 1f - num2 / magnitude;
			}
			else
			{
				_owner.cameraComponent.nearClipPlane = _owner.originalNearClip;
			}
			float num3 = Time.deltaTime * _owner.TimeScale;
			float value = ((num3 != 0f) ? (vector2.magnitude / Time.deltaTime) : 0f);
			float num4 = Miscs.NormalizedClamp(value, 5f, 12f);
			if (needLerpPositionThisFrame)
			{
				float num5 = Time.deltaTime * 7.9f * (1f + num4) * posLerpRatio;
				Vector3 a = cameraPosition - followData.followCenterPosition;
				Vector3 b = vector4 - followData.followCenterPosition;
				Vector3 vector6 = Vector3.Slerp(a, b, Mathf.Clamp01(num5));
				cameraPosition = vector6 + followData.followCenterPosition;
				lerpPositionOvershootLastFrame = num5 >= 1f;
			}
			else
			{
				cameraPosition = vector4;
			}
			if (needLerpForwardThisFrame)
			{
				float num6 = Time.deltaTime * 5f * (1f + num4) * forwardLerpRatio;
				cameraForward.Normalize();
				vector5.Normalize();
				cameraForward = MonoMainCamera.CameraForwardLerp(cameraForward, vector5, Mathf.Clamp01(num6));
				lerpForwardOvershootLastFrame = num6 >= 1f;
			}
			else if (focusOnAvatar)
			{
				Vector3 vector7 = followData.followCenterPosition - cameraPosition;
				cameraForward = vector7;
				cameraForward.Normalize();
			}
			else
			{
				cameraForward = vector5;
			}
			if (cameraFOV > 0f)
			{
				_owner.cameraComponent.fieldOfView = cameraFOV;
			}
		}

		public void TransitBaseState(BaseFollowBaseState toSubState, bool forceTransit = false)
		{
			if (CanBaseStateTransit(toSubState, forceTransit))
			{
				if (_nextBaseState != null)
				{
				}
				_nextBaseState = toSubState;
			}
		}

		private bool CanBaseStateTransit(BaseFollowBaseState toSubState, bool forceTransit = false)
		{
			if (forceTransit)
			{
				return true;
			}
			if (_baseState == followAvatarState && toSubState == followAvatarState)
			{
				return false;
			}
			if (toSubState == followAvatarControlledRotate)
			{
				return true;
			}
			if (_baseState == followAvatarAndBossState)
			{
				return false;
			}
			if (_baseState == followAvatarAndCrowdState)
			{
				return false;
			}
			return true;
		}

		public void AddShortState(BaseFollowShortState shortState)
		{
			if (_baseState != null && !_baseState.maskedShortStates.Contains(shortState))
			{
				_shortState = shortState;
				_shortState.SetActive(true);
				_shortState.Enter();
				SubStateStatusReset();
			}
		}

		public void RemoveShortState()
		{
			_shortState.SetActive(false);
			_shortState.Exit();
			_shortState = null;
		}

		public void TryRemoveShortState()
		{
			if (_shortState != null)
			{
				RemoveShortState();
			}
		}

		public void AddOrReplaceShortState(BaseFollowShortState shortState)
		{
			if (_shortState != null)
			{
				RemoveShortState();
				AddShortState(shortState);
			}
			else
			{
				AddShortState(shortState);
			}
		}

		private void SubStateStatusReset()
		{
			_stableModeCDTimer = 0f;
		}

		private void SubStateTransitionUpdate()
		{
			if (_shortState == standByState)
			{
				return;
			}
			if (recoverState.active || followAvatarAndTargetState.active || followAvatarAndBossState.active || followAvatarAndCrowdState.active)
			{
				_stableModeCDTimer = 0f;
			}
			else if (avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
			{
				_stableModeCDTimer += Time.deltaTime * _owner.TimeScale;
				if (_stableModeCDTimer >= 3f)
				{
					AddOrReplaceShortState(standByState);
					_stableModeCDTimer = 0f;
				}
			}
			else
			{
				_stableModeCDTimer = 0f;
			}
		}

		private void OnAvatarAttackTargetChanged(BaseMonoEntity attackTarget)
		{
			if (_baseState == followAvatarControlledRotate)
			{
				TryToTransitToOtherBaseState(false, attackTarget);
			}
			else if (attackTarget != null && attackTarget.IsActive() && _baseState == followAvatarState)
			{
				TransitBaseState(followAvatarAndTargetState);
			}
			else if (attackTarget == null)
			{
				if (avatar.IsLockDirection)
				{
					followAvatarAndTargetState.SwitchMode(FollowAvatarAndTargetState.FollowMode.DirectionMode);
				}
				else
				{
					TransitBaseState(followAvatarState);
				}
			}
		}

		private void OnAvatarLockDirectionChanged(bool direction)
		{
			if (!direction && avatar.AttackTarget == null)
			{
				TransitBaseState(followAvatarState);
			}
		}

		public void SetupFollowAvatar(uint avatarID)
		{
			if (avatar != null)
			{
				BaseMonoAvatar baseMonoAvatar = avatar;
				baseMonoAvatar.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Remove(baseMonoAvatar.onAttackTargetChanged, new Action<BaseMonoEntity>(OnAvatarAttackTargetChanged));
				BaseMonoAvatar baseMonoAvatar2 = avatar;
				baseMonoAvatar2.onLockDirectionChanged = (Action<bool>)Delegate.Remove(baseMonoAvatar2.onLockDirectionChanged, new Action<bool>(OnAvatarLockDirectionChanged));
			}
			avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(avatarID);
			BaseMonoAvatar baseMonoAvatar3 = avatar;
			baseMonoAvatar3.onAttackTargetChanged = (Action<BaseMonoEntity>)Delegate.Combine(baseMonoAvatar3.onAttackTargetChanged, new Action<BaseMonoEntity>(OnAvatarAttackTargetChanged));
			BaseMonoAvatar baseMonoAvatar4 = avatar;
			baseMonoAvatar4.onLockDirectionChanged = (Action<bool>)Delegate.Combine(baseMonoAvatar4.onLockDirectionChanged, new Action<bool>(OnAvatarLockDirectionChanged));
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("NG_HSOD_DEBUG")]
		public void DebugDrawSphereCoords(Color color, float duration)
		{
			Vector3 vector = new Vector3
			{
				x = anchorRadius * Mathf.Cos(anchorPolar * ((float)Math.PI / 180f)) * Mathf.Cos(anchorElevation * ((float)Math.PI / 180f)),
				z = anchorRadius * Mathf.Sin(anchorPolar * ((float)Math.PI / 180f)) * Mathf.Cos(anchorElevation * ((float)Math.PI / 180f)),
				y = anchorRadius * Mathf.Sin(anchorElevation * ((float)Math.PI / 180f))
			};
			Vector3 vector2 = followCenterXZPosition;
			vector2.y = followCenterY;
			Vector3 vector3 = vector2 + vector * cameraLocateRatio;
			(vector2 - vector3).Normalize();
			UnityEngine.Debug.DrawLine(vector2, vector2 + vector, color, duration);
		}
	}
}
