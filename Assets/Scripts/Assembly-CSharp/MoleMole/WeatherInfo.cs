using System;

namespace MoleMole
{
	[Serializable]
	public class WeatherInfo
	{
		public float temperature;

		public WeatherType weatherType;

		public string extraInfo;

		public DateTime infoTime;

		public override string ToString()
		{
			return string.Format("[{0}] {1} ({2}) : {3}", weatherType, temperature.ToString(), infoTime.ToString(), (extraInfo != null) ? extraInfo : "<NULL>");
		}
	}
}
