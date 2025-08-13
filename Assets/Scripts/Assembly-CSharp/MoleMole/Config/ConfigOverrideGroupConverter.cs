using System;
using System.Collections.Generic;
using FullSerializer;

namespace MoleMole.Config
{
	public class ConfigOverrideGroupConverter : fsBaseConfigOverrideConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new ConfigOverrideGroup();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(ConfigOverrideGroup);
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
			Dictionary<string, fsData> asDictionary = data.AsDictionary;
			fsData subitem;
			if ((success += CheckKey(data, "Default", out subitem)).Failed)
			{
				return success;
			}
			List<string> list = new List<string>(asDictionary.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				if (!(text == "Default"))
				{
					fsData outData;
					success += Override(subitem, asDictionary[text], out outData);
					if (success.Failed)
					{
						return success;
					}
					asDictionary[text] = outData;
				}
			}
			ConfigOverrideGroup configOverrideGroup = (ConfigOverrideGroup)instance;
			fsData data2 = asDictionary["Default"];
			object result = null;
			Serializer.TryDeserialize(data2, typeof(object), ref result);
			configOverrideGroup.Default = result;
			if (result is IOnLoaded)
			{
				((IOnLoaded)result).OnLoaded();
			}
			bool flag = list.Remove("Default");
			if (list.Count > 0)
			{
				configOverrideGroup.Overrides = new Dictionary<string, object>();
				for (int j = 0; j < list.Count; j++)
				{
					string key = list[j];
					data2 = asDictionary[key];
					result = null;
					Serializer.TryDeserialize(data2, typeof(object), ref result);
					configOverrideGroup.Overrides[key] = result;
					if (result is IOnLoaded)
					{
						((IOnLoaded)result).OnLoaded();
					}
				}
			}
			return fsResult.Success;
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			fsResult success = fsResult.Success;
			serialized = fsData.CreateDictionary();
			Dictionary<string, fsData> asDictionary = serialized.AsDictionary;
			ConfigOverrideGroup configOverrideGroup = (ConfigOverrideGroup)instance;
			fsData data;
			success += Serializer.TrySerialize(configOverrideGroup.Default.GetType(), configOverrideGroup.Default, out data);
			if (success.Failed)
			{
				return success;
			}
			asDictionary.Add("Default", data);
			foreach (string key in configOverrideGroup.Overrides.Keys)
			{
				object obj = configOverrideGroup.Overrides[key];
				success += Serializer.TrySerialize(obj.GetType(), obj, out data);
				if (success.Failed)
				{
					return success;
				}
				asDictionary.Add(key, data);
			}
			return success;
		}
	}
}
