using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Avatar")]
	public class CheckCanAttack : BaseAvatarAction
	{
		public override TaskStatus OnUpdate()
		{
			if (_avatarActor.CanUseSkill("ATK"))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
