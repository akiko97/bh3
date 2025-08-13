using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Avatar")]
	public class AttackTargetSelectLockMonster : Action
	{
		private BaseMonoAvatar _avatar;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoEntity baseMonoEntity = SelectTarget();
			if (baseMonoEntity != null)
			{
				_avatar.GetActiveAIController().TrySetAttackTarget(baseMonoEntity);
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		private BaseMonoEntity SelectTarget()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar.AttackTarget != null && localAvatar.AttackTarget.IsActive())
			{
				return localAvatar.AttackTarget;
			}
			return null;
		}
	}
}
