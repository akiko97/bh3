using System;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class SettingPageContext : BasePageContext
	{
		public const string AudioTab = "AudioTab";

		public const string GraphicsTab = "GraphicsTab";

		public const string ImageTab = "ImageTab";

		public const string PushTab = "PushTab";

		public const string OtherTab = "OtherTab";

		public readonly string defaultTab;

		private TabManager _tabManager;

		private MonoSettingGraphicsTab _graphicSetting;

		public SettingPageContext(string defaultTab = "AudioTab")
		{
			config = new ContextPattern
			{
				contextName = "SettingPageContext",
				viewPrefabPath = "UI/Menus/Page/Setting/SettingPage"
			};
			showSpaceShip = true;
			this.defaultTab = defaultTab;
			_tabManager = new TabManager();
			_tabManager.onSetActive += OnTabSetActive;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("TabBtns/AudioTabBtn").GetComponent<Button>(), OnAudioTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/GraphicsTabBtn").GetComponent<Button>(), OnGraphicsTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/ImageTabBtn").GetComponent<Button>(), OnImageTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/PushTabBtn").GetComponent<Button>(), OnPushTabBtnClick);
			BindViewCallback(base.view.transform.Find("TabBtns/OtherTabBtn").GetComponent<Button>(), OnOtherTabBtnClick);
			BindViewCallback(base.view.transform.Find("ECOMode/Choice/Button").GetComponent<Button>(), OnEcoModeBtnClick);
		}

		protected override bool SetupView()
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			string text = ((!string.IsNullOrEmpty(showingTabKey)) ? showingTabKey : defaultTab);
			_tabManager.Clear();
			SetupAudioTab();
			SetupGraphicsTab();
			SetupImageTab();
			SetupPushTab();
			SetupOtherTab();
			if (text == "ImageTab")
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(true, false)));
			}
			else if (text == "GraphicsTab")
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, true)));
			}
			else
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
			}
			_tabManager.ShowTab(text);
			if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
			{
				base.view.GetComponent<MonoFadeInAnimManager>().Play("AudioTabFadeIn");
			}
			return false;
		}

		public override void BackPage()
		{
			DoLevelCurrentPageOrTab(base.BackPage);
		}

		public override void BackToMainMenuPage()
		{
			DoLevelCurrentPageOrTab(base.BackToMainMenuPage);
		}

		public override void Destroy()
		{
			base.view.transform.Find("ImageTab").Find("Content/RT/3dModel").GetComponent<MonoGammaSettingRenderImage>()
				.ReleaseRenderTexture();
			base.Destroy();
		}

		public void OnAudioTabBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
			DoLevelCurrentPageOrTab(delegate
			{
				_tabManager.ShowTab("AudioTab");
				base.view.transform.Find("AudioTab").GetComponent<MonoSettingAudioTab>().SetupView();
				if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
				{
					base.view.GetComponent<MonoFadeInAnimManager>().Play("AudioTabFadeIn");
				}
			});
		}

		public void OnGraphicsTabBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, true)));
			DoLevelCurrentPageOrTab(delegate
			{
				_tabManager.ShowTab("GraphicsTab");
				_graphicSetting.SetupView();
				if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
				{
					base.view.GetComponent<MonoFadeInAnimManager>().Play("GraphicsTabFadeIn");
				}
			});
		}

		public void OnImageTabBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(true, false)));
			DoLevelCurrentPageOrTab(delegate
			{
				_tabManager.ShowTab("ImageTab");
				base.view.transform.Find("ImageTab").GetComponent<MonoSettingImageTab>().SetupView();
				if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
				{
					base.view.GetComponent<MonoFadeInAnimManager>().Play("ImageTabFadeIn");
				}
			});
		}

		public void OnPushTabBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
			DoLevelCurrentPageOrTab(delegate
			{
				_tabManager.ShowTab("PushTab");
				base.view.transform.Find("PushTab").GetComponent<MonoSettingPushTab>().SetupView();
			});
		}

		public void OnOtherTabBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
			DoLevelCurrentPageOrTab(delegate
			{
				_tabManager.ShowTab("OtherTab");
				base.view.transform.Find("OtherTab").GetComponent<MonoSettingOtherTab>().SetupView();
			});
		}

		public void OnEcoModeBtnClick()
		{
			_graphicSetting.SwitchEcoMode();
		}

		private void OnTabSetActive(bool active, GameObject go, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.transform.Find("Image").GetComponent<Image>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
			go.SetActive(active);
		}

		private void SetupAudioTab()
		{
			GameObject gameObject = base.view.transform.Find("AudioTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/AudioTabBtn").GetComponent<Button>();
			_tabManager.SetTab("AudioTab", component, gameObject);
			MonoSettingAudioTab component2 = gameObject.GetComponent<MonoSettingAudioTab>();
			component2.SetupView();
		}

		private void SetupGraphicsTab()
		{
			GameObject gameObject = base.view.transform.Find("GraphicsTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/GraphicsTabBtn").GetComponent<Button>();
			_tabManager.SetTab("GraphicsTab", component, gameObject);
			_graphicSetting = gameObject.GetComponent<MonoSettingGraphicsTab>();
			_graphicSetting.SetupView();
		}

		private void SetupImageTab()
		{
			GameObject gameObject = base.view.transform.Find("ImageTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/ImageTabBtn").GetComponent<Button>();
			_tabManager.SetTab("ImageTab", component, gameObject);
			MonoSettingImageTab component2 = gameObject.GetComponent<MonoSettingImageTab>();
			component2.SetupView();
		}

		private void SetupPushTab()
		{
			GameObject gameObject = base.view.transform.Find("PushTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/PushTabBtn").GetComponent<Button>();
			_tabManager.SetTab("PushTab", component, gameObject);
			MonoSettingPushTab component2 = gameObject.GetComponent<MonoSettingPushTab>();
			component2.SetupView();
		}

		public void SetupOtherTab()
		{
			GameObject gameObject = base.view.transform.Find("OtherTab").gameObject;
			Button component = base.view.transform.Find("TabBtns/OtherTabBtn").GetComponent<Button>();
			_tabManager.SetTab("OtherTab", component, gameObject);
			MonoSettingOtherTab component2 = gameObject.GetComponent<MonoSettingOtherTab>();
			component2.SetupView();
		}

		private void DoLevelCurrentPageOrTab(Action callback)
		{
			string showingTabKey = _tabManager.GetShowingTabKey();
			if (!string.IsNullOrEmpty(showingTabKey))
			{
				GameObject gameObject = base.view.transform.Find(showingTabKey).gameObject;
				Action noSaveAction = null;
				Action saveAction = null;
				bool flag = false;
				switch (showingTabKey)
				{
				case "AudioTab":
					flag = gameObject.GetComponent<MonoSettingAudioTab>().CheckNeedSave();
					noSaveAction = gameObject.GetComponent<MonoSettingAudioTab>().OnNoSaveBtnClick;
					saveAction = gameObject.GetComponent<MonoSettingAudioTab>().OnSaveBtnClick;
					break;
				case "GraphicsTab":
					flag = gameObject.GetComponent<MonoSettingGraphicsTab>().CheckNeedSave();
					noSaveAction = gameObject.GetComponent<MonoSettingGraphicsTab>().OnNoSaveBtnClick;
					saveAction = gameObject.GetComponent<MonoSettingGraphicsTab>().OnSaveBtnClick;
					break;
				case "ImageTab":
					flag = gameObject.GetComponent<MonoSettingImageTab>().CheckNeedSave();
					noSaveAction = gameObject.GetComponent<MonoSettingImageTab>().OnNoSaveBtnClick;
					saveAction = gameObject.GetComponent<MonoSettingImageTab>().OnSaveBtnClick;
					break;
				case "PushTab":
					flag = gameObject.GetComponent<MonoSettingPushTab>().CheckNeedSave();
					noSaveAction = gameObject.GetComponent<MonoSettingPushTab>().OnNoSaveBtnClick;
					saveAction = gameObject.GetComponent<MonoSettingPushTab>().OnSaveBtnClick;
					break;
				case "OtherTab":
					flag = gameObject.GetComponent<MonoSettingOtherTab>().CheckNeedSave();
					noSaveAction = gameObject.GetComponent<MonoSettingOtherTab>().OnNoSaveBtnClick;
					saveAction = gameObject.GetComponent<MonoSettingOtherTab>().OnSaveBtnClick;
					break;
				}
				if (flag)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						type = GeneralDialogContext.ButtonType.DoubleButton,
						title = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgTitle"),
						desc = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgDesc"),
						okBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgSave"),
						cancelBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgNoSave"),
						buttonCallBack = delegate(bool confirmed)
						{
							if (confirmed)
							{
								if (saveAction != null)
								{
									saveAction();
								}
								callback();
							}
							else
							{
								if (noSaveAction != null)
								{
									noSaveAction();
								}
								callback();
							}
						}
					});
					return;
				}
			}
			callback();
		}
	}
}
