using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class TriggerAnim : Action
	{
		public enum EntityType
		{
			Avatar = 0,
			Monster = 1,
			None = 2
		}

		public string triggerName = string.Empty;

		public bool completely;

		public bool onlyTriggerInSkill;

		public string skillName = string.Empty;

		public EntityType entityType = EntityType.None;

		private BaseMonoAnimatorEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoAnimatorEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			if (onlyTriggerInSkill)
			{
				if (entityType == EntityType.Avatar && GetComponent<BaseMonoAvatar>().CurrentSkillID != skillName)
				{
					return TaskStatus.Failure;
				}
				if (entityType == EntityType.Monster && GetComponent<BaseMonoMonster>().CurrentSkillID != skillName)
				{
					return TaskStatus.Failure;
				}
			}
			if (_entity.IsActive())
			{
				if (!completely)
				{
					_entity.SetTrigger(triggerName + "Trigger");
				}
				else
				{
					_entity.SetTrigger(triggerName);
				}
			}
			return TaskStatus.Success;
		}
	}
}
