using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoleMole
{
	public class LoadingPageContext : BasePageContext
	{
		private List<GameObject> _sceneGameObjects;

		private bool _destroyUntilNotify;

		public LoadingPageContext(bool destroyUntilNotify = false)
		{
			config = new ContextPattern
			{
				contextName = "LoadingPageContext",
				viewPrefabPath = "UI/Menus/Page/Loading/LoadingPage"
			};
			_destroyUntilNotify = destroyUntilNotify;
			_sceneGameObjects = new List<GameObject>();
			uiType = UIType.Page;
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			_sceneGameObjects.Add(sceneCanvas.gameObject);
			_sceneGameObjects.Add(sceneCanvas.GetComponent<Canvas>().worldCamera.gameObject);
		}

		protected override void BindViewCallbacks()
		{
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.DestroyLoadingScene && _destroyUntilNotify)
			{
				DestroyLoadingScene();
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.LoadingSceneDestroyed));
			}
			return false;
		}

		protected override bool SetupView()
		{
			SetupTips();
			LoadLevelWithProgress();
			return false;
		}

		public void LoadLevelWithProgress()
		{
			string sceneAfterLoading = Singleton<MainUIManager>.Instance.SceneAfterLoading;
			Singleton<MainUIManager>.Instance.ResetOnMoveToNextScene(sceneAfterLoading);
			SceneManager.LoadScene(sceneAfterLoading, LoadSceneMode.Additive);
			if (Singleton<MainUIManager>.Instance.SceneAfterLoading == "TestLevel01")
			{
				Singleton<MainMenuBGM>.Instance.TryExitMainMenu();
			}
			if (_destroyUntilNotify)
			{
				Singleton<NotifyManager>.Instance.RegisterContext(this);
			}
			else
			{
				DestroyLoadingScene();
			}
		}

		private void SetupTips()
		{
			if (Singleton<MainUIManager>.Instance.bShowLoadingTips)
			{
				base.view.transform.Find("Tips/Content").GetComponent<Text>().text = GetRandomLoadingTip();
			}
			else
			{
				base.view.transform.Find("Tips").gameObject.SetActive(false);
			}
		}

		private void DestroyLoadingScene()
		{
			foreach (GameObject sceneGameObject in _sceneGameObjects)
			{
				Object.Destroy(sceneGameObject);
			}
			Object.Destroy(base.view);
			Singleton<NotifyManager>.Instance.RemoveContext(this);
			Resources.UnloadUnusedAssets();
		}

		private string GetRandomLoadingTip()
		{
			int num = 100;
			int num2 = 0;
			string empty = string.Empty;
			do
			{
				num2 = Random.Range(1, num);
				empty = LocalizationGeneralLogic.GetText("LoadingTips_" + num2.ToString("D2"));
				num = num2;
			}
			while (num > 1 && string.IsNullOrEmpty(empty));
			return empty;
		}
	}
}
