using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using proto;
using Avatar = proto.Avatar;
using Retcode = proto.StageEndRsp.Retcode;
using Material = proto.Material;

namespace MoleMole
{
	public static class FakePacketHelper
	{
		private const string JSON_FILE = "DataDev/FakePacketRsp.json";

		private static JSONNode inputDict;

		private static uint uid;

		private static Dictionary<int, uint> _weaponUidSet;

		public static void LoadFromFile()
		{
			string aJSON = Miscs.LoadTextFileToString("DataDev/FakePacketRsp.json");
			inputDict = JSON.Parse(aJSON);
		}

		public static T GetFakeRsp<T>()
		{
			string classNameByType = GetClassNameByType(typeof(T));
			JSONNode json = inputDict[classNameByType];
			return (T)JsonDeserialize(json, typeof(T));
		}

		private static uint GenerateNewUid()
		{
			return ++uid;
		}

		public static void FakeConnectDispatch()
		{
			string aJSON = "{\"account_url\" : \"fake\"}";
			Singleton<NetworkManager>.Instance.DispatchSeverData = new DispatchServerDataItem(JSON.Parse(aJSON));
		}

		public static GetAvatarDataRsp GetFakeAvatarDataRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			GetAvatarDataRsp val = new GetAvatarDataRsp();
			List<int> list = new List<int>();
			list.Add(401);
			list.Add(402);
			list.Add(403);
			list.Add(404);
			List<int> list2 = list;
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			foreach (AvatarMetaData item in itemList)
			{
				if (!list2.Contains(item.avatarID))
				{
					Avatar val2 = new Avatar();
					val2.avatar_id = (uint)item.avatarID;
					val2.star = (uint)item.unlockStar;
					val2.level = 10u;
					val2.exp = 0u;
					val2.fragment = 100u;
					val2.stigmata_unique_id_1 = 0u;
					val2.stigmata_unique_id_2 = 0u;
					val2.stigmata_unique_id_3 = 0u;
					val2.touch_goodfeel = 0u;
					val2.today_has_add_goodfeel = 0u;
					val2.stage_goodfeel = 0u;
					val2.weapon_unique_id = _weaponUidSet[item.avatarID];
					val.avatar_list.Add(val2);
				}
			}
			return val;
		}

