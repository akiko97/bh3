using FullInspector;

namespace MoleMole.Config
{
	public class ConfigSubWeatherCollection
	{
		public ConfigRain configRain;

		[InspectorNullable]
		public ConfigStageEffectSetting stageEffectSetting;

		public static ConfigSubWeatherCollection LoadFromFile(ConfigWeather config)
		{
			ConfigSubWeatherCollection configSubWeatherCollection = new ConfigSubWeatherCollection();
			if (!string.IsNullOrEmpty(config.rainPath))
			{
				configSubWeatherCollection.configRain = ConfigUtil.LoadConfig<ConfigRain>(config.rainPath);
			}
			return configSubWeatherCollection;
		}

		public static ConfigSubWeatherCollection CreateDefault()
		{
			ConfigSubWeatherCollection configSubWeatherCollection = new ConfigSubWeatherCollection();
			configSubWeatherCollection.configRain = ConfigRain.CreatDefault();
			return configSubWeatherCollection;
		}

		public ConfigSubWeatherCollection Copy()
		{
			ConfigSubWeatherCollection configSubWeatherCollection = new ConfigSubWeatherCollection();
			configSubWeatherCollection.configRain = ((!(configRain != null)) ? null : configRain.Clone());
			return configSubWeatherCollection;
		}

		public static ConfigSubWeatherCollection Lerp(ConfigSubWeatherCollection config1, ConfigSubWeatherCollection config2, float t)
		{
			ConfigSubWeatherCollection configSubWeatherCollection = new ConfigSubWeatherCollection();
			configSubWeatherCollection.configRain = ConfigRain.Lerp(config1.configRain, config2.configRain, t);
			return configSubWeatherCollection;
		}

		public static void LerpPreparation(ConfigSubWeatherCollection config1, ConfigSubWeatherCollection config2)
		{
			if (config1.configRain == null && config2.configRain != null)
			{
				config1.configRain = config2.configRain.GetNullLerpAble();
			}
			if (config1.configRain != null && config2.configRain == null)
			{
				config2.configRain = config1.configRain.GetNullLerpAble();
			}
		}
	}
}
