using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class GraphicsSettingData
	{
		private static ConfigOverrideGroup _recommendVolatileSettingGroup;

		private static Dictionary<string, ConfigOverrideGroup> _recommendSettingGroupMap;

		private static ConfigGraphicsRecommendSetting _recommendSetting;

		private static string _recommendSettingName = "Default";

		private static bool _hasSettingGrade = false;

		private static GraphicsRecommendGrade _recommendGrade;

		private static bool _hasGetGyroscope;

		private static bool _enableGyroscope;

		private static Action<string> _loadJsonConfigCallback = null;

		private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();

		public static void ReloadFromFile()
		{
			_recommendVolatileSettingGroup = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>(GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath);
			_recommendSettingGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			string[] graphicsSettingRegistryPathes = GlobalDataManager.metaConfig.graphicsSettingRegistryPathes;
			string targetPlatform = GetTargetPlatform();
			if (targetPlatform == string.Empty)
			{
				return;
			}
			string[] array = graphicsSettingRegistryPathes;
			foreach (string text in array)
			{
				if (text.Contains(targetPlatform))
				{
					ConfigOverrideGroup value = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>(text);
					string[] array2 = text.Split('/');
					string key = array2[array2.Length - 1];
					_recommendSettingGroupMap.Add(key, value);
				}
			}
			_recommendSetting = null;
			_hasSettingGrade = false;
			_hasGetGyroscope = false;
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
		{
			_loadJsonConfigCallback = finishCallback;
			AsyncAssetRequst asyncRequest = ConfigUtil.LoadJsonConfigAsync(GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath);
			SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null graphicsVolatileSettingRegistryPath :" + GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath);
			if (asyncRequest == null)
			{
				yield break;
			}
			yield return asyncRequest.operation;
			_recommendVolatileSettingGroup = ConfigUtil.LoadJSONStrConfig<ConfigOverrideGroup>(asyncRequest.asset.ToString());
			_recommendSettingGroupMap = new Dictionary<string, ConfigOverrideGroup>();
			string[] graphicsSettingRegistryPathes = GlobalDataManager.metaConfig.graphicsSettingRegistryPathes;
			float step = progressSpan / (float)graphicsSettingRegistryPathes.Length;
			string targetPlatform = GetTargetPlatform();
			if (targetPlatform == string.Empty)
			{
				yield break;
			}
			_loadDataBackGroundWorker.StartBackGroundWork("GraphicsSettingData");
			string[] array = graphicsSettingRegistryPathes;
			foreach (string configFilePath in array)
			{
				if (!configFilePath.Contains(targetPlatform))
				{
					continue;
				}
				asyncRequest = ConfigUtil.LoadJsonConfigAsync(configFilePath);
				SuperDebug.VeryImportantAssert(asyncRequest != null, "assetRequest is null graphicsPath :" + configFilePath);
				if (asyncRequest != null)
				{
					yield return asyncRequest.operation;
					if (moveOneStepCallback != null)
					{
						moveOneStepCallback(step);
					}
					ConfigUtil.LoadJSONStrConfigMultiThread<ConfigOverrideGroup>(asyncRequest.asset.ToString(), _loadDataBackGroundWorker, OnLoadOneJsonConfigFinish, configFilePath);
				}
				break;
			}
		}

		private static void OnLoadOneJsonConfigFinish(ConfigOverrideGroup configGroup, string configPath)
		{
			string[] array = configPath.Split('/');
			string key = array[array.Length - 1];
			_recommendSettingGroupMap.Add(key, configGroup);
			_loadDataBackGroundWorker.StopBackGroundWork(false);
			_recommendSetting = null;
			_hasSettingGrade = false;
			_hasGetGyroscope = false;
			if (_loadJsonConfigCallback != null)
			{
				_loadJsonConfigCallback("GraphicsSettingData");
				_loadJsonConfigCallback = null;
			}
		}

		public static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig()
		{
			string targetPlatform = GetTargetPlatform();
			return GetGraphicsRecommendCompleteConfig(targetPlatform, SystemInfo.deviceModel);
		}

		public static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(GraphicsRecommendGrade grade)
		{
			string targetPlatform = GetTargetPlatform();
			return GetGraphicsRecommendCompleteConfig(targetPlatform, SystemInfo.deviceModel, grade);
		}

		public static ConfigGraphicsSetting GetGraphicsEcoModeConfig()
		{
			string targetPlatform = GetTargetPlatform();
			return GetGraphicsEcoModeConfig(targetPlatform);
		}

		public static bool IsEnableGyroscope()
		{
			if (_hasGetGyroscope)
			{
				return _enableGyroscope;
			}
			string targetPlatform = GetTargetPlatform();
			if (!_recommendSettingGroupMap.ContainsKey(targetPlatform))
			{
				_hasGetGyroscope = true;
				_enableGyroscope = true;
				return _enableGyroscope;
			}
			ConfigGraphicsRecommendSetting config = _recommendSettingGroupMap[targetPlatform].GetConfig<ConfigGraphicsRecommendSetting>("Gyroscope");
			if (config == null)
			{
				_hasGetGyroscope = true;
				config = (ConfigGraphicsRecommendSetting)_recommendSettingGroupMap[targetPlatform].Default;
				return config.EnableGyroscope;
			}
			_hasGetGyroscope = true;
			List<string> excludeDeviceModels = config.ExcludeDeviceModels;
			string deviceModel = SystemInfo.deviceModel;
			foreach (string item in excludeDeviceModels)
			{
				if (deviceModel.ToLower() == item.ToLower())
				{
					_enableGyroscope = false;
					return _enableGyroscope;
				}
			}
			_enableGyroscope = true;
			return _enableGyroscope;
		}

		public static ConfigGraphicsSetting GetGraphicsPersonalSettingConfig(ConfigGraphicsPersonalSetting personalSetting)
		{
			if (personalSetting.IsUserDefinedGrade && personalSetting.IsUserDefinedVolatile)
			{
				Debug.LogError("IsUserDefinedGrade and IsUserDefinedVolatile both true");
				return null;
			}
			if (!personalSetting.IsUserDefinedGrade && !personalSetting.IsUserDefinedVolatile)
			{
				return GetGraphicsRecommendCompleteConfig();
			}
			if (personalSetting.IsUserDefinedGrade)
			{
				return GetGraphicsRecommendCompleteConfig(personalSetting.RecommendGrade);
			}
			ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
			personalSetting.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
			personalSetting.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
			personalSetting.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
			personalSetting.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
			return personalSetting;
		}

		public static GraphicsRecommendGrade GetGraphicsRecommendGrade()
		{
			if (_hasSettingGrade)
			{
				return _recommendGrade;
			}
			ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
			_hasSettingGrade = true;
			_recommendGrade = graphicsRecommendConfig.RecommendGrade;
			return _recommendGrade;
		}

		public static string GetGraphicsRecommendSettingName()
		{
			return _recommendSettingName;
		}

		public static void ApplySettingConfig()
		{
			if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode)
			{
				ConfigGraphicsSetting graphicsEcoModeConfig = GetGraphicsEcoModeConfig();
				ApplySettingConfig(graphicsEcoModeConfig);
			}
			else if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsUserDefinedGrade || Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsUserDefinedVolatile)
			{
				ConfigGraphicsSetting graphicsPersonalSettingConfig = GetGraphicsPersonalSettingConfig(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting);
				ApplySettingConfig(graphicsPersonalSettingConfig);
			}
			else
			{
				ConfigGraphicsSetting graphicsRecommendCompleteConfig = GetGraphicsRecommendCompleteConfig();
				ApplySettingConfig(graphicsRecommendCompleteConfig);
			}
			ApplyPersonalContrastDelta();
		}

		public static void ApplySettingConfig(ConfigGraphicsSetting setting)
		{
			GraphicsSettingUtil.SetTargetFrameRate(setting.TargetFrameRate);
			bool forceWhenDisable = true;
			GraphicsSettingUtil.EnablePostFX(setting.VolatileSetting.UsePostFX, forceWhenDisable);
			GraphicsSettingUtil.ApplyResolution(setting.ResolutionPercentage, setting.ResolutionQuality, setting.RecommendResolutionX, setting.RecommendResolutionY);
			GraphicsSettingUtil.SetPostEffectBufferSizeByQuality(setting.PostFxGradeBufferSize, setting.VolatileSetting.PostFXGrade);
			GraphicsSettingUtil.EnableHDR(setting.VolatileSetting.UseHDR);
			GraphicsSettingUtil.EnableDistortion(setting.VolatileSetting.UseDistortion);
			GraphicsSettingUtil.EnableReflection(setting.VolatileSetting.UseReflection);
			GraphicsSettingUtil.EnableFXAA(setting.VolatileSetting.UseFXAA);
			GraphicsSettingUtil.EnableDynamicBone(setting.VolatileSetting.UseDynamicBone);
			GraphicsSettingUtil.EnableStaticCloudMode(!setting.VolatileSetting.UseDynamicBone);
		}

		public static void ApplyPersonalContrastDelta()
		{
			if (Singleton<MiHoYoGameData>.Instance != null)
			{
				GraphicsSettingUtil.SetPostFXContrast(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta);
			}
		}

		public static void SavePersonalConfigIgnoreContrast(ConfigGraphicsPersonalSetting settingConfig)
		{
			if (!Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode || !settingConfig.IsEcoMode)
			{
				if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode != settingConfig.IsEcoMode)
				{
					Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode = settingConfig.IsEcoMode;
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EcoModeVisible, settingConfig.IsEcoMode));
				}
				if (settingConfig.IsEcoMode)
				{
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					ApplySettingConfig();
				}
				else if (settingConfig.IsUserDefinedVolatile)
				{
					SavePersonalConfigIgnoreContrast((ConfigGraphicsSetting)settingConfig);
				}
				else if (settingConfig.IsUserDefinedGrade)
				{
					SavePersonalConfigIgnoreContrast(settingConfig.RecommendGrade);
				}
				else
				{
					Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
					ApplySettingConfig();
				}
			}
		}

		public static void SavePersonalContrastDelta(float contrastDelta)
		{
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			personalGraphicsSetting.ContrastDelta = contrastDelta;
			Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			ApplyPersonalContrastDelta();
		}

		public static void CopyPersonalGraphicsConfig(ref ConfigGraphicsPersonalSetting to)
		{
			CopyPersonalGraphicsConfig(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode, ref to);
		}

		public static void CopyPersonalGraphicsConfig(bool isEcoMode, ref ConfigGraphicsPersonalSetting to)
		{
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			if (isEcoMode)
			{
				ConfigGraphicsSetting graphicsEcoModeConfig = GetGraphicsEcoModeConfig();
				CopyToPersonalGraphicsConfig(graphicsEcoModeConfig, ref to);
				to.IsUserDefinedGrade = personalGraphicsSetting.IsUserDefinedGrade;
				to.IsUserDefinedVolatile = personalGraphicsSetting.IsUserDefinedVolatile;
			}
			else if (personalGraphicsSetting.IsUserDefinedGrade && personalGraphicsSetting.IsUserDefinedVolatile)
			{
				Debug.LogError("IsUserDefinedGrade and IsUserDefinedVolatile both true");
			}
			else if (!personalGraphicsSetting.IsUserDefinedGrade && !personalGraphicsSetting.IsUserDefinedVolatile)
			{
				ConfigGraphicsSetting graphicsRecommendCompleteConfig = GetGraphicsRecommendCompleteConfig();
				CopyToPersonalGraphicsConfig(graphicsRecommendCompleteConfig, ref to);
				to.IsUserDefinedGrade = false;
				to.IsUserDefinedVolatile = false;
			}
			else if (personalGraphicsSetting.IsUserDefinedGrade)
			{
				ConfigGraphicsSetting graphicsRecommendCompleteConfig2 = GetGraphicsRecommendCompleteConfig(personalGraphicsSetting.RecommendGrade);
				CopyToPersonalGraphicsConfig(graphicsRecommendCompleteConfig2, ref to);
				to.RecommendGrade = personalGraphicsSetting.RecommendGrade;
				to.IsUserDefinedGrade = true;
				to.IsUserDefinedVolatile = false;
			}
			else
			{
				ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
				to.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
				to.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
				to.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
				to.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
				to.ResolutionQuality = personalGraphicsSetting.ResolutionQuality;
				to.TargetFrameRate = personalGraphicsSetting.TargetFrameRate;
				to.ContrastDelta = 0f;
				to.VolatileSetting = new ConfigGraphicsVolatileSetting();
				CopyGraphicsVolatileConfig(personalGraphicsSetting.VolatileSetting, ref to.VolatileSetting);
				to.RecommendGrade = personalGraphicsSetting.RecommendGrade;
				to.IsUserDefinedGrade = false;
				to.IsUserDefinedVolatile = true;
			}
			to.IsEcoMode = isEcoMode;
		}

		public static void CopyToPersonalGraphicsConfig(ConfigGraphicsSetting setting, ref ConfigGraphicsPersonalSetting to)
		{
			ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
			to.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
			to.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
			to.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
			to.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
			to.ResolutionQuality = setting.ResolutionQuality;
			to.TargetFrameRate = setting.TargetFrameRate;
			to.ContrastDelta = setting.ContrastDelta;
			to.VolatileSetting = new ConfigGraphicsVolatileSetting();
			CopyGraphicsVolatileConfig(setting.VolatileSetting, ref to.VolatileSetting);
		}

		public static void CopyPersonalContrastDelta(ref float contrastDelta)
		{
			contrastDelta = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta;
		}

		public static bool IsEqualToPersonalConfigIgnoreContrast(ConfigGraphicsPersonalSetting to)
		{
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			if (personalGraphicsSetting.IsEcoMode != to.IsEcoMode)
			{
				return false;
			}
			if (personalGraphicsSetting.IsEcoMode && to.IsEcoMode)
			{
				return true;
			}
			return (!personalGraphicsSetting.IsUserDefinedGrade && !to.IsUserDefinedGrade && !personalGraphicsSetting.IsUserDefinedVolatile && !to.IsUserDefinedVolatile) || (personalGraphicsSetting.IsUserDefinedGrade && to.IsUserDefinedGrade && personalGraphicsSetting.RecommendGrade == to.RecommendGrade) || (personalGraphicsSetting.IsUserDefinedVolatile && to.IsUserDefinedVolatile && personalGraphicsSetting.ResolutionQuality == to.ResolutionQuality && personalGraphicsSetting.TargetFrameRate == to.TargetFrameRate && personalGraphicsSetting.VolatileSetting.PostFXGrade == to.VolatileSetting.PostFXGrade && personalGraphicsSetting.VolatileSetting.UseDistortion == to.VolatileSetting.UseDistortion && personalGraphicsSetting.VolatileSetting.UseDynamicBone == to.VolatileSetting.UseDynamicBone && personalGraphicsSetting.VolatileSetting.UseFXAA == to.VolatileSetting.UseFXAA && personalGraphicsSetting.VolatileSetting.UseHDR == to.VolatileSetting.UseHDR && personalGraphicsSetting.VolatileSetting.UsePostFX == to.VolatileSetting.UsePostFX && personalGraphicsSetting.VolatileSetting.UseReflection == to.VolatileSetting.UseReflection);
		}

		public static bool IsEqualToPersonalContrastDelta(float contrastDelta)
		{
			return contrastDelta == Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta;
		}

		private static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(string platformName, string deviceModel)
		{
			if (!_recommendSettingGroupMap.ContainsKey(platformName))
			{
				return null;
			}
			ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig(platformName, deviceModel);
			ConfigGraphicsSetting configGraphicsSetting = new ConfigGraphicsSetting();
			configGraphicsSetting.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
			configGraphicsSetting.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
			configGraphicsSetting.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
			configGraphicsSetting.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
			configGraphicsSetting.ResolutionQuality = graphicsRecommendConfig.ResolutionQuality;
			configGraphicsSetting.TargetFrameRate = graphicsRecommendConfig.TargetFrameRate;
			configGraphicsSetting.ContrastDelta = 0f;
			ConfigGraphicsVolatileSetting config = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>(graphicsRecommendConfig.RecommendGrade.ToString());
			configGraphicsSetting.VolatileSetting = new ConfigGraphicsVolatileSetting();
			CopyGraphicsVolatileConfig(config, ref configGraphicsSetting.VolatileSetting);
			return configGraphicsSetting;
		}

		private static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(string platformName, string deviceModel, GraphicsRecommendGrade grade)
		{
			if (!_recommendSettingGroupMap.ContainsKey(platformName))
			{
				return null;
			}
			ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig(platformName, deviceModel);
			ConfigGraphicsSetting configGraphicsSetting = new ConfigGraphicsSetting();
			configGraphicsSetting.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
			configGraphicsSetting.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
			configGraphicsSetting.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
			configGraphicsSetting.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
			configGraphicsSetting.ResolutionQuality = graphicsRecommendConfig.ResolutionQuality;
			configGraphicsSetting.TargetFrameRate = graphicsRecommendConfig.TargetFrameRate;
			configGraphicsSetting.ContrastDelta = 0f;
			ConfigGraphicsVolatileSetting config = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>(grade.ToString());
			configGraphicsSetting.VolatileSetting = new ConfigGraphicsVolatileSetting();
			CopyGraphicsVolatileConfig(config, ref configGraphicsSetting.VolatileSetting);
			return configGraphicsSetting;
		}

		private static ConfigGraphicsSetting GetGraphicsEcoModeConfig(string platformName)
		{
			if (!_recommendSettingGroupMap.ContainsKey(platformName))
			{
				return null;
			}
			bool flag = false;
			ConfigGraphicsRecommendSetting configGraphicsRecommendSetting = _recommendSettingGroupMap[platformName].GetConfig<ConfigGraphicsRecommendSetting>("EcoMode");
			if (configGraphicsRecommendSetting == null)
			{
				flag = true;
				configGraphicsRecommendSetting = (ConfigGraphicsRecommendSetting)_recommendSettingGroupMap[platformName].Default;
			}
			ConfigGraphicsSetting configGraphicsSetting = new ConfigGraphicsSetting();
			configGraphicsSetting.PostFxGradeBufferSize = configGraphicsRecommendSetting.PostFxGradeBufferSize;
			configGraphicsSetting.RecommendResolutionX = configGraphicsRecommendSetting.RecommendResolutionX;
			configGraphicsSetting.RecommendResolutionY = configGraphicsRecommendSetting.RecommendResolutionY;
			configGraphicsSetting.ResolutionPercentage = configGraphicsRecommendSetting.ResolutionPercentage;
			configGraphicsSetting.ResolutionQuality = ((!flag) ? configGraphicsRecommendSetting.ResolutionQuality : ResolutionQualityGrade.Low);
			configGraphicsSetting.TargetFrameRate = ((!flag) ? configGraphicsRecommendSetting.TargetFrameRate : 30);
			configGraphicsSetting.ContrastDelta = 0f;
			GraphicsRecommendGrade graphicsRecommendGrade = ((!flag) ? configGraphicsRecommendSetting.RecommendGrade : GraphicsRecommendGrade.Off);
			ConfigGraphicsVolatileSetting config = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>(graphicsRecommendGrade.ToString());
			configGraphicsSetting.VolatileSetting = new ConfigGraphicsVolatileSetting();
			CopyGraphicsVolatileConfig(config, ref configGraphicsSetting.VolatileSetting);
			GraphicsRecommendGrade graphicsRecommendGrade2 = GetGraphicsRecommendGrade();
			configGraphicsSetting.VolatileSetting.UsePostFX = graphicsRecommendGrade2 >= GraphicsRecommendGrade.High;
			configGraphicsSetting.VolatileSetting.UseHDR = graphicsRecommendGrade2 >= GraphicsRecommendGrade.High;
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			if (personalGraphicsSetting.IsUserDefinedVolatile && !personalGraphicsSetting.VolatileSetting.UsePostFX)
			{
				configGraphicsSetting.VolatileSetting.UsePostFX = false;
			}
			if (personalGraphicsSetting.IsUserDefinedVolatile && !personalGraphicsSetting.VolatileSetting.UseHDR)
			{
				configGraphicsSetting.VolatileSetting.UseHDR = false;
			}
			return configGraphicsSetting;
		}

		private static ConfigGraphicsRecommendSetting GetGraphicsRecommendConfig()
		{
			string targetPlatform = GetTargetPlatform();
			return GetGraphicsRecommendConfig(targetPlatform, SystemInfo.deviceModel);
		}

		private static ConfigGraphicsRecommendSetting GetGraphicsRecommendConfig(string platformName, string deviceModel)
		{
			if (!_recommendSettingGroupMap.ContainsKey(platformName))
			{
				return null;
			}
			if (platformName == "PC" || platformName == "IOS")
			{
				_recommendSettingName = deviceModel;
				return _recommendSettingGroupMap[platformName].GetConfig<ConfigGraphicsRecommendSetting>(deviceModel);
			}
			if (_recommendSetting == null)
			{
				ConfigOverrideGroup configOverrideGroup = _recommendSettingGroupMap[platformName];
				if (configOverrideGroup.Overrides != null && configOverrideGroup.Overrides.Count > 0)
				{
					string graphicsDeviceName = GetGraphicsDeviceName();
					string[] names = Enum.GetNames(typeof(GraphicsRecommendGrade));
					for (int num = names.Length - 1; num >= 0; num--)
					{
						string text = graphicsDeviceName + " " + names[num];
						if (configOverrideGroup.Overrides.ContainsKey(text))
						{
							ConfigGraphicsRecommendSetting configGraphicsRecommendSetting = (ConfigGraphicsRecommendSetting)configOverrideGroup.Overrides[text];
							if (configGraphicsRecommendSetting.MatchRequirements())
							{
								_recommendSetting = configGraphicsRecommendSetting;
								_recommendSettingName = text;
								return _recommendSetting;
							}
						}
					}
					for (int num2 = names.Length - 1; num2 >= 0; num2--)
					{
						string text2 = names[num2];
						if (configOverrideGroup.Overrides.ContainsKey(text2))
						{
							ConfigGraphicsRecommendSetting configGraphicsRecommendSetting2 = (ConfigGraphicsRecommendSetting)configOverrideGroup.Overrides[text2];
							if (configGraphicsRecommendSetting2.MatchRequirements())
							{
								_recommendSetting = configGraphicsRecommendSetting2;
								_recommendSettingName = text2;
								return _recommendSetting;
							}
						}
					}
				}
				_recommendSetting = (ConfigGraphicsRecommendSetting)configOverrideGroup.Default;
				_recommendSettingName = "Default";
			}
			return _recommendSetting;
		}

		private static string GetGraphicsDeviceName()
		{
			for (int i = 0; i < SystemInfo.graphicsDeviceName.Length; i++)
			{
				if (!char.IsLetter(SystemInfo.graphicsDeviceName[i]))
				{
					return SystemInfo.graphicsDeviceName.Substring(0, i);
				}
			}
			return SystemInfo.graphicsDeviceName;
		}

		private static string GetTargetPlatform()
		{
			switch (SystemInfo.deviceType)
			{
			case DeviceType.Unknown:
				Debug.LogWarning("unknown device type!");
				return string.Empty;
			case DeviceType.Handheld:
			{
				string deviceModel = SystemInfo.deviceModel;
				if (deviceModel.StartsWith("iPhone") || deviceModel.StartsWith("iPod") || deviceModel.StartsWith("iPad"))
				{
					return "IOS";
				}
				return "Android";
			}
			case DeviceType.Desktop:
				return "PC";
			case DeviceType.Console:
				Debug.LogWarning("device type is Console, we do not know how to set graphics!");
				return string.Empty;
			default:
				return string.Empty;
			}
		}

		private static void CopyGraphicsVolatileConfig(ConfigGraphicsVolatileSetting from, ref ConfigGraphicsVolatileSetting to)
		{
			to.PostFXGrade = from.PostFXGrade;
			to.UsePostFX = from.UsePostFX;
			to.UseDistortion = from.UseDistortion;
			to.UseDynamicBone = from.UseDynamicBone;
			to.UseFXAA = from.UseFXAA;
			to.UseHDR = from.UseHDR;
			to.UseReflection = from.UseReflection;
		}

		private static void SavePersonalConfigIgnoreContrast(ConfigGraphicsSetting settingConfig)
		{
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			personalGraphicsSetting.ResolutionQuality = settingConfig.ResolutionQuality;
			personalGraphicsSetting.TargetFrameRate = settingConfig.TargetFrameRate;
			personalGraphicsSetting.VolatileSetting = new ConfigGraphicsVolatileSetting();
			CopyGraphicsVolatileConfig(settingConfig.VolatileSetting, ref personalGraphicsSetting.VolatileSetting);
			personalGraphicsSetting.IsUserDefinedGrade = false;
			personalGraphicsSetting.IsUserDefinedVolatile = true;
			Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			ApplySettingConfig();
		}

		private static void SavePersonalConfigIgnoreContrast(GraphicsRecommendGrade grade)
		{
			ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
			personalGraphicsSetting.RecommendGrade = grade;
			personalGraphicsSetting.IsUserDefinedGrade = true;
			personalGraphicsSetting.IsUserDefinedVolatile = false;
			Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			ApplySettingConfig();
		}
	}
}
