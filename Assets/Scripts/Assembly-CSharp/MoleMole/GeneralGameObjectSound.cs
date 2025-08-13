using UnityEngine;

namespace MoleMole
{
	public class GeneralGameObjectSound : MonoBehaviour
	{
		public string enterEventName;

		public string exitEventName;

		private void OnEnable()
		{
			PlayPatterns(enterEventName);
		}

		private void OnDisable()
		{
			PlayPatterns(exitEventName);
		}

		[AnimationCallback]
		private void TriggerAudioPattern(string name)
		{
			PlayPatterns(name);
		}

		private void PlayPatterns(string content)
		{
			string[] array = content.Split(';');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					Singleton<WwiseAudioManager>.Instance.Post(array[i]);
				}
			}
		}
	}
}
