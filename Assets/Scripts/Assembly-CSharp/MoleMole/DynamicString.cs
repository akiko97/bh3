using FullSerializer;

namespace MoleMole
{
	[fsObject(Converter = typeof(DynamicStringConverter))]
	public class DynamicString
	{
		public bool isDynamic;

		public string fixedValue;

		public string dynamicKey;
	}
}
