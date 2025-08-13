using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Ability")]
	public class GetAnimatorInteger : Action
	{
		public string AnimatorInteger;

		public SharedInt SharedInteger;

		private BaseMonoAnimatorEntity _animatorEntity;

		public override void OnAwake()
		{
			BaseMonoEntity component = GetComponent<BaseMonoEntity>();
			_animatorEntity = (BaseMonoAnimatorEntity)component;
		}

		public override TaskStatus OnUpdate()
		{
			SharedInteger.SetValue(_animatorEntity.GetLocomotionInteger(AnimatorInteger));
			return TaskStatus.Success;
		}
	}
}
