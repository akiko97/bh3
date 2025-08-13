using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class ConfigIndoor
	{
		public Color TintColor;

		public static ConfigIndoor Lerp(ConfigIndoor config1, ConfigIndoor config2, float t)
		{
			ConfigIndoor configIndoor = new ConfigIndoor();
			configIndoor.TintColor = Color.Lerp(config1.TintColor, config2.TintColor, t);
			return configIndoor;
		}

		public void CopyFrom(ConfigIndoor other)
		{
			if (other != null)
			{
				TintColor = other.TintColor;
			}
		}
	}
}
