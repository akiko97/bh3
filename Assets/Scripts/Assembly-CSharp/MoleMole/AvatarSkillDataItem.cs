using System.Collections.Generic;
using System.Text.RegularExpressions;
using proto;

namespace MoleMole
{
	public class AvatarSkillDataItem
	{
		private const char LOGIC_NONE = '0';

		private const char LOGIC_ADD = '+';

		private const char LOGIC_MINUS = '-';

		private const char LOGIC_REPLACE = 'R';

		private const int LEADER_SKILL_ORDER = 5;

		private int avatarID;

		public int skillID;

		private AvatarSkillMetaData _metaData;

		public bool UnLocked;

		public List<AvatarSubSkillDataItem> avatarSubSkillList;

		private Dictionary<int, AvatarSubSkillDataItem> _avatarSubSkillMap;

		public int UnLockLv
		{
			get
			{
				return _metaData.unlockLv;
			}
		}

		public int UnLockStar
		{
			get
			{
				return _metaData.unlockStar;
			}
		}

		public int ShowOrder
		{
			get
			{
				return _metaData.showOrder;
			}
		}

		public bool IsLeaderSkill
		{
			get
			{
				return ShowOrder == 5;
			}
		}

		public string SkillName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.name);
			}
		}

		public string SkillInfo
		{
			get
			{
				string text = LocalizationGeneralLogic.GetText(_metaData.info, GetParam1(), GetParam2(), GetParam3());
				text = text.Replace("{{", string.Empty);
				return text.Replace("}}", string.Empty);
			}
		}

		public string SkillShortInfo
		{
			get
			{
				string pattern = "{{(.*)}}";
				string text = LocalizationGeneralLogic.GetText(_metaData.info, GetParam1(), GetParam2(), GetParam3());
				if (Regex.IsMatch(text, pattern))
				{
					return Regex.Match(text, pattern).Groups[1].Value.ToString();
				}
				return SkillInfo;
			}
		}

		public string SkillStep
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.skillStep);
			}
		}

		public bool CanTry
		{
			get
			{
				return _metaData.canTry == 1;
			}
		}

		public string IconPath
		{
			get
			{
				return _metaData.iconPath;
			}
		}

		public string IconPathInLevel
		{
			get
			{
				return _metaData.iconPathInLevel;
			}
		}

		public string ButtonName
		{
			get
			{
				return _metaData.buttonName;
			}
		}

		public AvatarSkillDataItem(int skillID, int avatarID)
		{
			this.skillID = skillID;
			this.avatarID = avatarID;
			UnLocked = false;
			_metaData = AvatarSkillMetaDataReader.GetAvatarSkillMetaDataByKey(skillID);
			SetupDefaultSubSkillList();
		}

		public int GetLevelSum()
		{
			int num = 0;
			foreach (AvatarSubSkillDataItem avatarSubSkill in avatarSubSkillList)
			{
				num += avatarSubSkill.level;
			}
			return num;
		}

		public int GetMaxLevelSum()
		{
			int num = 0;
			foreach (AvatarSubSkillDataItem avatarSubSkill in avatarSubSkillList)
			{
				num += avatarSubSkill.MaxLv;
			}
			return num;
		}

		private void SetupDefaultSubSkillList()
		{
			avatarSubSkillList = new List<AvatarSubSkillDataItem>();
			_avatarSubSkillMap = new Dictionary<int, AvatarSubSkillDataItem>();
			List<int> avatarSubSkillIdList = AvatarSubSkillMetaDataReaderExtend.GetAvatarSubSkillIdList(skillID);
			foreach (int item in avatarSubSkillIdList)
			{
				AvatarSubSkillDataItem avatarSubSkillDataItem = new AvatarSubSkillDataItem(item, avatarID);
				avatarSubSkillList.Add(avatarSubSkillDataItem);
				_avatarSubSkillMap.Add(item, avatarSubSkillDataItem);
			}
		}

		public void SetupSubSkillListFromServer(AvatarSkillDetailData skillData, int avatarLevel, int avatarStar)
		{
			SetupDefaultSubSkillList();
			if (skillID == (int)skillData.skill_id)
			{
				foreach (AvatarSubSkillDetailData item in skillData.sub_skill_list)
				{
					if (_avatarSubSkillMap.ContainsKey((int)item.sub_skill_id))
					{
						_avatarSubSkillMap[(int)item.sub_skill_id].level = (int)item.level;
					}
				}
			}
			UnLocked = avatarLevel >= UnLockLv && avatarStar >= UnLockStar;
		}

		public AvatarSubSkillDataItem GetAvatarSubSkillBySubSkillId(int subSkillID)
		{
			return _avatarSubSkillMap[subSkillID];
		}

		public float GetParam1()
		{
			float paraValue = _metaData.paramBase_1;
			if (_metaData.paramLogic_1 != '0' && _avatarSubSkillMap.ContainsKey(_metaData.paramSubID_1) && _avatarSubSkillMap[_metaData.paramSubID_1].UnLocked)
			{
				float paraValue2 = _avatarSubSkillMap[_metaData.paramSubID_1].GetParaValue(_metaData.paramSubIndex_1);
				ApplySubSkillToSkillPara(paraValue2, _metaData.paramLogic_1, ref paraValue);
			}
			return paraValue;
		}

		public float GetParam2()
		{
			float paraValue = _metaData.paramBase_2;
			if (_metaData.paramLogic_2 != '0' && _avatarSubSkillMap.ContainsKey(_metaData.paramSubID_2) && _avatarSubSkillMap[_metaData.paramSubID_2].UnLocked)
			{
				float paraValue2 = _avatarSubSkillMap[_metaData.paramSubID_2].GetParaValue(_metaData.paramSubIndex_2);
				ApplySubSkillToSkillPara(paraValue2, _metaData.paramLogic_2, ref paraValue);
			}
			return paraValue;
		}

		public float GetParam3()
		{
			float paraValue = _metaData.paramBase_3;
			if (_metaData.paramLogic_3 != '0' && _avatarSubSkillMap.ContainsKey(_metaData.paramSubID_3) && _avatarSubSkillMap[_metaData.paramSubID_3].UnLocked)
			{
				float paraValue2 = _avatarSubSkillMap[_metaData.paramSubID_3].GetParaValue(_metaData.paramSubIndex_3);
				ApplySubSkillToSkillPara(paraValue2, _metaData.paramLogic_3, ref paraValue);
			}
			return paraValue;
		}

		private void ApplySubSkillToSkillPara(float subParaValue, char logic, ref float paraValue)
		{
			switch (logic)
			{
			case '+':
				paraValue += subParaValue;
				break;
			case '-':
				paraValue -= subParaValue;
				break;
			case 'R':
				paraValue = subParaValue;
				break;
			}
		}

		public bool ShouldShowHintPointForSubSkill()
		{
			if (!UnLocked)
			{
				return false;
			}
			foreach (AvatarSubSkillDataItem avatarSubSkill in avatarSubSkillList)
			{
				if (avatarSubSkill.ShouldShowHintPoint())
				{
					return true;
				}
			}
			return false;
		}
	}
}
