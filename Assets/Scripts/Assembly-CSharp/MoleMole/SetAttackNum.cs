using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class SetAttackNum : Action
	{
		public enum SetType
		{
			BEGIN = 0,
			END = 1
		}

		public SharedInt AvatarBeAttackNum;

		public SharedBool IsAttacking;

		public SetType attackType;

		private IAIEntity _aiEntity;

		private BaseMonoMonster monster;

		private bool _isTargetLocalAvatar;

		private LevelAIPlugin _levelAIPlugin;

		public override void OnAwake()
		{
			monster = GetComponent<BaseMonoMonster>();
			_aiEntity = monster;
			_levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoAvatar baseMonoAvatar = _aiEntity.AttackTarget as BaseMonoAvatar;
			if (baseMonoAvatar == null)
			{
				_isTargetLocalAvatar = false;
			}
			else
			{
				_isTargetLocalAvatar = Singleton<AvatarManager>.Instance.IsLocalAvatar(baseMonoAvatar.GetRuntimeID());
			}
			if (_isTargetLocalAvatar)
			{
				if (attackType == SetType.BEGIN)
				{
					_levelAIPlugin.AddAttackingMonster(monster);
					IsAttacking.Value = true;
				}
				else
				{
					_levelAIPlugin.RemoveAttackingMonster(monster);
					IsAttacking.Value = false;
				}
			}
			return TaskStatus.Success;
		}
	}
}
