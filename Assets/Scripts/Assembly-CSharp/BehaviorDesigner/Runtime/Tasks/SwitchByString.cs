namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskCategory("Switch")]
	public class SwitchByString : BaseSwitch
	{
		public SharedString target;

		public string[] cases;

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
