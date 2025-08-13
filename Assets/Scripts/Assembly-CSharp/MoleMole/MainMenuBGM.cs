namespace MoleMole
{
	public class MainMenuBGM
	{
		private bool _mainMenuEntered;

		private string _currentWeatherName = "Default";

		public void InitAtAwakes()
		{
		}

		public void TryEnterMainMenu()
		{
			if (!_mainMenuEntered)
			{
				_mainMenuEntered = true;
				Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[3] { "BK_MainMenu", "BK_Mei_C1", "BK_Bronya_Common" });
				Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu");
			}
		}

		public void UpdateBGMByWeather(string weatherName)
		{
			if (_currentWeatherName == weatherName)
			{
				return;
			}
			if (!string.IsNullOrEmpty(weatherName))
			{
				if (weatherName == "Lightning")
				{
					Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Lightning");
				}
				else if (weatherName == "Cloudy")
				{
					Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Cloudy");
				}
				else
				{
					Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Default");
				}
			}
			_currentWeatherName = weatherName;
			if (_mainMenuEntered)
			{
				Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu_Stop");
				Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu");
			}
		}

		public void SetBGMSwitchByStage(bool isLast)
		{
			Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", (!isLast) ? "Default" : "Lightning");
		}

		public void TryExitMainMenu()
		{
			if (_mainMenuEntered)
			{
				Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu_Stop");
				Singleton<WwiseAudioManager>.Instance.PopSoundBankScale();
				_mainMenuEntered = false;
			}
		}
	}
}
