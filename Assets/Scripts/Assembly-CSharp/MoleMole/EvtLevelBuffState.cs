namespace MoleMole
{
	public class EvtLevelBuffState : BaseLevelEvent
	{
		public LevelBuffType levelBuff;

		public LevelBuffState state;

		public LevelBuffSide side;

		public uint sourceId;

		public EvtLevelBuffState(LevelBuffType levelBuff, LevelBuffState state, LevelBuffSide side, uint ownerRuntimeID)
		{
			this.levelBuff = levelBuff;
			this.state = state;
			this.side = side;
			sourceId = ownerRuntimeID;
		}

		public override string ToString()
		{
			return string.Format("LevelBuff <{0}> state: <{1}>, side <{2}> sourceId <{3}>", levelBuff, state, side, sourceId);
		}
	}
}
