using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public static class GalTouchData
	{
		public const int BODY_PART_INDEX_FACE = 1;

		public const int BODY_PART_INDEX_HEAD = 2;

		public const int BODY_PART_INDEX_CHEST = 3;

		public const int BODY_PART_INDEX_CHEST_ADV = 4;

		public const int BODY_PART_INDEX_PRIVATE = 5;

		public const int BODY_PART_INDEX_PRIVATE_ADV = 6;

		public const int BODY_PART_INDEX_ARM = 7;

		public const int BODY_PART_INDEX_STOMACH = 8;

		public const int BODY_PART_INDEX_LEG = 9;

		private static List<TouchDataItem> _touchDataItemList;

		private static List<TouchLevelItem> _touchLevelItemList;

		private static List<TouchBuffItem> _touchBuffItemList;

		private static List<TouchMissionItem> _touchMissionItemList;

		public static void LoadFromFile()
		{
			LoadTouchData();
			LoadTouchLevelData();
			LoadTouchBuffData();
			LoadTouchMissionData();
		}

		private static void LoadTouchData()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/TouchData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_touchDataItemList = new List<TouchDataItem>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TouchDataItem touchDataItem = new TouchDataItem();
				touchDataItem.touchId = int.Parse(array2[0]);
				touchDataItem.level = int.Parse(array2[1]);
				touchDataItem.point = int.Parse(array2[2]);
				TouchDataItem touchDataItem2 = touchDataItem;
				string[] array3 = array2[3].Split(',');
				touchDataItem2.buff = new int[array3.Length];
				for (int k = 0; k < array3.Length; k++)
				{
					touchDataItem2.buff[k] = int.Parse(array3[k]);
				}
				_touchDataItemList.Add(touchDataItem2);
			}
		}

		private static void LoadTouchLevelData()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/TouchLevelData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_touchLevelItemList = new List<TouchLevelItem>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TouchLevelItem touchLevelItem = new TouchLevelItem();
				touchLevelItem.level = int.Parse(array2[0]);
				touchLevelItem.touchExp = int.Parse(array2[1]);
				touchLevelItem.prop = float.Parse(array2[2]);
				touchLevelItem.rate = float.Parse(array2[3]);
				touchLevelItem.battleGain = int.Parse(array2[4]);
				TouchLevelItem item = touchLevelItem;
				_touchLevelItemList.Add(item);
			}
		}

		private static void LoadTouchBuffData()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/TouchBuffData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_touchBuffItemList = new List<TouchBuffItem>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TouchBuffItem touchBuffItem = new TouchBuffItem();
				touchBuffItem.buffId = int.Parse(array2[0]);
				touchBuffItem.effect = array2[1];
				touchBuffItem.detail = array2[2];
				TouchBuffItem touchBuffItem2 = touchBuffItem;
				float.TryParse(array2[3], out touchBuffItem2.param1);
				float.TryParse(array2[4], out touchBuffItem2.param2);
				float.TryParse(array2[5], out touchBuffItem2.param3);
				float.TryParse(array2[6], out touchBuffItem2.param1Add);
				float.TryParse(array2[7], out touchBuffItem2.param2Add);
				float.TryParse(array2[8], out touchBuffItem2.param3Add);
				_touchBuffItemList.Add(touchBuffItem2);
			}
		}

		private static void LoadTouchMissionData()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/AvatarGoodfeelData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_touchMissionItemList = new List<TouchMissionItem>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				TouchMissionItem touchMissionItem = new TouchMissionItem();
				touchMissionItem.avatarId = int.Parse(array2[0]);
				touchMissionItem.goodFeelLevel = int.Parse(array2[1]);
				touchMissionItem.missionId = int.Parse(array2[2]);
				TouchMissionItem item = touchMissionItem;
				_touchMissionItemList.Add(item);
			}
		}

		public static int QueryLevelUpFeelNeed(int level)
		{
			if (level <= 0 || level > 4)
			{
				return 0;
			}
			TouchLevelItem touchLevelItem = null;
			int i = 0;
			for (int count = _touchLevelItemList.Count; i < count; i++)
			{
				if (_touchLevelItemList[i].level == level)
				{
					touchLevelItem = _touchLevelItemList[i];
					break;
				}
			}
			if (touchLevelItem == null)
			{
				return 0;
			}
			return touchLevelItem.touchExp;
		}

		public static int QueryLevelUpFeelNeedTouch(int level)
		{
			if (level <= 0 || level > 4)
			{
				return 0;
			}
			TouchLevelItem touchLevelItem = null;
			int i = 0;
			for (int count = _touchLevelItemList.Count; i < count; i++)
			{
				if (_touchLevelItemList[i].level == level)
				{
					touchLevelItem = _touchLevelItemList[i];
					break;
				}
			}
			if (touchLevelItem == null)
			{
				return 0;
			}
			float num = (float)touchLevelItem.touchExp * touchLevelItem.rate;
			return touchLevelItem.touchExp - (int)num;
		}

		public static int QueryLevelUpFeelNeedBattle(int level)
		{
			if (level <= 0 || level > 4)
			{
				return 0;
			}
			TouchLevelItem touchLevelItem = null;
			int i = 0;
			for (int count = _touchLevelItemList.Count; i < count; i++)
			{
				if (_touchLevelItemList[i].level == level)
				{
					touchLevelItem = _touchLevelItemList[i];
					break;
				}
			}
			if (touchLevelItem == null)
			{
				return 0;
			}
			float num = (float)touchLevelItem.touchExp * touchLevelItem.rate;
			return (int)num;
		}

		public static int QueryBattleGain(int level)
		{
			if (level <= 0 || level > 4)
			{
				return 0;
			}
			TouchLevelItem touchLevelItem = null;
			int i = 0;
			for (int count = _touchLevelItemList.Count; i < count; i++)
			{
				if (_touchLevelItemList[i].level == level)
				{
					touchLevelItem = _touchLevelItemList[i];
					break;
				}
			}
			if (touchLevelItem == null)
			{
				return 0;
			}
			return touchLevelItem.battleGain;
		}

		public static int QueryTouchFeel(int avatarID, int partIndex, int heartLevel)
		{
			int touchID = GetTouchID(avatarID, partIndex, heartLevel);
			if (touchID == 0)
			{
				return 0;
			}
			TouchDataItem touchDataItem = null;
			int i = 0;
			for (int count = _touchDataItemList.Count; i < count; i++)
			{
				if (_touchDataItemList[i].touchId == touchID)
				{
					touchDataItem = _touchDataItemList[i];
					break;
				}
			}
			if (touchDataItem == null)
			{
				return 0;
			}
			return touchDataItem.point;
		}

		public static int[] QueryTouchBuff(int avatarID, int partIndex, int heartLevel)
		{
			int touchID = GetTouchID(avatarID, partIndex, heartLevel);
			if (touchID == 0)
			{
				return null;
			}
			TouchDataItem touchDataItem = null;
			int i = 0;
			for (int count = _touchDataItemList.Count; i < count; i++)
			{
				if (_touchDataItemList[i].touchId == touchID)
				{
					touchDataItem = _touchDataItemList[i];
					break;
				}
			}
			if (touchDataItem == null)
			{
				return null;
			}
			return touchDataItem.buff;
		}

		public static TouchBuffItem GetTouchBuffItem(int buffId)
		{
			int i = 0;
			for (int count = _touchBuffItemList.Count; i < count; i++)
			{
				if (_touchBuffItemList[i].buffId == buffId)
				{
					return _touchBuffItemList[i];
				}
			}
			return null;
		}

		public static TouchLevelItem GetTouchLevelItem(int level)
		{
			int i = 0;
			for (int count = _touchLevelItemList.Count; i < count; i++)
			{
				if (_touchLevelItemList[i].level == level)
				{
					return _touchLevelItemList[i];
				}
			}
			return null;
		}

		public static TouchMissionItem GetTouchMissionItem(int avatarId, int heartLevel)
		{
			int i = 0;
			for (int count = _touchMissionItemList.Count; i < count; i++)
			{
				if (_touchMissionItemList[i].avatarId == avatarId && _touchMissionItemList[i].goodFeelLevel == heartLevel)
				{
					return _touchMissionItemList[i];
				}
			}
			return null;
		}

		private static int GetTouchID(int avatarID, int partIndex, int heartLevel)
		{
			if (partIndex <= 0 || partIndex > 9 || heartLevel <= 0 || heartLevel > 4)
			{
				return 0;
			}
			return avatarID / 100 * 10000 + heartLevel * 100 + partIndex;
		}
	}
}
