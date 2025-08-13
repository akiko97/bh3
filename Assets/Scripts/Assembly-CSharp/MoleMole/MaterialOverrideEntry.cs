using System;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class MaterialOverrideEntry
	{
		public static MaterialOverrideEntry[] EMPTY = new MaterialOverrideEntry[0];

		public string materialOverrideKey;

		public Material material;
	}
}
