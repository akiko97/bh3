using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class HasAlliedAround : Action
	{
		public float range = 10f;

		public override void OnAwake()
		{
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = component as BaseMonoMonster;
				List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
				for (int i = 0; i < allMonsters.Count; i++)
				{
					if (allMonsters[i] != baseMonoMonster && allMonsters[i].IsActive() && Vector3.Distance(allMonsters[i].XZPosition, baseMonoMonster.XZPosition) < range)
					{
						return TaskStatus.Success;
					}
				}
				return TaskStatus.Failure;
			}
			if (component is BaseMonoAvatar)
			{
				BaseMonoAvatar baseMonoAvatar = component as BaseMonoAvatar;
				List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
				for (int j = 0; j < allAvatars.Count; j++)
				{
					if (allAvatars[j] != baseMonoAvatar && allAvatars[j].IsActive() && Vector3.Distance(allAvatars[j].XZPosition, baseMonoAvatar.XZPosition) < range)
					{
						return TaskStatus.Success;
					}
				}
				return TaskStatus.Failure;
			}
			return TaskStatus.Failure;
		}
	}
}
