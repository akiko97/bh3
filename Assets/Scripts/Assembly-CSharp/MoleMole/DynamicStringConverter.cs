using System;
using FullSerializer;

namespace MoleMole
{
	public class DynamicStringConverter : fsConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new DynamicString();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(DynamicString);
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			DynamicString dynamicString = (DynamicString)instance;
			if (data.IsString)
			{
				string asString = data.AsString;
				if (asString.StartsWith("%"))
				{
					dynamicString.isDynamic = true;
					dynamicString.dynamicKey = asString.TrimStart('%');
				}
				else
				{
					dynamicString.isDynamic = false;
					dynamicString.fixedValue = asString;
				}
				return fsResult.Success;
			}
			return fsResult.Fail("DynamicString fields needs to be either a '%key' or a string value.");
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			DynamicString dynamicString = instance as DynamicString;
			if (dynamicString == null)
			{
				serialized = new fsData();
				return fsResult.Fail("Failed to convert field to DynamicInt on serialization");
			}
			serialized = ((!dynamicString.isDynamic) ? new fsData(dynamicString.fixedValue) : new fsData("%" + dynamicString.dynamicKey));
			return fsResult.Success;
		}
	}
}
