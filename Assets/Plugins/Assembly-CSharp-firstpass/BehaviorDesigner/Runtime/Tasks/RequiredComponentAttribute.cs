using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RequiredComponentAttribute : Attribute
	{
		public readonly Type mComponentType;

		public Type ComponentType
		{
			get
			{
				return mComponentType;
			}
		}

		public RequiredComponentAttribute(Type componentType)
		{
			mComponentType = componentType;
		}
	}
}
