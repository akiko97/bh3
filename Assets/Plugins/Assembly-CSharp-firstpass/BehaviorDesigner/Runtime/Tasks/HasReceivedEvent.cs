namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
	[TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=123")]
	public class HasReceivedEvent : Conditional
	{
		[Tooltip("The name of the event to receive")]
		public SharedString eventName = string.Empty;

		[Tooltip("Optionally store the first sent argument")]
		[SharedRequired]
		public SharedVariable storedValue1;

		[Tooltip("Optionally store the second sent argument")]
		[SharedRequired]
		public SharedVariable storedValue2;

		[SharedRequired]
		[Tooltip("Optionally store the third sent argument")]
		public SharedVariable storedValue3;

		private bool eventReceived;

		public override void OnAwake()
		{
			base.Owner.RegisterEvent(eventName.Value, ReceivedEvent);
			base.Owner.RegisterEvent<object>(eventName.Value, ReceivedEvent);
			base.Owner.RegisterEvent<object, object>(eventName.Value, ReceivedEvent);
			base.Owner.RegisterEvent<object, object, object>(eventName.Value, ReceivedEvent);
		}

		public override TaskStatus OnUpdate()
		{
			return (!eventReceived) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnEnd()
		{
			eventReceived = false;
		}

		private void ReceivedEvent()
		{
			eventReceived = true;
		}

		private void ReceivedEvent(object arg1)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.IsNone)
			{
				storedValue1.SetValue(arg1);
			}
		}

		private void ReceivedEvent(object arg1, object arg2)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.IsNone)
			{
				storedValue1.SetValue(arg1);
			}
			if (storedValue2 != null && !storedValue2.IsNone)
			{
				storedValue2.SetValue(arg2);
			}
		}

		private void ReceivedEvent(object arg1, object arg2, object arg3)
		{
			ReceivedEvent();
			if (storedValue1 != null && !storedValue1.IsNone)
			{
				storedValue1.SetValue(arg1);
			}
			if (storedValue2 != null && !storedValue2.IsNone)
			{
				storedValue2.SetValue(arg2);
			}
			if (storedValue3 != null && !storedValue3.IsNone)
			{
				storedValue3.SetValue(arg3);
			}
		}

		public override void OnBehaviorComplete()
		{
			base.Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
			base.Owner.UnregisterEvent<object>(eventName.Value, ReceivedEvent);
			base.Owner.UnregisterEvent<object, object>(eventName.Value, ReceivedEvent);
			base.Owner.UnregisterEvent<object, object, object>(eventName.Value, ReceivedEvent);
		}

		public override void OnReset()
		{
			eventName = string.Empty;
		}
	}
}
