using System.Collections.Generic;
using FullSerializer;

namespace MoleMole.Config
{
	[GeneratePartialHash]
	[fsObject(Converter = typeof(ConfigDynamicArgumentsConverter))]
	public class ConfigDynamicArguments : Dictionary<string, object>, IHashable
	{
		public static ConfigDynamicArguments EMPTY = new ConfigDynamicArguments();

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (this == null)
			{
				return;
			}
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, object> current = enumerator.Current;
					HashUtils.ContentHashOnto(current.Key, ref lastHash);
					HashUtils.ContentHashOntoFallback(current.Value, ref lastHash);
				}
			}
		}
	}
}
