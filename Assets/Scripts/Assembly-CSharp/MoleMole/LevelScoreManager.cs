using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelScoreManager
	{
		private LevelDataItem _level;

		public string luaFile;

		public int progress;

		public int difficulty;

		public LevelActor.Mode levelMode;

		public StageType LevelType = (StageType)1;

		public List<DropItem> configLevelDrops;

		public List<DropItem> endlessConfigLevelDrops;

		public List<int> configChallengeIds;

		public List<int> trackChallengeIds;

		public List<AvatarDataItem> memberList;

		public float configAvatarExpInside;

		public float configScoinInside;

		public int NPCHardLevel;

		public FriendDetailDataItem friendDetailItem;

		public List<string> appliedToolList;

		public float levelTimer;

		public bool isDebugLevel;

		public bool isDebugDynamicLevel;

		public bool isTryLevel;

		public bool isLevelEnd;

		public StageEndStatus endStatus;

		public float avatarExpInside;

		public float scoinInside;

		public List<int> finishedChallengeIndexList;

		public List<DropItem> dropItemList;

		public List<DropItem> dropItemListToShow;

		public int playerLevelBefore;

		public int playerExpBefore;

		public bool hasNuclearActivityBefore;

		public int avaiableReviveNum;

		public int maxReviveNum;

		public StageEndRsp stageEndRsp;

		public bool isLevelSuccess;

		public bool useDebugFunction;

		public SafeInt32 maxComboNum = 0;

		public bool collectAntiCheatData;

		public string signKey;

		public int LevelId
		{
			get
			{
				return (_level != null) ? _level.levelId : 0;
			}
		}

		public string LevelTitle
		{
			get
			{
				return (_level != null) ? _level.Title : string.Empty;
			}
		}

		public string chapterTitle
		{
			get
			{
				return (_level != null) ? Singleton<LevelModule>.Instance.GetChapterById(_level.ChapterID).Title : string.Empty;
			}
		}

		public string actTitle
		{
			get
			{
				return (_level != null) ? new ActDataItem(_level.ActID).actTitle : string.Empty;
			}
		}

		public string stageName
		{
			get
			{
				return (_level != null) ? _level.StageName : string.Empty;
			}
		}

		public void SetLevelBeginIntent(LevelDataItem level, int progress, List<DropItem> drops, LevelActor.Mode levelMode = LevelActor.Mode.Single, FriendDetailDataItem friendDetailItem = null)
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			_level = level;
			this.progress = progress;
			configLevelDrops = drops;
			this.friendDetailItem = friendDetailItem;
			NPCHardLevel = _level.NPCHardLevel;
			luaFile = _level.LuaFile;
			if (_level.levelId == 10101 && Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(_level.levelId).Count > 0)
			{
				luaFile = "Lua/Levels/MainLine_Stage01/Level Tutorial.lua";
			}
			difficulty = (int)_level.Diffculty;
			this.levelMode = levelMode;
			LevelType = _level.LevelType;
			configChallengeIds = level.GetAllChallengeIdList();
			trackChallengeIds = level.GetTrackChallengeIdList();
			configAvatarExpInside = level.AvatarExpInside;
			configScoinInside = level.ScoinInside;
			maxReviveNum = level.maxReviveTimes;
			avaiableReviveNum = maxReviveNum;
			memberList = new List<AvatarDataItem>();
			List<int> list = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
			for (int i = 0; i < list.Count; i++)
			{
				AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(list[i]).Clone();
				memberList.Add(item);
			}
			InitLevelScore();
			hasNuclearActivityBefore = HasNuclearActivity(level.levelId);
		}

		public void SetDefendModeLevelBeginIntent(int progress, int difficulty)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			luaFile = MiscData.Config.BasicConfig.DefendModeLevelLuaFilePath;
			levelMode = LevelActor.Mode.Single;
			this.difficulty = difficulty;
			this.progress = progress;
			LevelType = (StageType)2;
		}

		public void SetEndlessLevelBeginIntent(int progress, int hardLevel, List<string> appliedToolList, EndlessStageBeginRsp endlessStageBeginRsp, float levelTimer = -1f, int difficulty = 1)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			luaFile = MiscData.Config.BasicConfig.EndlessLevelLuaFilePath;
			levelMode = LevelActor.Mode.Single;
			this.difficulty = difficulty;
			this.progress = progress;
			LevelType = (StageType)4;
			this.appliedToolList = appliedToolList;
			SetEndlessDropItem(endlessStageBeginRsp);
			configChallengeIds = new List<int>();
			trackChallengeIds = new List<int>();
			configAvatarExpInside = 0f;
			configScoinInside = 0f;
			NPCHardLevel = hardLevel;
			maxReviveNum = 0;
			avaiableReviveNum = maxReviveNum;
			this.levelTimer = levelTimer;
			memberList = new List<AvatarDataItem>();
			List<int> list = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4);
			for (int i = 0; i < list.Count; i++)
			{
				AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(list[i]).Clone();
				memberList.Add(item);
			}
			InitLevelScore();
			_level = CreateEndlessLevelItem();
		}

		public void SetDevLevelBeginIntent(string levelPath, LevelActor.Mode mode, int hardLevel, int difficulty, FriendDetailDataItem friend)
		{
			isDebugLevel = true;
			luaFile = levelPath;
			levelMode = mode;
			this.difficulty = difficulty;
			configLevelDrops = new List<DropItem>();
			configChallengeIds = new List<int>();
			trackChallengeIds = new List<int>();
			configAvatarExpInside = 0f;
			configScoinInside = 0f;
			NPCHardLevel = hardLevel;
			maxReviveNum = 3;
			avaiableReviveNum = maxReviveNum;
			friendDetailItem = friend;
			InitLevelScore();
			_level = CreateDummyLevelItem(levelPath);
		}

		public void SetDebugLevelBeginIntent(string luaName)
		{
			if (string.IsNullOrEmpty(luaName))
			{
				luaName = "Level Analyse Auto.lua";
			}
			isDebugLevel = true;
			NPCHardLevel = 1;
			luaFile = "Lua/Levels/Common/" + luaName;
			difficulty = 1;
			levelMode = LevelActor.Mode.Single;
			configLevelDrops = new List<DropItem>();
			configChallengeIds = new List<int>();
			configAvatarExpInside = 0f;
			configScoinInside = 0f;
			maxReviveNum = 3;
			avaiableReviveNum = maxReviveNum;
			memberList = new List<AvatarDataItem>();
			List<int> list = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)1);
			for (int i = 0; i < list.Count; i++)
			{
				AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(list[i]).Clone();
				memberList.Add(item);
			}
			InitLevelScore();
			_level = CreateDummyLevelItem(luaName);
		}

		public void SetTryLevelBeginIntent(int avatarId, string luaPath, int trySkillId = 0, int trySubSkillId = 0)
		{
			isTryLevel = true;
			NPCHardLevel = 1;
			luaFile = luaPath;
			difficulty = 1;
			levelMode = LevelActor.Mode.Single;
			configLevelDrops = new List<DropItem>();
			configChallengeIds = new List<int>();
			configAvatarExpInside = 0f;
			configScoinInside = 0f;
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarId).Clone();
			if (trySkillId != 0)
			{
				AvatarSkillDataItem avatarSkillBySkillID = avatarDataItem.GetAvatarSkillBySkillID(trySkillId);
				avatarSkillBySkillID.UnLocked = true;
				if (trySubSkillId != 0)
				{
					avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId(trySubSkillId).level = 1;
				}
			}
			memberList = new List<AvatarDataItem> { avatarDataItem };
			InitLevelScore();
		}

		public void AddDropItem(int metaId, int level, int num)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (CheckDropItem(metaId, level, num))
			{
				List<DropItem> list = dropItemList;
				DropItem val = new DropItem();
				val.item_id = (uint)metaId;
				val.level = (uint)level;
				val.num = (uint)num;
				list.Add(val);
			}
		}

		public void AddDropItemToShow(int metaId, int level, int num)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (CheckDropItem(metaId, level, num))
			{
				List<DropItem> list = dropItemListToShow;
				DropItem val = new DropItem();
				val.item_id = (uint)metaId;
				val.level = (uint)level;
				val.num = (uint)num;
				list.Add(val);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DropItemConutChanged, dropItemListToShow.Count));
			}
		}

		private bool CheckDropItem(int metaId, int level, int num)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			if ((int)LevelType == 4)
			{
				if (configLevelDrops.Find((DropItem x) => x.item_id == metaId) == null && endlessConfigLevelDrops.Find((DropItem x) => x.item_id == metaId) == null)
				{
					return false;
				}
			}
			else if (configLevelDrops.Find((DropItem x) => x.item_id == metaId) == null)
			{
				return false;
			}
			return true;
		}

		private LevelDataItem CreateDummyLevelItem(string luaFile)
		{
			LevelMetaData levelMetaData = new LevelMetaData(0, "DEV_LEVEL", 0, 0, 0, 1, 1, 0, 1, 10, 0, 0, 0, 0, 0, 0f, 0, 0f, 0, 0, 0, new List<int>(), string.Empty, string.Empty, string.Empty, 1, 1, 1, new List<int>(), "DEV_LEVEL", "DEV_LEVEL", string.Empty, string.Empty, "Lua/Levels/Common/" + luaFile, new List<LevelMetaData.LevelChallengeMetaNode>(), 0, 0, 1, 10000, 1, 10, new List<string>(), 100);
			return new LevelDataItem(0, levelMetaData);
		}

		private LevelDataItem CreateEndlessLevelItem()
		{
			LevelMetaData levelMetaData = new LevelMetaData(0, "ENDLESS_LEVEL", 0, 0, 0, 1, 1, 0, 1, 10, 0, 0, 0, 0, 0, 0f, 0, 0f, 0, 0, 0, new List<int>(), string.Empty, string.Empty, string.Empty, 1, 1, 1, new List<int>(), "ENDLESS_LEVEL", "ENDLESS_LEVEL", string.Empty, string.Empty, MiscData.Config.BasicConfig.EndlessLevelLuaFilePath, new List<LevelMetaData.LevelChallengeMetaNode>(), 0, 0, 1, 10000, 1, 10, new List<string>(), 100);
			return new LevelDataItem(900101, levelMetaData);
		}

		public void HandleLevelEnd(EvtLevelState.LevelEndReason endReason)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Invalid comparison between Unknown and I4
			isLevelEnd = true;
			switch (endReason)
			{
			case EvtLevelState.LevelEndReason.EndWin:
				endStatus = (StageEndStatus)1;
				break;
			case EvtLevelState.LevelEndReason.EndLoseAllDead:
				endStatus = (StageEndStatus)3;
				break;
			case EvtLevelState.LevelEndReason.EndLoseNotMeetCondition:
				endStatus = (StageEndStatus)2;
				break;
			case EvtLevelState.LevelEndReason.EndLoseQuit:
				endStatus = (StageEndStatus)4;
				break;
			default:
				endStatus = (StageEndStatus)4;
				break;
			}
			if (endReason == EvtLevelState.LevelEndReason.EndWin)
			{
				LevelChallengeHelperPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelChallengeHelperPlugin>();
				if (plugin != null)
				{
					HashSet<int> hashSet = new HashSet<int>();
					for (int i = 0; i < plugin.challengeList.Count; i++)
					{
						BaseLevelChallenge baseLevelChallenge = plugin.challengeList[i];
						if (baseLevelChallenge.IsFinished())
						{
							hashSet.Add(baseLevelChallenge.challengeId);
						}
					}
					int count = configChallengeIds.Count;
					for (int j = 0; j < count; j++)
					{
						if (hashSet.Contains(configChallengeIds[j]))
						{
							finishedChallengeIndexList.Add(j);
						}
					}
				}
				if ((int)LevelType != 4)
				{
					return;
				}
				{
					foreach (DropItem endlessConfigLevelDrop in endlessConfigLevelDrops)
					{
						AddDropItem((int)endlessConfigLevelDrop.item_id, (int)endlessConfigLevelDrop.level, (int)endlessConfigLevelDrop.num);
					}
					return;
				}
			}
			dropItemList.Clear();
			dropItemListToShow.Clear();
		}

		public bool RequestLevelEnd()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Invalid comparison between Unknown and I4
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Invalid comparison between Unknown and I4
			bool flag = false;
			if (MiscData.Config.EnableHashCheck)
			{
				int num = GlobalDataManager.HashDataContent();
				flag = num != GlobalDataManager.contentHash;
			}
			if ((int)LevelType == 4)
			{
				bool flag2 = false;
				List<EndlessAvatarHp> list = new List<EndlessAvatarHp>();
				List<int> list2 = new List<int>();
				foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
				{
					AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatar.GetRuntimeID());
					EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(actor.avatarDataItem.avatarID);
					int num2 = Mathf.Clamp(allPlayerAvatar.IsAlive() ? Mathf.FloorToInt((float)actor.HP / (float)actor.maxHP * 100f) : 0, 0, 100);
					int sp_percent = Mathf.Clamp(allPlayerAvatar.IsAlive() ? Mathf.FloorToInt((float)actor.SP / (float)actor.maxSP * 100f) : 0, 0, 100);
					if ((int)endStatus != 1)
					{
						num2 = Mathf.Clamp(num2 - 50, 0, 100);
					}
					endlessAvatarHPData.hp_percent = (uint)num2;
					endlessAvatarHPData.sp_percent = (uint)sp_percent;
					list.Add(endlessAvatarHPData);
					Singleton<EndlessModule>.Instance.SetAvatarHP((int)endlessAvatarHPData.hp_percent, (int)endlessAvatarHPData.avatar_id);
					if (endlessAvatarHPData.hp_percent != 0)
					{
						list2.Add(actor.avatarDataItem.avatarID);
					}
					else
					{
						flag2 = true;
					}
				}
				Singleton<PlayerModule>.Instance.playerData.SetTeamMember((StageType)4, list2);
				if (flag2)
				{
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarDie));
				}
				Singleton<NetworkManager>.Instance.RequestEndlessFloorEndReq(endStatus, dropItemList, list, flag);
			}
			else
			{
				PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
				playerLevelBefore = playerData.teamLevel;
				playerExpBefore = playerData.teamExp;
				Singleton<LevelManager>.Instance.levelActor.ControlLevelDamageStastics(DamageStastcisControlType.DamageStasticsResult);
				List<StageCheatData> cheatDataList = null;
				if (collectAntiCheatData)
				{
					LevelAntiCheatPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAntiCheatPlugin>();
					if (plugin != null)
					{
						plugin.CollectAntiCheatData();
						cheatDataList = plugin.cheatDataList;
					}
				}
				Singleton<NetworkManager>.Instance.RequestLevelEndReq(_level.levelId, endStatus, Mathf.FloorToInt(_level.ScoinFixed + scoinInside), Mathf.FloorToInt(_level.AvatarExpFixed + avatarExpInside), finishedChallengeIndexList, dropItemList, cheatDataList, flag, signKey);
			}
			if (MiscData.Config.EnableHashCheck && flag)
			{
				Singleton<ApplicationManager>.Instance.AntiCheatQuit("Menu_Title_HashCheatQuit", "Menu_Desc_HashCheatQuit");
				return false;
			}
			return true;
		}

		private void InitLevelScore()
		{
			isLevelEnd = false;
			finishedChallengeIndexList = new List<int>();
			dropItemList = new List<DropItem>();
			dropItemListToShow = new List<DropItem>();
			avatarExpInside = 0f;
			scoinInside = 0f;
			maxComboNum = 0;
		}

		public int GetReviveCost()
		{
			int reviveTime = maxReviveNum - avaiableReviveNum + 1;
			return _level.GetReviveCost(reviveTime);
		}

		public List<DropItem> GetTotalDropList()
		{
			return GetDropList(dropItemList);
		}

		public List<DropItem> GetDropListToShow()
		{
			return GetDropList(dropItemListToShow);
		}

		private List<DropItem> GetDropList(List<DropItem> list)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			List<DropItem> list2 = new List<DropItem>();
			Dictionary<int, DropItem> dictionary = new Dictionary<int, DropItem>();
			foreach (DropItem item in list)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.item_id);
				dummyStorageDataItem.number = (int)item.num;
				if (dummyStorageDataItem is WeaponDataItem || dummyStorageDataItem is StigmataDataItem || dummyStorageDataItem is AvatarCardDataItem)
				{
					list2.Add(item);
					continue;
				}
				if (dictionary.ContainsKey(dummyStorageDataItem.ID))
				{
					DropItem obj = dictionary[dummyStorageDataItem.ID];
					obj.num += (uint)dummyStorageDataItem.number;
					continue;
				}
				DropItem val = new DropItem();
				val.item_id = item.item_id;
				val.level = item.level;
				val.num = item.num;
				val.rarity = item.rarity;
				dictionary[dummyStorageDataItem.ID] = val;
			}
			foreach (KeyValuePair<int, DropItem> item2 in dictionary)
			{
				list2.Add(item2.Value);
			}
			list2.Sort(SortDropItem);
			return list2;
		}

		private int SortDropItem(DropItem one, DropItem two)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)one.item_id);
			StorageDataItemBase dummyStorageDataItem2 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)one.item_id);
			bool flag = Singleton<StorageModule>.Instance.IsItemNew((int)one.item_id);
			bool flag2 = Singleton<StorageModule>.Instance.IsItemNew((int)two.item_id);
			if (flag)
			{
				return -1;
			}
			if (flag2)
			{
				return 1;
			}
			int itemTypeIndex = GetItemTypeIndex(dummyStorageDataItem);
			int itemTypeIndex2 = GetItemTypeIndex(dummyStorageDataItem2);
			if (itemTypeIndex != itemTypeIndex2)
			{
				return itemTypeIndex - itemTypeIndex2;
			}
			if (one.rarity != two.rarity)
			{
				return (int)(one.rarity - two.rarity);
			}
			if (one.level != two.level)
			{
				return (int)(one.level - two.level);
			}
			return 0;
		}

		private int GetItemTypeIndex(StorageDataItemBase itemData)
		{
			if (itemData is WeaponDataItem)
			{
				return 1;
			}
			if (itemData is StigmataDataItem)
			{
				return 2;
			}
			if (itemData is MaterialDataItem)
			{
				switch (itemData.GetBaseType())
				{
				case 1:
					return 3;
				case 2:
					return 4;
				default:
					return 20;
				}
			}
			if (itemData is AvatarFragmentDataItem)
			{
				return 21;
			}
			return 100;
		}

		private void SetEndlessDropItem(EndlessStageBeginRsp rsp)
		{
			configLevelDrops = new List<DropItem>();
			endlessConfigLevelDrops = new List<DropItem>();
			foreach (DropItem item in rsp.drop_item_list)
			{
				if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int)item.item_id) != null)
				{
					endlessConfigLevelDrops.Add(item);
				}
				else
				{
					configLevelDrops.Add(item);
				}
			}
		}

		public bool IsLevelDone()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			return _level == null || (int)_level.status == 3;
		}

		public bool HasNuclearActivity(int levelID)
		{
			WeekDayActivityDataItem weekDayActivityDataItem = Singleton<LevelModule>.Instance.TryGetWeekDayActivityByLevelID(levelID);
			if (weekDayActivityDataItem == null)
			{
				return false;
			}
			SeriesDataItem weekDaySeriesByActivityID = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(weekDayActivityDataItem.GetActivityID());
			if (weekDaySeriesByActivityID == null)
			{
				return false;
			}
			return weekDaySeriesByActivityID.weekActivityList.Exists((WeekDayActivityDataItem x) => (int)x.GetActivityType() == 3 && x.GetStatus() == ActivityDataItemBase.Status.InProgress);
		}

		public bool IsAllowLevelPunish()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Invalid comparison between Unknown and I4
			bool flag = Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.BasicConfig.MinPlayerPunishLevel;
			return (int)LevelType != 4 && flag;
		}
	}
}
