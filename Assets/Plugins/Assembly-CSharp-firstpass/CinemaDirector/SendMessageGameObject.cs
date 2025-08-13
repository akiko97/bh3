using UnityEngine;

namespace CinemaDirector
{
	[CutsceneItem("Game Object", "Send Message", new CutsceneItemGenre[] { CutsceneItemGenre.ActorItem })]
	public class SendMessageGameObject : CinemaActorEvent
	{
		public string MethodName = string.Empty;

		public object Parameter;

		public string messageID = string.Empty;

		public SendMessageOptions SendMessageOptions = SendMessageOptions.DontRequireReceiver;

		public override void Trigger(GameObject actor)
		{
			if (actor != null)
			{
				actor.SendMessage(MethodName, messageID, SendMessageOptions);
			}
		}
	}
}
