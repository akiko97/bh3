namespace MoleMole
{
	public class EvadeEntityDummy : BaseActor
	{
		private BaseMonoDynamicObject _dynamicObject;

		public override void Init(BaseMonoEntity entity)
		{
			_dynamicObject = (BaseMonoDynamicObject)entity;
			runtimeID = _dynamicObject.GetRuntimeID();
		}

		public void Setup(uint ownerID)
		{
			base.ownerID = ownerID;
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (!_dynamicObject.IsActive())
			{
				return true;
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtEvadeSuccess(ownerID, evt.sourceID, evt.animEventID, evt.attackData), MPEventDispatchMode.CheckRemoteMode);
			Kill();
			return true;
		}

		public void Kill()
		{
			_dynamicObject.SetDied();
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID));
		}
	}
}
