using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("AttackTarget/Monster")]
	public class AttackTargetSelectByThreat : Action
	{
		private BaseMonoMonster _monster;

		private MonsterAIPlugin _monsterAIPlugin;

		public SharedEntity retargetEntity;

		public override void OnAwake()
		{
			_monster = GetComponent<BaseMonoMonster>();
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(_monster.GetRuntimeID());
			_monsterAIPlugin = actor.GetPlugin<MonsterAIPlugin>();
		}

		public override TaskStatus OnUpdate()
		{
			if (retargetEntity.Value != null && retargetEntity.Value.IsActive())
			{
				_monster.SetAttackTarget(retargetEntity.Value);
				retargetEntity.SetValue(null);
				return TaskStatus.Success;
			}
			if (_monster.AttackTarget == null)
			{
				BaseMonoAvatar baseMonoAvatar = SelectNearestAvatar();
				if (baseMonoAvatar != null)
				{
					_monsterAIPlugin.InitNearestAvatarThreat(baseMonoAvatar.GetRuntimeID());
					_monster.SetAttackTarget(baseMonoAvatar);
					return TaskStatus.Success;
				}
				return TaskStatus.Running;
			}
			uint runtimeID = _monster.AttackTarget.GetRuntimeID();
			uint num = _monsterAIPlugin.RetargetByThreat(runtimeID);
			if (runtimeID != num)
			{
				_monster.SetAttackTarget(Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(num));
			}
			return TaskStatus.Success;
		}

		private BaseMonoAvatar SelectNearestAvatar()
		{
			BaseMonoAvatar result = null;
			float num = float.MaxValue;
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allAvatars[i];
				if (!(baseMonoAvatar == null) && baseMonoAvatar.IsActive() && !baseMonoAvatar.denySelect)
				{
					float num2 = Miscs.DistancForVec3IgnoreY(_monster.XZPosition, baseMonoAvatar.XZPosition);
					if (num2 < num)
					{
						num = num2;
						result = baseMonoAvatar;
					}
				}
			}
			return result;
		}
	}
}
