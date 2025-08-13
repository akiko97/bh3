namespace MoleMole
{
	public class MonsterStastics
	{
		public bool isAlive = true;

		public MonsterKey key;

		public SafeFloat damage = 0f;

		public SafeFloat aliveTime = 0f;

		public SafeInt32 hitAvatarTimes = 0;

		public SafeInt32 breakAvatarTimes = 0;

		public SafeFloat dps = 0f;

		public SafeInt32 monsterCount = 0;

		public MonsterStastics(string monsterName, string configType, int level)
		{
			key = new MonsterKey
			{
				monsterName = monsterName,
				configType = configType,
				level = level
			};
			dps = 0f;
		}
	}
}
