using MoleMole.Config;

namespace MoleMole
{
	public class ConfigWeatherData
	{
		public ConfigBaseRenderingData configRenderingData;

		public ConfigSubWeatherCollection configSubWeathers;

		public static ConfigWeatherData CrateDefault()
		{
			ConfigWeatherData configWeatherData = new ConfigWeatherData();
			configWeatherData.configRenderingData = ConfigStageRenderingData.CreateDefault();
			configWeatherData.configSubWeathers = ConfigSubWeatherCollection.CreateDefault();
			return configWeatherData;
		}

		public static ConfigWeatherData LoadFromFile(ConfigWeather config)
		{
			if (config == null)
			{
				return null;
			}
			ConfigWeatherData configWeatherData = new ConfigWeatherData();
			if (!string.IsNullOrEmpty(config.renderingDataPath))
			{
				configWeatherData.configRenderingData = ConfigUtil.LoadConfig<ConfigBaseRenderingData>(config.renderingDataPath);
			}
			configWeatherData.configSubWeathers = ConfigSubWeatherCollection.LoadFromFile(config);
			configWeatherData.configSubWeathers.stageEffectSetting = config.stageEffectSetting;
			return configWeatherData;
		}
	}
}
