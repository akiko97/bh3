using System;

namespace MoleMole
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CheckForHashableAttribute : Attribute
	{
		public bool CombineGeneratedFile;
	}
}
