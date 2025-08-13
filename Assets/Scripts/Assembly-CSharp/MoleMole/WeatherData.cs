using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class WeatherData
	{
		private static Dictionary<string, ConfigWeatherData> _weatherDataDict;

		public static void ReloadFromFile()
		{
			_weatherDataDict = new Dictionary<string, ConfigWeatherData>();
			string[] weatherEntryPathes = GlobalDataManager.metaConfig.weatherEntryPathes;
			for (int i = 0; i < weatherEntryPathes.Length; i++)
			{
				ConfigWeatherRegistry configWeatherRegistry = ConfigUtil.LoadConfig<ConfigWeatherRegistry>(weatherEntryPathes[i]);
				if (configWeatherRegistry.entries != null)
				{
					for (int j = 0; j < configWeatherRegistry.entries.Length; j++)
					{
						ConfigWeatherEntry configWeatherEntry = configWeatherRegistry.entries[j];
						ConfigWeather config = Miscs.LoadResource<ConfigWeather>(configWeatherEntry.dataPath);
						_weatherDataDict.Add(configWeatherEntry.name, ConfigWeatherData.LoadFromFile(config));
					}
				}
			}
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			_weatherDataDict = new Dictionary<string, ConfigWeatherData>();
			string[] weatherRegistryPathes = GlobalDataManager.metaConfig.weatherEntryPathes;
			float step = progressSpan / (float)weatherRegistryPathes.Length;
			for (int ix = 0; ix < weatherRegistryPathes.Length; ix++)
			{
				AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync(weatherRegistryPathes[ix]);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null weatherRegistryPath :" + weatherRegistryPathes[ix]);
				if (asyncRequest == null)
				{
					continue;
				}
				yield return asyncRequest.operation;
				if (moveOneStepCallback != null)
				{
					moveOneStepCallback(step);
				}
				ConfigWeatherRegistry weatherRegistry = (ConfigWeatherRegistry)asyncRequest.asset;
				SuperDebug.VeryImportantAssert(weatherRegistry != null, "weatherRegistry is null weatherRegistryPath :" + weatherRegistryPathes[ix]);
				if (!(weatherRegistry == null) && weatherRegistry.entries != null)
				{
					for (int jx = 0; jx < weatherRegistry.entries.Length; jx++)
					{
						ConfigWeatherEntry entry = weatherRegistry.entries[jx];
						ConfigWeather configWeather = Miscs.LoadResource<ConfigWeather>(entry.dataPath);
						_weatherDataDict.Add(entry.name, ConfigWeatherData.LoadFromFile(configWeather));
					}
				}
			}
		}

		public static ConfigWeatherData GetWeatherDataConfig(string name)
		{
			return _weatherDataDict[name];
		}
	}
}
