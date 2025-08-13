namespace MoleMole
{
	public class EvtCamearaAnimState : BaseEvent
	{
		public enum State
		{
			None = 0,
			Begin = 1,
			Finish = 2
		}

		private State _state;

		public State CameraAnimState
		{
			get
			{
				return _state;
			}
		}

		public EvtCamearaAnimState(uint targetID, State state)
			: base(targetID)
		{
			_state = state;
		}

		public override string ToString()
		{
			return string.Format("{0}  Evt Camera Anim State : {1}", targetID, _state.ToString());
		}
	}
}
