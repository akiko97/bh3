using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSettingGraphicsTab : MonoBehaviour
	{
		public Transform[] grades;

		public Transform[] processes;

		public Button[] configButtons;

		public Transform ecoMode;

		private ConfigGraphicsPersonalSetting _modifiedPersonalSetting;

		private bool isInLevelSimpleSetting;

		public void SetupView(bool inLevelSimpleSetting = false)
		{
			isInLevelSimpleSetting = inLevelSimpleSetting;
			_modifiedPersonalSetting = new ConfigGraphicsPersonalSetting();
			GraphicsSettingData.CopyPersonalGraphicsConfig(ref _modifiedPersonalSetting);
			ResetView();
		}

		public void ResetView()
		{
			if (_modifiedPersonalSetting.IsEcoMode)
			{
				ShowEcoModeConfig();
			}
			else if (_modifiedPersonalSetting.IsUserDefinedGrade || _modifiedPersonalSetting.IsUserDefinedVolatile)
			{
				ShowPersonalConfig();
			}
			else
			{
				ShowDefaultRecommendCompleteConfig();
			}
			ShowRecommendMark();
			SetupEcoMode();
		}

		public bool CheckNeedSave()
		{
			return !GraphicsSettingData.IsEqualToPersonalConfigIgnoreContrast(_modifiedPersonalSetting);
		}

		public void OnRecommendGradeBtnClick(int index)
		{
			if (_modifiedPersonalSetting.IsEcoMode || !_modifiedPersonalSetting.IsUserDefinedGrade || index != (int)_modifiedPersonalSetting.RecommendGrade)
			{
				_modifiedPersonalSetting.IsUserDefinedGrade = true;
				_modifiedPersonalSetting.IsUserDefinedVolatile = false;
				_modifiedPersonalSetting.IsEcoMode = false;
				_modifiedPersonalSetting.RecommendGrade = (GraphicsRecommendGrade)index;
				ConfigGraphicsSetting graphicsRecommendCompleteConfig = GraphicsSettingData.GetGraphicsRecommendCompleteConfig((GraphicsRecommendGrade)index);
				GraphicsSettingData.ApplySettingConfig(graphicsRecommendCompleteConfig);
				EnableAllConfigBtns(false);
				ShowRecommendCompleteConfig((GraphicsRecommendGrade)index);
				SetupEcoMode();
			}
		}

		public void OnPersonalGradeBtnClick()
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			if (personalGraphicsSetting.IsUserDefinedVolatile)
			{
				GraphicsSettingData.CopyPersonalGraphicsConfig(ref _modifiedPersonalSetting);
			}
			else
			{
				GraphicsSettingData.CopyToPersonalGraphicsConfig(GraphicsSettingData.GetGraphicsRecommendCompleteConfig(), ref _modifiedPersonalSetting);
			}
			ConfigGraphicsSetting graphicsPersonalSettingConfig = GraphicsSettingData.GetGraphicsPersonalSettingConfig(_modifiedPersonalSetting);
			GraphicsSettingData.ApplySettingConfig(graphicsPersonalSettingConfig);
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			_modifiedPersonalSetting.IsEcoMode = false;
			ShowPersonalConfig();
			SetupEcoMode();
		}

		public void OnResolutionBtnClick(int grade)
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			_modifiedPersonalSetting.ResolutionQuality = (ResolutionQualityGrade)grade;
			GraphicsSettingUtil.ApplyResolution(_modifiedPersonalSetting.ResolutionPercentage, _modifiedPersonalSetting.ResolutionQuality, _modifiedPersonalSetting.RecommendResolutionX, _modifiedPersonalSetting.RecommendResolutionY);
			ShowResolution(_modifiedPersonalSetting.ResolutionQuality, false);
		}

		public void OnTargetFrameRateBtnClick(bool is30Rate)
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			if (is30Rate)
			{
				_modifiedPersonalSetting.TargetFrameRate = 30;
			}
			else
			{
				_modifiedPersonalSetting.TargetFrameRate = 60;
			}
			GraphicsSettingUtil.SetTargetFrameRate(_modifiedPersonalSetting.TargetFrameRate);
			ShowTargetFrameRate(_modifiedPersonalSetting.TargetFrameRate, false);
		}

		public void OnPostFXBtnClick(bool willBeOn)
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			_modifiedPersonalSetting.VolatileSetting.UsePostFX = willBeOn;
			bool forceWhenDisable = true;
			GraphicsSettingUtil.EnablePostFX(willBeOn, forceWhenDisable);
			ShowPostFX(_modifiedPersonalSetting, false);
		}

		public void OnReflectionBtnClick(bool willBeOn)
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			_modifiedPersonalSetting.VolatileSetting.UseReflection = willBeOn;
			GraphicsSettingUtil.EnableReflection(willBeOn);
			ShowReflection(_modifiedPersonalSetting.VolatileSetting.UseReflection, false);
		}

		public void OnDynamicBoneBtnClick(bool willBeOn)
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			_modifiedPersonalSetting.VolatileSetting.UseDynamicBone = willBeOn;
			GraphicsSettingUtil.EnableDynamicBone(willBeOn);
			ShowDynamicBone(_modifiedPersonalSetting.VolatileSetting.UseDynamicBone, false);
		}

		public void OnPostFXGradeBtnClick()
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			if (_modifiedPersonalSetting.VolatileSetting.PostFXGrade == PostEffectQualityGrade.Low)
			{
				_modifiedPersonalSetting.VolatileSetting.PostFXGrade = PostEffectQualityGrade.High;
			}
			else
			{
				_modifiedPersonalSetting.VolatileSetting.PostFXGrade = PostEffectQualityGrade.Low;
			}
			GraphicsSettingUtil.SetPostEffectBufferSizeByQuality(_modifiedPersonalSetting.PostFxGradeBufferSize, _modifiedPersonalSetting.VolatileSetting.PostFXGrade);
			ShowPostFXGrade(_modifiedPersonalSetting.VolatileSetting.PostFXGrade, false, true);
		}

		public void OnHDRBtnClick()
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			bool useHDR = _modifiedPersonalSetting.VolatileSetting.UseHDR;
			_modifiedPersonalSetting.VolatileSetting.UseHDR = !useHDR;
			GraphicsSettingUtil.EnableHDR(useHDR);
			ShowHDR(_modifiedPersonalSetting.VolatileSetting.UseHDR, false, true);
		}

		public void OnAABtnClick()
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			bool useFXAA = _modifiedPersonalSetting.VolatileSetting.UseFXAA;
			_modifiedPersonalSetting.VolatileSetting.UseFXAA = !useFXAA;
			GraphicsSettingUtil.EnableFXAA(useFXAA);
			ShowAA(_modifiedPersonalSetting.VolatileSetting.UseFXAA, false, true);
		}

		public void OnDistortionBtnClick()
		{
			_modifiedPersonalSetting.IsUserDefinedGrade = false;
			_modifiedPersonalSetting.IsUserDefinedVolatile = true;
			bool useDistortion = _modifiedPersonalSetting.VolatileSetting.UseDistortion;
			_modifiedPersonalSetting.VolatileSetting.UseDistortion = !useDistortion;
			GraphicsSettingUtil.EnableDistortion(useDistortion);
			ShowDistortion(_modifiedPersonalSetting.VolatileSetting.UseDistortion, false, true);
		}

		public void OnEcoModeBtnClick(bool willBeOn)
		{
			GraphicsSettingData.CopyPersonalGraphicsConfig(willBeOn, ref _modifiedPersonalSetting);
			ResetView();
		}

		public void OnNoSaveBtnClick()
		{
			GraphicsSettingData.ApplySettingConfig();
			GraphicsSettingData.CopyPersonalGraphicsConfig(ref _modifiedPersonalSetting);
			ResetView();
		}

		public void OnSaveBtnClick()
		{
			GraphicsSettingData.SavePersonalConfigIgnoreContrast(_modifiedPersonalSetting);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess")));
		}

		public void SwitchEcoMode()
		{
			GraphicsSettingData.CopyPersonalGraphicsConfig(!_modifiedPersonalSetting.IsEcoMode, ref _modifiedPersonalSetting);
			GraphicsSettingData.SavePersonalConfigIgnoreContrast(_modifiedPersonalSetting);
			ResetView();
			string textID = ((!_modifiedPersonalSetting.IsEcoMode) ? "Menu_SettingEcoModeOffTip" : "Menu_SettingEcoModeOnTip");
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText(textID)));
		}

		private void ShowDefaultRecommendCompleteConfig()
		{
			EnableAllConfigBtns(false);
			GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
			ShowRecommendCompleteConfig(graphicsRecommendGrade);
		}

		private void ShowRecommendCompleteConfig(GraphicsRecommendGrade grade)
		{
			ShowRecommendGradeInfo(grade);
			ConfigGraphicsSetting graphicsRecommendCompleteConfig = GraphicsSettingData.GetGraphicsRecommendCompleteConfig(grade);
			ShowGraphicsSettingConfig(graphicsRecommendCompleteConfig, true);
		}

		private void ShowGraphicsSettingConfig(ConfigGraphicsSetting setting, bool isRecommend)
		{
			if (!isInLevelSimpleSetting)
			{
				Transform transform = base.transform.Find("Content/DetailSetting/BG");
				transform.Find("Grey").gameObject.SetActive(isRecommend);
				transform.Find("Blue").gameObject.SetActive(!isRecommend);
				ShowResolution(setting.ResolutionQuality, isRecommend);
				ShowTargetFrameRate(setting.TargetFrameRate, isRecommend);
				ShowPostFX(setting, isRecommend);
				ShowReflection(setting.VolatileSetting.UseReflection, isRecommend);
				ShowDynamicBone(setting.VolatileSetting.UseDynamicBone, isRecommend);
				ShowPostFXGrade(setting.VolatileSetting.PostFXGrade, isRecommend, setting.VolatileSetting.UsePostFX);
				ShowHDR(setting.VolatileSetting.UseHDR, isRecommend, setting.VolatileSetting.UsePostFX);
				ShowAA(setting.VolatileSetting.UseFXAA, isRecommend, setting.VolatileSetting.UsePostFX);
				ShowDistortion(setting.VolatileSetting.UseDistortion, isRecommend, setting.VolatileSetting.UsePostFX);
			}
		}

		private void ShowResolution(ResolutionQualityGrade resolutionGrade, bool isRecommend)
		{
			Transform transform = base.transform.Find("Content/DetailSetting/FirstLine/Resolution");
			Transform transform2 = transform.Find("Mark");
			transform2.FindChild("Enable").gameObject.SetActive(!isRecommend);
			transform2.FindChild("Disable").gameObject.SetActive(isRecommend);
			Transform transform3 = transform.FindChild("Choice/Low");
			Transform transform4 = transform.FindChild("Choice/Middle");
			Transform transform5 = transform.FindChild("Choice/High");
			Transform transform6 = transform.FindChild("Label");
			Transform transform7 = transform3.FindChild("Text");
			Transform transform8 = transform4.FindChild("Text");
			Transform transform9 = transform5.FindChild("Text");
			if (isRecommend)
			{
				transform3.FindChild("Blue").gameObject.SetActive(false);
				transform3.FindChild("Grey").gameObject.SetActive(false);
				transform3.FindChild("Disable").gameObject.SetActive(true);
				transform4.FindChild("Blue").gameObject.SetActive(false);
				transform4.FindChild("Grey").gameObject.SetActive(false);
				transform4.FindChild("Disable").gameObject.SetActive(true);
				transform5.FindChild("Blue").gameObject.SetActive(false);
				transform5.FindChild("Grey").gameObject.SetActive(false);
				transform5.FindChild("Disable").gameObject.SetActive(true);
				transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform7.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform8.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform9.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform3.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
				transform4.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
				transform5.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
			}
			else
			{
				transform3.FindChild("Disable").gameObject.SetActive(false);
				transform4.FindChild("Disable").gameObject.SetActive(false);
				transform5.FindChild("Disable").gameObject.SetActive(false);
				transform6.GetComponent<Text>().color = Color.white;
				transform7.GetComponent<Text>().color = Color.white;
				transform8.GetComponent<Text>().color = Color.white;
				transform9.GetComponent<Text>().color = Color.white;
				transform3.FindChild("Check").GetComponent<Image>().color = Color.white;
				transform4.FindChild("Check").GetComponent<Image>().color = Color.white;
				transform5.FindChild("Check").GetComponent<Image>().color = Color.white;
			}
			switch (resolutionGrade)
			{
			case ResolutionQualityGrade.High:
				transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				transform4.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				transform5.FindChild("Button").GetComponent<Button>().interactable = false;
				if (!isRecommend)
				{
					transform3.FindChild("Blue").gameObject.SetActive(false);
					transform3.FindChild("Grey").gameObject.SetActive(true);
					transform4.FindChild("Blue").gameObject.SetActive(false);
					transform4.FindChild("Grey").gameObject.SetActive(true);
					transform5.FindChild("Blue").gameObject.SetActive(true);
					transform5.FindChild("Grey").gameObject.SetActive(false);
				}
				transform3.FindChild("Check").gameObject.SetActive(false);
				transform4.FindChild("Check").gameObject.SetActive(false);
				transform5.FindChild("Check").gameObject.SetActive(true);
				break;
			case ResolutionQualityGrade.Middle:
				transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				transform4.FindChild("Button").GetComponent<Button>().interactable = false;
				transform5.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				if (!isRecommend)
				{
					transform3.FindChild("Blue").gameObject.SetActive(false);
					transform3.FindChild("Grey").gameObject.SetActive(true);
					transform4.FindChild("Blue").gameObject.SetActive(true);
					transform4.FindChild("Grey").gameObject.SetActive(false);
					transform5.FindChild("Blue").gameObject.SetActive(false);
					transform5.FindChild("Grey").gameObject.SetActive(true);
				}
				transform3.FindChild("Check").gameObject.SetActive(false);
				transform4.FindChild("Check").gameObject.SetActive(true);
				transform5.FindChild("Check").gameObject.SetActive(false);
				break;
			default:
				transform3.FindChild("Button").GetComponent<Button>().interactable = false;
				transform4.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				transform5.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				if (!isRecommend)
				{
					transform5.FindChild("Blue").gameObject.SetActive(false);
					transform5.FindChild("Grey").gameObject.SetActive(true);
					transform4.FindChild("Blue").gameObject.SetActive(false);
					transform4.FindChild("Grey").gameObject.SetActive(true);
					transform3.FindChild("Blue").gameObject.SetActive(true);
					transform3.FindChild("Grey").gameObject.SetActive(false);
				}
				transform5.FindChild("Check").gameObject.SetActive(false);
				transform4.FindChild("Check").gameObject.SetActive(false);
				transform3.FindChild("Check").gameObject.SetActive(true);
				break;
			}
		}

		private void ShowTargetFrameRate(int targetFrameRate, bool isRecommend)
		{
			ShowFirstLineElement(base.transform.Find("Content/DetailSetting/FirstLine/FrameLimit"), isRecommend, targetFrameRate == 60);
		}

		private void ShowFirstLineElement(Transform elementTransform, bool isRecommend, bool isHigh)
		{
			Transform transform = elementTransform.Find("Mark");
			transform.FindChild("Enable").gameObject.SetActive(!isRecommend);
			transform.FindChild("Disable").gameObject.SetActive(isRecommend);
			Transform transform2 = elementTransform.FindChild("Choice/Low");
			Transform transform3 = elementTransform.FindChild("Choice/High");
			Transform transform4 = elementTransform.FindChild("Label");
			Transform transform5 = transform2.FindChild("Text");
			Transform transform6 = transform3.FindChild("Text");
			if (isRecommend)
			{
				transform2.FindChild("Blue").gameObject.SetActive(false);
				transform2.FindChild("Grey").gameObject.SetActive(false);
				transform2.FindChild("Disable").gameObject.SetActive(true);
				transform3.FindChild("Blue").gameObject.SetActive(false);
				transform3.FindChild("Grey").gameObject.SetActive(false);
				transform3.FindChild("Disable").gameObject.SetActive(true);
				transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform2.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
				transform3.FindChild("Check").GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
			}
			else
			{
				transform2.FindChild("Disable").gameObject.SetActive(false);
				transform3.FindChild("Disable").gameObject.SetActive(false);
				transform4.GetComponent<Text>().color = Color.white;
				transform5.GetComponent<Text>().color = Color.white;
				transform6.GetComponent<Text>().color = Color.white;
				transform2.FindChild("Check").GetComponent<Image>().color = Color.white;
				transform3.FindChild("Check").GetComponent<Image>().color = Color.white;
			}
			if (isHigh)
			{
				transform2.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				transform3.FindChild("Button").GetComponent<Button>().interactable = false;
				if (!isRecommend)
				{
					transform2.FindChild("Blue").gameObject.SetActive(false);
					transform2.FindChild("Grey").gameObject.SetActive(true);
					transform3.FindChild("Blue").gameObject.SetActive(true);
					transform3.FindChild("Grey").gameObject.SetActive(false);
				}
				transform2.FindChild("Check").gameObject.SetActive(false);
				transform3.FindChild("Check").gameObject.SetActive(true);
			}
			else
			{
				transform2.FindChild("Button").GetComponent<Button>().interactable = false;
				transform3.FindChild("Button").GetComponent<Button>().interactable = !isRecommend;
				if (!isRecommend)
				{
					transform3.FindChild("Blue").gameObject.SetActive(false);
					transform3.FindChild("Grey").gameObject.SetActive(true);
					transform2.FindChild("Blue").gameObject.SetActive(true);
					transform2.FindChild("Grey").gameObject.SetActive(false);
				}
				transform3.FindChild("Check").gameObject.SetActive(false);
				transform2.FindChild("Check").gameObject.SetActive(true);
			}
		}

		private void ShowPostFX(ConfigGraphicsSetting setting, bool isRecommend)
		{
			ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/PostFX"), isRecommend, setting.VolatileSetting.UsePostFX);
			ShowPostFXGrade(setting.VolatileSetting.PostFXGrade, isRecommend, setting.VolatileSetting.UsePostFX);
			ShowHDR(setting.VolatileSetting.UseHDR, isRecommend, setting.VolatileSetting.UsePostFX);
			ShowAA(setting.VolatileSetting.UseFXAA, isRecommend, setting.VolatileSetting.UsePostFX);
			ShowDistortion(setting.VolatileSetting.UseDistortion, isRecommend, setting.VolatileSetting.UsePostFX);
		}

		private void ShowReflection(bool useReflection, bool isRecommend)
		{
			ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/Reflection"), isRecommend, useReflection);
		}

		private void ShowDynamicBone(bool useDynamicBone, bool isRecommend)
		{
			ShowSecondLineElement(base.transform.Find("Content/DetailSetting/SecondLine/DynamicBone"), isRecommend, useDynamicBone);
		}

		private void ShowSecondLineElement(Transform elementTransform, bool isRecommend, bool use)
		{
			Transform transform = elementTransform.Find("Mark");
			transform.FindChild("Enable").gameObject.SetActive(!isRecommend);
			transform.FindChild("Disable").gameObject.SetActive(isRecommend);
			Transform transform2 = elementTransform.Find("Choice/On");
			Transform transform3 = elementTransform.Find("Choice/Off");
			Transform transform4 = elementTransform.FindChild("Label");
			Transform transform5 = transform2.FindChild("Text");
			Transform transform6 = transform3.FindChild("Text");
			if (isRecommend)
			{
				transform2.FindChild("Blue").gameObject.SetActive(false);
				transform2.FindChild("Grey").gameObject.SetActive(false);
				transform2.FindChild("Disable").gameObject.SetActive(true);
				transform3.FindChild("Grey").gameObject.SetActive(false);
				transform3.FindChild("Disable").gameObject.SetActive(true);
				transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform6.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
			}
			else
			{
				transform2.FindChild("Blue").gameObject.SetActive(use);
				transform2.FindChild("Grey").gameObject.SetActive(!use);
				transform3.FindChild("Grey").gameObject.SetActive(!use);
				transform2.FindChild("Disable").gameObject.SetActive(false);
				transform3.FindChild("Disable").gameObject.SetActive(false);
				transform4.GetComponent<Text>().color = Color.white;
				transform5.GetComponent<Text>().color = Color.white;
				transform6.GetComponent<Text>().color = Color.white;
			}
			transform2.gameObject.SetActive(use);
			transform3.gameObject.SetActive(!use);
			transform2.GetComponent<Button>().interactable = !isRecommend && use;
			transform3.GetComponent<Button>().interactable = !isRecommend && !use;
		}

		private void ShowPostFXGrade(PostEffectQualityGrade postFXGrade, bool isRecommend, bool usePostFX)
		{
			ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/PostFXQuality"), isRecommend, usePostFX, postFXGrade == PostEffectQualityGrade.High);
		}

		private void ShowHDR(bool useHDR, bool isRecommend, bool usePostFX)
		{
			ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/HDR"), isRecommend, usePostFX, useHDR);
		}

		private void ShowAA(bool useFXAA, bool isRecommend, bool usePostFX)
		{
			ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/AA"), isRecommend, usePostFX, useFXAA);
		}

		private void ShowDistortion(bool useDistortion, bool isRecommend, bool usePostFX)
		{
			ShowThirdLineElement(base.transform.Find("Content/DetailSetting/ThirdLine/Content/Distortion"), isRecommend, usePostFX, useDistortion);
		}

		private void ShowThirdLineElement(Transform elementTransform, bool isRecommend, bool usePostFX, bool use)
		{
			Transform transform = elementTransform.FindChild("Text");
			Transform transform2 = elementTransform.FindChild("Check");
			if (isRecommend)
			{
				elementTransform.FindChild("Blue").gameObject.SetActive(false);
				elementTransform.FindChild("Grey").gameObject.SetActive(false);
				elementTransform.FindChild("Disable").gameObject.SetActive(true);
				transform.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
				transform2.GetComponent<Image>().color = MiscData.GetColor("GraphicsSettingDisableRadioboxCheck");
			}
			else
			{
				elementTransform.FindChild("Blue").gameObject.SetActive(usePostFX && use);
				elementTransform.FindChild("Grey").gameObject.SetActive(!usePostFX || !use);
				elementTransform.FindChild("Disable").gameObject.SetActive(false);
				transform.GetComponent<Text>().color = Color.white;
				transform2.GetComponent<Image>().color = Color.white;
			}
			transform2.gameObject.SetActive(use);
			elementTransform.FindChild("Button").GetComponent<Button>().interactable = !isRecommend && usePostFX;
		}

		private void ShowEcoModeConfig()
		{
			EnableAllConfigBtns(false);
			ShowEcoModeGradeInfo();
			ConfigGraphicsSetting graphicsEcoModeConfig = GraphicsSettingData.GetGraphicsEcoModeConfig();
			ShowGraphicsSettingConfig(graphicsEcoModeConfig, true);
		}

		private void ShowPersonalConfig()
		{
			EnableAllConfigBtns(true);
			ShowPersonalGradeInfo();
			if (_modifiedPersonalSetting.IsUserDefinedGrade && _modifiedPersonalSetting.IsUserDefinedVolatile)
			{
				return;
			}
			if (_modifiedPersonalSetting.IsUserDefinedGrade || _modifiedPersonalSetting.IsUserDefinedVolatile)
			{
				if (_modifiedPersonalSetting.IsUserDefinedGrade)
				{
					ShowRecommendCompleteConfig(_modifiedPersonalSetting.RecommendGrade);
				}
				else
				{
					ShowGraphicsSettingConfig(_modifiedPersonalSetting, false);
				}
			}
			else
			{
				ConfigGraphicsSetting graphicsPersonalSettingConfig = GraphicsSettingData.GetGraphicsPersonalSettingConfig(_modifiedPersonalSetting);
				ShowGraphicsSettingConfig(graphicsPersonalSettingConfig, false);
			}
		}

		private void ShowRecommendMark()
		{
			GraphicsRecommendGrade graphicsRecommendGrade = GraphicsSettingData.GetGraphicsRecommendGrade();
			for (int i = 0; i < grades.Length - 1; i++)
			{
				Transform transform = grades[i];
				transform.FindChild("Recommend").gameObject.SetActive(false);
			}
			Transform transform2 = grades[(int)graphicsRecommendGrade];
			transform2.FindChild("Recommend").gameObject.SetActive(true);
		}

		private void ShowRecommendGradeInfo(GraphicsRecommendGrade grade)
		{
			for (int i = 0; i < grades.Length; i++)
			{
				if (i != (int)grade)
				{
					Transform transform = grades[i];
					transform.FindChild("Blue").gameObject.SetActive(true);
					transform.FindChild("Check").gameObject.SetActive(false);
					processes[i].gameObject.SetActive(false);
				}
			}
			Transform transform2 = grades[(int)grade];
			transform2.FindChild("Blue").gameObject.SetActive(false);
			transform2.FindChild("Check").gameObject.SetActive(true);
			processes[(int)grade].gameObject.SetActive(true);
		}

		private void ShowPersonalGradeInfo()
		{
			for (int i = 0; i < grades.Length - 1; i++)
			{
				Transform transform = grades[i];
				if (transform.gameObject.activeSelf)
				{
					transform.FindChild("Blue").gameObject.SetActive(true);
					transform.FindChild("Check").gameObject.SetActive(false);
					processes[i].gameObject.SetActive(false);
				}
			}
			if (!isInLevelSimpleSetting)
			{
				Transform transform2 = grades[grades.Length - 1];
				transform2.FindChild("Blue").gameObject.SetActive(false);
				transform2.FindChild("Check").gameObject.SetActive(true);
				processes[grades.Length - 1].gameObject.SetActive(true);
			}
		}

		private void ShowEcoModeGradeInfo()
		{
			for (int i = 0; i < grades.Length; i++)
			{
				grades[i].FindChild("Blue").gameObject.SetActive(true);
				grades[i].FindChild("Check").gameObject.SetActive(false);
				processes[i].gameObject.SetActive(false);
			}
			processes[0].gameObject.SetActive(true);
		}

		private void SetupEcoMode()
		{
			bool isEcoMode = _modifiedPersonalSetting.IsEcoMode;
			if (isInLevelSimpleSetting)
			{
				ecoMode.FindChild("Mode/Desc/On").gameObject.SetActive(isEcoMode);
				ecoMode.FindChild("Mode/Desc/Off").gameObject.SetActive(!isEcoMode);
				Transform transform = ecoMode.Find("Mode/Choice/On");
				Transform transform2 = ecoMode.Find("Mode/Choice/Off");
				transform.FindChild("Blue").gameObject.SetActive(isEcoMode);
				transform.FindChild("Grey").gameObject.SetActive(!isEcoMode);
				transform.FindChild("Disable").gameObject.SetActive(false);
				transform.gameObject.SetActive(isEcoMode);
				transform.GetComponent<Button>().interactable = isEcoMode;
				transform2.FindChild("Grey").gameObject.SetActive(!isEcoMode);
				transform2.FindChild("Disable").gameObject.SetActive(false);
				transform2.gameObject.SetActive(!isEcoMode);
				transform2.GetComponent<Button>().interactable = !isEcoMode;
			}
			else
			{
				ecoMode.FindChild("Desc/ON").gameObject.SetActive(isEcoMode);
				ecoMode.FindChild("Desc/OFF").gameObject.SetActive(!isEcoMode);
				ecoMode.FindChild("Choice/Blue").gameObject.SetActive(isEcoMode);
				ecoMode.FindChild("Choice/Grey").gameObject.SetActive(!isEcoMode);
				ecoMode.FindChild("Choice/Disable").gameObject.SetActive(false);
				ecoMode.FindChild("Choice/Text").GetComponent<Text>().color = Color.white;
				Transform transform3 = ecoMode.FindChild("Choice/Check");
				transform3.GetComponent<Image>().color = Color.white;
				transform3.gameObject.SetActive(isEcoMode);
			}
		}

		private void EnableAllConfigBtns(bool enable)
		{
			Button[] array = configButtons;
			foreach (Button button in array)
			{
				if (button != null)
				{
					button.interactable = enable;
				}
			}
		}
	}
}
