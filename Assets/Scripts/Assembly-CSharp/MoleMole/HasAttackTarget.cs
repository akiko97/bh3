using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class HasAttackTarget : Conditional
	{
		private BaseMonoAbilityEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAbilityEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoEntity attackTarget = _entity.GetAttackTarget();
			if (attackTarget != null)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
