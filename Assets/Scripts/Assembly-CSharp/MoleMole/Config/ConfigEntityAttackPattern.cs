using System;
using UnityEngine;

namespace MoleMole.Config
{
	public abstract class ConfigEntityAttackPattern
	{
		[NonSerialized]
		public Action<string, ConfigEntityAttackPattern, IAttacker, LayerMask> patternMethod;
	}
}
