using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	public class TaskUtility
	{
		[NonSerialized]
		private static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();

		private static List<string> loadedAssemblies = null;

		public static object CreateInstance(Type t)
		{
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				t = Nullable.GetUnderlyingType(t);
			}
			return Activator.CreateInstance(t, true);
		}

		public static FieldInfo[] GetAllFields(Type t)
		{
			List<FieldInfo> fieldList = new List<FieldInfo>();
			BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			GetFields(t, ref fieldList, (int)flags);
			return fieldList.ToArray();
		}

		public static FieldInfo[] GetPublicFields(Type t)
		{
			List<FieldInfo> fieldList = new List<FieldInfo>();
			BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
			GetFields(t, ref fieldList, (int)flags);
			return fieldList.ToArray();
		}

		private static void GetFields(Type t, ref List<FieldInfo> fieldList, int flags)
		{
			if (t != null && !t.Equals(typeof(ParentTask)) && !t.Equals(typeof(Task)) && !t.Equals(typeof(SharedVariable)))
			{
				FieldInfo[] fields = t.GetFields((BindingFlags)flags);
				for (int i = 0; i < fields.Length; i++)
				{
					fieldList.Add(fields[i]);
				}
				GetFields(t.BaseType, ref fieldList, flags);
			}
		}

		public static Type GetTypeWithinAssembly(string typeName)
		{
			if (typeDictionary.ContainsKey(typeName))
			{
				return typeDictionary[typeName];
			}
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				if (loadedAssemblies == null)
				{
					loadedAssemblies = new List<string>();
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < assemblies.Length; i++)
					{
						loadedAssemblies.Add(assemblies[i].FullName);
					}
				}
				for (int j = 0; j < loadedAssemblies.Count; j++)
				{
					type = Type.GetType(typeName + "," + loadedAssemblies[j]);
					if (type != null)
					{
						break;
					}
				}
			}
			if (type != null)
			{
				typeDictionary.Add(typeName, type);
			}
			return type;
		}

		public static bool CompareType(Type t, string typeName)
		{
			Type type = Type.GetType(typeName + ", Assembly-CSharp");
			if (type == null)
			{
				type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
			}
			return t.Equals(type);
		}

		public static bool HasAttribute(FieldInfo field, Type attribute)
		{
			if (field == null)
			{
				return false;
			}
			return field.GetCustomAttributes(attribute, false).Length > 0;
		}

		public static Type SharedVariableToConcreteType(Type sharedVariableType)
		{
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedInt")))
			{
				return typeof(int);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedFloat")))
			{
				return typeof(float);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedBool")))
			{
				return typeof(bool);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString")))
			{
				return typeof(string);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedVector2")))
			{
				return typeof(Vector2);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedVector3")))
			{
				return typeof(Vector3);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedGameObject")))
			{
				return typeof(GameObject);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedSafeInt")))
			{
				return typeof(int);
			}
			if (sharedVariableType.Equals(GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedSafeFloat")))
			{
				return typeof(float);
			}
			return null;
		}
	}
}
