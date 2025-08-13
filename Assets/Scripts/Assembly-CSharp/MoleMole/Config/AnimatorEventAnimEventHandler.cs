namespace MoleMole.Config
{
	public class AnimatorEventAnimEventHandler : AnimatorEvent
	{
		public string AnimEventID;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.AnimEventHandler(AnimEventID);
		}
	}
}
