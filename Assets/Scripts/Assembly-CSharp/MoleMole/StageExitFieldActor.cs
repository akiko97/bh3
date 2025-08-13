namespace MoleMole
{
	public class StageExitFieldActor : TriggerFieldActor
	{
		protected MonoTriggerField _triggerField;

		public override void Init(BaseMonoEntity entity)
		{
			_triggerField = (MonoTriggerField)entity;
			runtimeID = _triggerField.GetRuntimeID();
			Singleton<EventManager>.Instance.FireEvent(new EvtStageTriggerCreated(runtimeID));
		}

		public override bool OnFieldEnter(EvtFieldEnter evt)
		{
			base.OnFieldEnter(evt);
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.otherID))
			{
				Kill();
			}
			return true;
		}

		public override void Kill()
		{
			_triggerField.SetDied();
			base.Kill();
		}
	}
}
