using System.Collections.Generic;
using FullInspector;
using FullSerializer;

namespace MoleMole.Config
{
	[fsObject(Converter = typeof(ConfigOverrideGroupConverter))]
	public class ConfigOverrideGroup
	{
		public object Default;

		[InspectorNullable]
		public Dictionary<string, object> Overrides;

		public T GetConfig<T>(string name)
		{
			object value;
			if (name == "Default")
			{
				value = Default;
			}
			else if (Overrides == null)
			{
				value = Default;
			}
			else
			{
				Overrides.TryGetValue(name, out value);
				if (value == null)
				{
					value = Default;
				}
			}
			return (T)value;
		}
	}
}
