using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigBaseRenderingData : BaseScriptableObject
	{
		public BaseRenderingProperty[] properties;

		public virtual void SetupTransition(ConfigBaseRenderingData target)
		{
			Dictionary<string, BaseRenderingProperty> dictionary = new Dictionary<string, BaseRenderingProperty>();
			for (int i = 0; i < target.properties.Length; i++)
			{
				dictionary.Add(target.properties[i].propertyName, target.properties[i]);
			}
			for (int j = 0; j < properties.Length; j++)
			{
				if (dictionary.ContainsKey(properties[j].propertyName))
				{
					properties[j].SetupTransition(dictionary[properties[j].propertyName]);
				}
				else
				{
					properties[j].SetupTransition(null);
				}
			}
		}

		public virtual void LerpStep(float t)
		{
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i].LerpStep(t);
			}
		}

		public virtual void ApplyGlobally()
		{
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i].ApplyGlobally();
			}
		}

		public virtual ConfigBaseRenderingData Clone()
		{
			return Object.Instantiate(this as ConfigStageRenderingData);
		}

		public void CopyFrom(ConfigBaseRenderingData source)
		{
			properties = new BaseRenderingProperty[source.properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i] = source.properties[i].Clone();
			}
		}
	}
}
