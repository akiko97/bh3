using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class SetSkillAnimatorPattern : Action
	{
		public string skillName;

		public string animatorEventPatternName;

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = component as BaseMonoMonster;
				baseMonoMonster.SetSoleSkillAnimatorEventPattern(skillName, animatorEventPatternName);
			}
			return TaskStatus.Success;
		}
	}
}
