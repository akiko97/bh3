namespace MoleMole
{
	public class EvtLoadingState : BaseLevelEvent
	{
		public enum State
		{
			Start = 0,
			Destroy = 1
		}

		public readonly State state;

		public EvtLoadingState(State state)
		{
			this.state = state;
		}

		public override string ToString()
		{
			return string.Format("Loading state: {0}", state.ToString());
		}
	}
}
