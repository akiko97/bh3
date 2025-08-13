using System;
using UnityEngine;

namespace MoleMole
{
	public class RealTimeWeatherManager
	{
		private const string REALTIME_WEATHER_CONFIG_PATH = "RealTimeWeather/ConfigRealTimeWeather";

		private WeatherInfoProvider[] _weatherInfoProviders;

		private DateTime _lastQueryTime;

		private ConfigRealTimeWeather _config;

		private int _failingCounter;

		private bool _available = true;

		public bool Available
		{
			get
			{
				return _available && Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather;
			}
		}

		public RealTimeWeatherManager()
		{
			_config = Resources.Load<ConfigRealTimeWeather>("RealTimeWeather/ConfigRealTimeWeather");
			_weatherInfoProviders = WeatherInfoProvider.CreateWeatherInfoProviders(_config.siteUsage);
			if (_weatherInfoProviders.Length == 0)
			{
				_available = false;
				return;
			}
			int i = 0;
			for (int num = _weatherInfoProviders.Length; i < num; i++)
			{
				_weatherInfoProviders[i].Init();
			}
		}

		public WeatherInfo GetWeatherInfo()
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.LastWeatherInfo;
		}

		public DateTime GetLastQueryTime()
		{
			return _lastQueryTime;
		}

		public void QueryWeatherInfo(Action<WeatherInfo> callback = null)
		{
			if (_available)
			{
				_lastQueryTime = DateTime.Now;
				QueryWeatherInfoByIndexOfProvider(0, callback);
			}
		}

		public void GetWeatherConfig(WeatherType type, out string configName, out int sceneId)
		{
			configName = _config.defaultConfigName;
			sceneId = _config.defaultSceneId;
			int i = 0;
			for (int num = _config.weatherMatch.Length; i < num; i++)
			{
				if (_config.weatherMatch[i].realTimeWeatherType != type)
				{
					continue;
				}
				float value = UnityEngine.Random.value;
				int j = 0;
				for (int num2 = _config.weatherMatch[i].weatherPatterns.Length; j < num2; j++)
				{
					float num3 = value * (float)num2;
					float num4 = j + 1;
					if (num3 < num4)
					{
						configName = _config.weatherMatch[i].weatherPatterns[j].configName;
						sceneId = _config.weatherMatch[i].weatherPatterns[j].sceneId;
						break;
					}
				}
				break;
			}
		}

		public bool IsWeatherInfoExpired()
		{
			WeatherInfo weatherInfo = GetWeatherInfo();
			if (weatherInfo.weatherType == WeatherType.None)
			{
				return true;
			}
			return DateTime.Now.Subtract(weatherInfo.infoTime).TotalSeconds >= (double)_config.weatherInfoPeriod;
		}

		public bool IsReadyToRetryQuery()
		{
			WeatherInfo weatherInfo = GetWeatherInfo();
			if (weatherInfo.weatherType != WeatherType.None && !IsWeatherInfoExpired())
			{
				return false;
			}
			return DateTime.Now.Subtract(_lastQueryTime).TotalSeconds >= (double)_config.retryInterval;
		}

		private void QueryWeatherInfoByIndexOfProvider(int index, Action<WeatherInfo> callback = null)
		{
			if (index < 0 || index >= _weatherInfoProviders.Length)
			{
				return;
			}
			WeatherInfoProvider weatherInfoProvider = _weatherInfoProviders[index];
			if (weatherInfoProvider == null)
			{
				return;
			}
			weatherInfoProvider.QueryWeatherInfo(delegate(WeatherInfo info)
			{
				if (info == null)
				{
					if (index + 1 < _weatherInfoProviders.Length)
					{
						QueryWeatherInfoByIndexOfProvider(index + 1, callback);
					}
					else
					{
						IncreateFail();
					}
				}
				else
				{
					info.infoTime = DateTime.Now;
					Singleton<MiHoYoGameData>.Instance.LocalData.LastWeatherInfo = info;
					Singleton<MiHoYoGameData>.Instance.Save();
					if (callback != null)
					{
						callback(info);
					}
				}
			}, delegate
			{
				if (index + 1 < _weatherInfoProviders.Length)
				{
					QueryWeatherInfoByIndexOfProvider(index + 1, callback);
				}
				else
				{
					IncreateFail();
				}
			});
		}

		private void IncreateFail()
		{
			if (_failingCounter == _config.maxRetryTime)
			{
				_available = false;
			}
			else
			{
				_failingCounter++;
			}
		}
	}
}
