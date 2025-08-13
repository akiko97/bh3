using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Avatar")]
	public class AvatarAttack : BehaviorDesigner.Runtime.Tasks.Action
	{
		private enum AvatarAttackState
		{
			Idle = 0,
			WaitingForAttackStart = 1,
			InAttack = 2,
			BeHitCanceled = 3
		}

		private const string TriggerAttackName = "ATK";

		public string[] AttackSkillIDs;

		public float RetryTimeOut = 0.8f;

		public float SuccessSetCD;

		public float FailSetCD;

		public SharedFloat AttackCD;

		private BaseMonoAvatar _avatar;

		private AvatarAttackState _state;

		private float _timer;

		private int _skillIDIx;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(_avatar.GetRuntimeID());
		}

		public override void OnStart()
		{
			_state = AvatarAttackState.Idle;
			_timer = RetryTimeOut;
			_skillIDIx = 0;
			BaseMonoAvatar avatar = _avatar;
			avatar.onBeHitCanceled = (Action<string>)Delegate.Combine(avatar.onBeHitCanceled, new Action<string>(AvatarBeHitCancelCallback));
		}

		public override void OnEnd()
		{
			BaseMonoAvatar avatar = _avatar;
			avatar.onBeHitCanceled = (Action<string>)Delegate.Remove(avatar.onBeHitCanceled, new Action<string>(AvatarBeHitCancelCallback));
		}

		private void AvatarBeHitCancelCallback(string skillID)
		{
			_state = AvatarAttackState.BeHitCanceled;
		}

		public override TaskStatus OnUpdate()
		{
			if (_state == AvatarAttackState.Idle)
			{
				_avatar.GetActiveAIController().TryUseSkill("ATK");
				_state = AvatarAttackState.WaitingForAttackStart;
				return TaskStatus.Running;
			}
			if (_state == AvatarAttackState.WaitingForAttackStart)
			{
				if (_avatar.CurrentSkillID == AttackSkillIDs[0])
				{
					_state = AvatarAttackState.InAttack;
					return TaskStatus.Running;
				}
				_timer -= Time.deltaTime * _avatar.TimeScale;
				if (_timer < 0f)
				{
					return TaskStatus.Failure;
				}
				_avatar.GetActiveAIController().TryUseSkill("ATK");
				return TaskStatus.Running;
			}
			if (_state == AvatarAttackState.InAttack)
			{
				if (_skillIDIx == AttackSkillIDs.Length - 1)
				{
					if (_avatar.CurrentSkillID == AttackSkillIDs[_skillIDIx])
					{
						return TaskStatus.Running;
					}
					return TaskStatus.Success;
				}
				if (_avatar.CurrentSkillID == AttackSkillIDs[_skillIDIx])
				{
					_avatar.GetActiveAIController().TryUseSkill("ATK");
					return TaskStatus.Running;
				}
				if (_avatar.CurrentSkillID == AttackSkillIDs[_skillIDIx + 1])
				{
					_avatar.GetActiveAIController().TryUseSkill("ATK");
					_skillIDIx++;
					return TaskStatus.Running;
				}
				return TaskStatus.Failure;
			}
			if (_state == AvatarAttackState.BeHitCanceled)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Failure;
		}
	}
}
