using MoleMole;
using MoleMole.Config;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskCategory("Switch")]
	public class SwitchByAttackType : BaseSwitch
	{
		public SharedGroupAttackType target;

		public ConfigGroupAIMinionOld.AttackType[] cases;

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
