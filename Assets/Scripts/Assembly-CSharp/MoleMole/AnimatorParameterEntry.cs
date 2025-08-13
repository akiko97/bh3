using UnityEngine;

namespace MoleMole
{
	public struct AnimatorParameterEntry
	{
		public int stateHash;

		public AnimatorControllerParameterType type;

		public int intValue;

		public float floatValue;

		public bool boolValue;

		public override string ToString()
		{
			object arg = null;
			if (type == AnimatorControllerParameterType.Bool)
			{
				arg = boolValue;
			}
			else if (type == AnimatorControllerParameterType.Int)
			{
				arg = intValue;
			}
			else if (type == AnimatorControllerParameterType.Float)
			{
				arg = floatValue;
			}
			return string.Format("param {0}, {1}", stateHash, arg);
		}
	}
}
