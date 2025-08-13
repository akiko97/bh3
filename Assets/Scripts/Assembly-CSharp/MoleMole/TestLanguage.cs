using UnityEngine;

namespace MoleMole
{
	public class TestLanguage : MonoBehaviour
	{
		private string language = string.Empty;

		private string[] targetLanguages = new string[2] { "Chinese(PRC)", "Japanese" };

		private int curLangIndex;

		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
		}

		private void OnGUI()
		{
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			GUILayout.Label("=");
			string text = Singleton<WwiseAudioManager>.Instance.GetLanguage();
			if (GUILayout.Button("Lang : " + ((text != null) ? text : "<null>")))
			{
				curLangIndex = (curLangIndex + 1) % targetLanguages.Length;
				Singleton<WwiseAudioManager>.Instance.SetLanguage(targetLanguages[curLangIndex]);
			}
			GUILayout.Label("====================================================");
			language = GUILayout.TextField(language);
			if (GUILayout.Button("Set"))
			{
				Singleton<WwiseAudioManager>.Instance.SetLanguage(language);
			}
		}
	}
}
