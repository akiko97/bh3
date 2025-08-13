using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class WwiseController : MonoBehaviour
	{
		private string _eventName;

		private string _stateMachineName;

		private string _switchGroupName;

		private string _switchName;

		public Text eventNameText;

		public Text stateGroupNameText;

		public Text stateNameText;

		public Text switchGroupNameText;

		public Text switchNameText;

		public Text paramNameText;

		private void Awake()
		{
			Singleton<WwiseAudioManager>.Create();
			Singleton<WwiseAudioManager>.Instance.InitAtAwake();
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[2] { "MainMenuBank", "Avatar_Generic_Bank" });
		}

		public void OnPlayButton()
		{
			Singleton<WwiseAudioManager>.Instance.Post(eventNameText.text);
		}

		public void OnSetStateButton()
		{
			Singleton<WwiseAudioManager>.Instance.SetState(stateGroupNameText.text, stateNameText.text);
		}

		public void OnSlideValueChanged(float val)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam(paramNameText.text, val);
		}

		public void OnSwitchButton()
		{
			Singleton<WwiseAudioManager>.Instance.SetSwitch(switchGroupNameText.text, switchNameText.text);
		}

		public void OnUnloadBank()
		{
		}
	}
}
