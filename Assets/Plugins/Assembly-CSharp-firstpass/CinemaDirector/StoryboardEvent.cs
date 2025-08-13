using UnityEngine;

namespace CinemaDirector
{
	[CutsceneItem("Utility", "Storyboard", new CutsceneItemGenre[] { CutsceneItemGenre.GlobalItem })]
	public class StoryboardEvent : CinemaGlobalEvent
	{
		public string FolderName = "Storyboard";

		public static int Count;

		public override void Trigger()
		{
			Application.CaptureScreenshot(string.Format("Assets\\{0}{1}.png", base.gameObject.name, Count++));
		}
	}
}
