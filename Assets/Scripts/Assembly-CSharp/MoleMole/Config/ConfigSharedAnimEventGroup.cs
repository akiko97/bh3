using System.Collections.Generic;
using FullSerializer;

namespace MoleMole.Config
{
	[fsObject(Converter = typeof(ConfigSharedAnimEventGroupConverter))]
	public class ConfigSharedAnimEventGroup
	{
		public string Prefix;

		public string Type = "ConfigEntityAnimEvent";

		public Dictionary<string, ConfigEntityAnimEvent> AnimEvents;
	}
}
