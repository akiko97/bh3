using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class GMTalkDialogContext : BaseDialogContext
	{
		private const string BUTTON_PREFAB_PATH = "UI/GMTalk/GMTalkButton";

		public const string NETWORK_DELAY_ON = "模拟网络延迟<color=red>(已经开启)({0}秒)</color>";

		public const string NETWORK_DELAY_OFF = "模拟网络延迟<color=red>(已经关闭)</color>";

		private List<string> _showCommandList = new List<string> { "FETCH ALL", "FETCH ALLAVATAR", "CLEAR PACK", "CLEAR STAGE", "CLEAR ALL", "CLEAR GUIDE", "FSALL", "CLEAR SIGN", "CLEAR ACTIVITY" };

		public GMTalkDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "GMTalkDialogContext",
				viewPrefabPath = "UI/GMTalk/GMTalkDialog",
				cacheType = ViewCacheType.DontCache
			};
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/SingleButton/Button").GetComponent<Button>(), OnOkButtonCallBack);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/ClearLocalData").GetComponent<Button>(), OnClearLocalDataCallBack);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllTutorial").GetComponent<Button>(), OnSkipAllTutorialCallBack);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllUITutorial").GetComponent<Button>(), OnSkipAllUITutorialCallback);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllLevelTutorial").GetComponent<Button>(), OnSkipAllLevelTutorialCallback);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/UnlockAllCG").GetComponent<Button>(), OnUnlockAllCGCallBack);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllPlot").GetComponent<Button>(), OnSkipAllPlotCallback);
			BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/ShittyNetwork").GetComponent<Button>(), OnNetworkDelayCallback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/TestAllEquip").GetComponent<Button>(), OnTestAllEquipCallBack);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel1").GetComponent<Button>(), OnOriginGoodFeelCallback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel10").GetComponent<Button>(), OnGoodFeel10Callback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel100").GetComponent<Button>(), OnGoodFeel100Callback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel1000").GetComponent<Button>(), OnGoodFeel1000Callback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel10000").GetComponent<Button>(), OnGoodFeel10000Callback);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/RestartGame").GetComponent<Button>(), OnRestartGameCallBack);
			BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/BenchmarkSwitches").GetComponent<Button>(), OnToggleBenchmarkCallBack);
		}

		protected override bool SetupView()
		{
			EventSystem.current.SetSelectedGameObject(base.view.transform.Find("Dialog/InputField").gameObject);
			Transform parent = base.view.transform.Find("GMBtnsPanel/Content");
			foreach (string showCommand in _showCommandList)
			{
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/GMTalk/GMTalkButton"));
				gameObject.transform.SetParent(parent, false);
				gameObject.GetComponent<MonoGMTalkButton>().SetupView(showCommand, GMTalkButtonCallback);
			}
			SyncNetworkDelayDisplay();
			return false;
		}

		public void OnOkButtonCallBack()
		{
			string text = base.view.transform.Find("Dialog/InputField").GetComponent<InputField>().text;
			if (MonoMemoryProfiler.ParseCommand(text))
			{
				MonoMemoryProfiler.CreateMemoryProfiler();
			}
			string[] array = text.Split(";"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				Singleton<TestModule>.Instance.RequestGMTalk(array[i].Trim());
			}
			Destroy();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void OnClearLocalDataCallBack()
		{
			MiHoYoGameData.DeleteAllData();
			Destroy();
		}

		public void OnSkipAllTutorialCallBack()
		{
			SkipAllUITutorial();
			SkipAllLevelTutorial();
			SkipAllPlot();
			Destroy();
		}

		private void SkipAllUITutorial()
		{
			List<TutorialData> itemList = TutorialDataReader.GetItemList();
			foreach (TutorialData item in itemList)
			{
				if (item.id < LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID)
				{
					Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)item.id, true);
				}
			}
		}

		private void SkipAllLevelTutorial()
		{
			List<LevelTutorialMetaData> itemList = LevelTutorialMetaDataReader.GetItemList();
			foreach (LevelTutorialMetaData item in itemList)
			{
				if (item.tutorialId > LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID && item.tutorialId < LevelPlotModule.BASE_LEVEL_PLOT_ID)
				{
					Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)item.tutorialId, true);
				}
			}
		}

		private void SkipAllPlot()
		{
			List<PlotMetaData> itemList = PlotMetaDataReader.GetItemList();
			foreach (PlotMetaData item in itemList)
			{
				if (item.plotID > LevelPlotModule.BASE_LEVEL_PLOT_ID && item.plotID < CGModule.BASE_CG_ID)
				{
					Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)item.plotID, true);
				}
			}
		}

		private void UnlockAllCG()
		{
			List<CgMetaData> itemList = CgMetaDataReader.GetItemList();
			foreach (CgMetaData item in itemList)
			{
				if (item.CgID > CGModule.BASE_CG_ID)
				{
					Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)item.CgID, true);
				}
			}
		}

		public void OnSkipAllUITutorialCallback()
		{
			SkipAllUITutorial();
			Destroy();
		}

		public void OnSkipAllLevelTutorialCallback()
		{
			SkipAllLevelTutorial();
			Destroy();
		}

		public void OnSkipAllPlotCallback()
		{
			SkipAllPlot();
			Destroy();
		}

		public void OnUnlockAllCGCallBack()
		{
			UnlockAllCG();
			Destroy();
		}

		public void OnNetworkDelayCallback()
		{
			GlobalVars.DEBUG_NETWORK_DELAY_LEVEL = (GlobalVars.DEBUG_NETWORK_DELAY_LEVEL + 1) % 5;
			SyncNetworkDelayDisplay();
		}

		public void OnTestAllEquipCallBack()
		{
			Destroy();
			Singleton<ApplicationManager>.Instance.StartCoroutine(TestAllEquip());
		}

		public void OnRestartGameCallBack()
		{
			Destroy();
			GeneralLogicManager.RestartGame();
		}

		public void OnToggleBenchmarkCallBack()
		{
			Destroy();
			MonoBenchmarkSwitches monoBenchmarkSwitches = Object.FindObjectOfType<MonoBenchmarkSwitches>();
			if (monoBenchmarkSwitches == null)
			{
				GameObject gameObject = new GameObject();
				Object.DontDestroyOnLoad(gameObject);
				gameObject.name = "__Benchmark";
				gameObject.AddComponent<MonoBenchmarkSwitches>();
			}
			else
			{
				Object.Destroy(monoBenchmarkSwitches.gameObject);
			}
		}

		public void OnOriginGoodFeelCallback()
		{
		}

		public void OnGoodFeel10Callback()
		{
		}

		public void OnGoodFeel100Callback()
		{
		}

		public void OnGoodFeel1000Callback()
		{
		}

		public void OnGoodFeel10000Callback()
		{
		}

		private IEnumerator TestAllEquip()
		{
			float waitSec = 0.35f;
			if (!(Singleton<MainUIManager>.Instance.CurrentPageContext is MainPageContext))
			{
				Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
			}
			yield return new WaitForSeconds(waitSec);
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext());
			yield return new WaitForSeconds(waitSec);
			foreach (StorageDataItemBase item in Singleton<StorageModule>.Instance.UserStorageItemList)
			{
				UIUtil.ShowItemDetail(item);
				yield return new WaitForSeconds(waitSec);
				if (item is WeaponDataItem || item is StigmataDataItem)
				{
					Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
				}
				else
				{
					Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList[0].Destroy();
				}
				yield return new WaitForSeconds(waitSec);
			}
		}

		private void GMTalkButtonCallback(string command)
		{
			Singleton<TestModule>.Instance.RequestGMTalk(command);
			Destroy();
		}

		private void SyncNetworkDelayDisplay()
		{
			base.view.transform.Find("GMBtnsPanel/Content/ShittyNetwork/Text").GetComponent<Text>().text = ((GlobalVars.DEBUG_NETWORK_DELAY_LEVEL <= 0) ? "模拟网络延迟<color=red>(已经关闭)</color>" : string.Format("模拟网络延迟<color=red>(已经开启)({0}秒)</color>", GlobalVars.DEBUG_NETWORK_DELAY_LEVEL));
		}
	}
}
