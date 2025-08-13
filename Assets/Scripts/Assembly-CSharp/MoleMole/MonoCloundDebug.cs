using MoleMole.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoCloundDebug : MonoBehaviour
	{
		public Text ProfileNameText;

		public Text CloudNameText;

		public Toggle StaticModeToggle;

		public Toggle FarPrefabVisibleToggle;

		public Toggle TimeSettingEnableToggle;

		public Slider TimeSlider;

		public Text TimeText;

		public Text WeatherText;

		private int _currentKey = -1;

		private MainMenuStage __stage;

		public Toggle cameraToggle;

		public Toggle cameraAutoToggle;

		public Toggle cloudToggle;

		public Toggle cloudAutoToggle;

		public Toggle spaceShipToggle;

		public Toggle spaceShipAutoToggle;

		private GameObject _cameraObj;

		private GameObject _cloudObj;

		private float period = 0.5f;

		private float timer;

		private MainMenuStage _stage
		{
			get
			{
				if (__stage == null)
				{
					__stage = Object.FindObjectOfType<MainMenuStage>();
				}
				return __stage;
			}
		}

		public void Update()
		{
			if (TimeSettingEnableToggle.isOn)
			{
				_stage.DayTime = TimeSlider.value;
			}
			else
			{
				_stage.DayTime = -1f;
			}
			CloudNameText.text = _stage.CloudSceneName;
			SetProfileName();
			if (_cameraObj == null)
			{
				_cameraObj = Object.FindObjectOfType<MainMenuCamera>().gameObject;
			}
			if (_cloudObj == null)
			{
				CloudEmitter cloudEmitter = Object.FindObjectOfType<CloudEmitter>();
				_cloudObj = ((!(cloudEmitter != null)) ? null : cloudEmitter.gameObject);
			}
			timer += Time.deltaTime;
			if (timer >= period)
			{
				timer = 0f;
				if (cameraAutoToggle.isOn && _cameraObj != null)
				{
					_cameraObj.SetActive(!_cameraObj.activeSelf);
				}
				if (cloudAutoToggle.isOn && _cloudObj != null)
				{
					_cloudObj.SetActive(!_cloudObj.activeSelf);
				}
				if (spaceShipAutoToggle.isOn && _stage != null)
				{
					_stage.gameObject.SetActive(!_stage.gameObject.activeSelf);
				}
			}
			WeatherInfo weatherInfo = Singleton<RealTimeWeatherManager>.Instance.GetWeatherInfo();
			if (WeatherText != null && weatherInfo != null)
			{
				WeatherText.text = weatherInfo.ToString();
			}
		}

		public void OnCloundBtnDebugBtnClick()
		{
			base.transform.gameObject.SetActive(!base.transform.gameObject.activeSelf);
			if (base.transform.gameObject.activeSelf)
			{
				SetProfileName();
				CloudNameText.text = _stage.CloudSceneName;
				TimeText.text = TimeSlider.value.ToString();
			}
		}

		public void OnEnableToggle()
		{
			_stage.UpdateAtmosphereWithTransition = !TimeSettingEnableToggle.isOn;
			_stage.ForceUpdateAtmosphere = true;
		}

		public void OnChangeProfile()
		{
			_stage.ChooseAtmosphereSeriesNext();
			_currentKey = -1;
		}

		public void OnChangeCloud()
		{
			_stage.ChooseCloudSceneNext();
		}

		public void OnTimeSliderDrag()
		{
			_stage.UpdateAtmosphereWithTransition = false;
			_stage.IsInTransition = false;
			TimeText.text = TimeSlider.value.ToString();
		}

		public void OnPrevButtonClick()
		{
			if (_currentKey != -1)
			{
				_currentKey--;
			}
			else
			{
				_currentKey = _stage.AtmosphereConfigSeries.KeyBeforeTime(TimeSlider.value);
			}
			if (_currentKey == -1)
			{
				_currentKey = _stage.AtmosphereConfigSeries.KeyCount() - 1;
			}
			TimeSlider.value = _stage.AtmosphereConfigSeries.TimeAtKey(_currentKey);
			_stage.UpdateAtmosphereWithTransition = true;
		}

		public void OnNextButtonClick()
		{
			if (_currentKey != -1)
			{
				_currentKey++;
			}
			else
			{
				_currentKey = _stage.AtmosphereConfigSeries.KeyBeforeTime(TimeSlider.value);
			}
			if (_currentKey == _stage.AtmosphereConfigSeries.KeyCount())
			{
				_currentKey = 0;
			}
			TimeSlider.value = _stage.AtmosphereConfigSeries.TimeAtKey(_currentKey);
			_stage.UpdateAtmosphereWithTransition = true;
		}

		public void OnStaticModeToggle()
		{
			GlobalVars.STATIC_CLOUD_MODE = StaticModeToggle.isOn;
		}

		public void OnBenchmarkToggle(bool isOn)
		{
			MonoBenchmarkSwitches monoBenchmarkSwitches = Object.FindObjectOfType<MonoBenchmarkSwitches>();
			if (monoBenchmarkSwitches == null)
			{
				if (isOn)
				{
					new GameObject("__benchmark", typeof(MonoBenchmarkSwitches));
				}
			}
			else
			{
				monoBenchmarkSwitches.gameObject.SetActive(isOn);
			}
		}

		public void OnFarPrefabVisibleToggle()
		{
			Transform[] componentsInChildren = _stage.GetComponentsInChildren<Transform>(true);
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.name == "Warship")
				{
					transform.gameObject.SetActive(FarPrefabVisibleToggle.isOn);
				}
			}
		}

		public void OnCameraToggle()
		{
			_cameraObj.SetActive(cameraToggle.isOn);
		}

		public void OnCloudToggle()
		{
			_cloudObj.SetActive(cloudToggle.isOn);
		}

		public void OnSpaceShipToggle()
		{
			_stage.gameObject.SetActive(spaceShipToggle.isOn);
		}

		private void SetProfileName()
		{
			string atmosphereConfigSeriesPath = _stage.AtmosphereConfigSeriesPath;
			ProfileNameText.text = atmosphereConfigSeriesPath.Substring(atmosphereConfigSeriesPath.LastIndexOf('/') + 1);
		}

		public void OnManualUpdateWeatherButtonClicked()
		{
			Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo(delegate
			{
				UIUtil.SpaceshipCheckWeather();
			});
		}
	}
}
