namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}ParallelSelectorIcon.png")]
	[TaskDescription("Similar to the selector task, the parallel selector task will return success as soon as a child task returns success. The difference is that the parallel task will run all of its children tasks simultaneously versus running each task one at a time. If one tasks returns success the parallel selector task will end all of the child tasks and return success. If every child task returns failure then the parallel selector task will return failure.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=28")]
	public class ParallelSelector : Composite
	{
		private int currentChildIndex;

		protected TaskStatus[] executionStatus;

		public override void OnAwake()
		{
			executionStatus = new TaskStatus[children.Count];
		}

		public override void OnChildStarted(int childIndex)
		{
			currentChildIndex++;
			executionStatus[childIndex] = TaskStatus.Running;
		}

		public override bool CanRunParallelChildren()
		{
			return true;
		}

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override bool CanExecute()
		{
			return currentChildIndex < children.Count;
		}

		public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
		{
			executionStatus[childIndex] = childStatus;
		}

		public override void OnConditionalAbort(int childIndex)
		{
			currentChildIndex = 0;
			for (int i = 0; i < executionStatus.Length; i++)
			{
				executionStatus[i] = TaskStatus.Inactive;
			}
		}

		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			bool flag = true;
			for (int i = 0; i < executionStatus.Length; i++)
			{
				if (executionStatus[i] == TaskStatus.Running)
				{
					flag = false;
				}
				else if (executionStatus[i] == TaskStatus.Success)
				{
					return TaskStatus.Success;
				}
			}
			return flag ? TaskStatus.Failure : TaskStatus.Running;
		}

		public override void OnEnd()
		{
			for (int i = 0; i < executionStatus.Length; i++)
			{
				executionStatus[i] = TaskStatus.Inactive;
			}
			currentChildIndex = 0;
		}
	}
}
