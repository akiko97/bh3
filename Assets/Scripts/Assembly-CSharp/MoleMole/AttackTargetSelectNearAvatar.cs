using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Monster")]
	public class AttackTargetSelectNearAvatar : Action
	{
		private const float NEAR_FAR_ATTACK_DIS_THRESHOLD = 5f;

		public float ChangeAvatarDistanceRatioBias = 0.8f;

		public SharedAttackType TargetType;

		protected BaseMonoMonster _monster;

		public override void OnAwake()
		{
			_monster = GetComponent<BaseMonoMonster>();
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			SelectNearestAvatar();
			if (_monster.AttackTarget != null && _monster.AttackTarget.IsActive())
			{
				if (TargetType != null)
				{
					if ((_monster.AttackTarget as BaseMonoAvatar).config.AIArguments.AttackDistance > 5f)
					{
						TargetType.SetValue(AttackType.FarAttack);
					}
					else
					{
						TargetType.SetValue(AttackType.NearAttack);
					}
				}
				return TaskStatus.Success;
			}
			return TaskStatus.Running;
		}

		protected void SelectNearestAvatar()
		{
			List<BaseMonoAvatar> list = new List<BaseMonoAvatar>();
			foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				if (allAvatar.IsActive())
				{
					list.Add(allAvatar);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			BaseMonoEntity attackTarget = null;
			bool flag = false;
			if (_monster.AttackTarget != null && _monster.AttackTarget.IsActive())
			{
				flag = !(_monster.AttackTarget as BaseMonoAnimatorEntity).denySelect;
			}
			float num = float.PositiveInfinity;
			if (flag)
			{
				num = Miscs.DistancForVec3IgnoreY(_monster.AttackTarget.XZPosition, _monster.XZPosition) * ChangeAvatarDistanceRatioBias;
			}
			float num2 = float.PositiveInfinity;
			foreach (BaseMonoAvatar item in list)
			{
				if (!(item == null) && item.IsActive() && !item.denySelect)
				{
					if (flag && Miscs.DistancForVec3IgnoreY(item.XZPosition, _monster.XZPosition) < num)
					{
						attackTarget = item;
					}
					else if (!flag && Miscs.DistancForVec3IgnoreY(item.XZPosition, _monster.XZPosition) < num2)
					{
						attackTarget = item;
						num2 = Miscs.DistancForVec3IgnoreY(item.XZPosition, _monster.XZPosition);
					}
				}
			}
			_monster.SetAttackTarget(attackTarget);
		}
	}
}
