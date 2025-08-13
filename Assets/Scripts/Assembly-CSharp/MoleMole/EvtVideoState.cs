namespace MoleMole
{
	public class EvtVideoState : BaseEvent
	{
		public enum State
		{
			None = 0,
			Begin = 1,
			Finish = 2
		}

		private State _state;

		public State VideoState
		{
			get
			{
				return _state;
			}
		}

		public EvtVideoState(uint targetID, State state)
			: base(targetID)
		{
			_state = state;
		}

		public override string ToString()
		{
			return string.Format("{0}  Evt Video State : {1}", targetID, _state.ToString());
		}
	}
}
