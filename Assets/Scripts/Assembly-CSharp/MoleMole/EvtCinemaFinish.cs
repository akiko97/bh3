using CinemaDirector;

namespace MoleMole
{
	public class EvtCinemaFinish : BaseEvent
	{
		private Cutscene _cutScene;

		public EvtCinemaFinish(uint targetID, Cutscene cutScene)
			: base(targetID)
		{
			_cutScene = cutScene;
		}

		public Cutscene GetCutscene()
		{
			return _cutScene;
		}

		public override string ToString()
		{
			return string.Format("{0}  Evt SlowMotionKillFinish", targetID);
		}
	}
}
