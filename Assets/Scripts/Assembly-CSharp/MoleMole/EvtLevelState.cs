namespace MoleMole
{
	public class EvtLevelState : BaseLevelEvent
	{
		public enum State
		{
			Start = 0,
			EndWin = 1,
			EndLose = 2,
			EnterTransition = 3,
			ExitTransition = 4,
			PostStageReady = 5
		}

		public enum LevelEndReason
		{
			EndUncertainReason = 0,
			EndWin = 1,
			EndLoseNotMeetCondition = 2,
			EndLoseAllDead = 3,
			EndLoseQuit = 4
		}

		public readonly State state;

		public readonly int cgId;

		public readonly LevelEndReason levelEndReason;

		public readonly string endReason;

		public EvtLevelState(State state, LevelEndReason reason = LevelEndReason.EndUncertainReason, int cgId = 0)
		{
			this.state = state;
			levelEndReason = reason;
			this.cgId = cgId;
		}

		public override string ToString()
		{
			return string.Format("level state: {0} with reason : {1} (with cgID {2})", state.ToString(), levelEndReason.ToString(), cgId.ToString());
		}
	}
}
