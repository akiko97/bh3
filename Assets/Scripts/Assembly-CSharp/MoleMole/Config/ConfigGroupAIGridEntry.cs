using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigGroupAIGridEntry : IOnLoaded
	{
		public string Name;

		public Dictionary<string, List<ConfigLeaderToMinionAction>> LeaderActions = new Dictionary<string, List<ConfigLeaderToMinionAction>>();

		public ConfigOverrideList Minions = ConfigOverrideList.EMPTY;

		public void OnLoaded()
		{
		}
	}
}
