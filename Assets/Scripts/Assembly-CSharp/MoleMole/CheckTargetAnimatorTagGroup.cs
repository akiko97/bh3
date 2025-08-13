using System;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Monster")]
	[TaskDescription("Check whether target in is certain animator tag group, e.g. Attack or Skill, Movement, etc.")]
	public class CheckTargetAnimatorTagGroup : BehaviorDesigner.Runtime.Tasks.Action
	{
		public string animatorTag;

		private IAIEntity _aiEntity;

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			_aiEntity = (BaseMonoMonster)component;
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = _aiEntity.AttackTarget as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity == null)
			{
				return TaskStatus.Failure;
			}
			if (baseMonoAnimatorEntity is BaseMonoAvatar)
			{
				AvatarData.AvatarTagGroup tagGroup = (AvatarData.AvatarTagGroup)(int)Enum.Parse(typeof(AvatarData.AvatarTagGroup), animatorTag);
				if ((baseMonoAnimatorEntity as BaseMonoAvatar).IsAnimatorInTag(tagGroup))
				{
					return TaskStatus.Success;
				}
			}
			else if (baseMonoAnimatorEntity is BaseMonoMonster)
			{
				MonsterData.MonsterTagGroup tagGroup2 = (MonsterData.MonsterTagGroup)(int)Enum.Parse(typeof(MonsterData.MonsterTagGroup), animatorTag);
				if ((baseMonoAnimatorEntity as BaseMonoMonster).IsAnimatorInTag(tagGroup2))
				{
					return TaskStatus.Success;
				}
			}
			return TaskStatus.Failure;
		}
	}
}
