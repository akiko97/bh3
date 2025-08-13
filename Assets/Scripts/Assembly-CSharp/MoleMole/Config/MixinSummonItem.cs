using System.Collections.Generic;

namespace MoleMole.Config
{
	public class MixinSummonItem
	{
		public DynamicString MonsterName;

		public DynamicString TypeName;

		public bool BaseOnTarget;

		public float Distance;

		public float Angle;

		public float EffectDelay;

		public float SummonDelay;

		public bool UseCoffinAnim;

		public int CoffinIndex;

		public Dictionary<string, ConfigEntityAbilityEntry> Abilities;
	}
}
