using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoTestUI : BaseMonoCanvas
	{
		public GameObject MainCamera;

		public GameObject MainMenu_SpaceShip;

		public bool testLocalization;

		public bool disableNetWork;

		public Avatar3dModelContext avatar3dModelContext;

		public bool isBenchmark;

		private void CreateSingleButtonTemplate()
		{
			GeneralDialogContext generalDialogContext = new GeneralDialogContext();
			generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
			generalDialogContext.title = "TestUI";
			generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
			BaseDialogContext dialogContext = generalDialogContext;
			Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
		}

		private void CreateDoubleButtonTemplate()
		{
			GeneralDialogContext generalDialogContext = new GeneralDialogContext();
			generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
			generalDialogContext.title = "TestUI";
			generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
			generalDialogContext.notDestroyAfterTouchBG = true;
			BaseDialogContext dialogContext = generalDialogContext;
			Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
		}

		public void Awake()
		{
			GlobalVars.DISABLE_NETWORK_DEBUG = disableNetWork;
			SuperDebug.DEBUG_SWITCH[6] = true;
			MainUIData.USE_VIEW_CACHING = false;
			GeneralLogicManager.InitAll();
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[3] { "All_In_One_Bank", "BK_Global", "BK_Events" });
			Singleton<IslandModule>.Instance.InitTechTree();
		}

		public override void Start()
		{
			TestUIContext widget = new TestUIContext(base.gameObject);
			Singleton<MainUIManager>.Instance.ShowWidget(widget);
			TestLocalization();
			if (!disableNetWork)
			{
				StartCoroutine(Singleton<NetworkManager>.Instance.ConnectGlobalDispatchServer(Login));
			}
			else
			{
				Login();
			}
			PostStartHandleBenchmark();
			if (Singleton<EffectManager>.Instance == null)
			{
				Singleton<EffectManager>.Create();
				Singleton<EffectManager>.Instance.InitAtAwake();
				Singleton<EffectManager>.Instance.InitAtStart();
			}
			base.Start();
		}

		private void Login()
		{
			Singleton<NetworkManager>.Instance.QuickLogin();
		}

		public void TestLocalization()
		{
			if (!testLocalization)
			{
				return;
			}
			Text[] componentsInChildren = GetComponentsInChildren<Text>();
			foreach (Text text in componentsInChildren)
			{
				if (text.GetComponent<LocalizedText>() == null)
				{
					text.text = "XXXXX";
				}
			}
		}

		public override GameObject GetSpaceShipObj()
		{
			return MainMenu_SpaceShip;
		}

		private void PostStartHandleBenchmark()
		{
			if (GlobalVars.IS_BENCHMARK || isBenchmark)
			{
				Screen.sleepTimeout = -1;
				SuperDebug.CloseAllDebugs();
				GameObject gameObject = new GameObject();
				Object.DontDestroyOnLoad(gameObject);
				gameObject.name = "__Benchmark";
				gameObject.AddComponent<MonoBenchmarkSwitches>();
			}
		}
	}
}
