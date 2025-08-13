using MoleMole.Config;

namespace MoleMole
{
	public class LevelMissionStatisticsPlugin : BaseActorPlugin
	{
		private LevelActor _levelActor;

		public LevelMissionStatisticsPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override bool OnEvent(BaseEvent evt)
		{
			bool flag = base.OnEvent(evt);
			if (evt is EvtLevelState)
			{
				flag |= OnLevelState((EvtLevelState)evt);
			}
			return flag;
		}

		public bool OnLevelState(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.EndWin || evt.state == EvtLevelState.State.EndLose)
			{
				Singleton<MissionModule>.Instance.FlushMissionDataToServer();
			}
			return true;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return ListenKill((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenKill(EvtKilled evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			if (num == 4)
			{
				BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetAvatarByRuntimeID(evt.killerID);
				if (baseMonoAvatar == null)
				{
					return true;
				}
				Singleton<MissionModule>.Instance.TryToUpdateKillAnyEnemy();
				MonsterActor monsterActor = (MonsterActor)Singleton<EventManager>.Instance.GetActor(evt.targetID);
				if (monsterActor != null)
				{
					if (monsterActor.uniqueMonsterID != 0)
					{
						Singleton<MissionModule>.Instance.TryToUpdateKillUniqueMonsterMission(monsterActor.uniqueMonsterID);
					}
					else
					{
						string subTypeName = monsterActor.metaConfig.subTypeName;
						MonsterUIMetaData monsterUIMetaDataByName = MonsterUIMetaDataReaderExtend.GetMonsterUIMetaDataByName(subTypeName);
						int monsterID = monsterUIMetaDataByName.monsterID;
						Singleton<MissionModule>.Instance.TryToUpdateKillMonsterMission(monsterID);
					}
					Singleton<MissionModule>.Instance.TryToUpdateKillMonsterWithCategoryName(monsterActor.metaConfig.categoryName);
				}
				ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(baseMonoAvatar.config, evt.killerAnimEventID);
				if (configAvatarAnimEvent != null && configAvatarAnimEvent.AttackProperty != null && configAvatarAnimEvent.AttackProperty.CategoryTag != null)
				{
					AttackResult.AttackCategoryTag[] categoryTag = configAvatarAnimEvent.AttackProperty.CategoryTag;
					if (categoryTag.Length > 0)
					{
						Singleton<MissionModule>.Instance.TryToUpdateKillByAttackCategoryTag(categoryTag);
					}
				}
			}
			return true;
		}
	}
}
