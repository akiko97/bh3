using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class TooltipAttribute : Attribute
	{
		public readonly string mTooltip;

		public string Tooltip
		{
			get
			{
				return mTooltip;
			}
		}

		public TooltipAttribute(string tooltip)
		{
			mTooltip = tooltip;
		}
	}
}
