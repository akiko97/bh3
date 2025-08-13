using System;
using UnityEngine;

namespace MoleMole.Config
{
	[Serializable]
	public class ConfigRain : ScriptableObject
	{
		public float density;

		public float splashDensity;

		public float speed;

		public float speedStrech;

		public float size;

		public float opaqueness;

		public float splashOpaqueness;

		public string audioName;

		public float audioVolumn;

		public float audioPitch;

		public static ConfigRain CreatDefault()
		{
			ConfigRain configRain = new ConfigRain();
			configRain.density = 0f;
			configRain.splashDensity = 0.796f;
			configRain.speed = 1f;
			configRain.speedStrech = 0.02f;
			configRain.size = 0.007f;
			configRain.opaqueness = 1f;
			configRain.splashOpaqueness = 1f;
			configRain.audioVolumn = 1f;
			configRain.audioPitch = 1f;
			return configRain;
		}

		public static ConfigRain Lerp(ConfigRain config1, ConfigRain config2, float t)
		{
			if (config1 == null && config2 == null)
			{
				return null;
			}
			ConfigRain configRain = ScriptableObject.CreateInstance<ConfigRain>();
			configRain.density = Mathf.Lerp(config1.density, config2.density, t);
			configRain.splashDensity = Mathf.Lerp(config1.splashDensity, config2.splashDensity, t);
			configRain.speed = Mathf.Lerp(config1.speed, config2.speed, t);
			configRain.speedStrech = Mathf.Lerp(config1.speedStrech, config2.speedStrech, t);
			configRain.size = Mathf.Lerp(config1.size, config2.size, t);
			configRain.opaqueness = Mathf.Lerp(config1.opaqueness, config2.opaqueness, t);
			configRain.splashOpaqueness = Mathf.Lerp(config1.splashOpaqueness, config2.splashOpaqueness, t);
			configRain.audioVolumn = Mathf.Lerp(config1.audioVolumn, config2.audioVolumn, t);
			configRain.audioPitch = Mathf.Lerp(config1.audioPitch, config2.audioPitch, t);
			configRain.audioName = config1.audioName;
			return configRain;
		}

		public ConfigRain Clone()
		{
			return UnityEngine.Object.Instantiate(this);
		}

		public void CopyFrom(ConfigRain source)
		{
			density = source.density;
			splashDensity = source.splashDensity;
			speed = source.speed;
			speedStrech = source.speedStrech;
			size = source.size;
			opaqueness = source.opaqueness;
			splashOpaqueness = source.splashOpaqueness;
			audioName = source.audioName;
			audioVolumn = source.audioVolumn;
			audioPitch = source.audioPitch;
		}

		public ConfigRain GetNullLerpAble()
		{
			ConfigRain configRain = Clone();
			configRain.density = 0f;
			configRain.splashDensity = 0f;
			configRain.audioName = audioName;
			configRain.audioVolumn = audioVolumn * 0.7f;
			configRain.audioPitch = audioPitch * 0.3f;
			return configRain;
		}
	}
}
