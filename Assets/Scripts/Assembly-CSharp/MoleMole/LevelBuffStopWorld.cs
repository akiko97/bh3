using System.Collections.Generic;

namespace MoleMole
{
	public class LevelBuffStopWorld : BaseLevelBuff
	{
		private const float STOP_WORLD_TIMESCALE = 0.05f;

		private EntityTimer _timer;

		private uint _stopWorldOwnerID;

		private bool _enteringTimeSlow;

		public LevelBuffStopWorld(LevelActor levelActor)
		{
			base.levelActor = levelActor;
			levelBuffType = LevelBuffType.StopWorld;
			_timer = new EntityTimer();
		}

		public void Setup(bool enteringTimeSlow, float duration, uint ownerID)
		{
			_timer.timespan = duration;
			_timer.Reset(true);
			_stopWorldOwnerID = ownerID;
			_enteringTimeSlow = enteringTimeSlow;
		}

		public override void OnAdded()
		{
			if (_enteringTimeSlow)
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.3f, 0.05f, delegate
				{
					if (isActive)
					{
						ApplyStopWorld();
					}
				});
			}
			else
			{
				ApplyStopWorld();
			}
		}

		public override void OnRemoved()
		{
			_timer.Reset(false);
			RemoveStopWorld();
		}

		private void ApplyStopWorld()
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				if (allAvatars[i].GetRuntimeID() != _stopWorldOwnerID)
				{
					ApplyStopEffectOn(allAvatars[i]);
				}
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int j = 0; j < allMonsters.Count; j++)
			{
				if (allMonsters[j].GetRuntimeID() != _stopWorldOwnerID)
				{
					ApplyStopEffectOn(allMonsters[j]);
				}
			}
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			for (int k = 0; k < allPropObjects.Count; k++)
			{
				if (allPropObjects[k].GetRuntimeID() != _stopWorldOwnerID)
				{
					ApplyStopEffectOn(allPropObjects[k]);
				}
			}
			Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Push(2, 0.05f);
		}

		private void RemoveStopWorld()
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				if (allAvatars[i].GetRuntimeID() != _stopWorldOwnerID)
				{
					RemoveStopEffectOn(allAvatars[i]);
				}
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int j = 0; j < allMonsters.Count; j++)
			{
				if (allMonsters[j].GetRuntimeID() != _stopWorldOwnerID)
				{
					RemoveStopEffectOn(allMonsters[j]);
				}
			}
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			for (int k = 0; k < allPropObjects.Count; k++)
			{
				if (allPropObjects[k].GetRuntimeID() != _stopWorldOwnerID)
				{
					RemoveStopEffectOn(allPropObjects[k]);
				}
			}
			Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Pop(2);
		}

		private void ApplyStopEffectOn(BaseMonoAbilityEntity target)
		{
			target.PushTimeScale(0.05f, 2);
		}

		private void RemoveStopEffectOn(BaseMonoAbilityEntity target)
		{
			target.PopTimeScale(2);
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtMonsterCreated)
			{
				return OnPostMonsterCreated((EvtMonsterCreated)evt);
			}
			if (evt is EvtPropObjectCreated)
			{
				return OnPostPropObjectCreated((EvtPropObjectCreated)evt);
			}
			return false;
		}

		private bool OnPostMonsterCreated(EvtMonsterCreated evt)
		{
			if (evt.monsterID == _stopWorldOwnerID)
			{
				return false;
			}
			BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(evt.monsterID);
			if (baseMonoMonster != null)
			{
				ApplyStopEffectOn(baseMonoMonster);
			}
			return true;
		}

		private bool OnPostPropObjectCreated(EvtPropObjectCreated evt)
		{
			if (evt.objectID == _stopWorldOwnerID)
			{
				return false;
			}
			BaseMonoPropObject baseMonoPropObject = Singleton<PropObjectManager>.Instance.TryGetPropObjectByRuntimeID(evt.objectID);
			if (baseMonoPropObject != null)
			{
				ApplyStopEffectOn(baseMonoPropObject);
			}
			return true;
		}

		public override void Core()
		{
			if (!muteUpdateDuration)
			{
				_timer.Core(1f);
				if (_timer.isTimeUp)
				{
					levelActor.StopLevelBuff(this);
					_timer.Reset(false);
				}
			}
		}
	}
}
