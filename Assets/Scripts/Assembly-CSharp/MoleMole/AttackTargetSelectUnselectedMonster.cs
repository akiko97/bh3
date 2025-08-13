using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Avatar")]
	public class AttackTargetSelectUnselectedMonster : Action
	{
		private BaseMonoAvatar _avatar;

		public bool hasDistanceLimit;

		public float minDistance;

		public float maxDistance;

		public SharedBool isNewTarget;

		public bool muteAnimRetarget = true;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
		}

		public override TaskStatus OnUpdate()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			List<BaseMonoMonster> list = new List<BaseMonoMonster>();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allAvatars[i];
				if (!(baseMonoAvatar == null) && baseMonoAvatar.IsActive())
				{
					BaseMonoMonster baseMonoMonster = baseMonoAvatar.AttackTarget as BaseMonoMonster;
					if (!(baseMonoMonster == null))
					{
						list.Add(baseMonoMonster);
					}
				}
			}
			if (allMonsters.Count > 0)
			{
				BaseMonoMonster baseMonoMonster2 = null;
				float num = float.MaxValue;
				for (int j = 0; j < allMonsters.Count; j++)
				{
					BaseMonoMonster baseMonoMonster3 = allMonsters[j];
					if (!baseMonoMonster3.IsActive())
					{
						continue;
					}
					float num2 = Miscs.DistancForVec3IgnoreY(baseMonoMonster3.XZPosition, _avatar.XZPosition);
					if (!hasDistanceLimit || (!(num2 < minDistance) && !(num2 > maxDistance)))
					{
						if (list.Contains(baseMonoMonster3))
						{
							num2 *= 2f;
						}
						if (num2 < num)
						{
							num = num2;
							baseMonoMonster2 = baseMonoMonster3;
						}
					}
				}
				if (baseMonoMonster2 == null)
				{
					return TaskStatus.Failure;
				}
				BaseMonoEntity attackTarget = _avatar.AttackTarget;
				BaseMonoMonster baseMonoMonster4 = baseMonoMonster2;
				isNewTarget.SetValue(attackTarget != baseMonoMonster4);
				if (muteAnimRetarget)
				{
					_avatar.SetMuteAnimRetarget(true);
					_avatar.SetAttackTarget(baseMonoMonster4);
				}
				else
				{
					_avatar.GetActiveAIController().TrySetAttackTarget(baseMonoMonster4);
				}
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
