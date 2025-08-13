using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskDescription("The force sequence task is similar to sequence. But it will never return failure! It will run all children tasks whatever the children return")]
	[TaskIcon("{SkinColor}SequenceIcon.png")]
	public class ForceSequence : Composite
	{
		private int currentChildIndex;

		protected TaskStatus executionStatus;

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override bool CanExecute()
		{
			return currentChildIndex < children.Count;
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
