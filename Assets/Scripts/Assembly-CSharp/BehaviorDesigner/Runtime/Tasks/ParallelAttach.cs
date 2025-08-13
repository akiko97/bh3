namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}ParallelSelectorIcon.png")]
	[TaskDescription("Only count on the first child!! Run all Children at the same time")]
	public class ParallelAttach : ParallelSelector
	{
		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			bool flag = true;
			if (executionStatus.Length > 0)
			{
				if (executionStatus[0] == TaskStatus.Running)
				{
					flag = false;
				}
				else if (executionStatus[0] == TaskStatus.Success)
				{
					return TaskStatus.Success;
				}
			}
			return flag ? TaskStatus.Failure : TaskStatus.Running;
		}
	}
}
