using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class ItempediaData
	{
		private static List<int> _blackList;

		public static void LoadFromFile()
		{
			List<string> list = new List<string>();
			TextAsset textAsset = Miscs.LoadResource("Data/_ExcelOutput/BlacklistData", BundleType.DATA_FILE) as TextAsset;
			string[] array = textAsset.text.Split("\n"[0]);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length >= 1)
				{
					list.Add(array[i]);
				}
			}
			int num = list.Count - 1;
			_blackList = new List<int>(num);
			for (int j = 1; j <= num; j++)
			{
				string[] array2 = list[j].Split("\t"[0]);
				_blackList.Add(int.Parse(array2[0]));
			}
		}

		public static bool IsInBlacklist(int id)
		{
			int i = 0;
			for (int count = _blackList.Count; i < count; i++)
			{
				if (_blackList[i] == id)
				{
					return true;
				}
			}
			return false;
		}
	}
}
