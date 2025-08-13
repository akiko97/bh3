using UnityEngine;

namespace MoleMole
{
	public class MonoSettingOtherTab : MonoBehaviour
	{
		public Transform realTimeWeatherSettingBtn;

		private bool _enableRealTimeWeather;

		public void SetupView()
		{
			RecoverOriginState();
		}

		public bool CheckNeedSave()
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather != _enableRealTimeWeather;
		}

		public void OnRealTimeChoiseBtnClicked(bool enable)
		{
			_enableRealTimeWeather = enable;
			UpdateView();
		}

		public void OnSaveBtnClick()
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather = _enableRealTimeWeather;
			Singleton<MiHoYoGameData>.Instance.Save();
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess")));
		}

		public void OnNoSaveBtnClick()
		{
			RecoverOriginState();
		}

		private void RecoverOriginState()
		{
			_enableRealTimeWeather = Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather;
			UpdateView();
		}

		private void UpdateView()
		{
			SetChoiceBtn(realTimeWeatherSettingBtn, _enableRealTimeWeather);
		}

		private void SetChoiceBtn(Transform btn, bool active)
		{
			if (!(btn == null))
			{
				Transform transform = btn.Find("Choice/On");
				Transform transform2 = btn.Find("Choice/Off");
				transform.gameObject.SetActive(active);
				transform2.gameObject.SetActive(!active);
			}
		}

		private void OnDestroy()
		{
			realTimeWeatherSettingBtn = null;
		}
	}
}
