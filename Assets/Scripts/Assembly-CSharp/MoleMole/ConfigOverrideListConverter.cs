using System;
using System.Collections.Generic;
using FullSerializer;
using UniRx;

namespace MoleMole
{
	public class ConfigOverrideListConverter : fsBaseConfigOverrideConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new ConfigOverrideList();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(ConfigOverrideList);
		}

		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult success = fsResult.Success;
			fsResult _fsResult = (success += CheckType(data, fsDataType.Array));
			if (_fsResult.Failed)
			{
				return success;
			}
			List<fsData> asList = data.AsList;
			ConfigOverrideList configOverrideList = (ConfigOverrideList)instance;
			configOverrideList.objects = new object[asList.Count];
			Tuple<string, bool, fsData>[] array = new Tuple<string, bool, fsData>[asList.Count];
			for (int i = 0; i < asList.Count; i++)
			{
				if ((success += CheckType(asList[i], fsDataType.Object)).Failed)
				{
					return success;
				}
				Dictionary<string, fsData> asDictionary = asList[i].AsDictionary;
				if (asDictionary.ContainsKey("$refbase"))
				{
					fsData fsData = asDictionary["$refbase"];
					fsResult fsResult2 = (success += CheckType(fsData, fsDataType.String));
					if (fsResult2.Failed)
					{
						return success;
					}
					string asString = fsData.AsString;
					bool item = true;
					array[i] = Tuple.Create(asString, item, asList[i]);
				}
				else if (asDictionary.ContainsKey("$refoverride"))
				{
					fsData fsData2 = asDictionary["$refoverride"];
					fsResult fsResult3 = (success += CheckType(fsData2, fsDataType.String));
					if (fsResult3.Failed)
					{
						return success;
					}
					string asString = fsData2.AsString;
					bool item = false;
					array[i] = Tuple.Create(asString, item, asList[i]);
				}
				else
				{
					array[i] = Tuple.Create<string, bool, fsData>(null, false, asList[i]);
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				Tuple<string, bool, fsData> tuple = array[j];
				if (tuple.Item2 || tuple.Item1 == null)
				{
					success += Serializer.TryDeserialize(tuple.Item3, typeof(object), ref configOverrideList.objects[j]);
				}
				else
				{
					fsData fsData3 = null;
					foreach (Tuple<string, bool, fsData> tuple2 in array)
					{
						if (tuple2.Item1 == tuple.Item1)
						{
							if (!tuple2.Item2)
							{
								return success + fsResult.Fail("$refoverride needs to point to a $refbase entry " + tuple.Item1);
							}
							fsData3 = tuple2.Item3;
							break;
						}
					}
					if (fsData3 == null)
					{
						return success + fsResult.Fail("missing base entry " + tuple.Item1);
					}
					fsData outData;
					success += Override(fsData3, tuple.Item3, out outData);
					success += Serializer.TryDeserialize(outData, typeof(object), ref configOverrideList.objects[j]);
				}
				object obj = configOverrideList.objects[j];
				if (obj is IOnLoaded)
				{
					((IOnLoaded)obj).OnLoaded();
				}
			}
			return fsResult.Success;
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			fsResult success = fsResult.Success;
			serialized = fsData.CreateList();
			ConfigOverrideList configOverrideList = (ConfigOverrideList)instance;
			object[] objects = configOverrideList.objects;
			foreach (object obj in objects)
			{
				fsData data;
				success += Serializer.TrySerialize(obj.GetType(), obj, out data);
				if (success.Failed)
				{
					return success;
				}
				serialized.AsList.Add(data);
			}
			return success;
		}
	}
}
