namespace MoleMole.Config
{
	public class AnimatorEventClearAttackTarget : AnimatorEvent
	{
		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.ClearAttackTarget();
		}
	}
}
