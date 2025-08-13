using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoleMole
{
	public class MonoShaderContainer : MonoBehaviour
	{
		[Header("!!! THIS PREFAB NEEDS TO BE IN GAME ENTRY SCENE && CAN NOT BE PUT IN 'RESOURCES' DIRECTORY !!!")]
		public Shader[] shaders;

		[Header("Tick this to go to game entry directly on awake")]
		public bool GoToGameEntry;

		private void Awake()
		{
			Object.DontDestroyOnLoad(this);
			GraphicsUtils.WarmupAllShaders();
			if (GoToGameEntry)
			{
				SceneManager.LoadScene("GameEntry");
			}
		}
	}
}
