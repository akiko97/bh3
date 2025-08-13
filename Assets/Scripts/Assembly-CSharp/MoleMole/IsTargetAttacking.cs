using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class IsTargetAttacking : Conditional
	{
		private BaseMonoAbilityEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAbilityEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoEntity attackTarget = _entity.GetAttackTarget();
			if (attackTarget == null || !attackTarget.IsActive())
			{
				return TaskStatus.Failure;
			}
			if (attackTarget is BaseMonoAvatar)
			{
				BaseMonoAvatar baseMonoAvatar = (BaseMonoAvatar)attackTarget;
				if (baseMonoAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill) && !baseMonoAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackWithNoTarget))
				{
					return TaskStatus.Success;
				}
				return TaskStatus.Failure;
			}
			if (attackTarget is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = (BaseMonoMonster)attackTarget;
				return (!baseMonoMonster.isGoingToAttack(0.5f)) ? TaskStatus.Failure : TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
