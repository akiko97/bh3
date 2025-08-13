using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class ConfigWeather : BaseScriptableObject
	{
		public string renderingDataPath;

		public string rainPath;

		[InspectorNullable]
		public ConfigStageEffectSetting stageEffectSetting;

		public static ConfigWeather CreateDefault()
		{
			ConfigWeather configWeather = new ConfigWeather();
			configWeather.rainPath = "Weather/Rain/Default";
			return configWeather;
		}

		public void CopyFrom(ConfigWeather source)
		{
			renderingDataPath = source.renderingDataPath;
			rainPath = source.rainPath;
		}
	}
}
