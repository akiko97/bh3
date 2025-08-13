using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class NamedVariable
	{
		[SerializeField]
		public string name = string.Empty;

		[SerializeField]
		public string type = "SharedString";

		[SerializeField]
		public SharedVariable value;
	}
}
