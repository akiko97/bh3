namespace MoleMole.Config
{
	public class AnimatorEventClearAttackTriggers : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.ClearAttackTriggers();
		}
	}
}
