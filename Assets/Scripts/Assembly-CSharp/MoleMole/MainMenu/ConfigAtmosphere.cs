using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class ConfigAtmosphere
	{
		public string Name;

		public float FrameTime;

		public ConfigCloudStyle CloudStyle;

		public ConfigBackground Background;

		public ConfigIndoor Indoor;

		public ConfigAtmosphere()
		{
			Background = new ConfigBackground();
		}

		public void InitAfterLoad()
		{
			Name = FrameTime.ToString();
		}

		public int CompareTo(ConfigAtmosphere target)
		{
			return FrameTime.CompareTo(target.FrameTime);
		}

		public static ConfigAtmosphere FromScriptable(ScriptableConfigAtmosphere target)
		{
			if (target == null)
			{
				return null;
			}
			ConfigAtmosphere configAtmosphere = new ConfigAtmosphere();
			configAtmosphere.Name = target.name;
			configAtmosphere.FrameTime = target.FrameTime;
			configAtmosphere.CloudStyle = target.CloudStyle;
			configAtmosphere.Background = target.Background;
			configAtmosphere.Indoor = target.Indoor;
			return configAtmosphere;
		}

		public static ScriptableConfigAtmosphere ToScriptable(ConfigAtmosphere source)
		{
			if (source == null)
			{
				return null;
			}
			ScriptableConfigAtmosphere scriptableConfigAtmosphere = ScriptableObject.CreateInstance<ScriptableConfigAtmosphere>();
			scriptableConfigAtmosphere.name = source.Name;
			scriptableConfigAtmosphere.FrameTime = source.FrameTime;
			scriptableConfigAtmosphere.CloudStyle = source.CloudStyle;
			scriptableConfigAtmosphere.Background = source.Background;
			scriptableConfigAtmosphere.Indoor = source.Indoor;
			return scriptableConfigAtmosphere;
		}

		public static ConfigAtmosphere Lerp(ConfigAtmosphere config1, ConfigAtmosphere config2, float t)
		{
			ConfigAtmosphere configAtmosphere = new ConfigAtmosphere();
			configAtmosphere.FrameTime = Mathf.Lerp(config1.FrameTime, config2.FrameTime, t);
			configAtmosphere.Name = configAtmosphere.FrameTime.ToString();
			configAtmosphere.CloudStyle = ConfigCloudStyle.Lerp(config1.CloudStyle, config2.CloudStyle, t);
			configAtmosphere.Background = ConfigBackground.Lerp(config1.Background, config2.Background, t);
			configAtmosphere.Indoor = ConfigIndoor.Lerp(config1.Indoor, config2.Indoor, t);
			return configAtmosphere;
		}
	}
}
