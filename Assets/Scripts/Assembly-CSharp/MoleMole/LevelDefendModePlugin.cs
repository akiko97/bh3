using System.Collections.Generic;

namespace MoleMole
{
	public class LevelDefendModePlugin : BaseActorPlugin
	{
		private class DefendModeData
		{
			public DefendModeType type;

			public int currentValue;

			public int targetValue;

			public int uniqueID;

			public bool isKey;

			public DefendModeData(DefendModeType type, int targetValue, bool isKey)
			{
				this.type = type;
				this.targetValue = targetValue;
				currentValue = 0;
				uniqueID = 0;
				this.isKey = isKey;
			}

			public DefendModeData(DefendModeType type, int value, int currentValue, bool isKey)
			{
				this.type = type;
				targetValue = value;
				this.currentValue = currentValue;
				uniqueID = 0;
				this.isKey = isKey;
			}

			public DefendModeData(int uniqueID, bool isKey)
			{
				type = DefendModeType.Certain;
				targetValue = 0;
				currentValue = 0;
				this.uniqueID = uniqueID;
				this.isKey = isKey;
			}
		}

		private LevelActor _levelActor;

		private List<DefendModeData> _defendModeDataList = new List<DefendModeData>();

		private List<int> _certainMonsterList = new List<int>();

		private bool _active;

		private List<TriggerFieldActor> _triggerFieldActorList = new List<TriggerFieldActor>();

		private int _monsterEnterAmount;

		private int _monsterKillAmount;

		private int _maxMonsterDisappearAmount;

		public int MonsterEnterAmount
		{
			get
			{
				return _monsterEnterAmount;
			}
		}

		public int MonsterKillAmount
		{
			get
			{
				return _monsterKillAmount;
			}
		}

		public LevelDefendModePlugin(LevelActor levelActor, int targetValue)
		{
			_levelActor = levelActor;
			_monsterEnterAmount = 0;
			_monsterKillAmount = 0;
			_maxMonsterDisappearAmount = targetValue;
			SetActive(false);
		}

		public LevelDefendModePlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
			_monsterEnterAmount = 0;
			_monsterKillAmount = 0;
			_maxMonsterDisappearAmount = 0;
			SetActive(false);
		}

		public void Reset(int targetValue = 0)
		{
			_monsterEnterAmount = 0;
			_monsterKillAmount = 0;
			_maxMonsterDisappearAmount = targetValue;
			_defendModeDataList.Clear();
			_certainMonsterList.Clear();
			SetActive(false);
		}

		public void Stop()
		{
			_monsterEnterAmount = 0;
			_monsterKillAmount = 0;
			_maxMonsterDisappearAmount = 0;
			_triggerFieldActorList.Clear();
			_defendModeDataList.Clear();
			_certainMonsterList.Clear();
			SetActive(false);
		}

		public void AddTriggerFieldActor(TriggerFieldActor triggerFieldActor)
		{
			if (triggerFieldActor != null)
			{
				_triggerFieldActorList.Add(triggerFieldActor);
			}
		}

