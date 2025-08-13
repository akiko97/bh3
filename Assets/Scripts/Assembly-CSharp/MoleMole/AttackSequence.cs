using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskIcon("{SkinColor}SequenceIcon.png")]
	[TaskDescription("Clear AttackNum On End")]
	public class AttackSequence : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		public SharedBool IsAttacking;

		public SharedInt avatarAttackNum;

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override bool CanExecute()
		{
			return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
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
			if (IsAttacking.Value)
			{
				Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>().RemoveAttackingMonster(GetComponent<BaseMonoMonster>());
				IsAttacking.Value = false;
			}
		}
	}
}
