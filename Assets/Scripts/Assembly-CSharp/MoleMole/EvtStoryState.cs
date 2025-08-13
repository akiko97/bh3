namespace MoleMole
{
	public class EvtStoryState : BaseEvent
	{
		public enum State
		{
			None = 0,
			Begin = 1,
			Finish = 2
		}

		private State _state;

		public State StoryState
		{
			get
			{
				return _state;
			}
		}

		public EvtStoryState(uint targetID, State state)
			: base(targetID)
		{
			_state = state;
		}

		public override string ToString()
		{
			return string.Format("{0}  Evt Story State : {1}", targetID, _state.ToString());
		}
	}
}
