namespace MoleMole
{
	public class EvtLevelDefendState : BaseLevelEvent
	{
		public readonly DefendModeType modeType;

		public readonly int targetValue;

		public readonly int uniqueID;

		public EvtLevelDefendState(DefendModeType defendModeType, int targetValue)
		{
			modeType = defendModeType;
			this.targetValue = targetValue;
			uniqueID = 0;
		}

		public EvtLevelDefendState(int uniqueID)
		{
			modeType = DefendModeType.Certain;
			targetValue = 0;
			this.uniqueID = uniqueID;
		}

		public override string ToString()
		{
			if (modeType == DefendModeType.Single || modeType == DefendModeType.Group)
			{
				return string.Format("DefendMode : {0}. Target Value : {1}", modeType.ToString(), targetValue);
			}
			if (modeType == DefendModeType.Certain)
			{
				return string.Format("DefendMode : {0}. Target monster tag id : {1}", modeType.ToString(), uniqueID.ToString());
			}
			return string.Format("DefendMode : {0}. Defend Target Value : {1}", modeType.ToString(), targetValue);
		}
	}
}
