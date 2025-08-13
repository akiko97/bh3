using CinemaDirector;

namespace MoleMole
{
	public class LDWaitCinemaReceiveMessage : BaseLDEvent
	{
		private Cutscene _cinema;

		private string _messageID = string.Empty;

		public LDWaitCinemaReceiveMessage(Cutscene cinema, string messageID)
		{
			_cinema = cinema;
			_messageID = messageID;
		}

		public override void OnEvent(BaseEvent evt)
		{
			EvtCinemaReceiveMessage evtCinemaReceiveMessage = evt as EvtCinemaReceiveMessage;
			if (evtCinemaReceiveMessage != null && evtCinemaReceiveMessage.GetCutscene() == _cinema && evtCinemaReceiveMessage.GetMessageID() == _messageID)
			{
				Done();
			}
		}
	}
}
