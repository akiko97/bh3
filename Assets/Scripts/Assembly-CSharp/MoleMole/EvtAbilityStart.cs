namespace MoleMole
{
	public class EvtAbilityStart : BaseEvent
	{
		public string abilityID;

		public string abilityName;

		public uint otherID;

		public AttackResult.HitCollsion hitCollision;

		public object abilityArgument;

		public BaseEvent TriggerEvent;

		public EvtAbilityStart(uint casterID, BaseEvent triggerEvt = null)
			: base(casterID)
		{
			if (triggerEvt != null)
			{
				TriggerEvent = triggerEvt;
			}
		}

		public EvtAbilityStart(uint casterID, uint otherID, BaseEvent triggerEvt = null)
			: base(casterID)
		{
			this.otherID = otherID;
			if (triggerEvt != null)
			{
				TriggerEvent = triggerEvt;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} triggers special skill ID: <{1}>, name <{2}>, other <{3}>", GetDebugName(targetID), abilityID, abilityName, GetDebugName(otherID));
		}
	}
}
