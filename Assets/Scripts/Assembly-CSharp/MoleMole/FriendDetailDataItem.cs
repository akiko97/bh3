using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class FriendDetailDataItem
	{
		public int uid;

		private string _nickName;

		public int level;

		public AvatarDataItem leaderAvatar;

		private string _desc;

		public string nickName
		{
			get
			{
				return (!string.IsNullOrEmpty(_nickName)) ? _nickName : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", uid);
			}
		}

		public string Desc
		{
			get
			{
				return (!string.IsNullOrEmpty(_desc)) ? _desc : LocalizationGeneralLogic.GetText("Menu_DefaultSelfDesc");
			}
		}

		public FriendDetailDataItem(int uid, string nickName, int level, AvatarDataItem leaderAvatar = null, string friendDesc = null)
		{
			this.uid = uid;
			_nickName = nickName;
			this.leaderAvatar = leaderAvatar;
			_desc = friendDesc;
		}

		public FriendDetailDataItem(PlayerDetailData playerDetailData)
		{
			uid = (int)playerDetailData.uid;
			_nickName = playerDetailData.nickname;
			level = (int)playerDetailData.level;
			SetLeaderAvatar(playerDetailData.leader_avatar);
			_desc = playerDetailData.self_desc;
		}

		public void SetLeaderAvatar(AvatarDetailData avatarDetailData)
		{
			leaderAvatar = new AvatarDataItem((int)avatarDetailData.avatar_id, (int)avatarDetailData.avatar_level, (int)avatarDetailData.avatar_star);
			WeaponDetailData weapon = avatarDetailData.weapon;
			if (weapon != null && WeaponMetaDataReader.GetWeaponMetaDataByKey((int)weapon.id) != null)
			{
				WeaponDataItem weaponDataItem = new WeaponDataItem(0, WeaponMetaDataReader.GetWeaponMetaDataByKey((int)weapon.id));
				weaponDataItem.level = (int)weapon.level;
				leaderAvatar.equipsMap[(EquipmentSlot)1] = weaponDataItem;
			}
			EquipmentSlot[] array = (EquipmentSlot[])(object)new EquipmentSlot[3]
			{
				(EquipmentSlot)2,
				(EquipmentSlot)3,
				(EquipmentSlot)4
			};
			List<StigmataDetailData> list = new List<StigmataDetailData>();
			list.Add(avatarDetailData.stigmata_1);
			list.Add(avatarDetailData.stigmata_2);
			list.Add(avatarDetailData.stigmata_3);
			List<StigmataDetailData> list2 = list;
			for (int i = 0; i < list2.Count; i++)
			{
				StigmataDetailData val = list2[i];
				if (val != null && StigmataMetaDataReader.GetStigmataMetaDataByKey((int)val.id) != null)
				{
					StigmataDataItem stigmataDataItem = new StigmataDataItem(0, StigmataMetaDataReader.GetStigmataMetaDataByKey((int)val.id));
					stigmataDataItem.level = (int)val.level;
					int pre_affix_id = (int)(val.pre_affix_idSpecified ? val.pre_affix_id : 0);
					int suf_affix_id = (int)(val.suf_affix_idSpecified ? val.suf_affix_id : 0);
					stigmataDataItem.SetAffixSkill(true, pre_affix_id, suf_affix_id);
					leaderAvatar.equipsMap[array[i]] = stigmataDataItem;
				}
			}
			List<AvatarSkillDetailData> skill_list = avatarDetailData.skill_list;
			foreach (AvatarSkillDetailData item in skill_list)
			{
				if (leaderAvatar.GetAvatarSkillBySkillID((int)item.skill_id) == null)
				{
					continue;
				}
				AvatarSkillDataItem avatarSkillBySkillID = leaderAvatar.GetAvatarSkillBySkillID((int)item.skill_id);
				foreach (AvatarSubSkillDetailData item2 in item.sub_skill_list)
				{
					if (avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int)item2.sub_skill_id) != null)
					{
						avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int)item2.sub_skill_id).level = (int)item2.level;
					}
				}
			}
		}
	}
}
