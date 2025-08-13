namespace MoleMole.Config
{
	public class AnimatorEventStopAllEffects : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.StopAllEffects();
		}
	}
}
