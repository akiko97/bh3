using System.Collections.Generic;

namespace MoleMole
{
	public class WeatherInfoProvider_WcnPage : WeatherInfoProvider
	{
		private Dictionary<string, WeatherType> _weatherTypeDict = new Dictionary<string, WeatherType>
		{
			{
				"00",
				WeatherType.Sunny
			},
			{
				"01",
				WeatherType.Cloudy
			},
			{
				"02",
				WeatherType.Cloudy
			},
			{
				"03",
				WeatherType.Rainy
			},
			{
				"04",
				WeatherType.Lightning
			},
			{
				"05",
				WeatherType.Lightning
			},
			{
				"06",
				WeatherType.Rainy
			},
			{
				"07",
				WeatherType.Rainy
			},
			{
				"08",
				WeatherType.Rainy
			},
			{
				"09",
				WeatherType.Rainy
			},
			{
				"10",
				WeatherType.Rainy
			},
			{
				"11",
				WeatherType.Rainy
			},
			{
				"12",
				WeatherType.Rainy
			},
			{
				"13",
				WeatherType.Sunny
			},
			{
				"14",
				WeatherType.Sunny
			},
			{
				"15",
				WeatherType.Sunny
			},
			{
				"16",
				WeatherType.Sunny
			},
			{
				"17",
				WeatherType.Sunny
			},
			{
				"18",
				WeatherType.Sunny
			},
			{
				"19",
				WeatherType.Rainy
			},
			{
				"20",
				WeatherType.Sunny
			},
			{
				"21",
				WeatherType.Rainy
			},
			{
				"22",
				WeatherType.Rainy
			},
			{
				"23",
				WeatherType.Rainy
			},
			{
				"24",
				WeatherType.Rainy
			},
			{
				"25",
				WeatherType.Rainy
			},
			{
				"26",
				WeatherType.Sunny
			},
			{
				"27",
				WeatherType.Sunny
			},
			{
				"28",
				WeatherType.Sunny
			},
			{
				"29",
				WeatherType.Sunny
			},
			{
				"30",
				WeatherType.Sunny
			},
			{
				"31",
				WeatherType.Sunny
			},
			{
				"53",
				WeatherType.Sunny
			}
		};

		protected override string GetUrl()
		{
			return "http://weather.gtimg.cn/city/01012601.js";
		}

		protected override WeatherInfo ParseWeatherInfo(string content)
		{
			int num = content.IndexOf("sk_wt", 0);
			if (num == -1)
			{
				return null;
			}
			int num2 = content.IndexOf("\"", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = content.IndexOf("\"", num2 + 1);
			if (num3 == -1)
			{
				return null;
			}
			if (num3 - num2 - 1 < 0)
			{
				return null;
			}
			string text = content.Substring(num2 + 1, num3 - num2 - 1);
			if (!_weatherTypeDict.ContainsKey(text))
			{
				return null;
			}
			WeatherInfo weatherInfo = new WeatherInfo();
			weatherInfo.weatherType = _weatherTypeDict[text];
			weatherInfo.temperature = 30f;
			weatherInfo.extraInfo = text;
			return weatherInfo;
		}
	}
}
