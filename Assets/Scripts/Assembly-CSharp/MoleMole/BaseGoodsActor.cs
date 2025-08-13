namespace MoleMole
{
	public abstract class BaseGoodsActor : BaseActor
	{
		protected MonoGoods _entity;

		public abstract void DoGoodsLogic(uint avatarRuntimeID);

		public override void Init(BaseMonoEntity entity)
		{
			_entity = entity as MonoGoods;
			runtimeID = _entity.GetRuntimeID();
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter)
			{
				return OnFieldEnter((EvtFieldEnter)evt);
			}
			return false;
		}

		private bool OnFieldEnter(EvtFieldEnter evt)
		{
			uint otherID = evt.otherID;
			DoGoodsLogic(otherID);
			return true;
		}

		protected void Kill()
		{
			_entity.SetDied();
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID));
		}
	}
}
