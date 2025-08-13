using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class GetSharedEntityAttackTarget : Action
	{
		public SharedEntity targetEntity;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoEntity baseMonoEntity = null;
			if (targetEntity.Value != null)
			{
				baseMonoEntity = (targetEntity.Value as BaseMonoMonster).AttackTarget;
			}
			if (baseMonoEntity != null)
			{
				GetComponent<BaseMonoMonster>().SetAttackTarget(baseMonoEntity);
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
