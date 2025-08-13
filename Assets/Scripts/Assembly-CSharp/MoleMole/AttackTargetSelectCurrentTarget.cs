using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Avatar")]
	public class AttackTargetSelectCurrentTarget : Action
	{
		private BaseMonoAvatar _avatar;

		public SharedBool isNewTarget;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
		}

		public override TaskStatus OnUpdate()
		{
			if (_avatar.AttackTarget != null && _avatar.AttackTarget.IsActive())
			{
				isNewTarget.SetValue(false);
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
