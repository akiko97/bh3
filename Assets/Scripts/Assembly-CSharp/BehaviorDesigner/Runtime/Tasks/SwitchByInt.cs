namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskCategory("Switch")]
	public class SwitchByInt : BaseSwitch
	{
		public SharedInt target;

		public int[] cases;

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
