using System;
using FullSerializer;

namespace MoleMole.Config
{
	public class DynamicIntConverter : fsConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new DynamicInt();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(DynamicInt);
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			DynamicInt dynamicInt = (DynamicInt)instance;
			if (data.IsInt64)
			{
				dynamicInt.isDynamic = false;
				dynamicInt.fixedValue = (int)data.AsInt64;
				return fsResult.Success;
			}
			if (data.IsString)
			{
				string asString = data.AsString;
				if (!asString.StartsWith("%"))
				{
					return fsResult.Fail("DynamicInt key needs to be like '%key' .");
				}
				dynamicInt.isDynamic = true;
				dynamicInt.dynamicKey = asString.TrimStart('%');
				return fsResult.Success;
			}
			return fsResult.Fail("DynamicInt fields needs to be either a '%key' or a int value.");
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			DynamicInt dynamicInt = instance as DynamicInt;
			if (dynamicInt == null)
			{
				serialized = new fsData();
				return fsResult.Fail("Failed to convert field to DynamicInt on serialization");
			}
			serialized = ((!dynamicInt.isDynamic) ? new fsData(dynamicInt.fixedValue) : new fsData("%" + dynamicInt.dynamicKey));
			return fsResult.Success;
		}
	}
}
