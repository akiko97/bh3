using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TaskIconAttribute : Attribute
	{
		public readonly string mIconPath;

		public string IconPath
		{
			get
			{
				return mIconPath;
			}
		}

		public TaskIconAttribute(string iconPath)
		{
			mIconPath = iconPath;
		}
	}
}
