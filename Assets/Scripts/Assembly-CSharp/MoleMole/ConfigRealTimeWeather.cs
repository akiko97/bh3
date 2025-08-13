using FullInspector;

namespace MoleMole
{
	public class ConfigRealTimeWeather : BaseScriptableObject
	{
		public int maxRetryTime = 5;

		public float retryInterval = 10f;

		public float weatherInfoPeriod = 3600f;

		public ConfigWeatherSiteUsage siteUsage;

		public string defaultConfigName;

		public int defaultSceneId;

		public ConfigWeatherMatchItem[] weatherMatch;
	}
}
