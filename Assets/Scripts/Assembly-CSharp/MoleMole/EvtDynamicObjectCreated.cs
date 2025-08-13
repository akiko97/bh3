namespace MoleMole
{
	public class EvtDynamicObjectCreated : BaseEvent
	{
		public readonly uint objectID;

		public BaseMonoDynamicObject.DynamicType dynamicType;

		public EvtDynamicObjectCreated(uint ownerID, uint objectID, BaseMonoDynamicObject.DynamicType dynamicType)
			: base(ownerID)
		{
			this.objectID = objectID;
			this.dynamicType = dynamicType;
		}

		public override string ToString()
		{
			return string.Format("{0} created {1}", GetDebugName(targetID), GetDebugName(objectID));
		}
	}
}
