using System.Collections.Generic;
using BehaviorDesigner.Runtime;

namespace MoleMole
{
	public class LevelAIPlugin : BaseActorPlugin
	{
		public const string AVATAR_BE_ATTACK_MAX_NUM_NAME = "AvatarBeAttackMaxNum";

		public const string AVATAR_BE_ATTACK_NUM_NAME = "AvatarBeAttackNum";

		private LevelActor _levelActor;

		private List<BaseMonoMonster> _attackingMonsters;

		public LevelAIPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
			_attackingMonsters = new List<BaseMonoMonster>();
		}

		public override void OnAdded()
		{
			SetAvatarBeAttackMaxNum(4);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public void SetAvatarBeAttackMaxNum(int maxNum)
		{
			GlobalVariables.Instance.SetVariableValue("AvatarBeAttackMaxNum", maxNum);
			GlobalVariables.Instance.SetVariableValue("AvatarBeAttackNum", 0);
		}

		public void RefreshAvatarBeAttackMaxNum()
		{
			GlobalVariables.Instance.SetVariableValue("AvatarBeAttackNum", _attackingMonsters.Count);
		}

		public void AddAttackingMonster(BaseMonoMonster monster)
		{
			if (!_attackingMonsters.Contains(monster))
			{
				_attackingMonsters.Add(monster);
			}
			RefreshAvatarBeAttackMaxNum();
		}

		public void RemoveAttackingMonster(BaseMonoMonster monster)
		{
			if (_attackingMonsters.Contains(monster))
			{
				_attackingMonsters.Remove(monster);
			}
			RefreshAvatarBeAttackMaxNum();
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
				BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(evt.targetID);
				if (baseMonoMonster != null && _attackingMonsters.Contains(baseMonoMonster))
				{
					RemoveAttackingMonster(baseMonoMonster);
				}
			}
			return true;
		}
	}
}
