using System;

namespace MoleMole
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class AnimationCallbackAttribute : Attribute
	{
	}
}
