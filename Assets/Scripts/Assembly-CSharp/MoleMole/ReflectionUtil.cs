using System;
using System.Collections.Generic;
using System.Reflection;

namespace MoleMole
{
	public class ReflectionUtil
	{
		private static Dictionary<Type, FieldInfo[]> _reflectionFieldCache = new Dictionary<Type, FieldInfo[]>();

		private static Dictionary<Type, PropertyInfo[]> _reflectionPropertyCache = new Dictionary<Type, PropertyInfo[]>();

		public static void SetValue(object instance, string propertyName, object value)
		{
			FieldInfo fieldInfo = TryGetFieldValue(instance, propertyName);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(instance, value);
				return;
			}
			PropertyInfo propertyInfo = TryGetPropertyValue(instance, propertyName);
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(instance, value, null);
			}
		}

		public static object GetValue(object instance, string propertyName)
		{
			FieldInfo fieldInfo = TryGetFieldValue(instance, propertyName);
			if (fieldInfo != null)
			{
				return fieldInfo.GetValue(instance);
			}
			PropertyInfo propertyInfo = TryGetPropertyValue(instance, propertyName);
			if (propertyInfo != null)
			{
				return propertyInfo.GetValue(instance, null);
			}
			return null;
		}

		public static Type GetValueType(object instance, string propertyName)
		{
			FieldInfo fieldInfo = TryGetFieldValue(instance, propertyName);
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = TryGetPropertyValue(instance, propertyName);
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			return null;
		}

		private static FieldInfo TryGetFieldValue(object instance, string propertyName)
		{
			Type type = instance.GetType();
			FieldInfo[] fieldsWithCache = GetFieldsWithCache(type);
			FieldInfo result = null;
			for (int i = 0; i < fieldsWithCache.Length; i++)
			{
				if (propertyName == fieldsWithCache[i].Name)
				{
					result = fieldsWithCache[i];
					break;
				}
			}
			return result;
		}

		private static PropertyInfo TryGetPropertyValue(object instance, string propertyName)
		{
			Type type = instance.GetType();
			PropertyInfo[] propertysWithCache = GetPropertysWithCache(type);
			PropertyInfo result = null;
			for (int i = 0; i < propertysWithCache.Length; i++)
			{
				if (propertyName == propertysWithCache[i].Name)
				{
					result = propertysWithCache[i];
					break;
				}
			}
			return result;
		}

		private static FieldInfo[] GetFieldsWithCache(Type type)
		{
			if (!_reflectionFieldCache.ContainsKey(type))
			{
				_reflectionFieldCache.Add(type, type.GetFields());
			}
			return _reflectionFieldCache[type];
		}

		private static PropertyInfo[] GetPropertysWithCache(Type type)
		{
			if (!_reflectionPropertyCache.ContainsKey(type))
			{
				_reflectionPropertyCache.Add(type, type.GetProperties());
			}
			return _reflectionPropertyCache[type];
		}
	}
}
