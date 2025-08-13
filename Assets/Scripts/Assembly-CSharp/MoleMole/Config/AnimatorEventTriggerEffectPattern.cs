using FullInspector;

namespace MoleMole.Config
{
	public class AnimatorEventTriggerEffectPattern : AnimatorEvent
	{
		public string EffectPatternName;

		[InspectorNullable]
		public string Predicate1;

		[InspectorNullable]
		public string Predicate2;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			entity.TriggerEffectPattern(EffectPatternName, Predicate1, Predicate2);
		}
	}
}
