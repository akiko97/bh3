using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoleMole.Config;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class GetMinions : Action
	{
		[RequiredField]
		public SharedEntityDictionary MinionDict;

		public SharedString GruopAIGridType;

		private BaseMonoMonster monster;

		private Dictionary<int, BaseMonoEntity> _minions;

		private ConfigOverrideList _minionConfigLs;

		public override void OnAwake()
		{
			if (!string.IsNullOrEmpty(GruopAIGridType.Value))
			{
				monster = GetComponent<BaseMonoMonster>();
				_minions = MinionDict.Value;
				if (_minions == null)
				{
					_minions = new Dictionary<int, BaseMonoEntity>();
					MinionDict.Value = _minions;
				}
				_minionConfigLs = AIData.GetGroupAIGridEntry(GruopAIGridType.Value).Minions;
			}
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (_minionConfigLs == null)
			{
				return TaskStatus.Success;
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < _minionConfigLs.length; i++)
			{
				ConfigGroupAIMinion config = _minionConfigLs.GetConfig<ConfigGroupAIMinion>(i);
				if (_minions.ContainsKey(i))
				{
					if (_minions[i] != null)
					{
						continue;
					}
					_minions.Remove(i);
				}
				for (int j = 0; j < allMonsters.Count; j++)
				{
					if (!(allMonsters[j].MonsterName == config.MonsterName) || !(allMonsters[j] != monster))
					{
						continue;
					}
					BehaviorTree component = allMonsters[j].GetComponent<BehaviorTree>();
					bool flag = true;
					SharedBool sharedBool = component.GetVariable("Group_IsMinion") as SharedBool;
					if (!sharedBool.Value)
					{
						flag = false;
					}
					SharedEntity sharedEntity = component.GetVariable("Group_LeaderEntity") as SharedEntity;
					if (sharedEntity != null && sharedEntity.Value != null)
					{
						flag = false;
					}
					SharedString sharedString = component.GetVariable("GroupAIGrid") as SharedString;
					if (sharedString != null && !string.IsNullOrEmpty(sharedString.Value))
					{
						flag = false;
					}
					if (flag && allMonsters[j].IsAIControllerActive())
					{
						BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)allMonsters[j].GetActiveAIController();
						if (bTreeMonsterAIController.IsBehaviorRunning())
						{
							bTreeMonsterAIController.SetActive(false);
							bTreeMonsterAIController.SetActive(true);
							component.SetVariableValue("Group_LeaderEntity", monster);
							AIData.SetSharedVariableCompat(component, config.AIParams);
							_minions.Add(i, allMonsters[j]);
							break;
						}
					}
				}
			}
			return TaskStatus.Success;
		}
	}
}
