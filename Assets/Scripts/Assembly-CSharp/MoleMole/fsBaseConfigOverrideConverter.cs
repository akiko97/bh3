using System.Collections.Generic;
using FullSerializer;

namespace MoleMole
{
	public abstract class fsBaseConfigOverrideConverter : fsConverter
	{
		protected fsResult Override(fsData defaultData, fsData targetData, out fsData outData)
		{
			fsResult success = fsResult.Success;
			outData = fsData.CreateDictionary();
			fsResult _fsResult = (success += CheckType(defaultData, fsDataType.Object));
			if (_fsResult.Failed)
			{
				return success;
			}
			if ((success += CheckType(targetData, fsDataType.Object)).Failed)
			{
				return success;
			}
			Dictionary<string, fsData> asDictionary = defaultData.AsDictionary;
			Dictionary<string, fsData> asDictionary2 = targetData.AsDictionary;
			Dictionary<string, fsData> asDictionary3 = outData.AsDictionary;
			foreach (string key in asDictionary.Keys)
			{
				fsData _fsData = asDictionary[key];
				fsData value = null;
				asDictionary2.TryGetValue(key, out value);
				if (_fsData.IsDictionary && value != null)
				{
					fsResult fsResult2 = (success += CheckType(value, fsDataType.Object));
					if (fsResult2.Failed)
					{
						return success;
					}
					fsData outData2;
					success += Override(_fsData, value, out outData2);
					asDictionary3.Add(key, outData2);
				}
				else if (value == null || _fsData.Type == value.Type)
				{
					asDictionary3.Add(key, (!(value != null)) ? _fsData : value);
				}
				else if ((_fsData.Type == fsDataType.Double && value.Type == fsDataType.Int64) || (_fsData.Type == fsDataType.Int64 && value.Type == fsDataType.Double))
				{
					asDictionary3.Add(key, value);
				}
				else
				{
					success += fsResult.Fail("override value type doesn't match: " + key);
				}
			}
			foreach (string key2 in asDictionary2.Keys)
			{
				if (!asDictionary3.ContainsKey(key2))
				{
					asDictionary3.Add(key2, asDictionary2[key2]);
				}
			}
			return success;
		}
	}
}
