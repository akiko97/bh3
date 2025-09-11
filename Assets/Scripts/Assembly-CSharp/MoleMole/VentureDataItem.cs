using System;
using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class VentureDataItem
	{
		public enum VentureStatus
		{
			None = 0,
			InProgress = 1,
			Done = 2
		}

		private VentureMetaData _metaData;

		public DateTime endTime;

		public List<int> dispatchAvatarIdList;

		public List<int> selectedAvatarList;

		public VentureStatus status;

		public string VentureName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.name);
			}
		}

		public int VentureID
		{
			get
			{
				return _metaData.ID;
			}
		}

		public int Level
		{
			get
			{
				return _metaData.level;
			}
		}

		public int Difficulty
		{
			get
			{
				return _metaData.difficulty;
			}
		}

		public string IconPath
		{
			get
			{
				return _metaData.iconPath;
			}
		}

		public string Desc
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.desc);
			}
		}

		public int StaminaCost
		{
			get
			{
				return _metaData.staminaCost;
			}
		}

		public int TimeCost
		{
			get
			{
				return _metaData.timeCost;
			}
		}

		public int AvatarMaxNum
		{
			get
			{
				return _metaData.avatarMaxNum;
			}
		}

		public float ExtraHcoinRatio
		{
			get
			{
				return (float)_metaData.extraHcoinChange / 10000f;
			}
		}

		public int ExtraHcoinNum
		{
			get
			{
				return _metaData.extraHcoinNum;
			}
		}

		public int RewardExp
		{
			get
			{
				RewardData rewardData = RewardDataReader.TryGetRewardDataByKey(_metaData.rewardId);
				if (rewardData != null && rewardData.RewardExp > 0)
				{
					return rewardData.RewardExp;
				}
				return 0;
			}
		}

		public List<int> RewardItemIDListToShow
		{
			get
			{
				return _metaData.rewardItemShowList;
			}
		}

		public VentureDataItem(int ventureMetaID)
		{
			_metaData = VentureMetaDataReader.TryGetVentureMetaDataByKey(ventureMetaID);
			status = VentureStatus.None;
			dispatchAvatarIdList = new List<int>();
			selectedAvatarList = new List<int>();
		}

		public void SetEndTime(uint timeStamp)
		{
			if (timeStamp == 0)
			{
				status = VentureStatus.None;
				return;
			}
			endTime = Miscs.GetDateTimeFromTimeStamp(timeStamp);
			if (endTime > TimeUtil.Now)
			{
				status = VentureStatus.InProgress;
			}
			else
			{
				status = VentureStatus.Done;
			}
		}

		public void SetDispatchAvatarList(List<uint> avatarIdList)
		{
			dispatchAvatarIdList.Clear();
			foreach (uint avatarId in avatarIdList)
			{
				dispatchAvatarIdList.Add((int)avatarId);
			}
		}

		public VentureCondition GetVentureCondition(int index)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			VentureCondition ventureCondition = new VentureCondition();
			switch (index)
			{
			case 0:
				if (_metaData.requestType1 < 1)
				{
					return null;
				}
				ventureCondition.condition = (IslandVentureDispatchCond)_metaData.requestType1;
				ventureCondition.para1 = _metaData.argument11;
				ventureCondition.para2 = _metaData.argument12;
				SetConditionDesc(ventureCondition);
				return ventureCondition;
			case 1:
				if (_metaData.requestType2 < 1)
				{
					return null;
				}
				ventureCondition.condition = (IslandVentureDispatchCond)_metaData.requestType2;
				ventureCondition.para1 = _metaData.argument21;
				ventureCondition.para2 = _metaData.argument22;
				SetConditionDesc(ventureCondition);
				return ventureCondition;
			case 2:
				if (_metaData.requestType3 < 1)
				{
					return null;
				}
				ventureCondition.condition = (IslandVentureDispatchCond)_metaData.requestType3;
				ventureCondition.para1 = _metaData.argument31;
				ventureCondition.para2 = _metaData.argument32;
				SetConditionDesc(ventureCondition);
				return ventureCondition;
			default:
				return null;
			}
		}

		public bool IsConditionMatch(VentureCondition condition)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected I4, but got Unknown
			if (condition == null)
			{
				return true;
			}
			if (selectedAvatarList.Count < 1)
			{
				return false;
			}
			AvatarModule instance = Singleton<AvatarModule>.Instance;
			IslandVentureDispatchCond condition2 = condition.condition;
			switch ((int)condition2 - 1)
			{
			case 0:
				return selectedAvatarList.Contains(condition.para1);
			case 1:
				foreach (int selectedAvatar in selectedAvatarList)
				{
					if (instance.GetAvatarByID(selectedAvatar).level < condition.para1)
					{
						return false;
					}
				}
				return true;
			case 2:
				foreach (int selectedAvatar2 in selectedAvatarList)
				{
					if (instance.GetAvatarByID(selectedAvatar2).level >= condition.para1)
					{
						return true;
					}
				}
				return false;
			case 3:
			{
				int num2 = 0;
				foreach (int selectedAvatar3 in selectedAvatarList)
				{
					num2 += instance.GetAvatarByID(selectedAvatar3).level;
				}
				return num2 >= condition.para1;
			}
			case 4:
				foreach (int selectedAvatar4 in selectedAvatarList)
				{
					if (instance.GetAvatarByID(selectedAvatar4).star < condition.para1)
					{
						return false;
					}
				}
				return true;
			case 5:
				foreach (int selectedAvatar5 in selectedAvatarList)
				{
					if (instance.GetAvatarByID(selectedAvatar5).star >= condition.para1)
					{
						return true;
					}
				}
				return false;
			case 6:
				return selectedAvatarList.Count >= condition.para1;
			case 7:
			{
				int num3 = 0;
				foreach (int selectedAvatar6 in selectedAvatarList)
				{
					AvatarDataItem avatarByID4 = instance.GetAvatarByID(selectedAvatar6);
					if (avatarByID4.Attribute == condition.para1)
					{
						num3++;
					}
				}
				return num3 >= condition.para2;
			}
			case 8:
			{
				int num = 0;
				foreach (int selectedAvatar7 in selectedAvatarList)
				{
					AvatarDataItem avatarByID3 = instance.GetAvatarByID(selectedAvatar7);
					if (avatarByID3.ClassId == condition.para1)
					{
						num++;
					}
				}
				return num >= condition.para2;
			}
			case 9:
			{
				HashSet<int> hashSet2 = new HashSet<int>();
				foreach (int selectedAvatar8 in selectedAvatarList)
				{
					AvatarDataItem avatarByID2 = instance.GetAvatarByID(selectedAvatar8);
					if (!hashSet2.Contains(avatarByID2.Attribute))
					{
						hashSet2.Add(avatarByID2.Attribute);
						continue;
					}
					return false;
				}
				return true;
			}
			case 10:
			{
				HashSet<int> hashSet = new HashSet<int>();
				foreach (int selectedAvatar9 in selectedAvatarList)
				{
					AvatarDataItem avatarByID = instance.GetAvatarByID(selectedAvatar9);
					if (!hashSet.Contains(avatarByID.ClassId))
					{
						hashSet.Add(avatarByID.ClassId);
						continue;
					}
					return false;
				}
				return true;
			}
			default:
				return false;
			}
		}

		public void SetConditionDesc(VentureCondition condition)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected I4, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected I4, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected I4, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected I4, but got Unknown
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Expected I4, but got Unknown
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Expected I4, but got Unknown
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Expected I4, but got Unknown
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Expected I4, but got Unknown
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Expected I4, but got Unknown
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Expected I4, but got Unknown
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Expected I4, but got Unknown
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Expected I4, but got Unknown
			if (condition != null)
			{
				AvatarModule instance = Singleton<AvatarModule>.Instance;
				IslandVentureDispatchCond condition2 = condition.condition;
				switch ((int)condition2 - 1)
				{
				case 0:
				{
					AvatarDataItem avatarByID = instance.GetAvatarByID(condition.para1);
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], avatarByID.FullName);
					break;
				}
				case 1:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], condition.para1);
					break;
				case 2:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], condition.para1);
					break;
				case 3:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], condition.para1);
					break;
				case 4:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], MiscData.Config.AvatarStarName[condition.para1]);
					break;
				case 5:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], MiscData.Config.AvatarStarName[condition.para1]);
					break;
				case 6:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], condition.para1);
					break;
				case 7:
				{
					string text2 = LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarAttributeName[condition.para1]);
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], text2, condition.para2);
					break;
				}
				case 8:
				{
					ClassMetaData classMetaDataByKey = ClassMetaDataReader.GetClassMetaDataByKey(condition.para1);
					string text = LocalizationGeneralLogic.GetText(classMetaDataByKey.firstName);
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition], text, condition.para2);
					break;
				}
				case 9:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition]);
					break;
				case 10:
					condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[(int)condition.condition]);
					break;
				}
			}
		}

		public bool IsConditionAllMatch()
		{
			if (selectedAvatarList.Count < 1)
			{
				return false;
			}
			for (int i = 0; i < 3; i++)
			{
				if (!IsConditionMatch(GetVentureCondition(i)))
				{
					return false;
				}
			}
			return true;
		}

		public void CleanUpSelectAvatarList()
		{
			List<int> toRemoveAvatarList = new List<int>();
			foreach (int selectedAvatar in selectedAvatarList)
			{
				if (Singleton<IslandModule>.Instance.IsAvatarDispatched(selectedAvatar))
				{
					toRemoveAvatarList.Add(selectedAvatar);
				}
			}
			selectedAvatarList.RemoveAll((int x) => toRemoveAvatarList.Contains(x));
		}

		public int GetStaminaReturnOnCancel()
		{
			return Mathf.FloorToInt((float)StaminaCost * (float)(endTime - TimeUtil.Now).TotalSeconds / (float)TimeCost);
		}
	}
}
