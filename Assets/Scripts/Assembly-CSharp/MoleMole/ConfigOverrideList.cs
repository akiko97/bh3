using FullSerializer;

namespace MoleMole
{
	[fsObject(Converter = typeof(ConfigOverrideListConverter))]
	public class ConfigOverrideList
	{
		public static ConfigOverrideList EMPTY = new ConfigOverrideList
		{
			objects = new object[0]
		};

		public object[] objects;

		public int length
		{
			get
			{
				return objects.Length;
			}
		}

		public T GetConfig<T>(int ix)
		{
			return (T)objects[ix];
		}
	}
}
