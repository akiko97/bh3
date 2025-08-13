using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigStageRenderingData : ConfigBaseRenderingData
	{
		private static BaseRenderingProperty[] DEFAULT_RENDERING_PROPERTIES = new BaseRenderingProperty[22]
		{
			new ColorRenderingProperty("_FogColorNear", new Color(0.66f, 0.65f, 0.99f, 1f)),
			new ColorRenderingProperty("_FogColorFar", new Color(0.66f, 0.65f, 0.99f, 1f)),
			new FloatRenderingProperty("_FogIntensity", 0.01f, 0f, 0.4f),
			new FloatRenderingProperty("_FogColorIntensity", 0.01f, 0f, 0.2f),
			new FloatRenderingProperty("_FogEffectStart", 0f, 0f, 1f),
			new FloatRenderingProperty("_FogEffectLimit", 0f, 0f, 1f),
			new FloatRenderingProperty("_SkyBoxFogEffectLimit", 0f, 0f, 1f),
			new FloatRenderingProperty("_SkyBoxBloomFactor", 1f, 0f, 100f),
			new ColorRenderingProperty("_SkyBoxColor", new Color(1f, 1f, 1f, 1f)),
			new FloatRenderingProperty("_SkyBoxColorScaler", 1f, 0f, 10f),
			new FloatRenderingProperty("_FogStartDistance", 30f, 0f, 10f),
			new ColorRenderingProperty("_SkyBoxTexRColor", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)),
			new ColorRenderingProperty("_SkyBoxTexGColor", new Color32(168, 204, 234, byte.MaxValue)),
			new ColorRenderingProperty("_SkyBoxTexBColor", new Color32(57, 244, 213, byte.MaxValue)),
			new FloatRenderingProperty("_SkyBoxTexXLocation", 0.177f, 0f, 1f),
			new FloatRenderingProperty("_SkyBoxTexYLocation", 0.092f, 0f, 1f),
			new FloatRenderingProperty("_SkyBoxTexHigh", 1f, 0.01f, 1f),
			new ColorRenderingProperty("_SkyBoxGradBottomColor", new Color32(88, 202, 225, byte.MaxValue)),
			new ColorRenderingProperty("_SkyBoxGradTopColor", new Color32(25, 67, 165, byte.MaxValue)),
			new FloatRenderingProperty("_SkyBoxGradLocation", 0.36f, -5f, 1f),
			new FloatRenderingProperty("_SkyBoxGradHigh", 0.1f, 0.01f, 10f),
			new ColorRenderingProperty("_SkyBoxMountainColor", new Color(1f, 1f, 1f, 1f))
		};

		public static BaseRenderingProperty[] GetDefaultProperties()
		{
			return DEFAULT_RENDERING_PROPERTIES;
		}

		public static ConfigStageRenderingData CreateDefault()
		{
			ConfigStageRenderingData configStageRenderingData = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
			configStageRenderingData.properties = new BaseRenderingProperty[DEFAULT_RENDERING_PROPERTIES.Length];
			for (int i = 0; i < DEFAULT_RENDERING_PROPERTIES.Length; i++)
			{
				configStageRenderingData.properties[i] = DEFAULT_RENDERING_PROPERTIES[i].Clone();
			}
			return configStageRenderingData;
		}

		public static void CompleteStageRenderingDataWithDefault(ConfigStageRenderingData data)
		{
			List<BaseRenderingProperty> list = new List<BaseRenderingProperty>(data.properties);
			BaseRenderingProperty[] dEFAULT_RENDERING_PROPERTIES = DEFAULT_RENDERING_PROPERTIES;
			foreach (BaseRenderingProperty baseRenderingProperty in dEFAULT_RENDERING_PROPERTIES)
			{
				bool flag = false;
				BaseRenderingProperty[] array = data.properties;
				foreach (BaseRenderingProperty baseRenderingProperty2 in array)
				{
					if (baseRenderingProperty.propertyName == baseRenderingProperty2.propertyName)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(baseRenderingProperty);
				}
			}
			data.properties = list.ToArray();
		}

		public static ConfigStageRenderingData CreateStageRenderingDataFromMaterials(Material[] mats)
		{
			ConfigStageRenderingData configStageRenderingData = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
			HashSet<string> hashSet = new HashSet<string>();
			List<BaseRenderingProperty> list = new List<BaseRenderingProperty>();
			foreach (Material material in mats)
			{
				BaseRenderingProperty[] dEFAULT_RENDERING_PROPERTIES = DEFAULT_RENDERING_PROPERTIES;
				foreach (BaseRenderingProperty baseRenderingProperty in dEFAULT_RENDERING_PROPERTIES)
				{
					if (material.HasProperty(baseRenderingProperty.propertyName) && !hashSet.Contains(baseRenderingProperty.propertyName))
					{
						if (baseRenderingProperty is ColorRenderingProperty)
						{
							list.Add(new ColorRenderingProperty(baseRenderingProperty.propertyName, material.GetColor(baseRenderingProperty.propertyName)));
						}
						else if (baseRenderingProperty is FloatRenderingProperty)
						{
							list.Add(new FloatRenderingProperty(baseRenderingProperty.propertyName, material.GetFloat(baseRenderingProperty.propertyName), 0f, 100f));
						}
						hashSet.Add(baseRenderingProperty.propertyName);
					}
				}
			}
			configStageRenderingData.properties = list.ToArray();
			return configStageRenderingData;
		}

		public static ConfigStageRenderingData CreateStageRenderingDataFromInstancedProperties(BaseInstancedRenderingProperty[] properties)
		{
			ConfigStageRenderingData configStageRenderingData = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
			HashSet<string> hashSet = new HashSet<string>();
			List<BaseRenderingProperty> list = new List<BaseRenderingProperty>();
			foreach (BaseInstancedRenderingProperty baseInstancedRenderingProperty in properties)
			{
				BaseRenderingProperty[] dEFAULT_RENDERING_PROPERTIES = DEFAULT_RENDERING_PROPERTIES;
				foreach (BaseRenderingProperty baseRenderingProperty in dEFAULT_RENDERING_PROPERTIES)
				{
					if (Shader.PropertyToID(baseRenderingProperty.propertyName) == baseInstancedRenderingProperty.propertyID && !hashSet.Contains(baseRenderingProperty.propertyName))
					{
						list.Add(baseInstancedRenderingProperty.CreateBaseRenderingProperty(baseRenderingProperty.propertyName));
						hashSet.Add(baseRenderingProperty.propertyName);
					}
				}
			}
			configStageRenderingData.properties = list.ToArray();
			return configStageRenderingData;
		}
	}
}
