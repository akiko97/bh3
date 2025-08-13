using System;

namespace MoleMole
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GeneratePartialHashAttribute : Attribute
	{
		public bool CombineGeneratedFile;
	}
}
