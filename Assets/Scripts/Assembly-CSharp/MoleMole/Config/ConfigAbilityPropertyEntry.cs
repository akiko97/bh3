namespace MoleMole.Config
{
	public class ConfigAbilityPropertyEntry
	{
		public enum PropertyType
		{
			Entity = 0,
			Actor = 1
		}

		public PropertyType Type;

		public float Default;

		public float Ceiling = float.MaxValue;

		public float Floor = float.MinValue;

		public FixedFloatStack.StackMethod Stacking = FixedFloatStack.StackMethod.Sum;

		public FixedFloatStack CreatePropertyStack()
		{
			return FixedFloatStack.CreateDefault(Default, Stacking, Floor, Ceiling);
		}

		public FixedSafeFloatStack CreatePropertySafeStack()
		{
			return FixedSafeFloatStack.CreateDefault(Default, Stacking, Floor, Ceiling);
		}
	}
}
