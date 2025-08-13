using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Ability")]
	public class CheckAnimatorBoolean : Action
	{
		public string AnimatorBoolean;

		private BaseMonoAnimatorEntity _animatorEntity;

		public override void OnAwake()
		{
			BaseMonoEntity component = GetComponent<BaseMonoEntity>();
			_animatorEntity = (BaseMonoAnimatorEntity)component;
		}

		public override TaskStatus OnUpdate()
		{
			if (_animatorEntity.GetLocomotionBool(AnimatorBoolean))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
