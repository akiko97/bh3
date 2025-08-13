namespace MoleMole.Config
{
	public class AnimatorEventEffect : AnimatorEvent
	{
		public string EffectPatternName;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.TriggerEffectPattern(EffectPatternName);
		}
	}
}
