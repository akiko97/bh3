using UnityEngine;

namespace MoleMole
{
	public class MonoWeatherTest : MonoBehaviour
	{
		private void OnGUI()
		{
			if (GUILayout.Button("Query Weather"))
			{
			}
			WeatherInfo weatherInfo = Singleton<RealTimeWeatherManager>.Instance.GetWeatherInfo();
			if (weatherInfo != null)
			{
				GUILayout.Label("WeatherType:" + weatherInfo.weatherType);
				GUILayout.Label("Temperature:" + weatherInfo.temperature);
				GUILayout.Label(("ExtraInfo:" + weatherInfo.extraInfo == null) ? string.Empty : weatherInfo.extraInfo);
			}
			else
			{
				GUILayout.Label("<No Weather Info>");
			}
		}
	}
}
