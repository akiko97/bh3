using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Avatar")]
	public class CheckTargetAttackStateAndRange : BaseAvatarAction
	{
		public const float DEFAULT_MONSTER_ATTACK_RANGE = 3f;

		public float TimeBeforeAttack = 0.48f;

		public override TaskStatus OnUpdate()
		{
			if (Singleton<LevelManager>.Instance.levelActor.witchTimeLevelBuff.isActive)
			{
				return TaskStatus.Failure;
			}
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			for (int i = 0; i < allMonsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[i];
				if (baseMonoMonster.isGoingToAttack(TimeBeforeAttack) && Miscs.DistancForVec3IgnoreY(_avatar.XZPosition, baseMonoMonster.XZPosition) < baseMonoMonster.config.AIArguments.AttackRange)
				{
					return TaskStatus.Success;
				}
			}
			return TaskStatus.Failure;
		}
	}
}
