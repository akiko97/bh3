using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoleMole
{
	public class MainUIManager
	{
		private const string UI_EFFECT_CONTAINER_TAG = "UIEffectContainer";

		private Stack<BasePageContext> _pageContextStackStash;

		private Stack<BasePageContext> _pageContextStack;

		private EmptyPageContext _emtryPageContext;

		private int _loadPageDialogTimes;

		public bool spaceShipVisibleOnPreviesPage;

		private bool _useViewCache;

		private ViewCache _pageViewCache;

		private ViewCache _dialogViewCache;

		private ViewCache _widgetViewCache;

		private Dictionary<string, Action> _sceneLoadCallBackDict;

		private Dictionary<Tuple<string, string>, GameObject> _uiEffectContainerDict;

		private Coroutine _waitMoveToSceneCoroutine;

		public BaseMonoCanvas SceneCanvas { get; private set; }

		public BasePageContext CurrentPageContext
		{
			get
			{
				if (_pageContextStack != null && _pageContextStack.Count > 0)
				{
					return _pageContextStack.Peek();
				}
				return _emtryPageContext;
			}
		}

		public string SceneAfterLoading { get; private set; }

		public bool bDestroyUntilNotify { get; private set; }

		public bool bShowLoadingTips { get; private set; }

		private MainUIManager()
		{
			bDestroyUntilNotify = true;
			bShowLoadingTips = true;
			_pageContextStack = new Stack<BasePageContext>();
			_pageContextStackStash = new Stack<BasePageContext>();
			_emtryPageContext = new EmptyPageContext();
			_useViewCache = MainUIData.USE_VIEW_CACHING;
			if (_useViewCache)
			{
				_pageViewCache = new ViewCache(10);
				_dialogViewCache = new ViewCache(10);
				_widgetViewCache = new ViewCache(5);
			}
			_uiEffectContainerDict = new Dictionary<Tuple<string, string>, GameObject>();
			_sceneLoadCallBackDict = new Dictionary<string, Action>();
		}

		private GameObject LoadAndInstantiateView(string path)
		{
			GameObject original = Miscs.LoadResource<GameObject>(path);
			return UnityEngine.Object.Instantiate(original);
		}

		public GameObject LoadInstancedView(BaseContext context)
		{
			ContextPattern config = context.config;
			if (!_useViewCache)
			{
				return LoadAndInstantiateView(config.viewPrefabPath);
			}
			GameObject gameObject = null;
			switch (context.uiType)
			{
			case UIType.Page:
				gameObject = _pageViewCache.LoadInstancedView(config);
				break;
			case UIType.Dialog:
				gameObject = _dialogViewCache.LoadInstancedView(config);
				break;
			default:
				gameObject = _widgetViewCache.LoadInstancedView(config);
				break;
			}
			SuperDebug.VeryImportantAssert(gameObject != null, "failed to create view for: " + config.viewPrefabPath);
			return gameObject;
		}

		public void ReleaseInstancedView(BaseContext context)
		{
			ContextPattern config = context.config;
			GameObject view = context.view;
			if (view == null)
			{
				return;
			}
			if (!_useViewCache)
			{
				if (view != null && !string.IsNullOrEmpty(config.viewPrefabPath) && !config.dontDestroyView)
				{
					UnityEngine.Object.Destroy(view);
				}
				return;
			}
			switch (context.uiType)
			{
			case UIType.Page:
				_pageViewCache.ReleaseInstancedView(view, config);
				break;
			case UIType.Dialog:
				_dialogViewCache.ReleaseInstancedView(view, config);
				break;
			default:
				_widgetViewCache.ReleaseInstancedView(view, config);
				break;
			}
		}

		public void ClearViewCache()
		{
			if (_useViewCache)
			{
				_pageViewCache.ClearLRUCache();
				_dialogViewCache.ClearLRUCache();
				_widgetViewCache.ClearLRUCache();
			}
		}

		public void ResetViewCache()
		{
			if (_useViewCache)
			{
				_pageViewCache.Reset();
				_dialogViewCache.Reset();
				_widgetViewCache.Reset();
			}
		}

		public void SetMainCanvas(BaseMonoCanvas canvas)
		{
			SceneCanvas = canvas;
			ResetViewCache();
		}

		public BaseMonoCanvas GetMainCanvas()
		{
			return SceneCanvas;
		}

		public MonoInLevelUICanvas GetInLevelUICanvas()
		{
			return SceneCanvas as MonoInLevelUICanvas;
		}

		public void ShowDialog(BaseDialogContext dialogContext, UIType uiType = UIType.Any)
		{
			Transform uIHolder = GetUIHolder((uiType != UIType.Any) ? uiType : dialogContext.uiType);
			if (_pageContextStack.Count > 0)
			{
				BasePageContext basePageContext = _pageContextStack.Peek();
				basePageContext.dialogContextList.Add(dialogContext);
				dialogContext.pageContext = basePageContext;
			}
			else
			{
				_emtryPageContext.dialogContextList.Add(dialogContext);
				dialogContext.pageContext = _emtryPageContext;
			}
			dialogContext.StartUp(SceneCanvas.transform, uIHolder);
		}

		public void ShowPage(BasePageContext context, UIType uiType = UIType.Page)
		{
			if (_pageContextStack.Count > 0)
			{
				BasePageContext basePageContext = _pageContextStack.Peek();
				basePageContext.SetActive(false);
				spaceShipVisibleOnPreviesPage = basePageContext.spaceShipVisible();
			}
			else
			{
				_emtryPageContext.SetActive(false);
			}
			_pageContextStack.Push(context);
			context.StartUp(SceneCanvas.transform, GetUIHolder(uiType));
		}

		public void ShowWidget(BaseWidgetContext widget, UIType uiType = UIType.Any)
		{
			widget.StartUp(SceneCanvas.transform, GetUIHolder((uiType != UIType.Any) ? uiType : widget.uiType));
		}

		public void BackPage()
		{
			if (_pageContextStack.Count == 0 || (_pageContextStack.Count == 1 && _pageContextStack.Peek() is MainPageContext))
			{
				return;
			}
			BasePageContext basePageContext = _pageContextStack.Pop();
			spaceShipVisibleOnPreviesPage = basePageContext.spaceShipVisible();
			basePageContext.Destroy();
			if (_pageContextStack.Count > 0)
			{
				BasePageContext basePageContext2 = _pageContextStack.Peek();
				if (basePageContext2.view != null)
				{
					basePageContext2.SetActive(true);
					basePageContext2.OnLandedFromBackPage();
				}
				else
				{
					basePageContext2.StartUp(SceneCanvas.transform);
				}
				if (basePageContext2.spaceShipVisible())
				{
					UIUtil.SpaceshipCheckWeather();
				}
			}
			else
			{
				_emtryPageContext.SetActive(true);
			}
		}

		public void BackToMainMenuPage()
		{
			if (SceneCanvas is MonoIslandUICanvas)
			{
				MoveToNextScene("MainMenuWithSpaceship", false, true);
			}
			else if (!BackPageTo("MainPageContext"))
			{
				_pageContextStack.Clear();
				_pageContextStackStash.Clear();
				_emtryPageContext.Destroy();
				ShowPage(new MainPageContext());
			}
		}

		public void PopTopPageOnly()
		{
			if (_pageContextStack.Count > 0)
			{
				BasePageContext basePageContext = _pageContextStack.Pop();
				basePageContext.DestroyContextOnly();
				spaceShipVisibleOnPreviesPage = basePageContext.spaceShipVisible();
			}
		}

		public bool BackPageTo(string contextName)
		{
			if (_pageContextStack.Count <= 0)
			{
				SuperDebug.VeryImportantError("The page stack is empty!!!");
				return false;
			}
			if (contextName != "MainPageContext")
			{
				bool flag = false;
				foreach (BasePageContext item in _pageContextStack)
				{
					if (item.config.contextName == contextName)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					SuperDebug.VeryImportantError("Can't find page in stack: " + contextName);
					return false;
				}
			}
			BasePageContext basePageContext = _pageContextStack.Peek();
			spaceShipVisibleOnPreviesPage = basePageContext.spaceShipVisible();
			while (_pageContextStack.Count > 0 && _pageContextStack.Peek().config.contextName != contextName)
			{
				BaseContext baseContext = _pageContextStack.Pop();
				baseContext.Destroy();
			}
			if (_pageContextStack.Count > 0)
			{
				BasePageContext basePageContext2 = _pageContextStack.Peek();
				if (basePageContext2.view != null)
				{
					basePageContext2.SetActive(true);
					basePageContext2.OnLandedFromBackPage();
				}
				else
				{
					basePageContext2.StartUp(SceneCanvas.transform);
				}
			}
			else
			{
				_pageContextStackStash.Clear();
				_emtryPageContext.Destroy();
				ShowPage(new MainPageContext());
			}
			return true;
		}

		public bool HasContextInStash()
		{
			return _pageContextStackStash.Count > 0;
		}

		public void CreateContextFromStash()
		{
			if (HasContextInStash())
			{
				_pageContextStack = _pageContextStackStash;
				_pageContextStackStash = new Stack<BasePageContext>();
				BaseContext baseContext = _pageContextStack.Peek();
				baseContext.StartUp(SceneCanvas.transform);
			}
		}

		public void MoveToNextScene(string sceneName, bool toKeepContextStack = false, bool isAsync = false, bool destroyUntilNotify = true, Action onSceneLoadedCallBack = null, bool showLoadingTips = true)
		{
			_sceneLoadCallBackDict[sceneName] = onSceneLoadedCallBack;
			if (_waitMoveToSceneCoroutine == null)
			{
				if (isAsync)
				{
					SceneAfterLoading = sceneName;
					bDestroyUntilNotify = destroyUntilNotify;
					bShowLoadingTips = showLoadingTips;
					_waitMoveToSceneCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(DoMoveToNextScene("Loading", toKeepContextStack));
				}
				else
				{
					_waitMoveToSceneCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(DoMoveToNextScene(sceneName, toKeepContextStack));
				}
			}
		}

		private IEnumerator DoMoveToNextScene(string sceneName, bool toKeepContextStack = false)
		{
			if (SceneAfterLoading == "TestLevel01")
			{
				while (GlobalDataManager.IsInRefreshDataAsync)
				{
					yield return null;
				}
			}
			if (toKeepContextStack)
			{
				_pageContextStackStash = _pageContextStack;
			}
			_waitMoveToSceneCoroutine = null;
			ResetOnMoveToNextScene(sceneName);
			spaceShipVisibleOnPreviesPage = false;
			SceneManager.LoadScene(sceneName);
		}

		public void ResetOnMoveToNextScene(string sceneName)
		{
			ResetViewCache();
			_useViewCache = !(sceneName == "TestLevel01") && MainUIData.USE_VIEW_CACHING;
			_pageContextStack = new Stack<BasePageContext>();
			Singleton<NotifyManager>.Instance.ClearAllContext();
			SceneCanvas = null;
			if (Singleton<TutorialModule>.Instance != null)
			{
				Singleton<TutorialModule>.Instance.SetTutorialFlag(false);
			}
			_loadPageDialogTimes = 0;
			Singleton<ApplicationManager>.Instance.StopAllCoroutines();
		}

		public void CheckResouceBeforeLoad()
		{
			_loadPageDialogTimes++;
			if (_loadPageDialogTimes >= 10)
			{
				Resources.UnloadUnusedAssets();
				_loadPageDialogTimes = 0;
			}
		}

		public Transform GetUIHolder(UIType uiType)
		{
			switch (uiType)
			{
			case UIType.Page:
				return SceneCanvas.transform.Find("Pages");
			case UIType.SpecialDialog:
				return SceneCanvas.transform.Find("SpecialDialogs");
			case UIType.SuspendBar:
				return SceneCanvas.transform.Find("SuspendBars");
			case UIType.Dialog:
				return SceneCanvas.transform.Find("Dialogs");
			case UIType.MostFront:
				return SceneCanvas.transform;
			case UIType.Root:
				return null;
			default:
				return null;
			}
		}

		private void ClearAllContext()
		{
			foreach (BasePageContext item in _pageContextStack)
			{
				item.Clear();
			}
			_emtryPageContext.Clear();
			SceneCanvas.ClearAllWidgetContext();
		}

		public void ShowUIEffect(string contextName, string effectPath)
		{
			Transform transform = GameObject.Find("UIEffectContainer").transform;
			Tuple<string, string> key = new Tuple<string, string>(contextName, effectPath);
			if (!_uiEffectContainerDict.ContainsKey(key) || _uiEffectContainerDict[key] == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(effectPath));
				if (gameObject != null)
				{
					gameObject.transform.SetParent(transform, false);
					_uiEffectContainerDict[key] = gameObject;
				}
				return;
			}
			ParticleSystem[] componentsInChildren = _uiEffectContainerDict[key].GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
				GeneralGameObjectSound component = particleSystem.GetComponent<GeneralGameObjectSound>();
				if (component != null)
				{
					Singleton<WwiseAudioManager>.Instance.Post(component.enterEventName);
				}
			}
		}

		public void LockUI(bool toLock, float timeSpan = 3f)
		{
			if (SceneCanvas == null)
			{
				return;
			}
			Transform transform = SceneCanvas.transform.Find("BlockPanel");
			if (transform != null)
			{
				if (toLock)
				{
					transform.GetComponent<MonoBlockPanel>().SetTimeSpanTakeEffect(timeSpan);
				}
				else
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		public Action GetCallBackWhenSceneLoaded(string sceneName)
		{
			Action value;
			_sceneLoadCallBackDict.TryGetValue(sceneName, out value);
			return value;
		}

		public void ResetSceneLoadedCallBack(string sceneName)
		{
			if (_sceneLoadCallBackDict.ContainsKey(sceneName))
			{
				_sceneLoadCallBackDict.Remove(sceneName);
			}
		}

		public string GetAllPageNamesInStack()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (BasePageContext item in _pageContextStack)
			{
				stringBuilder.Append(item.ToString());
				stringBuilder.Append(" ");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
	}
}
