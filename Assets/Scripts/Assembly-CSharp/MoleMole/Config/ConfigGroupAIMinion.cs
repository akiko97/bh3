using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigGroupAIMinion : IOnLoaded
	{
		private static Dictionary<string, ConfigGroupAIMinionParam[]> EMPTY_TRIGGER_ACTIONS = new Dictionary<string, ConfigGroupAIMinionParam[]>();

		public string MonsterName;

		public string AIName;

		public ConfigDynamicArguments AIParams = ConfigDynamicArguments.EMPTY;

		public Dictionary<string, ConfigGroupAIMinionParam[]> TriggerActions = EMPTY_TRIGGER_ACTIONS;

		public ConfigDynamicArguments AISpecials = ConfigDynamicArguments.EMPTY;

		public void OnLoaded()
		{
			PopulateDynamicArguments(AISpecials, AIParams);
			foreach (ConfigGroupAIMinionParam[] value in TriggerActions.Values)
			{
				ConfigGroupAIMinionParam[] array = value;
				foreach (ConfigGroupAIMinionParam configGroupAIMinionParam in array)
				{
					PopulateDynamicFloat(AISpecials, configGroupAIMinionParam.Delay);
					PopulateDynamicArguments(AISpecials, configGroupAIMinionParam.AIParams);
				}
			}
		}

		private void PopulateDynamicArguments(ConfigDynamicArguments source, ConfigDynamicArguments target)
		{
			List<string> list = new List<string>(target.Keys);
			foreach (string item in list)
			{
				string text = target[item] as string;
				if (text != null && text[0] == '%')
				{
					string key = text.Substring(1);
					target[item] = source[key];
				}
			}
		}

		private void PopulateDynamicFloat(ConfigDynamicArguments source, DynamicFloat df)
		{
			if (df.isDynamic)
			{
				object obj = source[df.dynamicKey];
				if (obj is int)
				{
					df.fixedValue = (int)obj;
				}
				else
				{
					df.fixedValue = (float)obj;
				}
				df.isDynamic = false;
				df.dynamicKey = null;
			}
		}
	}
}
