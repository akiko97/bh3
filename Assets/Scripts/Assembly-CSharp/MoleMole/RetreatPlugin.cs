using UnityEngine;

namespace MoleMole
{
	public sealed class RetreatPlugin : BaseEntityFuncPlugin
	{
		private enum RetreatMode
		{
			Retreat = 0,
			BlowVelocityScaled = 1,
			BlowDecelerated = 2
		}

		private enum NamedStateRetreatState
		{
			WaitingForState = 0,
			InState = 1
		}

		public const int EXECUTION_ORDER = 100;

		private const float LERP_RATIO = 28f;

		private const float RETREAT_WALL_KEEP_DISTANCE_TO_WALL = 0.3f;

		private const float RETREAT_WALL_OFFSET_DISTANCE = 0.8f;

		private const float RETREAT_DECELERATE = 7f;

		private const float END_THRESHOLD = 0.1f;

		private Vector3 _retreatDir;

		private float _velocity;

		private bool active;

		private RetreatMode _retreatMode;

		private NamedStateRetreatState _namedRetreatState;

		private string _namedRetreatName;

		private int _scaleVelocityStackIx;

		private float _scaleVelocityScale;

		private float _decelerateInitVelocity;

		private float _decelerateEndNormalizedTime;

		private IRetreatable _retreatEntity;

		public RetreatPlugin(BaseMonoEntity entity)
			: base(entity)
		{
			_retreatEntity = (IRetreatable)entity;
		}

		public void CancelActiveRetreat()
		{
			if (_retreatMode == RetreatMode.Retreat)
			{
				_retreatEntity.SetNeedOverrideVelocity(false);
			}
			else if (_retreatMode == RetreatMode.BlowVelocityScaled)
			{
				if (_namedRetreatState == NamedStateRetreatState.InState)
				{
					_retreatEntity.PopProperty("Animator_RigidBodyVelocityRatio", _scaleVelocityStackIx);
				}
			}
			else if (_retreatMode == RetreatMode.BlowDecelerated)
			{
				_retreatEntity.SetNeedOverrideVelocity(false);
			}
			active = false;
		}

		public void BlowVelocityScaledRetreat(Vector3 retreatDir, float velocity, string namedState, float velocityScale)
		{
			if (active)
			{
				CancelActiveRetreat();
			}
			_retreatMode = RetreatMode.BlowVelocityScaled;
			_retreatDir = retreatDir;
			_velocity = velocity;
			_namedRetreatName = namedState;
			_scaleVelocityScale = velocityScale;
			_namedRetreatState = NamedStateRetreatState.WaitingForState;
			active = true;
		}

		private void BlowVelocityScaledRetreatCore()
		{
			if (_namedRetreatState == NamedStateRetreatState.WaitingForState)
			{
				if (_retreatEntity.GetCurrentNamedState() == _namedRetreatName)
				{
					_namedRetreatState = NamedStateRetreatState.InState;
					_scaleVelocityStackIx = _retreatEntity.PushProperty("Animator_RigidBodyVelocityRatio", _scaleVelocityScale);
				}
			}
			else if (_namedRetreatState == NamedStateRetreatState.InState && _retreatEntity.GetCurrentNamedState() != _namedRetreatName)
			{
				_retreatEntity.PopProperty("Animator_RigidBodyVelocityRatio", _scaleVelocityStackIx);
				active = false;
			}
		}

		public void StandRetreat(Vector3 retreatDir, float velocity)
		{
			retreatDir.y = 0f;
			if (IsActive())
			{
				if (_retreatMode == RetreatMode.Retreat)
				{
					Vector3 vector = _retreatDir * _velocity;
					Vector3 vector2 = retreatDir * velocity;
					Vector3 vector3 = vector + vector2;
					_retreatDir.y = 0f;
					_retreatDir = vector3.normalized;
					_velocity = vector3.magnitude;
				}
				else
				{
					CancelActiveRetreat();
					_retreatDir = retreatDir.normalized;
					_velocity = velocity;
				}
			}
			else
			{
				_retreatDir = retreatDir.normalized;
				_velocity = velocity;
			}
			_retreatMode = RetreatMode.Retreat;
			active = true;
		}

		private void StandRetreatFixedCore()
		{
			_retreatEntity.SetNeedOverrideVelocity(true);
			_retreatEntity.SetOverrideVelocity(_retreatDir * _velocity * _entity.TimeScale);
			_velocity = Mathf.Lerp(_velocity, 0f, 28f * Time.fixedDeltaTime * _entity.TimeScale);
			if (_velocity >= -0.1f && _velocity <= 0.1f)
			{
				_retreatEntity.SetNeedOverrideVelocity(false);
				active = false;
			}
		}

		public void BlowDecelerateRetreat(Vector3 retreatDir, float velocity, string namedState, float velocityRatio, float endNormalizedTime)
		{
			if (active)
			{
				CancelActiveRetreat();
			}
			_retreatMode = RetreatMode.BlowDecelerated;
			_retreatDir = retreatDir;
			_decelerateInitVelocity = (_velocity = velocity * velocityRatio);
			_decelerateEndNormalizedTime = endNormalizedTime;
			_namedRetreatName = namedState;
			_namedRetreatState = NamedStateRetreatState.WaitingForState;
			active = true;
		}

		private void BlowDecelerateRetreatFixedCore()
		{
			if (_namedRetreatState == NamedStateRetreatState.WaitingForState)
			{
				if (_retreatEntity.GetCurrentNamedState() == _namedRetreatName)
				{
					_namedRetreatState = NamedStateRetreatState.InState;
				}
			}
			else if (_namedRetreatState == NamedStateRetreatState.InState)
			{
				_velocity = Mathf.Lerp(_decelerateInitVelocity, 0f, Mathf.Clamp01(_retreatEntity.GetCurrentNormalizedTime() / _decelerateEndNormalizedTime));
				_retreatEntity.SetNeedOverrideVelocity(true);
				_retreatEntity.SetOverrideVelocity(_velocity * _retreatDir * _entity.TimeScale);
				if (_retreatEntity.GetCurrentNamedState() != _namedRetreatName || (_velocity >= -0.1f && _velocity <= 0.1f))
				{
					_retreatEntity.SetNeedOverrideVelocity(false);
					active = false;
				}
			}
		}

		public override void FixedCore()
		{
			if (active)
			{
				if (_retreatMode == RetreatMode.Retreat)
				{
					StandRetreatFixedCore();
				}
				else if (_retreatMode == RetreatMode.BlowDecelerated)
				{
					BlowDecelerateRetreatFixedCore();
				}
			}
		}

		public override void Core()
		{
			if (active && _retreatMode == RetreatMode.BlowVelocityScaled)
			{
				BlowVelocityScaledRetreatCore();
			}
		}

		public override bool IsActive()
		{
			return active;
		}
	}
}
