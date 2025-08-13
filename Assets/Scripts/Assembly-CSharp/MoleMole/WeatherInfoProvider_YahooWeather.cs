using System.Collections.Generic;

namespace MoleMole
{
	public class WeatherInfoProvider_YahooWeather : WeatherInfoProvider
	{
		private struct NameWeatherPair
		{
			public string pattern;

			public WeatherType weather;
		}

		private const string weatherStartPattern = "WeatherLocationAndTemperature.0.1.1.0.1";

		private const string temperatureStartPattern = "WeatherLocationAndTemperature.0.1.1.2.0";

		private List<NameWeatherPair> _nameWeatherList = new List<NameWeatherPair>
		{
			new NameWeatherPair
			{
				pattern = "Sunny",
				weather = WeatherType.Sunny
			},
			new NameWeatherPair
			{
				pattern = "Fair",
				weather = WeatherType.Sunny
			},
			new NameWeatherPair
			{
				pattern = "Clear",
				weather = WeatherType.Sunny
			},
			new NameWeatherPair
			{
				pattern = "Mostly Cloudy",
				weather = WeatherType.HeavyCloudy
			},
			new NameWeatherPair
			{
				pattern = "Cloudy",
				weather = WeatherType.Cloudy
			},
			new NameWeatherPair
			{
				pattern = "Shower",
				weather = WeatherType.Rainy
			},
			new NameWeatherPair
			{
				pattern = "Rain",
				weather = WeatherType.Rainy
			},
			new NameWeatherPair
			{
				pattern = "Snow",
				weather = WeatherType.Snowy
			},
			new NameWeatherPair
			{
				pattern = "Thunder",
				weather = WeatherType.Lightning
			}
		};

		protected override string GetUrl()
		{
			return "https://www.yahoo.com/news/weather/";
		}

		protected override WeatherInfo ParseWeatherInfo(string content)
		{
			string text = FetchInnerTextByPattern(content, "WeatherLocationAndTemperature.0.1.1.0.1");
			if (text == null)
			{
				return null;
			}
			string text2 = FetchInnerTextByPattern(content, "WeatherLocationAndTemperature.0.1.1.2.0");
			if (text2 == null)
			{
				return null;
			}
			WeatherInfo weatherInfo = new WeatherInfo();
			weatherInfo.weatherType = GetWeatherTypeFromPattern(text);
			if (!float.TryParse(text2, out weatherInfo.temperature))
			{
				weatherInfo.temperature = -100f;
			}
			weatherInfo.temperature = (weatherInfo.temperature - 32f) / 1.8f;
			return weatherInfo;
		}

		private string FetchInnerTextByPattern(string content, string pattern)
		{
			int num = content.IndexOf(pattern);
			if (num == -1)
			{
				return null;
			}
			int num2 = content.IndexOf(">", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = content.IndexOf("<", num2);
			if (num3 == -1)
			{
				return null;
			}
			int length = num3 - num2 - 1;
			return content.Substring(num2 + 1, length);
		}

		private WeatherType GetWeatherTypeFromPattern(string content)
		{
			int i = 0;
			for (int count = _nameWeatherList.Count; i < count; i++)
			{
				if (content.IndexOf(_nameWeatherList[i].pattern) != -1)
				{
					return _nameWeatherList[i].weather;
				}
			}
			return WeatherType.Sunny;
		}
	}
}
