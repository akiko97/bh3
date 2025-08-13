using FullSerializer;

namespace MoleMole.Config
{
	[fsObject(Converter = typeof(DynamicFloatConverter))]
	public class DynamicFloat
	{
		public static DynamicFloat ZERO = new DynamicFloat
		{
			fixedValue = 0f
		};

		public static DynamicFloat ONE = new DynamicFloat
		{
			fixedValue = 1f
		};

		public bool isDynamic;

		public float fixedValue;

		public string dynamicKey;
	}
}
