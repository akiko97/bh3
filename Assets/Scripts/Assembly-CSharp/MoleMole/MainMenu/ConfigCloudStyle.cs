using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class ConfigCloudStyle
	{
		public Color BrightColor;

		public Color DarkColor;

		public Color SecondDarkColor;

		public Color RimColor;

		public Color FlashColor;

		public Vector3 FlashAttenuationFactors;

		public static ConfigCloudStyle Lerp(ConfigCloudStyle config1, ConfigCloudStyle config2, float t)
		{
			ConfigCloudStyle configCloudStyle = new ConfigCloudStyle();
			configCloudStyle.BrightColor = Color.Lerp(config1.BrightColor, config2.BrightColor, t);
			configCloudStyle.DarkColor = Color.Lerp(config1.DarkColor, config2.DarkColor, t);
			configCloudStyle.SecondDarkColor = Color.Lerp(config1.SecondDarkColor, config2.SecondDarkColor, t);
			configCloudStyle.RimColor = Color.Lerp(config1.RimColor, config2.RimColor, t);
			configCloudStyle.FlashColor = Color.Lerp(config1.FlashColor, config2.FlashColor, t);
			configCloudStyle.FlashAttenuationFactors = Vector3.Lerp(config1.FlashAttenuationFactors, config2.FlashAttenuationFactors, t);
			return configCloudStyle;
		}

		public void CopyFrom(ConfigCloudStyle other)
		{
			if (other != null)
			{
				BrightColor = other.BrightColor;
				DarkColor = other.DarkColor;
				SecondDarkColor = other.SecondDarkColor;
				RimColor = other.RimColor;
				FlashColor = other.FlashColor;
				FlashAttenuationFactors = other.FlashAttenuationFactors;
			}
		}
	}
}
