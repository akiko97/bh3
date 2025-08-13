using System;

namespace MoleMole
{
	[Serializable]
	public class DevMonsterData
	{
		public string monsterName;

		public string typeName;

		public bool isElite;

		public uint uniqueMonsterID;

		public int level;

		public string[] abilities;

		public bool isStationary;

		public override string ToString()
		{
			return string.Format("{0} - {1}", monsterName, typeName);
		}

		public DevMonsterData Clone()
		{
			DevMonsterData devMonsterData = new DevMonsterData();
			devMonsterData.monsterName = monsterName;
			devMonsterData.typeName = typeName;
			devMonsterData.isStationary = isStationary;
			devMonsterData.isElite = isElite;
			devMonsterData.abilities = abilities;
			return devMonsterData;
		}
	}
}