		public void RemoveTriggerFieldActor(TriggerFieldActor triggerFieldActor)
		{
			if (triggerFieldActor != null && _triggerFieldActorList.Contains(triggerFieldActor))
			{
				_triggerFieldActorList.Remove(triggerFieldActor);
			}
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public void SetActive(bool active)
		{
			_active = active;
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDefendModeText, _active));
		}

		public void AddModeData(DefendModeType modeType, int targetValue, bool isKey = false)
		{
			DefendModeData item = new DefendModeData(modeType, targetValue, isKey);
			_defendModeDataList.Add(item);
			if (isKey)
			{
				RefreshDisplay(item);
			}
		}

		public void AddModeData(int uniqueID, bool isKey = false)
		{
			DefendModeData item = new DefendModeData(uniqueID, isKey);
			_defendModeDataList.Add(item);
			_certainMonsterList.Add(uniqueID);
			if (isKey)
			{
				RefreshDisplay(item, uniqueID);
			}
		}

		public void AddModeData(DefendModeType modeType, int targetValue, int currentValue, bool isKey = false)
		{
			DefendModeData item = new DefendModeData(modeType, targetValue, currentValue, isKey);
			_defendModeDataList.Add(item);
			if (isKey)
			{
				RefreshDisplay(item);
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			bool flag = base.OnEvent(evt);
			if (!_active)
			{
				return flag;
			}
			if (evt is EvtLevelState)
			{
				flag |= OnLevelState((EvtLevelState)evt);
			}
			return flag;
		}

		public bool OnLevelState(EvtLevelState evt)
		{
			if (evt.state == EvtLevelState.State.Start)
			{
				SetActive(true);
			}
			return true;
		}

		public bool OnKilled(EvtKilled evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.killerID) != 3)
			{
				return false;
			}
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4)
			{
				return false;
			}
			if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.killerID))
			{
				return false;
			}
			_monsterKillAmount++;
			if (_maxMonsterDisappearAmount != 0 && _monsterEnterAmount + _monsterKillAmount == _maxMonsterDisappearAmount)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(DefendModeType.Result, _maxMonsterDisappearAmount));
			}
			return false;
		}

		public bool OnFieldEnter(EvtFieldEnter evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.otherID) != 4)
			{
				return false;
			}
			bool flag = false;
			foreach (TriggerFieldActor triggerFieldActor in _triggerFieldActorList)
			{
				if (evt.targetID == triggerFieldActor.runtimeID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(evt.otherID);
			if (monsterByRuntimeID == null)
			{
				return false;
			}
			_monsterEnterAmount++;
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.otherID);
			if (actor != null)
			{
				actor.ForceRemoveImmediatelly();
			}
			if (_maxMonsterDisappearAmount != 0 && _monsterEnterAmount + _monsterKillAmount == _maxMonsterDisappearAmount)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(DefendModeType.Result, _maxMonsterDisappearAmount));
			}
			if (_defendModeDataList.Count <= 0)
			{
				return false;
			}
			List<DefendModeData> list = new List<DefendModeData>();
			foreach (DefendModeData defendModeData in _defendModeDataList)
			{
				if (defendModeData.type == DefendModeType.Single && defendModeData.targetValue > 0)
				{
					defendModeData.currentValue++;
					RefreshDisplay(defendModeData);
					if (defendModeData.currentValue >= defendModeData.targetValue)
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(defendModeData.type, defendModeData.targetValue));
						list.Add(defendModeData);
					}
				}
				else if (defendModeData.type == DefendModeType.Group && defendModeData.targetValue > 0)
				{
					defendModeData.currentValue++;
					RefreshDisplay(defendModeData);
					if (defendModeData.currentValue >= defendModeData.targetValue)
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(defendModeData.type, defendModeData.targetValue));
						defendModeData.currentValue = 0;
					}
				}
				else if (defendModeData.type == DefendModeType.Certain && defendModeData.uniqueID != 0)
				{
					if (_certainMonsterList.Contains(defendModeData.uniqueID) && monsterByRuntimeID.MonsterTagID == defendModeData.uniqueID)
					{
						_certainMonsterList.Remove(defendModeData.uniqueID);
						Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(defendModeData.uniqueID));
						RefreshDisplay(defendModeData, monsterByRuntimeID.MonsterTagID);
						list.Add(defendModeData);
					}
					else
					{
						RefreshDisplay(defendModeData, monsterByRuntimeID.MonsterTagID);
					}
				}
			}
			foreach (DefendModeData item in list)
			{
				if (_defendModeDataList.Contains(item))
				{
					_defendModeDataList.Remove(item);
				}
			}
			list.Clear();
			return false;
		}

		private void RefreshDisplay(DefendModeData item, int monsterTagID = 0)
		{
			string body = string.Empty;
			if (item.isKey)
			{
				if (item.type == DefendModeType.Single)
				{
					body = string.Format("{0}/{1}", item.currentValue, item.targetValue);
				}
				else if (item.type == DefendModeType.Group)
				{
					body = string.Format("{0}/{1}", item.currentValue, item.targetValue);
				}
				else if (item.type == DefendModeType.Certain)
				{
					body = ((monsterTagID == 0 || monsterTagID != item.uniqueID) ? "0/1" : "1/1");
				}
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetDefendModeText, body));
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool result = false;
			if (!_active)
			{
				return result;
			}
			if (evt is EvtFieldEnter)
			{
				return OnFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtKilled)
			{
				return OnKilled((EvtKilled)evt);
			}
			return result;
		}
	}
}
