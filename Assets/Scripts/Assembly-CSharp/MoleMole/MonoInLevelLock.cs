using UnityEngine;

namespace MoleMole
{
	public class MonoInLevelLock : MonoBehaviour
	{
		public enum Status
		{
			None = 0,
			Begin = 1,
			Loop = 2,
			End = 3
		}

		private const float BEGIN_EFFECT_PLAY_TIME = 0.2f;

		public ParticleSystem loopEffect;

		public ParticleSystem beginEffect;

		public ParticleSystem endEffect;

		private Status _status;

		private BaseMonoEntity _lockFollowTarget;

		private bool _triggerBegin;

		private bool _triggerEnd;

		private float _timer;

		public void Awake()
		{
			ClearAllEffect();
		}

		public void SetLockFollowTarget(BaseMonoEntity lockFollowTarget)
		{
			BaseMonoEntity baseMonoEntity = ((_status != Status.End) ? _lockFollowTarget : null);
			if (baseMonoEntity != null && lockFollowTarget == null)
			{
				_status = Status.End;
				_triggerEnd = true;
			}
			else if (baseMonoEntity != lockFollowTarget)
			{
				_lockFollowTarget = lockFollowTarget;
				_status = Status.Begin;
				_triggerBegin = true;
			}
		}

		public void Core()
		{
			if (GlobalVars.muteInlevelLock)
			{
				return;
			}
			if (_lockFollowTarget != null)
			{
				if (_lockFollowTarget.IsActive())
				{
					Vector3 position = _lockFollowTarget.GetAttachPoint("RootNode").position;
					Camera cameraComponent = Singleton<CameraManager>.Instance.GetMainCamera().cameraComponent;
					Vector3 position2 = cameraComponent.WorldToViewportPoint(position);
					position2.z = 10f;
					base.transform.position = cameraComponent.ViewportToWorldPoint(position2);
				}
				else
				{
					Reset();
				}
			}
			if (_status == Status.Begin)
			{
				if (_triggerBegin)
				{
					base.gameObject.SetActive(true);
					ClearAllEffect();
					beginEffect.gameObject.SetActive(true);
					_triggerBegin = false;
				}
				if (_timer <= 0.2f)
				{
					_timer += Time.unscaledDeltaTime;
					return;
				}
				_status = Status.Loop;
				loopEffect.gameObject.SetActive(true);
			}
			else if (_status == Status.End)
			{
				if (_triggerEnd)
				{
					loopEffect.gameObject.SetActive(false);
					endEffect.gameObject.SetActive(true);
					_triggerEnd = false;
				}
				if (!endEffect.IsAlive(false))
				{
					Reset();
				}
			}
		}

		private void ClearAllEffect()
		{
			beginEffect.Clear();
			loopEffect.Clear();
			endEffect.Clear();
			beginEffect.gameObject.SetActive(false);
			loopEffect.gameObject.SetActive(false);
			endEffect.gameObject.SetActive(false);
			_timer = 0f;
		}

		private void Reset()
		{
			_status = Status.None;
			base.gameObject.SetActive(false);
			_lockFollowTarget = null;
		}
	}
}
