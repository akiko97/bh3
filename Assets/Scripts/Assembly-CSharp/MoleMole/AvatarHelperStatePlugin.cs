using System;
using MoleMole.Config;

namespace MoleMole
{
	public class AvatarHelperStatePlugin : BaseActorPlugin
	{
		public enum State
		{
			Idle = 0,
			TriggerSwitchIn = 1,
			OnStage = 2,
			SwitchingOut = 3,
			Dead = 4
		}

		private AvatarActor _avatarActor;

		private BaseMonoAvatar _avatar;

		public State _state;

		public AvatarHelperStatePlugin(AvatarActor avatarActor)
		{
			_state = State.Idle;
			_avatarActor = avatarActor;
			_avatar = _avatarActor.avatar;
			BaseMonoAvatar avatar = _avatar;
			avatar.onDie = (Action<BaseMonoAvatar>)Delegate.Combine(avatar.onDie, new Action<BaseMonoAvatar>(OnHelperDie));
		}

		public override void Core()
		{
			if (_state == State.TriggerSwitchIn)
			{
				DoHelperSwitchIn();
				_state = State.OnStage;
			}
		}

		public void TriggerSwitchIn()
		{
			if (_state != State.Dead)
			{
				_state = State.TriggerSwitchIn;
			}
		}

		public void TriggerSwitchOut(bool force)
		{
			if (_state == State.OnStage)
			{
				_avatar.TriggerSwitchOut(force ? BaseMonoAvatar.AvatarSwapOutType.Force : BaseMonoAvatar.AvatarSwapOutType.Delayed);
				_state = State.SwitchingOut;
			}
		}

		public bool IsOnStage()
		{
			return _state == State.OnStage;
		}

		private void DoHelperSwitchIn()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowHelperCutIn));
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			_avatar.gameObject.SetActive(true);
			_avatar.transform.position = localAvatar.XZPosition;
			_avatar.transform.forward = localAvatar.FaceDirection;
			_avatar.TriggerSwitchIn();
			_avatar.SetShaderData(E_ShaderData.AvatarHelper, true);
			_avatar.ForceUseAIController();
		}

		private void OnHelperDie(BaseMonoAvatar avatar)
		{
			avatar.SetDied(KillEffect.KillImmediately);
			_state = State.Dead;
		}
	}
}
