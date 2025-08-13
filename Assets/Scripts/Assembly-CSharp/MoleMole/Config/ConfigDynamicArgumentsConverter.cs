using System;
using System.Collections.Generic;
using FullSerializer;

namespace MoleMole.Config
{
	public class ConfigDynamicArgumentsConverter : fsDirectConverter<ConfigDynamicArguments>
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new ConfigDynamicArguments();
		}

		protected override fsResult DoSerialize(ConfigDynamicArguments model, Dictionary<string, fsData> serialized)
		{
			foreach (string key in model.Keys)
			{
				object obj = model[key];
				if (obj is bool)
				{
					serialized.Add(key, new fsData((bool)obj));
					continue;
				}
				if (obj is float)
				{
					serialized.Add(key, new fsData((float)obj));
					continue;
				}
				if (obj is int)
				{
					serialized.Add(key, new fsData((int)obj));
					continue;
				}
				if (obj is string)
				{
					serialized.Add(key, new fsData((string)obj));
					continue;
				}
				return fsResult.Fail(string.Format("invalid dynamic argument type {0} - {1] ", key));
			}
			return fsResult.Success;
		}

		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ConfigDynamicArguments model)
		{
			foreach (string key in data.Keys)
			{
				fsData fsData = data[key];
				if (fsData.IsBool)
				{
					model.Add(key, fsData.AsBool);
					continue;
				}
				if (fsData.IsDouble)
				{
					model.Add(key, (float)fsData.AsDouble);
					continue;
				}
				if (fsData.IsInt64)
				{
					model.Add(key, (int)fsData.AsInt64);
					continue;
				}
				if (fsData.IsString)
				{
					model.Add(key, fsData.AsString);
					continue;
				}
				return fsResult.Fail(string.Format("invalid dynamic argument type {0} - {1} ", key, fsData.Type.ToString()));
			}
			return fsResult.Success;
		}
	}
}
