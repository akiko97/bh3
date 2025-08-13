using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoleMole
{
	public class DownloadPageContext : BasePageContext
	{
		private const int STEP = 2;

		private List<GameObject> _sceneGameObjects;

		private bool _destroyUntilNotify;

		public DownloadPageContext(bool destroyUntilNotify = false)
		{
			config = new ContextPattern
			{
				contextName = "DownLoadPageContext",
				viewPrefabPath = "UI/Menus/Page/Loading/DownloadPage"
			};
			_destroyUntilNotify = destroyUntilNotify;
			_sceneGameObjects = new List<GameObject>();
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
			}
			return false;
		}

		protected override bool SetupView()
		{
			SetupTips();
			Singleton<ApplicationManager>.Instance.StartCoroutine(LoadLevelWithProgress());
			return false;
		}

		public IEnumerator LoadLevelWithProgress()
		{
			string nextScene = Singleton<MainUIManager>.Instance.SceneAfterLoading;
			Singleton<MainUIManager>.Instance.ResetOnMoveToNextScene(nextScene);
			int displayProgress = 0;
			int toProgress = 0;
			AsyncOperation op = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
			op.allowSceneActivation = false;
			float targetProcess = 0.6f;
			bool targetProcessReach = false;
			while (op.progress < 0.9f)
			{
				toProgress = (int)op.progress * 100;
				if (!targetProcessReach && op.progress >= targetProcess)
				{
					targetProcessReach = true;
					Singleton<MainMenuBGM>.Instance.TryExitMainMenu();
				}
				while (displayProgress < toProgress)
				{
					displayProgress += 2;
					SetLoadingPercentage(displayProgress);
					yield return new WaitForEndOfFrame();
				}
			}
			while (displayProgress < toProgress)
			{
				displayProgress += 2;
				SetLoadingPercentage(displayProgress);
				yield return new WaitForEndOfFrame();
			}
			op.allowSceneActivation = true;
			if (_destroyUntilNotify)
			{
				Singleton<NotifyManager>.Instance.RegisterContext(this);
			}
			else
			{
				DestroyLoadingScene();
			}
			yield return Resources.UnloadUnusedAssets();
		}

		private void SetupTips()
		{
		}

		private void SetLoadingPercentage(int progress)
		{
			progress = Mathf.Clamp(progress, 0, 100);
			base.view.transform.Find("ProgressPanel/Num").GetComponent<Text>().text = progress + "%";
			base.view.transform.Find("ProgressPanel/ProgressBar").GetComponent<MonoMaskSlider>().UpdateValue(progress, 100f, 0f);
		}

		private void DestroyLoadingScene()
		{
			foreach (GameObject sceneGameObject in _sceneGameObjects)
			{
				Object.Destroy(sceneGameObject);
			}
			Singleton<NotifyManager>.Instance.RemoveContext(this);
		}
	}
}
