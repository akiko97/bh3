using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	[fiInspectorOnly]
	public class MonoEffectOverride : MonoBehaviour
	{
		public Dictionary<string, Material> materialOverrides;

		public Dictionary<string, Color> colorOverrides;

		public Dictionary<string, float> floatOverrides;

		public Dictionary<string, string> effectOverlays;

		public Dictionary<string, string> effectOverrides;

		public List<string> effectPredicates;

		private void Awake()
		{
			materialOverrides = new Dictionary<string, Material>();
			colorOverrides = new Dictionary<string, Color>();
			effectOverlays = new Dictionary<string, string>();
			floatOverrides = new Dictionary<string, float>();
			effectOverrides = new Dictionary<string, string>();
			effectPredicates = new List<string>();
		}
	}
}
