using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MaterialColorModifier
	{
		public class Multiplier
		{
			public Color mulColor = Color.white;
		}

		private int propId;

		private Material material;

		private Color originalColor;

		private Color currentColor;

		private List<Multiplier> multiplierList;

		public MaterialColorModifier()
		{
			propId = -1;
		}

		public MaterialColorModifier(Material material)
		{
			this.material = material;
			propId = -1;
			if (material.HasProperty("_Color"))
			{
				propId = Shader.PropertyToID("_Color");
			}
			else if (material.HasProperty("_MainColor"))
			{
				propId = Shader.PropertyToID("_MainColor");
			}
			if (IsValid())
			{
				originalColor = material.GetColor(propId);
				multiplierList = new List<Multiplier>();
			}
		}

		public void Multiply(Color mulColor)
		{
			if (IsValid())
			{
				currentColor *= mulColor;
			}
		}

		public Multiplier AddMultiplier()
		{
			if (!IsValid())
			{
				return null;
			}
			Multiplier multiplier = new Multiplier();
			multiplierList.Add(multiplier);
			return multiplier;
		}

		public void RemoveMultiplier(Multiplier multiplier)
		{
			if (IsValid())
			{
				multiplierList.Remove(multiplier);
			}
		}

		public void ApplyAndReset()
		{
			if (IsValid())
			{
				for (int i = 0; i < multiplierList.Count; i++)
				{
					currentColor *= multiplierList[i].mulColor;
				}
				material.SetColor(propId, currentColor);
				currentColor = originalColor;
			}
		}

		public bool IsValid()
		{
			return propId != -1;
		}
	}
}
