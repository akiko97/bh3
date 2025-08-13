using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class ConfigBackground
	{
		public Color RColor;

		public Color GColor;

		public Color BColor;

		public Color GradTopColor;

		public Color GradBottomColor;

		[Range(0f, 1f)]
		public float XLocation;

		[Range(0f, 1f)]
		public float YLocation;

		[Range(0.01f, 1f)]
		public float High;

		[Range(0f, 1f)]
		public float SecTexXLocation;

		[Range(0f, 1f)]
		public float SecTexYLocation;

		[Range(0.01f, 1f)]
		public float SecTexHigh;

		[Range(0f, 10f)]
		public float SecTexEmission;

		[Range(-5f, 1f)]
		public float GradLocation;

		[Range(0.01f, 10f)]
		public float GradHigh;

		public float BloomFactor;

		public static ConfigBackground Lerp(ConfigBackground config1, ConfigBackground config2, float t)
		{
			ConfigBackground configBackground = new ConfigBackground();
			configBackground.RColor = Color.Lerp(config1.RColor, config2.RColor, t);
			configBackground.GColor = Color.Lerp(config1.GColor, config2.GColor, t);
			configBackground.BColor = Color.Lerp(config1.BColor, config2.BColor, t);
			configBackground.GradTopColor = Color.Lerp(config1.GradTopColor, config2.GradTopColor, t);
			configBackground.GradBottomColor = Color.Lerp(config1.GradBottomColor, config2.GradBottomColor, t);
			configBackground.XLocation = Mathf.Lerp(config1.XLocation, config2.XLocation, t);
			configBackground.YLocation = Mathf.Lerp(config1.YLocation, config2.YLocation, t);
			configBackground.High = Mathf.Lerp(config1.High, config2.High, t);
			configBackground.SecTexXLocation = Mathf.Lerp(config1.SecTexXLocation, config2.SecTexXLocation, t);
			configBackground.SecTexYLocation = Mathf.Lerp(config1.SecTexYLocation, config2.SecTexYLocation, t);
			configBackground.SecTexHigh = Mathf.Lerp(config1.SecTexHigh, config2.SecTexHigh, t);
			configBackground.SecTexEmission = Mathf.Lerp(config1.SecTexEmission, config2.SecTexEmission, t);
			configBackground.GradLocation = Mathf.Lerp(config1.GradLocation, config2.GradLocation, t);
			configBackground.GradHigh = Mathf.Lerp(config1.GradHigh, config2.GradHigh, t);
			configBackground.BloomFactor = Mathf.Lerp(config1.BloomFactor, config2.BloomFactor, t);
			return configBackground;
		}

		public void CopyFrom(ConfigBackground other)
		{
			if (other != null)
			{
				RColor = other.RColor;
				GColor = other.GColor;
				BColor = other.BColor;
				GradTopColor = other.GradTopColor;
				GradBottomColor = other.GradBottomColor;
				XLocation = other.XLocation;
				YLocation = other.YLocation;
				High = other.High;
				SecTexXLocation = other.SecTexXLocation;
				SecTexYLocation = other.SecTexYLocation;
				SecTexHigh = other.SecTexHigh;
				SecTexEmission = other.SecTexEmission;
				GradLocation = other.GradLocation;
				GradHigh = other.GradHigh;
				BloomFactor = other.BloomFactor;
			}
		}
	}
}
