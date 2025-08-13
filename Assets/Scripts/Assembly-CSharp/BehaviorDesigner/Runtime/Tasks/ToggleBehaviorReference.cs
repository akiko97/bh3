namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("BehaviorTreeReferenceIcon.png")]
	[TaskDescription("Toggle Behavior Reference")]
	public class ToggleBehaviorReference : BehaviorReference
	{
		private static ExternalBehavior[] EMPTY = new ExternalBehavior[1];

		public ExternalBehavior OffBehavior;

		[Tooltip("If not toggled the referenced tree won't be loaded, this node act like it didn't exist. !!! the variable needs to be set before enabling the source for the first time !!!")]
		public SharedBool toggled;

		public override ExternalBehavior[] GetExternalBehaviors()
		{
			if (toggled.Value)
			{
				return base.GetExternalBehaviors();
			}
			EMPTY[0] = OffBehavior;
			return EMPTY;
		}
	}
}
