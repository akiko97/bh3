namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}SendEventIcon.png")]
	[TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=121")]
	public class SendEvent : Action
	{
		[Tooltip("The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
		public SharedGameObject targetGameObject;

		[Tooltip("The event to send")]
		public SharedString eventName;

		[Tooltip("The group of the behavior tree that the event should be sent to")]
		public SharedInt group;

		[SharedRequired]
		[Tooltip("Optionally specify a first argument to send")]
		public SharedVariable argument1;

		[SharedRequired]
		[Tooltip("Optionally specify a second argument to send")]
		public SharedVariable argument2;

		[Tooltip("Optionally specify a third argument to send")]
		[SharedRequired]
		public SharedVariable argument3;

		private BehaviorTree behaviorTree;

		public override void OnStart()
		{
			BehaviorTree[] components = GetDefaultGameObject(targetGameObject.Value).GetComponents<BehaviorTree>();
			if (components.Length == 1)
			{
				behaviorTree = components[0];
			}
			else
			{
				if (components.Length <= 1)
				{
					return;
				}
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].Group == group.Value)
					{
						behaviorTree = components[i];
						break;
					}
				}
				if (behaviorTree == null)
				{
					behaviorTree = components[0];
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (argument1 == null || argument1.IsNone)
			{
				behaviorTree.SendEvent(eventName.Value);
			}
			else if (argument2 == null || argument2.IsNone)
			{
				behaviorTree.SendEvent(eventName.Value, argument1.GetValue());
			}
			else if (argument3 == null || argument3.IsNone)
			{
				behaviorTree.SendEvent(eventName.Value, argument1.GetValue(), argument2.GetValue());
			}
			else
			{
				behaviorTree.SendEvent(eventName.Value, argument1.GetValue(), argument2.GetValue(), argument3.GetValue());
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			eventName = string.Empty;
		}
	}
}
