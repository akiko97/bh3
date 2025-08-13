using MoleMole;
using MoleMole.Config;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskCategory("Switch")]
	public class SwitchByMoveType : BaseSwitch
	{
		public SharedGroupMoveType target;

		public ConfigGroupAIMinionOld.MoveType[] cases;

		public override void OnAwake()
		{
		}

		protected override int CalculateChildIndex()
		{
			for (int i = 0; i < cases.Length; i++)
			{
				if (cases[i] == target.Value)
				{
					return i;
				}
			}
			return _currentChildIndex;
		}
	}
}
