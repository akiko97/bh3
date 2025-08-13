using UnityEngine;

namespace MoleMole
{
	public class MonoEffectOverrideSetting : MonoBehaviour
	{
		[HideInInspector]
		public MaterialOverrideEntry[] materialOverrides = MaterialOverrideEntry.EMPTY;

		[HideInInspector]
		public ColorOverrideEntry[] colorOverrides = ColorOverrideEntry.EMPTY;

		[HideInInspector]
		public FloatOverrideEntry[] floatOverrides = FloatOverrideEntry.EMPTY;
	}
}
