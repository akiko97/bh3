using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using proto;

namespace MoleMole
{
	[Serializable]
	public class UserLocalDataItem
	{
		[SerializeField]
		private uint _loginRandomNum;

		[SerializeField]
		private List<string> _doneBehaviourList;

		[SerializeField]
		private Dictionary<int, List<ChatMsgDataItem>> _friendChatMsgMap;

		[SerializeField]
		private int _lastChapterID;

		[SerializeField]
		private int _lastActIndex;

		[SerializeField]
		private int _difficulty;

		[SerializeField]
		private string _storageShowTabName;

		[SerializeField]
		private ConfigNotificationSetting _personalNotificationSetting;

		[SerializeField]
		private HashSet<int> _oldFriendUIDSet;

		[SerializeField]
		private HashSet<int> _oldRequestUIDSet;

		[SerializeField]
		private DateTime _lastShowBindAccountWarningTime;

		[SerializeField]
		private DateTime _lastShowBindIdentityWarningTime;

		[SerializeField]
		private List<MailCacheKey> _allMailSet;

		[SerializeField]
		private List<MailCacheKey> _readMailIdList;

		[SerializeField]
		private HashSet<int> _needPlayLevelAnimationSet;

		[SerializeField]
		private Dictionary<int, SubSkillStatus> _subSkillStatusDict;

		[SerializeField]
		private DateTime _endThunderWeatherTime;

		[SerializeField]
		private DateTime _nextRandomWeatherTime;

		[SerializeField]
		private string _currentWeatherConfigPath;

		[SerializeField]
		private int _currentWeatherSceneID;

		[SerializeField]
		private GetLastEndlessRewardDataRsp _lastRewardData;

		[SerializeField]
		private int _lastGalAvatarId;

		[SerializeField]
		private HashSet<uint> _oldBulletinIDSet;

		[SerializeField]
		private Dictionary<CabinType, bool> _cabinNeedToShowLevelUpCompleteSet;

		[SerializeField]
		private Dictionary<CabinType, bool> _cabinNeedToShowNewUnlockDict;

		[SerializeField]
		private string _receipt;

		[SerializeField]
		private int _payMethod;

		[SerializeField]
		private Dictionary<int, bool> _cabinTechTreeNodeVisited;

		[SerializeField]
		private DateTime _lastShowBulletinTime;

		[SerializeField]
		private byte[] _processingStageEndReq;

		[SerializeField]
		private WeatherInfo _lastWeatherInfo;

		[SerializeField]
		private Dictionary<int, int> _visitedTutorial;

		[SerializeField]
		private DateTime _lastCrashReportTime;

		[SerializeField]
		private bool _enableRealTimeWeather;

		[SerializeField]
		private bool _hasShowInviteHintDialog;

		private bool _isDirty = true;

		public List<string> DoneBehaviourList
		{
			get
			{
				if (_doneBehaviourList == null)
				{
					_doneBehaviourList = new List<string>();
				}
				return _doneBehaviourList;
			}
			set
			{
				_doneBehaviourList = value;
			}
		}

		public List<MailCacheKey> ReadMailIdList
		{
			get
			{
				if (_readMailIdList == null)
				{
					_readMailIdList = new List<MailCacheKey>();
				}
				return _readMailIdList;
			}
			set
			{
				_readMailIdList = value;
			}
		}

		public Dictionary<int, List<ChatMsgDataItem>> FriendChatMsgMap
		{
			get
			{
				if (_friendChatMsgMap == null)
				{
					_friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
				}
				return _friendChatMsgMap;
			}
			set
			{
				_friendChatMsgMap = value;
			}
		}

		public int LastChapterID
		{
			get
			{
				LevelModule instance = Singleton<LevelModule>.Instance;
				if (!instance.HasChapter(_lastChapterID))
				{
					_lastChapterID = Singleton<LevelModule>.Instance.GetOneUnlockChapterID();
				}
				return _lastChapterID;
			}
			set
			{
				_lastChapterID = value;
			}
		}

		public LevelDiffculty LastDifficulty
		{
			get
			{
				LevelModule instance = Singleton<LevelModule>.Instance;
				if (!instance.GetChapterById(LastChapterID).HasLevelsOfDifficulty((LevelDiffculty)_difficulty))
				{
					_difficulty = 1;
				}
				return (LevelDiffculty)(int)Enum.ToObject(typeof(LevelDiffculty), _difficulty);
			}
			set
			{
				_difficulty = (int)value;
			}
		}

		public int LastActIndex
		{
			get
			{
				return _lastActIndex;
			}
			set
			{
				_lastActIndex = value;
			}
		}

		public string StorageShowTabName
		{
			get
			{
				return _storageShowTabName;
			}
			set
			{
				_storageShowTabName = value;
			}
		}

