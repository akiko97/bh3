using System;

namespace MoleMole
{
	public class LDEvtWaitLevelDefendState : BaseLDEvent
	{
		public DefendModeType defendModeType;

		public int targetValue;

		public int uniqueID;

		public LDEvtWaitLevelDefendState(string typeStr, double value)
		{
			defendModeType = (DefendModeType)(int)Enum.Parse(typeof(DefendModeType), typeStr);
			if (defendModeType == DefendModeType.Single || defendModeType == DefendModeType.Group)
			{
				targetValue = (int)value;
				uniqueID = 0;
			}
			else if (defendModeType == DefendModeType.Certain)
			{
				targetValue = 0;
				uniqueID = (int)value;
			}
			else
			{
				targetValue = (int)value;
				uniqueID = 0;
			}
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (!(evt is EvtLevelDefendState))
			{
				return;
			}
			EvtLevelDefendState evtLevelDefendState = evt as EvtLevelDefendState;
			if (evtLevelDefendState.modeType != defendModeType)
			{
				return;
			}
			if (evtLevelDefendState.modeType == DefendModeType.Certain)
			{
				if (uniqueID == evtLevelDefendState.uniqueID)
				{
					Done();
				}
			}
			else if (evtLevelDefendState.modeType == DefendModeType.Result)
			{
				if (targetValue == evtLevelDefendState.targetValue)
				{
					Done();
				}
			}
			else if (targetValue == evtLevelDefendState.targetValue)
			{
				Done();
			}
		}
	}
}
