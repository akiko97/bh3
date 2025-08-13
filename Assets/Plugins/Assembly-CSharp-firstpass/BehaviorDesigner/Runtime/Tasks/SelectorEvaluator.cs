namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The selector evaluator is a selector task which reevaluates its children every tick. It will run the lowest priority child which returns a task status of running. This is done each tick. If a higher priority child is running and the next frame a lower priority child wants to run it will interrupt the higher priority child. The selector evaluator will return success as soon as the first child returns success otherwise it will keep trying higher priority children. This task mimics the conditional abort functionality except the child tasks don't always have to be conditional tasks.")]
	[TaskIcon("{SkinColor}SelectorEvaluatorIcon.png")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=109")]
	public class SelectorEvaluator : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		private int storedCurrentChildIndex = -1;

		private TaskStatus storedExecutionStatus;

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override void OnChildStarted(int childIndex)
		{
			currentChildIndex++;
			executionStatus = TaskStatus.Running;
		}

		public override bool CanExecute()
		{
			if (executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Running)
			{
				return false;
			}
			if (storedCurrentChildIndex != -1)
			{
				return currentChildIndex < storedCurrentChildIndex - 1;
			}
			return currentChildIndex < children.Count;
		}

		public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
		{
			if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running)
			{
				executionStatus = childStatus;
			}
		}

		public override void OnConditionalAbort(int childIndex)
		{
			currentChildIndex = childIndex;
			executionStatus = TaskStatus.Inactive;
		}

		public override void OnEnd()
		{
			executionStatus = TaskStatus.Inactive;
			currentChildIndex = 0;
		}

		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			return executionStatus;
		}

		public override bool CanRunParallelChildren()
		{
			return true;
		}

		public override bool CanReevaluate()
		{
			return true;
		}

		public override bool OnReevaluationStarted()
		{
			if (executionStatus == TaskStatus.Inactive)
			{
				return false;
			}
			storedCurrentChildIndex = currentChildIndex;
			storedExecutionStatus = executionStatus;
			currentChildIndex = 0;
			executionStatus = TaskStatus.Inactive;
			return true;
		}

		public override void OnReevaluationEnded(TaskStatus status)
		{
			if (executionStatus != TaskStatus.Failure && executionStatus != TaskStatus.Inactive)
			{
				BehaviorManager.instance.Interrupt(base.Owner, children[storedCurrentChildIndex - 1], this);
			}
			else
			{
				currentChildIndex = storedCurrentChildIndex;
				executionStatus = storedExecutionStatus;
			}
			storedCurrentChildIndex = -1;
			storedExecutionStatus = TaskStatus.Inactive;
		}
	}
}
