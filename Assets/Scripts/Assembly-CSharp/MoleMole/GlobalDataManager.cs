using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public static class GlobalDataManager
	{
		public const string META_CONFIG_PATH = "Common/MetaConfig";

		public static ConfigMetaConfig metaConfig;

		private static string _loadDataDesc = string.Empty;

		private static float _refreshProgress;

		private static List<string> _dataListMTNecessary = new List<string>();

		private static List<string> _dataListMTNonNecessary = new List<string>();

		private static float _loadDataReaderLastTime = 0f;

		private static float _loadDataReaderTimeDelta = 0f;

		private static int _calculatedContentHash;

		public static bool IsInRefreshDataAsync { get; set; }

		private static float RefreshProgress
		{
			get
			{
				return _refreshProgress;
			}
			set
			{
				_refreshProgress = value;
				if (_refreshProgress > 0.03f)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, _loadDataDesc, true, _refreshProgress)));
				}
			}
		}

		public static int contentHash
		{
			get
			{
				return _calculatedContentHash;
			}
		}

		public static void Refresh()
		{
			LoadDataReader();
			LoadInLevelData();
			CalculateAndStoreLoadedDataContentHash();
		}

		public static void RefreshAsync(Action refreshNecessaryfinishCallback = null)
		{
			if (!IsInRefreshDataAsync)
			{
				Singleton<ApplicationManager>.Instance.StartCoroutine(RefreshAsyncImp(refreshNecessaryfinishCallback));
			}
		}

		private static IEnumerator RefreshAsyncImp(Action refreshNecessaryfinishCallback = null)
		{
			IsInRefreshDataAsync = true;
			RefreshProgress = 0f;
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(LoadDataReaderAsync());
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(LoadInLevelDataAsync());
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(LoadDataUseMultiThread());
			while (_dataListMTNecessary.Count > 0)
			{
				yield return null;
			}
			RefreshProgress = 1f;
			if (refreshNecessaryfinishCallback != null)
			{
				refreshNecessaryfinishCallback();
			}
		}

		private static void LoadDataReader()
		{
			TextMapMetaDataReader.LoadFromFile();
			LocalDataVersion.LoadFromFile();
			MiscData.LoadFromFile();
			NetworkErrCodeMetaDataReader.LoadFromFile();
			PlayerLevelMetaDataReader.LoadFromFile();
			EquipmentLevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
			WeaponMetaDataReaderExtend.LoadFromFileAndBuildMap();
			EndlessToolMetaDataReader.LoadFromFile();
			StigmataMetaDataReaderExtend.LoadFromFileAndBuildMap();
			StigmataAffixMetaDataReader.LoadFromFile();
			ItemMetaDataReader.LoadFromFile();
			AvatarFragmentMetaDataReader.LoadFromFile();
			AvatarCardMetaDataReader.LoadFromFile();
			EquipSkillMetaDataReader.LoadFromFile();
			EquipmentSetMetaDataReader.LoadFromFile();
			MaterialExpBonusMetaDataReader.LoadFromFile();
			MaterialAvatarExpBonusMetaDataReader.LoadFromFile();
			PowerTypeMetaDataReader.LoadFromFile();
			AvatarAttackPunishMetaDataReader.LoadFromFile();
			AvatarDefencePunishMetaDataReader.LoadFromFile();
			AvatarSkillMetaDataReader.LoadFromFile();
			AvatarSubSkillMetaDataReaderExtend.LoadFromFileAndBuildMap();
			AvatarSubSkillLevelMetaDataReader.LoadFromFile();
			ClassMetaDataReader.LoadFromFile();
			AvatarLevelMetaDataReader.LoadFromFile();
			AvatarStarMetaDataReader.LoadFromFile();
			AvatarMetaDataReaderExtend.LoadFromFileAndBuildMap();
			MissionDataReader.LoadFromFile();
			LinearMissionDataReader.LoadFromFile();
			TutorialDataReader.LoadFromFile();
			TutorialStepDataReader.LoadFromFile();
			ChapterMetaDataReader.LoadFromFile();
			LevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
			LevelChallengeMetaDataReader.LoadFromFile();
			LevelTutorialMetaDataReader.LoadFromFile();
			ActMetaDataReader.LoadFromFile();
			BattleTypeMetaDataReader.LoadFromFile();
			LevelResetCostMetaDataReader.LoadFromFile();
			WeekDayActivityMetaDataReader.LoadFromFile();
			SeriesMetaDataReader.LoadFromFile();
			ReviveCostTypeMetaDataReader.LoadFromFile();
			EndlessDropMetaDataReader.LoadFromFile();
			MonsterUIMetaDataReaderExtend.LoadFromFileAndBuildMap();
			MonsterSkillMetaDataReader.LoadFromFile();
			NPCLevelMetaDataReader.LoadFromFile();
			EvenSignInRewardMetaDataReader.LoadFromFile();
			OddSignInRewardMetaDataReader.LoadFromFile();
			GalTouchData.LoadFromFile();
			RewardDataReader.LoadFromFile();
			UnlockUIDataReaderExtend.LoadFromFileAndBuildMap();
			PlotMetaDataReader.LoadFromFile();
			CgMetaDataReader.LoadFromFile();
			DialogMetaDataReader.LoadFromFile();
			CabinLevelMetaDataReader.LoadFromFile();
			CabinExtendGradeMetaDataReader.LoadFromFile();
			CabinTechTreeMetaDataReader.LoadFromFile();
			CabinLevelUpTimePriceMetaDataReader.LoadFromFile();
			CabinPowerCostMetaDataReader.LoadFromFile();
			CabinDisjointEquipmentMetaDataReader.LoadFromFile();
			CabinVentureLevelMetaDataReader.LoadFromFile();
			VentureMetaDataReader.LoadFromFile();
			CabinVentureRefreshMetaDataReader.LoadFromFile();
			ItempediaData.LoadFromFile();
			CabinCollectLevelMetaDataReader.LoadFromFile();
			MaterialVentureSpeedUpDataReader.LoadFromFile();
			ShopGoodsMetaDataReader.LoadFromFile();
			ShopGoodsPriceRateMetaDataReader.LoadFromFile();
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				FakePacketHelper.LoadFromFile();
			}
		}

		private static void LoadInLevelData()
		{
			metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
			AvatarData.ReloadFromFile();
			MonsterData.ReloadFromFile();
			AbilityData.ReloadFromFile();
			EffectData.ReloadFromFile();
			StageData.ReloadFromFile();
			AuxObjectData.ReloadFromFile();
			DynamicObjectData.ReloadFromFile();
			WeaponData.ReloadFromFile();
			EquipmentSkillData.ReloadFromFile();
			GalTouchBuffData.ReloadFromFile();
			PropObjectData.ReloadFromFile();
			RenderingData.ReloadFromFile();
			WeatherData.ReloadFromFile();
			AtmosphereSeriesData.ReloadFromFile();
			AnimatorEventData.ReloadFromFile();
			GraphicsSettingData.ReloadFromFile();
			CameraData.ReloadFromFile();
			TouchPatternData.ReloadFromFile();
			FaceAnimationData.ReloadFromFile();
			SharedAnimEventData.ReloadFromData();
			AIData.ReloadFromFile();
			InLevelData.ReloadFromFile();
		}

		private static IEnumerator LoadDataReaderAsync()
		{
			_loadDataReaderLastTime = Time.realtimeSinceStartup;
			_loadDataReaderTimeDelta = 0f;
			TextMapMetaDataReader.LoadFromFile();
			_loadDataDesc = LocalizationGeneralLogic.GetText("Menu_LoadData");
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			PlayerLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarSkillMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarSubSkillMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarSubSkillLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarAttackPunishMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarDefencePunishMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ClassMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarStarMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			if (GlobalVars.DISABLE_NETWORK_DEBUG)
			{
				FakePacketHelper.LoadFromFile();
				if (CheckMoveToNextFrame())
				{
					yield return null;
				}
			}
			LocalDataVersion.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MiscData.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			LevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			NetworkErrCodeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EquipmentLevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			WeaponMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EndlessToolMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			StigmataMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			StigmataAffixMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ItemMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarFragmentMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			AvatarCardMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EquipSkillMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EquipmentSetMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MaterialExpBonusMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MaterialAvatarExpBonusMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			PowerTypeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MissionDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			LinearMissionDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			TutorialDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			TutorialStepDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ChapterMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			LevelChallengeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			LevelTutorialMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ActMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			BattleTypeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			LevelResetCostMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			WeekDayActivityMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			SeriesMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ReviveCostTypeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EndlessGroupMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EndlessDropMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MonsterUIMetaDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MonsterSkillMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			NPCLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			EvenSignInRewardMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			OddSignInRewardMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			RewardDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			UnlockUIDataReaderExtend.LoadFromFileAndBuildMap();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			GalTouchData.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			PlotMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CgMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			DialogMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinExtendGradeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinTechTreeMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinLevelUpTimePriceMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinPowerCostMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinDisjointEquipmentMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinVentureLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			VentureMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinVentureRefreshMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ItempediaData.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			CabinCollectLevelMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			MaterialVentureSpeedUpDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ShopGoodsMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
			ShopGoodsPriceRateMetaDataReader.LoadFromFile();
			if (CheckMoveToNextFrame())
			{
				yield return null;
			}
		}

		private static bool CheckMoveToNextFrame()
		{
			RefreshProgress += 0.002f;
			float num = ((Application.targetFrameRate <= 0) ? 0.03f : (1f / (float)Application.targetFrameRate));
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - _loadDataReaderLastTime > num)
			{
				_loadDataReaderLastTime = realtimeSinceStartup;
				_loadDataReaderTimeDelta = 0f;
				return true;
			}
			_loadDataReaderLastTime = realtimeSinceStartup;
			_loadDataReaderTimeDelta += realtimeSinceStartup - _loadDataReaderLastTime;
			return false;
		}

		private static IEnumerator LoadInLevelDataAsync()
		{
			AsyncAssetRequst asyncRequest = ConfigUtil.LoadConfigAsync("Common/MetaConfig");
			yield return asyncRequest.operation;
			metaConfig = (ConfigMetaConfig)asyncRequest.asset;
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(EffectData.ReloadFromFileAsync(0.1f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(StageData.ReloadFromFileAsync(0.04f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(AuxObjectData.ReloadFromFileAsync(0.04f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(DynamicObjectData.ReloadFromFileAsync(0.03f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(GalTouchBuffData.ReloadFromFileAsync(0.01f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(RenderingData.ReloadFromFileAsync(0.04f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(WeatherData.ReloadFromFileAsync(0.08f, InlevelDataMoveOneStep));
			AtmosphereSeriesData.ReloadFromFile();
			yield return null;
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(CameraData.ReloadFromFileAsync(0.02f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(AnimatorEventData.ReloadFromFileAsync(0.08f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(TouchPatternData.ReloadFromFileAsync(0.03f, InlevelDataMoveOneStep));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(FaceAnimationData.ReloadFromFileAsync(0.084f, InlevelDataMoveOneStep));
			InLevelData.ReloadFromFile();
		}

		private static void InlevelDataMoveOneStep(float step)
		{
			RefreshProgress += step;
		}

		private static IEnumerator LoadDataUseMultiThread()
		{
			_dataListMTNecessary.Clear();
			_dataListMTNonNecessary.Clear();
			_dataListMTNecessary.Add("GraphicsSettingData");
			_dataListMTNecessary.Add("WeaponData");
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(GraphicsSettingData.ReloadFromFileAsync(0.01f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(WeaponData.ReloadFromFileAsync(0.06f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			_dataListMTNonNecessary.Add("EquipmentSkillData");
			_dataListMTNonNecessary.Add("PropObjectData");
			_dataListMTNonNecessary.Add("SharedAnimEventData");
			_dataListMTNonNecessary.Add("AIData");
			_dataListMTNonNecessary.Add("AvatarData");
			_dataListMTNonNecessary.Add("MonsterData");
			_dataListMTNonNecessary.Add("AbilityData");
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(EquipmentSkillData.ReloadFromFileAsync(0.02f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(PropObjectData.ReloadFromFileAsync(0.02f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(SharedAnimEventData.ReloadFromFileAsync(0.02f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(AIData.ReloadFromFileAsync(0.01f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(AvatarData.ReloadFromFileAsync(0.06f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(MonsterData.ReloadFromFileAsync(0.05f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
			yield return Singleton<ApplicationManager>.Instance.StartCoroutine(AbilityData.ReloadFromFileAsync(0.06f, InlevelDataMoveOneStep, OnLoadOneDataFinish));
		}

		private static void OnLoadOneDataFinish(string dataName)
		{
			if (_dataListMTNecessary.Contains(dataName))
			{
				_dataListMTNecessary.Remove(dataName);
			}
			if (_dataListMTNonNecessary.Contains(dataName))
			{
				_dataListMTNonNecessary.Remove(dataName);
			}
			if (_dataListMTNecessary.Count == 0 && _dataListMTNonNecessary.Count == 0)
			{
				IsInRefreshDataAsync = false;
				CalculateAndStoreLoadedDataContentHash();
			}
		}

		public static void CalculateAndStoreLoadedDataContentHash()
		{
			_calculatedContentHash = HashDataContent();
		}

		public static int HashDataContent()
		{
			int num = 0;
			num ^= ActMetaDataReader.CalculateContentHash();
			num ^= AvatarAttackPunishMetaDataReader.CalculateContentHash();
			num ^= AvatarCardMetaDataReader.CalculateContentHash();
			num ^= AvatarDefencePunishMetaDataReader.CalculateContentHash();
			num ^= AvatarFragmentMetaDataReader.CalculateContentHash();
			num ^= AvatarLevelMetaDataReader.CalculateContentHash();
			num ^= AvatarMetaDataReader.CalculateContentHash();
			num ^= AvatarSkillMetaDataReader.CalculateContentHash();
			num ^= AvatarStarMetaDataReader.CalculateContentHash();
			num ^= AvatarSubSkillLevelMetaDataReader.CalculateContentHash();
			num ^= AvatarSubSkillMetaDataReader.CalculateContentHash();
			num ^= BattleTypeMetaDataReader.CalculateContentHash();
			num ^= ChapterMetaDataReader.CalculateContentHash();
			num ^= ClassMetaDataReader.CalculateContentHash();
			num ^= EndlessToolMetaDataReader.CalculateContentHash();
			num ^= EquipmentLevelMetaDataReader.CalculateContentHash();
			num ^= EquipmentSetMetaDataReader.CalculateContentHash();
			num ^= EquipSkillMetaDataReader.CalculateContentHash();
			num ^= ItemMetaDataReader.CalculateContentHash();
			num ^= LevelChallengeMetaDataReader.CalculateContentHash();
			num ^= LevelMetaDataReader.CalculateContentHash();
			num ^= LinearMissionDataReader.CalculateContentHash();
			num ^= MissionDataReader.CalculateContentHash();
			num ^= MonsterConfigMetaDataReader.CalculateContentHash();
			num ^= MonsterSkillMetaDataReader.CalculateContentHash();
			num ^= NPCLevelMetaDataReader.CalculateContentHash();
			num ^= PlayerLevelMetaDataReader.CalculateContentHash();
			num ^= ReviveCostTypeMetaDataReader.CalculateContentHash();
			num ^= RewardDataReader.CalculateContentHash();
			num ^= SeriesMetaDataReader.CalculateContentHash();
			num ^= StigmataAffixMetaDataReader.CalculateContentHash();
			num ^= StigmataMetaDataReader.CalculateContentHash();
			num ^= UniqueMonsterMetaDataReader.CalculateContentHash();
			num ^= VentureMetaDataReader.CalculateContentHash();
			num ^= WeaponMetaDataReader.CalculateContentHash();
			num ^= AvatarData.CalculateContentHash();
			num ^= MonsterData.CalculateContentHash();
			num ^= AbilityData.CalculateContentHash();
			return num ^ PropObjectData.CalculateContentHash();
		}
	}
}
