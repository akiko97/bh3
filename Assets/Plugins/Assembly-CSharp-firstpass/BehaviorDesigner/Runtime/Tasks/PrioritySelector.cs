using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}PrioritySelectorIcon.png")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=29")]
	[TaskDescription("Similar to the selector task, the priority selector task will return success as soon as a child task returns success. Instead of running the tasks sequentially from left to right within the tree, the priority selector will ask the task what its priority is to determine the order. The higher priority tasks have a higher chance at being run first.")]
	public class PrioritySelector : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		private List<int> childrenExecutionOrder = new List<int>();

		public override void OnStart()
		{
			childrenExecutionOrder.Clear();
			for (int i = 0; i < children.Count; i++)
			{
				float priority = children[i].GetPriority();
				int index = childrenExecutionOrder.Count;
				for (int j = 0; j < childrenExecutionOrder.Count; j++)
				{
					if (children[childrenExecutionOrder[j]].GetPriority() < priority)
					{
						index = j;
						break;
					}
				}
				childrenExecutionOrder.Insert(index, i);
			}
		}

		public override int CurrentChildIndex()
		{
			return childrenExecutionOrder[currentChildIndex];
		}

		public override bool CanExecute()
		{
			return currentChildIndex < children.Count && executionStatus != TaskStatus.Success;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			currentChildIndex++;
			executionStatus = childStatus;
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
	}
}
