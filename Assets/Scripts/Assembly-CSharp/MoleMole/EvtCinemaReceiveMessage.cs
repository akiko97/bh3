using CinemaDirector;

namespace MoleMole
{
	public class EvtCinemaReceiveMessage : BaseEvent
	{
		private Cutscene _cutScene;

		private string _messageID = string.Empty;

		public EvtCinemaReceiveMessage(uint targetID, Cutscene cutScene, string messageID)
			: base(targetID)
		{
			_cutScene = cutScene;
			_messageID = messageID;
		}

		public Cutscene GetCutscene()
		{
			return _cutScene;
		}

		public string GetMessageID()
		{
			return _messageID;
		}

		public override string ToString()
		{
			return string.Format("{0}  Evt SlowMotionKillFinish", targetID);
		}
	}
}
