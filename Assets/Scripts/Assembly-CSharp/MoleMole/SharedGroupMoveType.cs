using BehaviorDesigner.Runtime;
using MoleMole.Config;

namespace MoleMole
{
	public class SharedGroupMoveType : SharedVariable<ConfigGroupAIMinionOld.MoveType>
	{
		public override string ToString()
		{
			return mValue.ToString();
		}

		public static implicit operator SharedGroupMoveType(ConfigGroupAIMinionOld.MoveType value)
		{
			SharedGroupMoveType sharedGroupMoveType = new SharedGroupMoveType();
			sharedGroupMoveType.mValue = value;
			return sharedGroupMoveType;
		}
	}
}
