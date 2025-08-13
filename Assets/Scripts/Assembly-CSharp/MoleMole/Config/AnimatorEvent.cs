namespace MoleMole.Config
{
	public abstract class AnimatorEvent
	{
		public float normalizedTime;

		public bool forceTrigger;

		public bool forceTriggerOnTransitionOut;

		public abstract void HandleAnimatorEvent(BaseMonoAnimatorEntity entity);

		public AnimatorEvent Clone()
		{
			return MemberwiseClone() as AnimatorEvent;
		}
	}
}
