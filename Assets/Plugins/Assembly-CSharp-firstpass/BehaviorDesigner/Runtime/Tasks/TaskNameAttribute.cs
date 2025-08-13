using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskNameAttribute : Attribute
	{
		public readonly string mName;

		public string Name
		{
			get
			{
				return mName;
			}
		}

		public TaskNameAttribute(string name)
		{
			mName = name;
		}
	}
}
