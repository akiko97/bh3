namespace MoleMole.Config
{
	public class AnimatorEventMultiEventHandler : AnimatorEvent
	{
		public string MultiEventID;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.MultiAnimEventHandler(MultiEventID);
		}
	}
}
