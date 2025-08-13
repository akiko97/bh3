using System.Collections.Generic;
using BehaviorDesigner.Runtime;

namespace MoleMole
{
	public class SharedEntityDictionary : SharedVariable<Dictionary<int, BaseMonoEntity>>
	{
		public override string ToString()
		{
			return (mValue != null) ? (mValue.Count + " BaseMonoEntity") : "null";
		}

		public static implicit operator SharedEntityDictionary(Dictionary<int, BaseMonoEntity> value)
		{
			SharedEntityDictionary sharedEntityDictionary = new SharedEntityDictionary();
			sharedEntityDictionary.mValue = value;
			return sharedEntityDictionary;
		}
	}
}