		public ConfigNotificationSetting PersonalNotificationSetting
		{
			get
			{
				return _personalNotificationSetting;
			}
			set
			{
				_personalNotificationSetting = value;
			}
		}

		public HashSet<int> OldFriendUIDSet
		{
			get
			{
				if (_oldFriendUIDSet == null)
				{
					_oldFriendUIDSet = new HashSet<int>();
				}
				return _oldFriendUIDSet;
			}
			set
			{
				_oldFriendUIDSet = value;
			}
		}

		public HashSet<int> OldRequestUIDSet
		{
			get
			{
				if (_oldRequestUIDSet == null)
				{
					_oldRequestUIDSet = new HashSet<int>();
				}
				return _oldRequestUIDSet;
			}
			set
			{
				_oldRequestUIDSet = value;
			}
		}

		public DateTime LastShowBindAccountWarningTime
		{
			get
			{
				return _lastShowBindAccountWarningTime;
			}
			set
			{
				_lastShowBindAccountWarningTime = value;
			}
		}

		public DateTime LastShowBindIdentityWarningTime
		{
			get
			{
				return _lastShowBindIdentityWarningTime;
			}
			set
			{
				_lastShowBindIdentityWarningTime = value;
			}
		}

		public List<MailCacheKey> OldMailCache
		{
			get
			{
				if (_allMailSet == null)
				{
					_allMailSet = new List<MailCacheKey>();
				}
				return _allMailSet;
			}
			set
			{
				_allMailSet = value;
			}
		}

		public HashSet<int> NeedPlayLevelAnimationSet
		{
			get
			{
				if (_needPlayLevelAnimationSet == null)
				{
					_needPlayLevelAnimationSet = new HashSet<int>();
				}
				return _needPlayLevelAnimationSet;
			}
			set
			{
				_needPlayLevelAnimationSet = value;
			}
		}

		public Dictionary<int, SubSkillStatus> SubSkillStatusDict
		{
			get
			{
				if (_subSkillStatusDict == null)
				{
					_subSkillStatusDict = GetDefaultSubSkillStatusDict();
				}
				return _subSkillStatusDict;
			}
			set
			{
				_subSkillStatusDict = value;
			}
		}

		public DateTime EndThunderDateTime
		{
			get
			{
				return _endThunderWeatherTime;
			}
			set
			{
				_endThunderWeatherTime = value;
			}
		}

		public DateTime NextRandomDateTime
		{
			get
			{
				return _nextRandomWeatherTime;
			}
			set
			{
				_nextRandomWeatherTime = value;
			}
		}

		public string CurrentWeatherConfigPath
		{
			get
			{
				if (string.IsNullOrEmpty(_currentWeatherConfigPath))
				{
					_currentWeatherConfigPath = "Rendering/MainMenuAtmosphereConfig/Lightning";
				}
				return _currentWeatherConfigPath;
			}
			set
			{
				_currentWeatherConfigPath = value;
			}
		}

		public int CurrentWeatherSceneID
		{
			get
			{
				return _currentWeatherSceneID;
			}
			set
			{
				_currentWeatherSceneID = value;
			}
		}

		public bool isDirty
		{
			get
			{
				return _isDirty;
			}
		}

		public GetLastEndlessRewardDataRsp LastRewardData
		{
			get
			{
				return _lastRewardData;
			}
			set
			{
				_lastRewardData = value;
			}
		}

		public HashSet<uint> OldBulletinIDSet
		{
			get
			{
				if (_oldBulletinIDSet == null)
				{
					_oldBulletinIDSet = new HashSet<uint>();
				}
				return _oldBulletinIDSet;
			}
			set
			{
				_oldBulletinIDSet = value;
			}
		}

