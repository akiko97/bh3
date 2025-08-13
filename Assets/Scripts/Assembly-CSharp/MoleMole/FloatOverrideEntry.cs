using System;

namespace MoleMole
{
	[Serializable]
	public class FloatOverrideEntry
	{
		public static FloatOverrideEntry[] EMPTY = new FloatOverrideEntry[0];

		public string floatOverrideKey;

		public float value;
	}
}
