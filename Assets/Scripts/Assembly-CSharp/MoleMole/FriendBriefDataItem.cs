using proto;

namespace MoleMole
{
	public class FriendBriefDataItem
	{
		public int uid;

		private string _nickName;

		public int level;

		public int showAvatarID;

		public int avatarCombat;

		public int avatarStar;

		public int avatarLevel;

		public AvatarSkillDataItem AvatarLeaderSkill;

		public string nickName
		{
			get
			{
				return (!string.IsNullOrEmpty(_nickName)) ? _nickName : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", uid);
			}
		}

		public string AvatarIconPath
		{
			get
			{
				return AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(showAvatarID, avatarStar).iconPath;
			}
		}

		public FriendBriefDataItem(PlayerFriendBriefData briefData)
		{
			uid = (int)briefData.uid;
			_nickName = briefData.nickname;
			level = (int)briefData.level;
			showAvatarID = (int)briefData.avatar_id;
			avatarCombat = (int)briefData.avatar_combat;
			avatarStar = (int)briefData.avatar_star;
			avatarLevel = (int)briefData.avatar_level;
			if (avatarLevel == 0)
			{
				avatarLevel = 1;
			}
			AvatarLeaderSkill = new AvatarDataItem(showAvatarID).GetLeaderSkill();
			AvatarLeaderSkill.UnLocked = avatarLevel >= AvatarLeaderSkill.UnLockLv && avatarStar >= AvatarLeaderSkill.UnLockStar;
			AvatarSubSkillDetailData subSkill;
			foreach (AvatarSubSkillDetailData item in briefData.main_sub_skill_list)
			{
				subSkill = item;
				AvatarSubSkillDataItem avatarSubSkillDataItem = AvatarLeaderSkill.avatarSubSkillList.Find((AvatarSubSkillDataItem x) => x.subSkillID == (int)subSkill.sub_skill_id);
				if (avatarSubSkillDataItem != null)
				{
					avatarSubSkillDataItem.level = (int)subSkill.level;
				}
			}
		}

		public static int CompareToFriendNew(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (!Singleton<FriendModule>.Instance.IsOldFriend(lobj.uid))
			{
				return -1;
			}
			if (!Singleton<FriendModule>.Instance.IsOldFriend(robj.uid))
			{
				return 1;
			}
			return CompareToLevelDesc(lobj, robj);
		}

		public static int CompareToRequestNew(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (!Singleton<FriendModule>.Instance.IsOldRequest(lobj.uid))
			{
				return -1;
			}
			if (!Singleton<FriendModule>.Instance.IsOldRequest(robj.uid))
			{
				return 1;
			}
			return CompareToLevelDesc(lobj, robj);
		}

		public static int CompareToLevelAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.level != robj.level)
			{
				return lobj.level - robj.level;
			}
			if (lobj.avatarStar != robj.avatarStar)
			{
				return lobj.avatarStar - robj.avatarStar;
			}
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return lobj.avatarCombat - robj.avatarCombat;
			}
			return lobj.uid - robj.uid;
		}

		public static int CompareToLevelDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.level != robj.level)
			{
				return robj.level - lobj.level;
			}
			if (lobj.avatarStar != robj.avatarStar)
			{
				return robj.avatarStar - lobj.avatarStar;
			}
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return robj.avatarCombat - lobj.avatarCombat;
			}
			return lobj.uid - robj.uid;
		}

		public static int CompareToAvatarStarAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.avatarStar != robj.avatarStar)
			{
				return lobj.avatarStar - robj.avatarStar;
			}
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return lobj.avatarCombat - robj.avatarCombat;
			}
			if (lobj.level != robj.level)
			{
				return lobj.level - robj.level;
			}
			return lobj.uid - robj.uid;
		}

		public static int CompareToAvatarStarDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.avatarStar != robj.avatarStar)
			{
				return robj.avatarStar - lobj.avatarStar;
			}
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return robj.avatarCombat - lobj.avatarCombat;
			}
			if (lobj.level != robj.level)
			{
				return robj.level - lobj.level;
			}
			return lobj.uid - robj.uid;
		}

		public static int CompareToAvatarCombatAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return lobj.avatarCombat - robj.avatarCombat;
			}
			if (lobj.level != robj.level)
			{
				return lobj.level - robj.level;
			}
			if (lobj.avatarStar != robj.avatarStar)
			{
				return lobj.avatarStar - robj.avatarStar;
			}
			return lobj.uid - robj.uid;
		}

		public static int CompareToAvatarCombatDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
		{
			if (lobj.avatarCombat != robj.avatarCombat)
			{
				return robj.avatarCombat - lobj.avatarCombat;
			}
			if (lobj.level != robj.level)
			{
				return robj.level - lobj.level;
			}
			if (lobj.avatarStar != robj.avatarStar)
			{
				return robj.avatarStar - lobj.avatarStar;
			}
			return lobj.uid - robj.uid;
		}
	}
}
