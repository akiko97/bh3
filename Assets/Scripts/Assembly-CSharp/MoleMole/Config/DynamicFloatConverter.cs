using System;
using FullSerializer;

namespace MoleMole.Config
{
	public class DynamicFloatConverter : fsConverter
	{
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new DynamicFloat();
		}

		public override bool CanProcess(Type type)
		{
			return type == typeof(DynamicFloat);
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			DynamicFloat dynamicFloat = (DynamicFloat)instance;
			if (data.IsDouble)
			{
				dynamicFloat.isDynamic = false;
				dynamicFloat.fixedValue = (float)data.AsDouble;
				return fsResult.Success;
			}
			if (data.IsInt64)
			{
				dynamicFloat.isDynamic = false;
				dynamicFloat.fixedValue = data.AsInt64;
				return fsResult.Success;
			}
			if (data.IsString)
			{
				string asString = data.AsString;
				if (!asString.StartsWith("%"))
				{
					return fsResult.Fail("DynamicFloat key needs to be like '%key' .");
				}
				dynamicFloat.isDynamic = true;
				dynamicFloat.dynamicKey = asString.TrimStart('%');
				return fsResult.Success;
			}
			return fsResult.Fail("DynamicFloat fields needs to be either a '%key' or a float value");
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			DynamicFloat dynamicFloat = instance as DynamicFloat;
			if (dynamicFloat == null)
			{
				serialized = new fsData();
				return fsResult.Fail("Failed to convert field to DynamicFloat on serialization.");
			}
			serialized = ((!dynamicFloat.isDynamic) ? new fsData(dynamicFloat.fixedValue) : new fsData("%" + dynamicFloat.dynamicKey));
			return fsResult.Success;
		}
	}
}