		public static AvatarReviveRsp GetAvatarReviveRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AvatarReviveRsp val = new AvatarReviveRsp();
			val.retcode = (AvatarReviveRsp.Retcode)0;
			val.revive_times = 1u;
			return val;
		}

		public static GetStageDataRsp GetFakeStageDataRsp()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			GetStageDataRsp val = new GetStageDataRsp();
			List<StageType> containType = new List<StageType> { (StageType)1 };
			List<LevelMetaData> list = LevelMetaDataReader.GetItemList().FindAll((LevelMetaData x) => containType.Contains((StageType)x.type));
			foreach (LevelMetaData item in list)
			{
				Stage val2 = new Stage();
				val2.id = (uint)item.levelId;
				val.stage_list.Add(val2);
			}
			return val;
		}

		public static StageEndRsp GetFakeStageEndRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StageEndRsp val = new StageEndRsp();
			val.retcode = (Retcode)0;
			return val;
		}

		public static GetEquipmentDataRsp GetFakeEquipmentDataRsp()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			int addNum = 100;
			int addNum2 = 10;
			int addNum3 = 10;
			GetEquipmentDataRsp val = new GetEquipmentDataRsp();
			AddWeaponIntoRsp(val, addNum);
			AddStigmataIntoRsp(val, addNum2);
			AddMaterialIntoRsp(val, addNum3);
			return val;
		}

		public static GetFriendListRsp GetFakeFriendListRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			GetFriendListRsp val = new GetFriendListRsp();
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			for (int i = 0; i < 100; i++)
			{
				PlayerFriendBriefData val2 = new PlayerFriendBriefData();
				val2.uid = (uint)(i + 10000);
				val2.nickname = "friend_" + i;
				val2.level = (uint)UnityEngine.Random.Range(1, 100);
				val2.avatar_combat = (uint)UnityEngine.Random.Range(100, 10000);
				val2.avatar_star = (uint)UnityEngine.Random.Range(1, 5);
				val2.avatar_id = (uint)itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID;
				val.friend_list.Add(val2);
			}
			return val;
		}

		public static GetAskAddFriendListRsp GetFakeAskAddFriendListRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			GetAskAddFriendListRsp val = new GetAskAddFriendListRsp();
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			for (int i = 0; i < 100; i++)
			{
				PlayerFriendBriefData val2 = new PlayerFriendBriefData();
				val2.uid = (uint)(i + 10000);
				val2.nickname = "friend_" + i;
				val2.level = (uint)UnityEngine.Random.Range(1, 100);
				val2.avatar_combat = (uint)UnityEngine.Random.Range(100, 10000);
				val2.avatar_star = (uint)UnityEngine.Random.Range(1, 5);
				val2.avatar_id = (uint)itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID;
				val.ask_list.Add(val2);
			}
			return val;
		}

		public static GetRecommendFriendListRsp GetFakeRecommendFriendListRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			GetRecommendFriendListRsp val = new GetRecommendFriendListRsp();
			List<AvatarMetaData> itemList = AvatarMetaDataReader.GetItemList();
			for (int i = 0; i < 100; i++)
			{
				PlayerFriendBriefData val2 = new PlayerFriendBriefData();
				val2.uid = (uint)(i + 10000);
				val2.nickname = "friend_" + i;
				val2.level = (uint)UnityEngine.Random.Range(1, 100);
				val2.avatar_combat = (uint)UnityEngine.Random.Range(100, 10000);
				val2.avatar_star = (uint)UnityEngine.Random.Range(1, 5);
				val2.avatar_id = (uint)itemList[UnityEngine.Random.Range(0, itemList.Count)].avatarID;
				val.recommend_list.Add(val2);
			}
			return val;
		}

		public static GetMailDataRsp GetFakeMailDataRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Expected O, but got Unknown
			GetMailDataRsp val = new GetMailDataRsp();
			for (int i = 0; i < 20; i++)
			{
				Mail val2 = new Mail();
				val2.id = (uint)(i + 10000);
				val2.type = (MailType)3;
				val2.title = "This is Test mail with id: " + i;
				val2.content = "This is Test mail with id: " + i;
				val2.sender = "Tester";
				proto.MailAttachment val3 = new proto.MailAttachment();
				val3.hcoin = 10u;
				val2.attachment = val3;
				val.mail_list.Add(val2);
			}
			return val;
		}

		public static GetPlayerDetailDataRsp GetFakePlayerDetailDataRsp(uint targetID)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			GetPlayerDetailDataRsp val = new GetPlayerDetailDataRsp();
			val.detail = new PlayerDetailData();
			val.detail.uid = targetID;
			val.detail.leader_avatar = new AvatarDetailData();
			val.detail.leader_avatar.avatar_id = 101u;
			WeaponDetailData val2 = new WeaponDetailData();
			val2.id = (uint)AvatarMetaDataReader.GetAvatarMetaDataByKey((int)val.detail.leader_avatar.avatar_id).initialWeapon;
			val2.level = 1u;
			val.detail.leader_avatar.weapon = val2;
			return val;
		}

		public static GetAvatarTeamDataRsp GetFakeGetAvatarTeamDataRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected I4, but got Unknown
			GetAvatarTeamDataRsp val = new GetAvatarTeamDataRsp();
			val.retcode = 0;
			List<StageType> list = new List<StageType>();
			list.Add((StageType)1);
			List<StageType> list2 = list;
			List<uint> list3 = new List<uint>();
			list3.Add(101u);
			list3.Add(102u);
			List<uint> collection = list3;
			foreach (StageType item in list2)
			{
				AvatarTeam val2 = new AvatarTeam();
				val2.stage_type = (uint)(int)item;
				val2.avatar_id_list.AddRange(collection);
				val.avatar_team_list.Add(val2);
			}
			return val;
		}

		public static GetFinishGuideDataRsp GetFakeFinishGuideDataRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			GetFinishGuideDataRsp val = new GetFinishGuideDataRsp();
			val.retcode = (GetFinishGuideDataRsp.Retcode)(Retcode)0;
			foreach (LevelTutorialMetaData item in LevelTutorialMetaDataReader.GetItemList())
			{
				val.guide_id_list.Add((uint)item.tutorialId);
			}
			foreach (TutorialData item2 in TutorialDataReader.GetItemList())
			{
				val.guide_id_list.Add((uint)item2.id);
			}
			return val;
		}

		public static GetGachaDisplayRsp GetFakeGetGachaDisplayRsp()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			GetGachaDisplayRsp val = new GetGachaDisplayRsp();
			val.retcode = 0;
			val.hcoin_gacha_data = new HcoinGachaData();
			val.hcoin_gacha_data.common_data = new GachaDisplayCommonData();
			val.friends_point_gacha_data = new FriendsPointGachaData();
			val.friends_point_gacha_data.friends_point_cost = 1u;
			val.friends_point_gacha_data.common_data = new GachaDisplayCommonData();
			return val;
		}

		private static object JsonDeserialize(JSONNode json, Type type)
		{
			object obj = Activator.CreateInstance(type);
			foreach (string key in json.Keys)
			{
				if (json[key] is JSONArray)
				{
					Type valueType = ReflectionUtil.GetValueType(obj, key);
					Type type2 = valueType.GetGenericArguments()[0];
					IList list = ReflectionUtil.GetValue(obj, key) as IList;
					foreach (JSONNode item in json[key] as JSONArray)
					{
						list.Add(JsonDeserialize(item, type2));
					}
					continue;
				}
				Type valueType2 = ReflectionUtil.GetValueType(obj, key);
				if (valueType2 == typeof(uint))
				{
					ReflectionUtil.SetValue(obj, key, (uint)json[key].AsInt);
				}
				else if (valueType2 == typeof(string))
				{
					ReflectionUtil.SetValue(obj, key, json[key].Value);
				}
				else if (valueType2.IsEnum)
				{
					ReflectionUtil.SetValue(obj, key, json[key].AsInt);
				}
			}
			return obj;
		}

		private static string GetClassNameByType(Type type)
		{
			string text = type.ToString();
			string[] array = text.Split("."[0]);
			return array[array.Length - 1];
		}

		private static void AddWeaponIntoRsp(GetEquipmentDataRsp rsp, int addNum)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			List<WeaponMetaData> itemList = WeaponMetaDataReader.GetItemList();
			int num = 0;
			foreach (WeaponMetaData item in itemList)
			{
				num++;
				if (num > addNum)
				{
					break;
				}
				Weapon val = new Weapon();
				val.unique_id = GenerateNewUid();
				val.id = (uint)item.ID;
				val.level = 1u;
				val.exp = 0u;
				rsp.weapon_list.Add(val);
				num++;
			}
			List<AvatarMetaData> itemList2 = AvatarMetaDataReader.GetItemList();
			_weaponUidSet = new Dictionary<int, uint>();
			foreach (AvatarMetaData item2 in itemList2)
			{
				WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey(item2.initialWeapon);
				Weapon val2 = new Weapon();
				val2.unique_id = GenerateNewUid();
				val2.id = (uint)weaponMetaDataByKey.ID;
				val2.level = 1u;
				val2.exp = 0u;
				rsp.weapon_list.Add(val2);
				_weaponUidSet[item2.avatarID] = val2.unique_id;
			}
		}

		private static void AddStigmataIntoRsp(GetEquipmentDataRsp rsp, int addNum)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
			int num = 0;
			foreach (StigmataMetaData item in itemList)
			{
				num++;
				if (num > addNum)
				{
					break;
				}
				Stigmata val = new Stigmata();
				val.unique_id = GenerateNewUid();
				val.id = (uint)item.ID;
				val.level = 1u;
				val.exp = 0u;
				rsp.stigmata_list.Add(val);
			}
		}

		private static void AddMaterialIntoRsp(GetEquipmentDataRsp rsp, int addNum)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			List<ItemMetaData> itemList = ItemMetaDataReader.GetItemList();
			int num = 0;
			foreach (ItemMetaData item in itemList)
			{
				num++;
				if (num > addNum)
				{
					break;
				}
				Material val = new Material();
				val.id = (uint)item.ID;
				val.num = 100u;
				rsp.material_list.Add(val);
			}
		}

		public static void AutoTestForAndroid()
		{
			GameObject gameObject = new GameObject("FakeAutoTest");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			MonoBehaviour monoBehaviour = gameObject.AddComponent<MonoBehaviour>();
			MiHoYoGameData.DeleteAllData();
			monoBehaviour.StartCoroutine(DoAutoTestForAndroid());
		}

		private static IEnumerator DoAutoTestForAndroid()
		{
			WaitForSeconds WAIT_STEP = new WaitForSeconds(1f);
			WaitForSeconds WAIT_LONG_STEP = new WaitForSeconds(5f);
			yield return null;
			Singleton<NetworkManager>.Instance.LoginGameServer();
			while (GameObject.Find("MainPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			Singleton<MainUIManager>.Instance.ShowPage(new SupplyEntrancePageContext());
			yield return WAIT_STEP;
			Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext());
			while (GameObject.Find("GachaMainPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			GameObject.Find("GachaMainPage(Clone)").transform.Find("HCoinTab/ActBtns/Ten/Btn").GetComponent<Button>().onClick.Invoke();
			while (GameObject.Find("GachaResultPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			yield return WAIT_LONG_STEP;
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
			while (GameObject.Find("MainPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext());
			while (GameObject.Find("StorageShowPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			yield return WAIT_STEP;
			GameObject.Find("StorageShowPage(Clone)").transform.Find("WeaponTab/ScrollView/Content/0").GetComponent<Button>().onClick.Invoke();
			while (GameObject.Find("WeaponDetailPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			yield return WAIT_LONG_STEP;
			GameObject.Find("WeaponDetailPage(Clone)").transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().onClick.Invoke();
			while (GameObject.Find("WeaponPowerUpPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			GameObject.Find("WeaponPowerUpPage(Clone)").transform.Find("ResourceList/Content/1").GetComponent<Button>().onClick.Invoke();
			while (GameObject.Find("StorageShowPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			GameObject.Find("StorageShowPage(Clone)").transform.Find("WeaponTab/ScrollView/Content/0").GetComponent<Button>().onClick.Invoke();
			yield return WAIT_STEP;
			GameObject.Find("StorageShowPage(Clone)").transform.Find("PowerUpPanel/OKBtn").GetComponent<Button>().onClick.Invoke();
			while (GameObject.Find("WeaponPowerUpPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			GameObject.Find("WeaponPowerUpPage(Clone)").transform.Find("ActionBtns/OkBtn").GetComponent<Button>().onClick.Invoke();
			yield return WAIT_LONG_STEP;
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
			while (GameObject.Find("MainPage(Clone)") == null)
			{
				yield return WAIT_STEP;
			}
			GameObject.Find("MainPage(Clone)").transform.Find("AutoBattlePanel/StartButton").GetComponent<Button>().onClick.Invoke();
		}
	}
}
