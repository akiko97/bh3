using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Avatar")]
	public class AttackTargetSelectNearMonster : Action
	{
		private BaseMonoAvatar _avatar;

		public bool hasDistanceLimit;

		public float minDistance;

		public float maxDistance;

		public SharedBool isNewTarget;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (_avatar.AttackTarget != null && _avatar.AttackTarget.IsActive())
			{
				isNewTarget.SetValue(false);
				return TaskStatus.Success;
			}
			BaseMonoEntity attackTarget = _avatar.AttackTarget;
			BaseMonoEntity baseMonoEntity = SelectTarget();
			if (attackTarget == null && baseMonoEntity != null)
			{
				isNewTarget.SetValue(true);
			}
			if (baseMonoEntity != null)
			{
				_avatar.GetActiveAIController().TrySetAttackTarget(baseMonoEntity);
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		protected BaseMonoEntity SelectTarget()
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			BaseMonoEntity baseMonoEntity = null;
			float num = float.MaxValue;
			for (int i = 0; i < allMonsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = allMonsters[i];
				if (!baseMonoMonster.IsActive() || baseMonoMonster.denySelect)
				{
					continue;
				}
				List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = baseMonoMonster.GetAllHitboxEnabledBodyParts();
				if (allHitboxEnabledBodyParts.Count > 0)
				{
					foreach (BaseMonoAbilityEntity item in allHitboxEnabledBodyParts)
					{
						float num2 = Miscs.DistancForVec3IgnoreY(item.XZPosition, _avatar.XZPosition);
						if ((!hasDistanceLimit || (!(num2 < minDistance) && !(num2 > maxDistance))) && num2 < num)
						{
							num = num2;
							baseMonoEntity = item;
						}
					}
				}
				else
				{
					float num3 = Miscs.DistancForVec3IgnoreY(baseMonoMonster.XZPosition, _avatar.XZPosition);
					if ((!hasDistanceLimit || (!(num3 < minDistance) && !(num3 > maxDistance))) && num3 < num)
					{
						num = num3;
						baseMonoEntity = baseMonoMonster;
					}
				}
			}
			if (baseMonoEntity != null && baseMonoEntity.IsActive())
			{
				return baseMonoEntity;
			}
			return null;
		}
	}
}
