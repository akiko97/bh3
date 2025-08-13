using MoleMole.Config;
using MoleMole.MainMenu;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class SpaceShipModelContext : BaseWidgetContext
	{
		private const string THUNDER_WEATHER_CONFIG_PATH = "Rendering/MainMenuAtmosphereConfig/Lightning";

		private const int THUNDER_WEATHER_SCENE_ID = 0;

		public readonly GameObject uiMainCamera;

		public SpaceShipModelContext(GameObject view, GameObject uiMainCamera)
		{
			config = new ContextPattern
			{
				contextName = "SpaceShipModelContext",
				viewPrefabPath = "Stage/MainMenu_SpaceShip/MainMenu_SpaceShip"
			};
			base.view = view;
			this.uiMainCamera = uiMainCamera;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SetSpaceShipActive)
			{
				return SetSpaceShipActive(((Tuple<bool, bool>)ntf.body).Item1, ((Tuple<bool, bool>)ntf.body).Item2);
			}
			if (ntf.type == NotifyTypes.ShowThunderWeather)
			{
				return OnShowThunderWeatherNotify();
			}
			if (ntf.type == NotifyTypes.ShowRandomWeather)
			{
				return OnShowRandomWeatherNotify();
			}
			if (ntf.type == NotifyTypes.ShowDefaultWeather)
			{
				return OnShowDefaultWeatherNotify();
			}
			if (ntf.type == NotifyTypes.SetSpaceShipLight)
			{
				return SetSpaceShipLight((bool)ntf.body);
			}
			return false;
		}

		protected override bool SetupView()
		{
			return false;
		}

		private bool SetSpaceShipActive(bool active, bool setCameraComponentOnly = false)
		{
			if (uiMainCamera == null || base.view == null)
			{
				return false;
			}
			if (!active)
			{
				if (setCameraComponentOnly)
				{
					uiMainCamera.gameObject.SetActive(true);
					uiMainCamera.GetComponent<Camera>().enabled = false;
				}
				else
				{
					uiMainCamera.gameObject.SetActive(false);
				}
			}
			else
			{
				uiMainCamera.GetComponent<Camera>().enabled = true;
				uiMainCamera.gameObject.SetActive(true);
			}
			base.view.SetActive(active);
			return false;
		}

		private bool SetSpaceShipLight(bool isGalTouch)
		{
			if (base.view != null && base.view.transform.Find("DirLight").GetComponent<MainMenuLight>() != null)
			{
				base.view.transform.Find("DirLight").GetComponent<MainMenuLight>().mode = (isGalTouch ? MainMenuLight.Mode.Fixed : MainMenuLight.Mode.Section);
			}
			return false;
		}

		private bool OnShowThunderWeatherNotify()
		{
			MainMenuStage component = base.view.transform.GetComponent<MainMenuStage>();
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach("Rendering/MainMenuAtmosphereConfig/Lightning");
			component.ChooseCloudScene(configAtmosphereSeries, 0);
			UserLocalDataItem localData = Singleton<MiHoYoGameData>.Instance.LocalData;
			localData.StartDirtyCheck();
			if (localData.CurrentWeatherConfigPath != "Rendering/MainMenuAtmosphereConfig/Lightning" || localData.CurrentWeatherSceneID != 0)
			{
				localData.SetDirty();
			}
			localData.CurrentWeatherConfigPath = "Rendering/MainMenuAtmosphereConfig/Lightning";
			localData.CurrentWeatherSceneID = 0;
			if (localData.EndDirtyCheck())
			{
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private bool OnShowRandomWeatherNotify()
		{
			string pathRandomly = AtmosphereSeriesData.GetPathRandomly();
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(pathRandomly);
			int sceneIdRandomly = configAtmosphereSeries.GetSceneIdRandomly();
			base.view.transform.GetComponent<MainMenuStage>().ChooseCloudScene(configAtmosphereSeries, sceneIdRandomly);
			UserLocalDataItem localData = Singleton<MiHoYoGameData>.Instance.LocalData;
			localData.StartDirtyCheck();
			if (localData.CurrentWeatherConfigPath != pathRandomly || localData.CurrentWeatherSceneID != sceneIdRandomly)
			{
				localData.SetDirty();
			}
			localData.CurrentWeatherConfigPath = pathRandomly;
			localData.CurrentWeatherSceneID = sceneIdRandomly;
			if (localData.EndDirtyCheck())
			{
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private bool OnShowDefaultWeatherNotify()
		{
			string currentWeatherConfigPath = Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherConfigPath;
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(currentWeatherConfigPath);
			int currentWeatherSceneID = Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherSceneID;
			base.view.transform.GetComponent<MainMenuStage>().ChooseCloudScene(configAtmosphereSeries, currentWeatherSceneID);
			return false;
		}
	}
}
