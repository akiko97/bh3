using System.Collections.Generic;
using SimpleJSON;

namespace MoleMole
{
	public class WeatherInfoProvider_SinaWeather : WeatherInfoProvider
	{
		private Dictionary<string, WeatherType> _weatherInfoDict = new Dictionary<string, WeatherType>
		{
			{
				"01_01",
				WeatherType.Sunny
			},
			{
				"02_08",
				WeatherType.Cloudy
			},
			{
				"03_01",
				WeatherType.Rainy
			},
			{
				"03_03",
				WeatherType.Rainy
			},
			{
				"03_06",
				WeatherType.Rainy
			},
			{
				"03_07",
				WeatherType.Rainy
			},
			{
				"03_19",
				WeatherType.Rainy
			},
			{
				"03_22",
				WeatherType.Rainy
			},
			{
				"03_26",
				WeatherType.Rainy
			},
			{
				"03_28",
				WeatherType.Rainy
			},
			{
				"03_30",
				WeatherType.Rainy
			},
			{
				"04_02",
				WeatherType.Snowy
			},
			{
				"04_07",
				WeatherType.Snowy
			},
			{
				"04_21",
				WeatherType.Snowy
			},
			{
				"04_27",
				WeatherType.Snowy
			},
			{
				"04_29",
				WeatherType.Snowy
			},
			{
				"05_13",
				WeatherType.Windy
			},
			{
				"05_20",
				WeatherType.Windy
			},
			{
				"05_23",
				WeatherType.Windy
			},
			{
				"05_31",
				WeatherType.Windy
			},
			{
				"07_25",
				WeatherType.HeavyCloudy
			},
			{
				"08_11",
				WeatherType.Lightning
			},
			{
				"09_10",
				WeatherType.Extreme
			},
			{
				"09_14",
				WeatherType.Extreme
			},
			{
				"09_15",
				WeatherType.Extreme
			},
			{
				"09_24",
				WeatherType.Extreme
			}
		};

		protected override string GetUrl()
		{
			return "http://weather.sina.com.cn/";
		}

		protected override WeatherInfo ParseWeatherInfo(string content)
		{
			string value = "var PAGECONF";
			int num = content.IndexOf(value);
			if (num == -1)
			{
				return null;
			}
			int num2 = content.IndexOf("{", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = content.IndexOf("};", num2);
			if (num3 == -1)
			{
				return null;
			}
			int num4 = num3 - num2 + 1;
			if (num4 <= 0)
			{
				return null;
			}
			string aJSON = content.Substring(num2, num4);
			try
			{
				JSONNode jSONNode = JSONNode.Parse(aJSON);
				string value2 = jSONNode["HOSTCITY"]["weatherCode"].Value;
				value2 = value2.Trim('\'').Substring(0, 5);
				string value3 = jSONNode["HOSTCITY"]["temp"].Value;
				value3 = value3.Trim('\'');
				WeatherInfo weatherInfo = new WeatherInfo();
				weatherInfo.weatherType = ((!_weatherInfoDict.ContainsKey(value2)) ? WeatherType.Sunny : _weatherInfoDict[value2]);
				if (!float.TryParse(value3, out weatherInfo.temperature))
				{
					weatherInfo.temperature = -100f;
				}
				return weatherInfo;
			}
			catch
			{
				return null;
			}
		}
	}
}
