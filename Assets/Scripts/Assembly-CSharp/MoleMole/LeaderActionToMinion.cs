using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class LeaderActionToMinion : Action
	{
		public SharedString LeaderAction;

		[RequiredField]
		public SharedEntityDictionary MinionDict;

		public SharedString GruopAIGridType;

		private ConfigGroupAIGridEntry _gridEntry;

		public override void OnAwake()
		{
			base.OnAwake();
			if (!string.IsNullOrEmpty(GruopAIGridType.Value))
			{
				_gridEntry = AIData.GetGroupAIGridEntry(GruopAIGridType.Value);
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (_gridEntry == null)
			{
				return TaskStatus.Success;
			}
			string text = null;
			if (_gridEntry.LeaderActions.ContainsKey(LeaderAction.Value))
			{
				List<ConfigLeaderToMinionAction> list = _gridEntry.LeaderActions[LeaderAction.Value];
				for (int i = 0; i < list.Count; i++)
				{
					if (Random.value < list[i].Probability)
					{
						text = list[i].Name;
						break;
					}
				}
				if (text != null)
				{
					foreach (KeyValuePair<int, BaseMonoEntity> item in MinionDict.Value)
					{
						if (!item.Value.IsActive() || !(item.Value is BaseMonoMonster))
						{
							continue;
						}
						BTreeMonsterAIController bTreeMonsterAIController = ((BaseMonoMonster)item.Value).GetActiveAIController() as BTreeMonsterAIController;
						if (bTreeMonsterAIController == null)
						{
							continue;
						}
						ConfigGroupAIMinionParam[] array = _gridEntry.Minions.GetConfig<ConfigGroupAIMinion>(item.Key).TriggerActions[text];
						ConfigGroupAIMinionParam[] array2 = array;
						foreach (ConfigGroupAIMinionParam configGroupAIMinionParam in array2)
						{
							if (configGroupAIMinionParam.Interuption)
							{
								bTreeMonsterAIController.btree.SendEvent("Interruption", (object)true);
								bTreeMonsterAIController.btree.SendEvent("Interruption", (object)false);
								bTreeMonsterAIController.btree.SetVariableValue("Group_TriggerAttack", false);
							}
							if (configGroupAIMinionParam.Delay.fixedValue == 0f)
							{
								AIData.SetSharedVariableCompat(bTreeMonsterAIController.btree, configGroupAIMinionParam.AIParams);
							}
							else
							{
								bTreeMonsterAIController.DelayedSetParameter(configGroupAIMinionParam.Delay.fixedValue, configGroupAIMinionParam.AIParams);
							}
						}
					}
				}
			}
			return TaskStatus.Success;
		}
	}
}
