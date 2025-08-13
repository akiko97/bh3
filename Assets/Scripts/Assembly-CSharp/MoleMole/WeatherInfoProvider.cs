using System;
using System.Collections.Generic;

namespace MoleMole
{
	public abstract class WeatherInfoProvider
	{
		private static Dictionary<string, string> _staticFakeHeaders = new Dictionary<string, string> { { "User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36" } };

		public virtual void Init()
		{
		}

		public virtual void QueryWeatherInfo(Action<WeatherInfo> callbackSucc, Action callbackFail)
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithTimeOut(GetUrl(), delegate(string responseText)
			{
				callbackSucc(ParseWeatherInfo(responseText));
			}, callbackFail, 5f, null, _staticFakeHeaders));
		}

		protected abstract string GetUrl();

		protected abstract WeatherInfo ParseWeatherInfo(string content);

		public static WeatherInfoProvider[] CreateWeatherInfoProviders(ConfigWeatherSiteUsage usage)
		{
			List<WeatherInfoProvider> list = new List<WeatherInfoProvider>();
			if (usage.sinaWeather)
			{
				list.Add(new WeatherInfoProvider_SinaWeather());
			}
			if (usage.yahooWeather)
			{
				list.Add(new WeatherInfoProvider_YahooWeather());
			}
			if (usage.sinaNews)
			{
				list.Add(new WeatherInfoProvider_SinaNews());
			}
			return list.ToArray();
		}
	}
}
