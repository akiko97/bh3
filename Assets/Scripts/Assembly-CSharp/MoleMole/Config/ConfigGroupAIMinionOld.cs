using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigGroupAIMinionOld
	{
		public enum MoveType
		{
			Sync = 0,
			Follow = 1,
			Free = 2
		}

		public enum AttackType
		{
			Trigger = 0,
			Free = 1
		}

		public string MonsterName;

		public string AIName;

		public MoveType DefaultMoveType;

		public AttackType DefaultAttackType;

		public ConfigGroupAIMinionParamOld[] AIParams = new ConfigGroupAIMinionParamOld[0];

		public Dictionary<string, ConfigGroupAIMinionParamOld[]> TriggerAtcions = new Dictionary<string, ConfigGroupAIMinionParamOld[]>();
	}
}
