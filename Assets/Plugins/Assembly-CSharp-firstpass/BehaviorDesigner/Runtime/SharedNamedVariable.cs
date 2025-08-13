using System;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedNamedVariable : SharedVariable<NamedVariable>
	{
		public override string ToString()
		{
			return (mValue != null) ? mValue.ToString() : "null";
		}

		public static implicit operator SharedNamedVariable(NamedVariable value)
		{
			SharedNamedVariable sharedNamedVariable = new SharedNamedVariable();
			sharedNamedVariable.mValue = value;
			return sharedNamedVariable;
		}
	}
}
