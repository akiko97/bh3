using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigGraphicsRecommendSetting
	{
		public int RecommendResolutionX;

		public int RecommendResolutionY;

		public Dictionary<ResolutionQualityGrade, int> ResolutionPercentage = new Dictionary<ResolutionQualityGrade, int>();

		public ResolutionQualityGrade ResolutionQuality;

		public Dictionary<PostEffectQualityGrade, int> PostFxGradeBufferSize = new Dictionary<PostEffectQualityGrade, int>();

		public int TargetFrameRate;

		public GraphicsRecommendGrade RecommendGrade;

		public bool EnableGyroscope;

		public List<string> ExcludeDeviceModels = new List<string>();

		public ConfigGraphicsRequirement[] Requirements;

		public bool MatchRequirements()
		{
			if (Requirements != null)
			{
				ConfigGraphicsRequirement[] requirements = Requirements;
				foreach (ConfigGraphicsRequirement configGraphicsRequirement in requirements)
				{
					switch (configGraphicsRequirement.Info)
					{
					case "GraphicsDeviceName":
					{
						bool flag = false;
						string[] values = configGraphicsRequirement.Values;
						foreach (string wildcard in values)
						{
							if (Miscs.WildcardMatch(wildcard, SystemInfo.graphicsDeviceName, true))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return false;
						}
						break;
					}
					case "ProcessorFrequency":
						if (SystemInfo.processorFrequency < int.Parse(configGraphicsRequirement.Values[0]))
						{
							return false;
						}
						if (configGraphicsRequirement.Values.Length > 1 && SystemInfo.processorFrequency > int.Parse(configGraphicsRequirement.Values[1]))
						{
							return false;
						}
						break;
					case "SystemMemorySize":
						if (SystemInfo.systemMemorySize < int.Parse(configGraphicsRequirement.Values[0]))
						{
							return false;
						}
						if (configGraphicsRequirement.Values.Length > 1 && SystemInfo.systemMemorySize > int.Parse(configGraphicsRequirement.Values[1]))
						{
							return false;
						}
						break;
					case "GraphicsMemorySize":
						if (SystemInfo.graphicsMemorySize < int.Parse(configGraphicsRequirement.Values[0]))
						{
							return false;
						}
						if (configGraphicsRequirement.Values.Length > 1 && SystemInfo.graphicsMemorySize > int.Parse(configGraphicsRequirement.Values[1]))
						{
							return false;
						}
						break;
					}
				}
			}
			return true;
		}
	}
}
