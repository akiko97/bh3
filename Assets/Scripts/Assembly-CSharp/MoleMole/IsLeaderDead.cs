using BehaviorDesigner.Runtime.Tasks;
using MoleMole.Config;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class IsLeaderDead : Conditional
	{
		public SharedEntity targetEntity;

		public SharedGroupAttackType attackType;

		public SharedGroupMoveType moveType;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override TaskStatus OnUpdate()
		{
			if (targetEntity.Value != null && targetEntity.Value.IsActive())
			{
				return TaskStatus.Failure;
			}
			if (attackType.Value != ConfigGroupAIMinionOld.AttackType.Free || moveType.Value != ConfigGroupAIMinionOld.MoveType.Free)
			{
				attackType.Value = ConfigGroupAIMinionOld.AttackType.Free;
				moveType.Value = ConfigGroupAIMinionOld.MoveType.Free;
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
