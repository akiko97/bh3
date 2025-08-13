using UnityEngine;

namespace MoleMole
{
	public class MainCameraActor : BasePluggedActor
	{
		private MonoMainCamera _mainCamera;

		private CameraActorLastKillCloseUpPlugin _closeUpPlugin;

		public override void Init(BaseMonoEntity entity)
		{
			_mainCamera = (MonoMainCamera)entity;
			runtimeID = _mainCamera.GetRuntimeID();
			_closeUpPlugin = new CameraActorLastKillCloseUpPlugin(this);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(runtimeID);
		}

		public void SetupLastKillCloseUp()
		{
			if (!HasPlugin<CameraActorLastKillCloseUpPlugin>())
			{
				AddPlugin(_closeUpPlugin);
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = base.ListenEvent(evt);
			if (evt is EvtStageReady)
			{
				flag |= ListenStageReady((EvtStageReady)evt);
			}
			else if (evt is EvtKilled)
			{
				flag |= ListenKill((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (_mainCamera.followState.active)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(baseMonoAvatar.GetRuntimeID());
			}
			else
			{
				Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(baseMonoAvatar.transform, new Vector3(0f, 2f, 0f));
				_mainCamera.SetupFollowAvatar(baseMonoAvatar.GetRuntimeID());
				_mainCamera.followState.SetEnterPolarMode(MainCameraFollowState.EnterPolarMode.AlongAvatarFacing, 0f);
				_mainCamera.TransitToFollow();
			}
			return true;
		}

		public bool ListenKill(EvtKilled evt)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (mainCamera.followState.active && mainCamera.followState.followAvatarAndBossState.active)
			{
				BaseMonoEntity bossTarget = mainCamera.followState.followAvatarAndBossState.bossTarget;
				if (bossTarget.GetRuntimeID() == evt.targetID)
				{
					Singleton<CameraManager>.Instance.DisableBossCamera();
				}
			}
			return true;
		}
	}
}
