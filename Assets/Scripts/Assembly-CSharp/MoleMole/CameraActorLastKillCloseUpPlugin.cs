using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class CameraActorLastKillCloseUpPlugin : BaseActorPlugin
	{
		public bool AlwaysTrigger;

		private MainCameraActor _cameraActor;

		private List<BaseMonoMonster> _waitDieMonsters;

		private bool _active;

		public CameraActorLastKillCloseUpPlugin(MainCameraActor cameraActor)
		{
			_cameraActor = cameraActor;
			_waitDieMonsters = new List<BaseMonoMonster>();
		}

		public bool IsPending()
		{
			return _active && _waitDieMonsters.Count > 0;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(_cameraActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_cameraActor.runtimeID);
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				AttachDieCallback(allMonsters[i]);
			}
			_active = true;
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(_cameraActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_cameraActor.runtimeID);
			for (int i = 0; i < _waitDieMonsters.Count; i++)
			{
				if (!(_waitDieMonsters[i] == null))
				{
					BaseMonoMonster baseMonoMonster = _waitDieMonsters[i];
					baseMonoMonster.onDie = (Action<BaseMonoMonster>)Delegate.Remove(baseMonoMonster.onDie, new Action<BaseMonoMonster>(MonsterDieCallback));
				}
			}
			_waitDieMonsters.Clear();
			_active = false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtLevelState)
			{
				return ListenLevelState((EvtLevelState)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			return false;
		}

		private void AttachDieCallback(BaseMonoMonster monster)
		{
			if (!_waitDieMonsters.Contains(monster))
			{
				monster.onDie = (Action<BaseMonoMonster>)Delegate.Combine(monster.onDie, new Action<BaseMonoMonster>(MonsterDieCallback));
				_waitDieMonsters.Add(monster);
			}
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			AttachDieCallback(Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(evt.monsterID));
			return true;
		}

		private void MonsterDieCallback(BaseMonoMonster monster)
		{
			if (Singleton<MonsterManager>.Instance.LivingMonsterCount() == 0 || AlwaysTrigger)
			{
				if (!Singleton<CameraManager>.Instance.GetMainCamera().levelAnimState.active)
				{
					ShowLastKillCameraEffect(monster);
				}
				if (!AlwaysTrigger)
				{
					_cameraActor.RemovePlugin(this);
				}
			}
		}

		private void ShowLastKillCameraEffect(BaseMonoMonster monster)
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			float magnitude = (monster.XZPosition - baseMonoAvatar.XZPosition).magnitude;
			string filePath = ((Singleton<MonsterManager>.Instance.LivingMonsterCount() != 0) ? "SlowMotionKill/Normal" : "SlowMotionKill/LastKill");
			ConfigCameraSlowMotionKill config = ConfigUtil.LoadConfig<ConfigCameraSlowMotionKill>(filePath);
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			float magnitude2 = (mainCamera.XZPosition - baseMonoAvatar.XZPosition).magnitude;
			mainCamera.SetSlowMotionKill(config, magnitude, magnitude2);
		}

		private bool ListenLevelState(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.EndWin || evt.state == EvtLevelState.State.EndLose)
			{
				_cameraActor.RemovePlugin(this);
			}
			return true;
		}
	}
}
