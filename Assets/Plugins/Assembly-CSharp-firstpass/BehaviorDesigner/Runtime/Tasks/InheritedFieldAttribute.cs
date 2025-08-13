using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	[Obsolete("The InheritedField attribute has been deprecated. Use SharedVariables instead.")]
	public class InheritedFieldAttribute : Attribute
	{
	}
}
