namespace MoleMole
{
	public class EvtTutorialState : BaseLevelEvent
	{
		public enum State
		{
			Start = 0
		}

		public readonly State state;

		public EvtTutorialState(State state)
		{
			this.state = state;
		}

		public override string ToString()
		{
			return string.Format("level state: {0}", state.ToString());
		}
	}
}
