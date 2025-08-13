using FullSerializer;

namespace MoleMole.Config
{
	[fsObject(Converter = typeof(DynamicIntConverter))]
	public class DynamicInt
	{
		public static DynamicInt ZERO = new DynamicInt
		{
			fixedValue = 0
		};

		public static DynamicInt ONE = new DynamicInt
		{
			fixedValue = 1
		};

		public bool isDynamic;

		public int fixedValue;

		public string dynamicKey;
	}
}
