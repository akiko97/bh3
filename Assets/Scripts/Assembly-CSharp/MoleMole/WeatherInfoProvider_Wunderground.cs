using SimpleJSON;

namespace MoleMole
{
	public class WeatherInfoProvider_Wunderground : WeatherInfoProvider
	{
		protected override string GetUrl()
		{
			return "http://api.wunderground.com/api/a48bebc6f61128cf/conditions/q/autoip.json";
		}

		protected override WeatherInfo ParseWeatherInfo(string content)
		{
			WeatherInfo weatherInfo = new WeatherInfo();
			JSONNode jSONNode = JSONNode.Parse(content);
			if (jSONNode != null)
			{
				weatherInfo.weatherType = WeatherType.Sunny;
				weatherInfo.temperature = jSONNode["current_observation"]["temp_c"].AsFloat;
			}
			weatherInfo.extraInfo = content;
			return weatherInfo;
		}
	}
}
