using System;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class ColorOverrideEntry
	{
		public static ColorOverrideEntry[] EMPTY = new ColorOverrideEntry[0];

		public string colorOverrideKey;

		public Color color;
	}
}
