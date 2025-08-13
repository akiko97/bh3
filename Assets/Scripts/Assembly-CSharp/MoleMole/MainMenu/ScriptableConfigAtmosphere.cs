using UnityEngine;

namespace MoleMole.MainMenu
{
	public class ScriptableConfigAtmosphere : ScriptableObject
	{
		[HideInInspector]
		public float FrameTime;

		public ConfigCloudStyle CloudStyle;

		public ConfigBackground Background;

		public ConfigIndoor Indoor;

		public ScriptableConfigAtmosphere()
		{
			Background = new ConfigBackground();
		}
	}
}
