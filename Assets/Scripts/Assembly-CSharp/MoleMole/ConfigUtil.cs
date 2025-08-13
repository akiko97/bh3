using System;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public static class ConfigUtil
	{
		private static object serializationHelperLock = new object();

		public static T LoadConfig<T>(string filePath) where T : ScriptableObject
		{
			return Miscs.LoadResource<T>(filePath);
		}

		public static AsyncAssetRequst LoadConfigAsync(string filePath, BundleType type = BundleType.RESOURCE_FILE)
		{
			return Miscs.LoadResourceAsync(filePath, type);
		}

		public static AsyncAssetRequst LoadJsonConfigAsync(string filePath, BundleType type = BundleType.DATA_FILE)
		{
			return Miscs.LoadResourceAsync(filePath, type);
		}

		public static T LoadJSONConfig<T>(string jsonPath) where T : class
		{
			try
			{
				string content = Miscs.LoadTextFileToString(jsonPath);
				lock (serializationHelperLock)
				{
					return SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(content);
				}
			}
			catch
			{
				Debug.LogError("Error during loading json config: " + jsonPath);
				throw;
			}
		}

		public static void LoadJSONStrConfigMultiThread<T>(string jsonText, BackGroundWorker worker, Action<T, string> callback, string param = "") where T : class
		{
			try
			{
				worker.AddBackGroundWork(delegate
				{
					T arg;
					lock (serializationHelperLock)
					{
						arg = SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(jsonText);
					}
					callback(arg, param);
				});
			}
			catch
			{
				Debug.LogError("Error during loading json config: " + jsonText);
				throw;
			}
		}

		public static string SaveJSONStrConfig<T>(T obj) where T : class
		{
			try
			{
				string result;
				lock (serializationHelperLock)
				{
					result = SerializationHelpers.SerializeToContent<T, FullSerializerSerializer>(obj);
				}
				return result;
			}
			catch
			{
				return string.Empty;
			}
		}

		public static T LoadJSONStrConfig<T>(string str) where T : class
		{
			try
			{
				T result;
				lock (serializationHelperLock)
				{
					result = SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(str);
				}
				return result;
			}
			catch
			{
				return (T)null;
			}
		}
	}
}
