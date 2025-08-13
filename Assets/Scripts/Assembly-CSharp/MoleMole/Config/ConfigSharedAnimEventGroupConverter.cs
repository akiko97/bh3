using System;
using System.Collections.Generic;
using FullSerializer;

namespace MoleMole.Config
{
	public class ConfigSharedAnimEventGroupConverter : fsConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new ConfigSharedAnimEventGroup();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(ConfigSharedAnimEventGroup);
		}

		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult success = fsResult.Success;
			fsResult _fsResult = (success += CheckType(data, fsDataType.Object));
			if (_fsResult.Failed)
			{
				return success;
			}
			fsData subitem;
			if ((success += CheckKey(data, "Prefix", out subitem)).Failed)
			{
				return success;
			}
			fsData subitem2;
			if ((success += CheckKey(data, "Type", out subitem2)).Failed)
			{
				return success;
			}
			fsData subitem3;
			if ((success += CheckKey(data, "AnimEvents", out subitem3)).Failed)
			{
				return success;
			}
			if ((success += CheckType(subitem, fsDataType.String)).Failed)
			{
				return success;
			}
			if ((success += CheckType(subitem2, fsDataType.String)).Failed)
			{
				return success;
			}
			if ((success += CheckType(subitem3, fsDataType.Object)).Failed)
			{
				return success;
			}
			ConfigSharedAnimEventGroup configSharedAnimEventGroup = (ConfigSharedAnimEventGroup)instance;
			configSharedAnimEventGroup.Prefix = subitem.AsString;
			if (!configSharedAnimEventGroup.Prefix.StartsWith("#"))
			{
				return success += fsResult.Fail("prefix should starts with '#': " + success);
			}
			configSharedAnimEventGroup.Type = subitem2.AsString;
			configSharedAnimEventGroup.AnimEvents = new Dictionary<string, ConfigEntityAnimEvent>();
			Dictionary<string, fsData> asDictionary = subitem3.AsDictionary;
			foreach (KeyValuePair<string, fsData> item in asDictionary)
			{
				string key = item.Key;
				fsData value = item.Value;
				if ((success += CheckType(subitem3, fsDataType.Object)).Failed)
				{
					return success;
				}
				Dictionary<string, fsData> asDictionary2 = value.AsDictionary;
				asDictionary2["$type"] = subitem2;
				ConfigEntityAnimEvent instance2 = null;
				Serializer.TryDeserialize(value, ref instance2);
				configSharedAnimEventGroup.AnimEvents.Add(string.Format("{0}/{1}", configSharedAnimEventGroup.Prefix, key), instance2);
			}
			return fsResult.Success;
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			fsResult success = fsResult.Success;
			Serializer.TrySerialize(instance.GetType(), instance, out serialized);
			return success;
		}
	}
}
