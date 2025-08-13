using System;

namespace MoleMole
{
	[Serializable]
	public class ConfigWeatherMatchItem
	{
		public WeatherType realTimeWeatherType;

		public MainMenuWeather[] weatherPatterns;
	}
}
