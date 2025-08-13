using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoPalsyBombProp : MonoTriggerProp
	{
		private enum PropState
		{
			Born = 0,
			Idle = 1,
			Chase = 2,
			Died = 3
		}

		private float _height = 1f;

		private float _currentSpeed;

		private float _selfRotateSpeed = 100f;

		private Vector3 _bornVelocity;

		private Vector3 _bornAcceleration;

		private PropState _state;

		private Rigidbody _rigidbody;

		protected override void Awake()
		{
			base.Awake();
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.velocity = Vector3.zero;
			_state = PropState.Idle;
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			_transform.position += new Vector3(0f, _height, 0f);
		}

		public void StartParabolaBorn(Vector3 bornPosition, Vector3 bornVelocity, Vector3 bornAcceleration)
		{
			_state = PropState.Born;
			_rigidbody.constraints &= (RigidbodyConstraints)(-5);
			base.transform.position = bornPosition;
			float num = Vector3.Dot(Vector3.up, bornVelocity);
			num = Vector3.Dot(Vector3.down, bornAcceleration);
			base.transform.position = bornPosition;
			_bornVelocity = bornVelocity;
			_bornAcceleration = bornAcceleration;
		}

		protected override void Update()
		{
			base.Update();
			float angle = Time.deltaTime * _selfRotateSpeed * TimeScale;
			base.transform.Rotate(base.transform.up, angle);
			if (_state == PropState.Born)
			{
				_bornVelocity += _bornAcceleration * TimeScale * Time.deltaTime;
				_rigidbody.velocity = _bornVelocity;
				float num = Vector3.Dot(Vector3.down, _bornVelocity);
				if (num > 0f && base.transform.position.y < _height)
				{
					_rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
					_state = PropState.Idle;
				}
				return;
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (!(localAvatar != null))
			{
				return;
			}
			if (_state == PropState.Idle)
			{
				_currentSpeed = 0f;
				_rigidbody.velocity = Vector3.zero;
				float warningRange = config.PropArguments.WarningRange;
				float num2 = Vector3.Distance(localAvatar.XZPosition, XZPosition);
				if (num2 < warningRange)
				{
					_state = PropState.Chase;
				}
			}
			else if (_state == PropState.Chase)
			{
				if (_currentSpeed < config.PropArguments.MaxMoveSpeed)
				{
					_currentSpeed += config.PropArguments.Acceleration * TimeScale * Time.deltaTime;
					if (_currentSpeed > config.PropArguments.MaxMoveSpeed)
					{
						_currentSpeed = config.PropArguments.MaxMoveSpeed;
					}
				}
				float escapeRange = config.PropArguments.EscapeRange;
				float num3 = Vector3.Distance(localAvatar.XZPosition, XZPosition);
				if (num3 > escapeRange)
				{
					_state = PropState.Idle;
				}
				Vector3 targetCorner = localAvatar.XZPosition;
				if (GlobalVars.USE_GET_PATH_SWITCH && Singleton<DetourManager>.Instance.GetTargetPosition(this, XZPosition, localAvatar.XZPosition, ref targetCorner))
				{
					Debug.DrawLine(XZPosition, targetCorner, Color.yellow, 0.1f);
				}
				_rigidbody.velocity = (targetCorner - XZPosition).normalized * _currentSpeed;
			}
			else if (_state != PropState.Died)
			{
			}
			base.transform.position = new Vector3(base.transform.position.x, _height, base.transform.position.z);
		}

		public override void SetDied(KillEffect killEffect)
		{
			base.SetDied(killEffect);
			if (config.PropArguments.OnKillEffectPattern != null)
			{
				FireEffect(config.PropArguments.OnKillEffectPattern, base.transform.position, base.transform.forward);
			}
		}

		protected override void OnEffectiveTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == InLevelData.STAGE_COLLIDER_LAYER)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectForceKilled(_runtimeID));
			}
			else
			{
				base.OnEffectiveTriggerEnter(other);
			}
		}
	}
}
