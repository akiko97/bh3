namespace MoleMole.Config
{
	public class AnimatorEventClearSkillTriggers : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			(entity as BaseMonoAvatar).ClearSkillTriggers();
		}
	}
}
