using UnityEngine;

namespace MoleMole
{
	public class AmbientSound : MonoBehaviour
	{
		public string enterEventName;

		public string exitEventName;

		private void Awake()
		{
			if (!string.IsNullOrEmpty(enterEventName))
			{
				Singleton<WwiseAudioManager>.Instance.Post(enterEventName);
			}
		}

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(exitEventName))
			{
				Singleton<WwiseAudioManager>.Instance.Post(exitEventName);
			}
		}
	}
}
