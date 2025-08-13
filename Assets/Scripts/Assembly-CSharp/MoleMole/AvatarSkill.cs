using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Avatar")]
	public class AvatarSkill : BehaviorDesigner.Runtime.Tasks.Action
	{
		private enum AvatarSkillState
		{
			Idle = 0,
			WaitingForSkillStart = 1,
			InSkill = 2,
			BeHitCanceled = 3
		}

		public string TriggerSkillName;

		public string SKillID;

		public bool StopMove;

		public float RetryTimeOut = 0.8f;

		public float SuccessSetCD;

		public float FailSetCD;

		public SharedFloat SkillCD;

		public float NormalizedEndTime = 1f;

		private BaseMonoAvatar _avatar;

		private AvatarActor _avatarActor;

		private AvatarSkillState _state;

		private float _timer;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
			_avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(_avatar.GetRuntimeID());
		}

		public override void OnStart()
		{
			_state = AvatarSkillState.Idle;
			_timer = RetryTimeOut;
			BaseMonoAvatar avatar = _avatar;
			avatar.onBeHitCanceled = (Action<string>)Delegate.Combine(avatar.onBeHitCanceled, new Action<string>(AvatarBeHitCancelCallback));
		}

		public override void OnEnd()
		{
			BaseMonoAvatar avatar = _avatar;
			avatar.onBeHitCanceled = (Action<string>)Delegate.Remove(avatar.onBeHitCanceled, new Action<string>(AvatarBeHitCancelCallback));
		}

		private void TryTriggerSkill()
		{
			_avatar.GetActiveAIController().TryUseSkill(TriggerSkillName);
			if (StopMove)
			{
				_avatar.GetActiveAIController().TryStop();
			}
		}

		private void AvatarBeHitCancelCallback(string skillID)
		{
			_state = AvatarSkillState.BeHitCanceled;
		}

		public override TaskStatus OnUpdate()
		{
			if (_state == AvatarSkillState.Idle)
			{
				if (_avatarActor.CanUseSkill(TriggerSkillName))
				{
					TryTriggerSkill();
					_state = AvatarSkillState.WaitingForSkillStart;
					return TaskStatus.Running;
				}
				SkillCD.SetValue(FailSetCD);
				return TaskStatus.Failure;
			}
			if (_state == AvatarSkillState.WaitingForSkillStart)
			{
				if (_avatar.CurrentSkillID == SKillID)
				{
					_state = AvatarSkillState.InSkill;
					return TaskStatus.Running;
				}
				_timer -= Time.deltaTime * _avatar.TimeScale;
				if (_timer < 0f)
				{
					SkillCD.SetValue(FailSetCD);
					return TaskStatus.Failure;
				}
				TryTriggerSkill();
				return TaskStatus.Running;
			}
			if (_state == AvatarSkillState.InSkill)
			{
				if (_avatar.CurrentSkillID == SKillID && _avatar.GetCurrentNormalizedTime() < NormalizedEndTime)
				{
					return TaskStatus.Running;
				}
				SkillCD.SetValue(SuccessSetCD);
				return TaskStatus.Success;
			}
			if (_state == AvatarSkillState.BeHitCanceled)
			{
				SkillCD.SetValue(FailSetCD);
				return TaskStatus.Failure;
			}
			return TaskStatus.Failure;
		}
	}
}
