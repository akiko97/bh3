namespace MoleMole.Config
{
	public class AnimatorEventDeadHandler : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.DeadHandler();
		}
	}
}
