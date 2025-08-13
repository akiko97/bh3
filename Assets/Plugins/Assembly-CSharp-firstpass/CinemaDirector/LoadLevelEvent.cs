using UnityEngine;
using UnityEngine.SceneManagement;

namespace CinemaDirector
{
	[CutsceneItem("Utility", "Load Level", new CutsceneItemGenre[] { CutsceneItemGenre.GlobalItem })]
	public class LoadLevelEvent : CinemaGlobalEvent
	{
		public enum LoadLevelArgument
		{
			ByIndex = 0,
			ByName = 1
		}

		public enum LoadLevelType
		{
			Standard = 0,
			Additive = 1,
			Async = 2,
			AdditiveAsync = 3
		}

		public LoadLevelArgument Argument;

		public LoadLevelType Type;

		public int Level;

		public string LevelName;

		public override void Trigger()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (Argument == LoadLevelArgument.ByIndex)
			{
				if (Type == LoadLevelType.Standard)
				{
					SceneManager.LoadScene(Level);
				}
				else if (Type == LoadLevelType.Additive)
				{
					SceneManager.LoadScene(Level);
				}
				else if (Type == LoadLevelType.Async)
				{
					SceneManager.LoadSceneAsync(Level);
				}
				else if (Type == LoadLevelType.AdditiveAsync)
				{
					SceneManager.LoadSceneAsync(Level);
				}
			}
			else if (Argument == LoadLevelArgument.ByName)
			{
				if (Type == LoadLevelType.Standard)
				{
					SceneManager.LoadScene(LevelName);
				}
				else if (Type == LoadLevelType.Additive)
				{
					SceneManager.LoadScene(LevelName);
				}
				else if (Type == LoadLevelType.Async)
				{
					SceneManager.LoadSceneAsync(LevelName);
				}
				else if (Type == LoadLevelType.AdditiveAsync)
				{
					SceneManager.LoadSceneAsync(LevelName);
				}
			}
		}
	}
}
