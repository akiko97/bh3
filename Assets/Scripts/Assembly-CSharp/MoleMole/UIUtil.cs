using System;
using System.Collections.Generic;
using MoleMole.MainMenu;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class UIUtil
	{
		public static void ShowItemDetail(StorageDataItemBase item, bool hideActionBtns = false, bool unlock = true)
		{
			if (item is WeaponDataItem || item is StigmataDataItem)
			{
				Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(item, hideActionBtns, unlock));
			}
			else if (item is AvatarCardDataItem)
			{
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
				Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(dummyAvatarDataItem));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ItemDetailDialogContext(item, hideActionBtns));
			}
		}

		public static void ShowResourceDetail(RewardUIData resourceData)
		{
			if (resourceData.rewardType == ResourceType.Item)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(resourceData.itemID, resourceData.level);
				dummyStorageDataItem.number = resourceData.value;
				ShowItemDetail(dummyStorageDataItem, true);
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ResourceDetailDialogContext(resourceData));
			}
		}

		public static string GetDifficultyMark(LevelDiffculty difficulty)
		{
			string result = "N";
			switch (difficulty)
			{
			case LevelDiffculty.Hard:
				result = "H";
				break;
			case LevelDiffculty.Hell:
				result = "S";
				break;
			}
			return result;
		}

		public static LevelDiffculty GetDifficultyFromMark(char mark)
		{
			LevelDiffculty result = LevelDiffculty.Normal;
			switch (mark)
			{
			case 'H':
				result = LevelDiffculty.Hard;
				break;
			case 'S':
				result = LevelDiffculty.Hell;
				break;
			}
			return result;
		}

		public static Color SetupColor(string hexString)
		{
			Color color = Color.white;
			if (ColorUtility.TryParseHtmlString(hexString, out color))
			{
				return color;
			}
			return color;
		}

		public static bool ShowFriendDetailInfo(FriendDetailDataItem detailData, bool fromDialog = false, Transform dialogTrans = null)
		{
			RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(detailData, fromDialog, dialogTrans);
			Singleton<MainUIManager>.Instance.ShowPage(context);
			return false;
		}

		public static void Create3DAvatarByPage(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName = "Default")
		{
			ConfigPageAvatarShowInfo pageAvatarShowInfo = MiscData.GetPageAvatarShowInfo(pageKey);
			ConfigAvatarShowInfo avatarShowInfo = GetAvatarShowInfo(avatar, pageKey, tabName);
			List<Avatar3dModelDataItem> list = new List<Avatar3dModelDataItem>();
			list.Add(new Avatar3dModelDataItem(avatar, avatarShowInfo.Avatar.Position, avatarShowInfo.Avatar.EulerAngle, pageAvatarShowInfo.ShowLockViewIfLock));
			List<Avatar3dModelDataItem> body = list;
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.CreateAvatarUIModels, body));
			GameObject gameObject = GameObject.Find("LookAt");
			if (gameObject != null)
			{
				gameObject.transform.position = avatarShowInfo.LookAt.Position;
				gameObject.transform.eulerAngles = avatarShowInfo.LookAt.EulerAngle;
			}
		}

		public static ConfigAvatarShowInfo GetAvatarShowInfo(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName)
		{
			ConfigPageAvatarShowInfo pageAvatarShowInfo = MiscData.GetPageAvatarShowInfo(pageKey);
			string avatarRegistryKey = avatar.AvatarRegistryKey;
			string[] array = avatarRegistryKey.Split('_');
			string key = array[0];
			ConfigTabAvatarTransformInfo configTabAvatarTransformInfo = (pageAvatarShowInfo.AvatarTabTransformInfos.ContainsKey(avatarRegistryKey) ? pageAvatarShowInfo.AvatarTabTransformInfos[avatarRegistryKey] : ((!pageAvatarShowInfo.AvatarTabTransformInfos.ContainsKey(key)) ? pageAvatarShowInfo.AvatarTabTransformInfos["Default"] : pageAvatarShowInfo.AvatarTabTransformInfos[key]));
			string key2 = tabName + "_" + avatar.avatarID;
			if (configTabAvatarTransformInfo.AvatarShowInfos.ContainsKey(key2))
			{
				return configTabAvatarTransformInfo.AvatarShowInfos[key2];
			}
			if (configTabAvatarTransformInfo.AvatarShowInfos.ContainsKey(tabName))
			{
				return configTabAvatarTransformInfo.AvatarShowInfos[tabName];
			}
			return configTabAvatarTransformInfo.AvatarShowInfos["Default"];
		}

		public static void SetCameraLookAt(AvatarDataItem avatar, MiscData.PageInfoKey pageKey, string tabName)
		{
			ConfigAvatarShowInfo avatarShowInfo = GetAvatarShowInfo(avatar, pageKey, tabName);
			Transform transform = GameObject.Find("LookAt").transform;
			transform.position = avatarShowInfo.LookAt.Position;
			transform.eulerAngles = avatarShowInfo.LookAt.EulerAngle;
		}

		public static void SetAvatarTattooVisible(bool visible, AvatarDataItem avatarData)
		{
			BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarData.avatarID);
			if (!(uIAvatar == null))
			{
				uIAvatar.tattooVisible = visible;
				uIAvatar.avatarData = avatarData;
				Transform uIAvatarTattooByID = GetUIAvatarTattooByID(avatarData.avatarID, "Stigmata");
				if (uIAvatarTattooByID != null)
				{
					uIAvatarTattooByID.gameObject.SetActive(visible);
				}
				if (visible)
				{
					UpdateAvatarTattoo(avatarData);
				}
			}
		}

		public static void UpdateAvatarTattoo(AvatarDataItem avatarData)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if (avatarData == null)
			{
				return;
			}
			Transform uIAvatarTattooByID = GetUIAvatarTattooByID(avatarData.avatarID, "Stigmata");
			if (uIAvatarTattooByID == null)
			{
				return;
			}
			BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarData.avatarID);
			if (uIAvatar == null)
			{
				return;
			}
			foreach (KeyValuePair<EquipmentSlot, StigmataDataItem> item in avatarData.GetStigmataDict())
			{
				Transform uIAvatarTattooByID2 = GetUIAvatarTattooByID(avatarData.avatarID, ((Enum)item.Key).ToString());
				if (!(uIAvatarTattooByID2 == null))
				{
					uIAvatarTattooByID2.gameObject.SetActive(item.Value != null);
					if (item.Value != null)
					{
						UnityEngine.Material material = uIAvatarTattooByID2.GetComponent<MeshRenderer>().material;
						material.SetTexture("_MainTex", Miscs.LoadResource<Texture>(item.Value.GetTattooPath()));
						uIAvatar.StigmataFadeIn(item.Key);
					}
				}
			}
		}

		public static Transform GetUIAvatarTattooByID(int avatarID, string attachmentName)
		{
			BaseMonoUIAvatar uIAvatar = GetUIAvatar(avatarID);
			if (uIAvatar == null)
			{
				return null;
			}
			if (!uIAvatar.HasAttachPoint(attachmentName))
			{
				return null;
			}
			return uIAvatar.GetAttachPoint(attachmentName);
		}

		public static bool ContainsUIAvatar(int avatarID)
		{
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			if (sceneCanvas is MonoMainCanvas)
			{
				return ((MonoMainCanvas)sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
			}
			if (sceneCanvas is MonoTestUI)
			{
				return ((MonoTestUI)sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
			}
			return ((MonoGameEntry)sceneCanvas).avatar3dModelContext.ContainUIAvatar(avatarID);
		}

		public static BaseMonoUIAvatar GetUIAvatar(int avatarID)
		{
			BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
			Transform transform = ((sceneCanvas is MonoMainCanvas) ? ((MonoMainCanvas)sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID) : ((!(sceneCanvas is MonoTestUI)) ? ((MonoGameEntry)sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID) : ((MonoTestUI)sceneCanvas).avatar3dModelContext.GetAvatarById(avatarID)));
			if (transform == null)
			{
				return null;
			}
			return transform.GetComponent<BaseMonoUIAvatar>();
		}

		public static void CalCulateExpFromItems(out float scoinNeed, out float expGet, List<StorageDataItemBase> dogFoodList, StorageDataItemBase equipToPowerUp)
		{
			float num = 0f;
			foreach (StorageDataItemBase dogFood in dogFoodList)
			{
				float num2 = dogFood.GetGearExp();
				MaterialExpBonusMetaData materialExpBonusMetaData = MaterialExpBonusMetaDataReader.TryGetMaterialExpBonusMetaDataByKey(dogFood.ID);
				if (materialExpBonusMetaData != null)
				{
					if (equipToPowerUp is WeaponDataItem)
					{
						num2 *= materialExpBonusMetaData.weaponExpBonus / 100f;
					}
					else if (equipToPowerUp is StigmataDataItem)
					{
						num2 *= materialExpBonusMetaData.stigmataExpBonus / 100f;
					}
				}
				if (dogFood.GetType() == equipToPowerUp.GetType())
				{
					num2 *= (float)Singleton<PlayerModule>.Instance.playerData.sameTypePowerUpRataInt / 100f;
				}
				num += num2;
			}
			expGet = Mathf.FloorToInt(num);
			scoinNeed = expGet * (float)Singleton<PlayerModule>.Instance.playerData.powerUpScoinCostRate / 100f;
		}

		public static int CalculateLvWithExp(float exp, StorageDataItemBase equipToPowerUp)
		{
			List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
			int expType = equipToPowerUp.GetExpType();
			int maxLevel = equipToPowerUp.GetMaxLevel();
			float num = exp + (float)equipToPowerUp.exp;
			int num2 = equipToPowerUp.level;
			while (num > 0f && num2 < maxLevel)
			{
				int num3 = itemList[num2 - 1].expList[expType];
				if ((float)num3 <= num)
				{
					num -= (float)num3;
					num2++;
					continue;
				}
				break;
			}
			return num2;
		}

		public static List<float> GetEquipmentMaxExpList(StorageDataItemBase equipToPowerUp, int fromLevel, int toLevel)
		{
			List<float> list = new List<float>();
			List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
			int expType = equipToPowerUp.GetExpType();
			for (int i = fromLevel; i <= toLevel; i++)
			{
				list.Add(itemList[i - 1].expList[expType]);
			}
			return list;
		}

		public static List<float> GetPlayerMaxExpList(int fromLevel, int toLevel)
		{
			List<float> list = new List<float>();
			List<PlayerLevelMetaData> itemList = PlayerLevelMetaDataReader.GetItemList();
			for (int i = fromLevel; i <= toLevel; i++)
			{
				list.Add(itemList[i - 1].exp);
			}
			return list;
		}

		public static List<float> GetAvatarMaxExpList(AvatarDataItem avatarData, int fromLevel, int toLevel)
		{
			List<float> list = new List<float>();
			List<AvatarLevelMetaData> itemList = AvatarLevelMetaDataReader.GetItemList();
			for (int i = fromLevel; i <= toLevel; i++)
			{
				list.Add(itemList[i - 1].exp);
			}
			return list;
		}

		public static int FloorToIntCustom(float num)
		{
			return (0f < num && num < 1f) ? 1 : Mathf.FloorToInt(num);
		}

		public static void ShowBindWarningDialog()
		{
			GeneralDialogContext generalDialogContext = new GeneralDialogContext();
			generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
			generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_BindAccount");
			generalDialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindAccount");
			generalDialogContext.okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount");
			generalDialogContext.cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindAccount");
			generalDialogContext.buttonCallBack = delegate(bool confirmed)
			{
				if (confirmed)
				{
					Singleton<AccountManager>.Instance.manager.BindUI();
				}
			};
			GeneralDialogContext dialogContext = generalDialogContext;
			Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
		}

		public static string ProcessStrWithNewLine(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("\\n", Environment.NewLine);
				text = text.Replace("{{{", Environment.NewLine);
			}
			return text;
		}

		public static bool TrySetupEventSprite(Image img, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			Sprite sprite = Miscs.LoadResource<Sprite>(path);
			if (sprite != null)
			{
				img.GetComponent<Image>().sprite = sprite;
				return true;
			}
			if (GlobalVars.ResourceUseAssetBundle)
			{
				Singleton<AssetBundleManager>.Instance.Loader.TryStartDownloadOneAssetBundle(path, null, delegate(bool downloadSucc)
				{
					if (downloadSucc)
					{
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DownloadResAssetSucc));
					}
				});
			}
			return false;
		}

		public static bool GetEquipmentSlot(StorageDataItemBase storageData, out EquipmentSlot slot)
		{
			if (storageData is WeaponDataItem)
			{
				slot = (EquipmentSlot)1;
				return true;
			}
			if (storageData is StigmataDataItem)
			{
				switch ((storageData as StigmataDataItem).GetBaseType())
				{
				case 1:
					slot = (EquipmentSlot)2;
					return true;
				case 2:
					slot = (EquipmentSlot)3;
					return true;
				case 3:
					slot = (EquipmentSlot)4;
					return true;
				default:
					slot = (EquipmentSlot)1;
					return false;
				}
			}
			slot = (EquipmentSlot)1;
			return false;
		}

		public static void UpdateAvatarSkillStatusInLocalData(AvatarDataItem avatarData)
		{
			Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
			foreach (AvatarSkillDataItem skillData in avatarData.skillDataList)
			{
				foreach (AvatarSubSkillDataItem avatarSubSkill in skillData.avatarSubSkillList)
				{
					if (avatarSubSkill.ShouldShowHintPoint())
					{
						subSkillStatusDict[avatarSubSkill.subSkillID] = avatarSubSkill.Status;
					}
					else
					{
						subSkillStatusDict.Remove(avatarSubSkill.subSkillID);
					}
				}
			}
			Singleton<MiHoYoGameData>.Instance.Save();
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate));
		}

		public static Sprite GetResourceSprite(ResourceType resourceType, StorageDataItemBase itemData = null)
		{
			switch (resourceType)
			{
			case ResourceType.Hcoin:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconHC");
			case ResourceType.Scoin:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconSC");
			case ResourceType.PlayerExp:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconEXP");
			case ResourceType.Stamina:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconST");
			case ResourceType.SkillPoint:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconSP");
			case ResourceType.FriendPoint:
				return Miscs.GetSpriteByPrefab("SpriteOutput/GeneralIcon/IconFP");
			default:
				if (itemData == null)
				{
					return null;
				}
				return Miscs.GetSpriteByPrefab(itemData.GetIconPath());
			}
		}

		public static string GetResourceIconPath(ResourceType resourceType, int itemID = 0)
		{
			switch (resourceType)
			{
			case ResourceType.Hcoin:
				return "SpriteOutput/GeneralIcon/IconHC";
			case ResourceType.Scoin:
				return "SpriteOutput/GeneralIcon/IconSC";
			case ResourceType.PlayerExp:
				return "SpriteOutput/GeneralIcon/IconEXP";
			case ResourceType.Stamina:
				return "SpriteOutput/GeneralIcon/IconST";
			case ResourceType.SkillPoint:
				return "SpriteOutput/GeneralIcon/IconSP";
			case ResourceType.FriendPoint:
				return "SpriteOutput/GeneralIcon/IconFP";
			default:
				if (itemID != 0)
				{
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(itemID);
					if (dummyStorageDataItem != null)
					{
						return dummyStorageDataItem.GetIconPath();
					}
					return null;
				}
				return null;
			}
		}

		public static void SpaceshipCheckWeather()
		{
			if (SpaceshipCheckWeatherRealTime())
			{
				return;
			}
			if (TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowThunderWeather));
			}
			else if (!Singleton<MainUIManager>.Instance.spaceShipVisibleOnPreviesPage)
			{
				if (TimeUtil.Now > Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowRandomWeather));
					Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddHours(2.0);
				}
				else
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDefaultWeather));
				}
			}
		}

		private static bool SpaceshipCheckWeatherRealTime()
		{
			RealTimeWeatherManager instance = Singleton<RealTimeWeatherManager>.Instance;
			if (instance == null || !instance.Available)
			{
				return false;
			}
			WeatherInfo weatherInfo = instance.GetWeatherInfo();
			if (weatherInfo == null || weatherInfo.weatherType == WeatherType.None)
			{
				return false;
			}
			string configName = null;
			int sceneId = -1;
			instance.GetWeatherConfig(weatherInfo.weatherType, out configName, out sceneId);
			if (configName == null || sceneId == -1)
			{
				return false;
			}
			MainMenuStage mainMenuStage = UnityEngine.Object.FindObjectOfType<MainMenuStage>();
			if (mainMenuStage == null)
			{
				return false;
			}
			string path = string.Format("Rendering/MainMenuAtmosphereConfig/{0}", configName);
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(path);
			if (configAtmosphereSeries == null)
			{
				return false;
			}
			mainMenuStage.ChooseCloudScene(configAtmosphereSeries, sceneId);
			return true;
		}

		public static void TryParseHexString(string str, out Color color)
		{
			ColorUtility.TryParseHtmlString(str, out color);
		}

		public static string GetPlayerNickname(PlayerFriendBriefData briefData)
		{
			if (!briefData.nicknameSpecified || string.IsNullOrEmpty(briefData.nickname))
			{
				return LocalizationGeneralLogic.GetText("Menu_DefaultNickname", briefData.uid);
			}
			return briefData.nickname;
		}

		public static Sprite GetAvatarCardIcon(int avatarID)
		{
			AvatarCardMetaData avatarCardMetaDataByKey = AvatarCardMetaDataReader.GetAvatarCardMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarID).avatarCardID);
			return Miscs.GetSpriteByPrefab(avatarCardMetaDataByKey.iconPath);
		}

		public static List<int> GetGoodsRealPrice(Goods goods)
		{
			ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goods.goods_id);
			List<int> goodsOriginPrice = GetGoodsOriginPrice(goods);
			for (int i = 0; i < goodsOriginPrice.Count; i++)
			{
				goodsOriginPrice[i] = (int)Mathf.Floor((float)goodsOriginPrice[i] * (float)shopGoodsMetaDataByKey.Discount / 10000f);
			}
			return goodsOriginPrice;
		}

		public static List<int> GetGoodsOriginPrice(Goods goods)
		{
			ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int)goods.goods_id);
			List<int> list = new List<int>();
			if (shopGoodsMetaDataByKey.HCoinCost > 0)
			{
				list.Add(shopGoodsMetaDataByKey.HCoinCost);
			}
			if (shopGoodsMetaDataByKey.SCoinCost > 0)
			{
				list.Add(shopGoodsMetaDataByKey.SCoinCost);
			}
			if (shopGoodsMetaDataByKey.CostItemId > 0)
			{
				list.Add(shopGoodsMetaDataByKey.CostItemNum);
			}
			if (shopGoodsMetaDataByKey.CostItemId2 > 0)
			{
				list.Add(shopGoodsMetaDataByKey.CostItemNum2);
			}
			if (shopGoodsMetaDataByKey.CostItemId3 > 0)
			{
				list.Add(shopGoodsMetaDataByKey.CostItemNum3);
			}
			if (shopGoodsMetaDataByKey.CostItemId4 > 0)
			{
				list.Add(shopGoodsMetaDataByKey.CostItemNum4);
			}
			if (shopGoodsMetaDataByKey.CostItemId5 > 0)
			{
				list.Add(shopGoodsMetaDataByKey.CostItemNum5);
			}
			float num = 1f;
			int buyTimes = (int)(goods.buy_times + 1);
			ShopGoodsPriceRateMetaData shopGoodsPriceRateMetaDataByKey = ShopGoodsPriceRateMetaDataReader.GetShopGoodsPriceRateMetaDataByKey(buyTimes);
			if (shopGoodsMetaDataByKey.PriceRateID == 1)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType1;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 2)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType2;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 3)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType3;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 4)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType4;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 5)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType5;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 6)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType6;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 7)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType7;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 8)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType8;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 9)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType9;
			}
			else if (shopGoodsMetaDataByKey.PriceRateID == 10)
			{
				num *= (float)shopGoodsPriceRateMetaDataByKey.PriceType10;
			}
			num /= 100f;
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = Mathf.FloorToInt((float)list[i] * num);
			}
			return list;
		}

		public static void ShowAppStoreComment(int appCommentId)
		{
			if (!Singleton<CommonIDModule>.Instance.IsCommonFinished(appCommentId))
			{
				ShowAppstoreCommentDialog();
				Singleton<CommonIDModule>.Instance.MarkCommonIDFinish(appCommentId);
			}
		}

		public static void ShowAppstoreCommentDialog()
		{
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return;
			}
			int times = 1;
			if (Singleton<CommonIDModule>.Instance.IsCommonFinished(CommonIDModule.APP_STORE_COMMENT_ID_1))
			{
				times++;
			}
			if (Singleton<CommonIDModule>.Instance.IsCommonFinished(CommonIDModule.APP_STORE_COMMENT_ID_2))
			{
				times++;
			}
			times = Mathf.Clamp(times, 1, 2);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_AppstoreComment"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_AppstoreComment"),
				okBtnText = LocalizationGeneralLogic.GetText("Menu_OkBtn_AppstoreComment"),
				cancelBtnText = LocalizationGeneralLogic.GetText("Menu_CancelBtn_AppstoreComment"),
				notDestroyAfterTouchBG = true,
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						Application.OpenURL(MiscData.Config.BasicConfig.AppstoreUrl);
					}
					Singleton<NetworkManager>.Instance.RequestCommentReport((CommentType)(confirmed ? 1 : 2), (uint)times);
				},
				destroyCallBack = delegate
				{
					Singleton<NetworkManager>.Instance.RequestCommentReport((CommentType)3, (uint)times);
				}
			});
		}
	}
}