		public Dictionary<CabinType, bool> CabinNeedToShowLevelUpCompleteSet
		{
			get
			{
				if (_cabinNeedToShowLevelUpCompleteSet == null)
				{
					_cabinNeedToShowLevelUpCompleteSet = new Dictionary<CabinType, bool>();
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)2] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)6] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)7] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)3] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)4] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)5] = false;
					_cabinNeedToShowLevelUpCompleteSet[(CabinType)1] = false;
				}
				return _cabinNeedToShowLevelUpCompleteSet;
			}
			set
			{
				_cabinNeedToShowLevelUpCompleteSet = value;
			}
		}

		public Dictionary<CabinType, bool> CabinNeedToShowNewUnlockDict
		{
			get
			{
				if (_cabinNeedToShowNewUnlockDict == null)
				{
					_cabinNeedToShowNewUnlockDict = new Dictionary<CabinType, bool>();
				}
				return _cabinNeedToShowNewUnlockDict;
			}
			set
			{
				_cabinNeedToShowNewUnlockDict = value;
			}
		}

		public int LastGalAvatarId
		{
			get
			{
				return _lastGalAvatarId;
			}
			set
			{
				_lastGalAvatarId = value;
			}
		}

		public string Receipt
		{
			get
			{
				if (_receipt == null)
				{
					return string.Empty;
				}
				return _receipt;
			}
			set
			{
				_receipt = value;
			}
		}

		public int PayMethod
		{
			get
			{
				return _payMethod;
			}
			set
			{
				_payMethod = value;
			}
		}

		public DateTime LastShowBulletinTime
		{
			get
			{
				return _lastShowBulletinTime;
			}
			set
			{
				_lastShowBulletinTime = value;
			}
		}

		public uint LoginRandomNum
		{
			get
			{
				return _loginRandomNum;
			}
			set
			{
				_loginRandomNum = value;
			}
		}

		public byte[] ProcessingStageEndReq
		{
			get
			{
				return _processingStageEndReq;
			}
			set
			{
				_processingStageEndReq = value;
			}
		}

		public WeatherInfo LastWeatherInfo
		{
			get
			{
				return _lastWeatherInfo;
			}
			set
			{
				_lastWeatherInfo = value;
			}
		}

		public DateTime LastCrashReportTime
		{
			get
			{
				return _lastCrashReportTime;
			}
			set
			{
				_lastCrashReportTime = value;
			}
		}

		public bool EnableRealTimeWeather
		{
			get
			{
				return _enableRealTimeWeather;
			}
			set
			{
				_enableRealTimeWeather = value;
			}
		}

		public bool HasShowInviteHintDialog
		{
			get
			{
				return _hasShowInviteHintDialog;
			}
			set
			{
				_hasShowInviteHintDialog = value;
			}
		}

		public UserLocalDataItem()
		{
			_doneBehaviourList = new List<string>();
			_readMailIdList = new List<MailCacheKey>();
			_friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
			_lastChapterID = 1;
			_lastActIndex = 0;
			_difficulty = 1;
			_storageShowTabName = "WeaponTab";
			_personalNotificationSetting = new ConfigNotificationSetting();
			_oldFriendUIDSet = new HashSet<int>();
			_oldRequestUIDSet = new HashSet<int>();
			_allMailSet = new List<MailCacheKey>();
			_needPlayLevelAnimationSet = new HashSet<int>();
			_currentWeatherSceneID = 0;
			_endThunderWeatherTime = TimeUtil.Now;
			_nextRandomWeatherTime = TimeUtil.Now;
			_oldBulletinIDSet = new HashSet<uint>();
			_lastGalAvatarId = 0;
			_loginRandomNum = 0u;
			_lastWeatherInfo = new WeatherInfo();
			_enableRealTimeWeather = true;
			_hasShowInviteHintDialog = false;
		}

		private Dictionary<int, SubSkillStatus> GetDefaultSubSkillStatusDict()
		{
			Dictionary<int, SubSkillStatus> dictionary = new Dictionary<int, SubSkillStatus>();
			foreach (AvatarDataItem userAvatar in Singleton<AvatarModule>.Instance.UserAvatarList)
			{
				foreach (AvatarSkillDataItem skillData in userAvatar.skillDataList)
				{
					foreach (AvatarSubSkillDataItem avatarSubSkill in skillData.avatarSubSkillList)
					{
						if (avatarSubSkill.ShouldShowHintPoint())
						{
							dictionary[avatarSubSkill.subSkillID] = avatarSubSkill.Status;
						}
					}
				}
			}
			return dictionary;
		}

		public void StartDirtyCheck()
		{
			_isDirty = false;
		}

		public bool EndDirtyCheck()
		{
			bool result = _isDirty;
			_isDirty = true;
			return result;
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		public int IsVisited_Tutorial(int stepID)
		{
			if (_visitedTutorial == null)
			{
				_visitedTutorial = new Dictionary<int, int>();
			}
			if (!_visitedTutorial.ContainsKey(stepID))
			{
				return -1;
			}
			return _visitedTutorial[stepID];
		}

		public void SetVisited_Tutorial(int stepID)
		{
			if (!_visitedTutorial.ContainsKey(stepID))
			{
				_visitedTutorial.Add(stepID, 0);
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			else if (_visitedTutorial[stepID] == 0)
			{
				_visitedTutorial[stepID] = 1;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public bool IsVisited_CabinTechTreeNode(int id)
		{
			if (_cabinTechTreeNodeVisited == null)
			{
				_cabinTechTreeNodeVisited = new Dictionary<int, bool>();
			}
			if (!_cabinTechTreeNodeVisited.ContainsKey(id))
			{
				_cabinTechTreeNodeVisited.Add(id, false);
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return _cabinTechTreeNodeVisited[id];
		}

		public void SetVisited_CabinTechTreeNode(int id)
		{
			if (!_cabinTechTreeNodeVisited[id])
			{
				_cabinTechTreeNodeVisited[id] = true;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}
	}
}
