using CinemaDirector;

namespace MoleMole
{
	public class LDWaitCinemaFinish : BaseLDEvent
	{
		private Cutscene _cinema;

		public LDWaitCinemaFinish(Cutscene cinema)
		{
			_cinema = cinema;
		}

		public override void OnEvent(BaseEvent evt)
		{
			EvtCinemaFinish evtCinemaFinish = evt as EvtCinemaFinish;
			if (evtCinemaFinish != null && evtCinemaFinish.GetCutscene() == _cinema)
			{
				Done();
			}
		}
	}
}
