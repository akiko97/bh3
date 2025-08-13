namespace MoleMole.Config
{
	public class AnimatorEventStopAllEffectsImmediately : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.StopAllEffectsImmediately();
		}
	}
}
