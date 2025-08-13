using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class IsInAnimtorTag : Conditional
	{
		public List<AvatarData.AvatarTagGroup> animatorTagNamesAvatar;

		public List<MonsterData.MonsterTagGroup> animatorTagNamesMonster;

		private BaseMonoAnimatorEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			if (_entity is BaseMonoAvatar)
			{
				BaseMonoAvatar baseMonoAvatar = _entity as BaseMonoAvatar;
				for (int i = 0; i < animatorTagNamesAvatar.Count; i++)
				{
					if (baseMonoAvatar.IsAnimatorInTag(animatorTagNamesAvatar[i]))
					{
						return TaskStatus.Success;
					}
				}
			}
			else if (_entity is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = _entity as BaseMonoMonster;
				for (int j = 0; j < animatorTagNamesMonster.Count; j++)
				{
					if (baseMonoMonster.IsAnimatorInTag(animatorTagNamesMonster[j]))
					{
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Failure;
		}
	}
}
