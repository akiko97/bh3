using System.Collections.Generic;

namespace MoleMole
{
	public static class CommonUtils
	{
		public delegate string CommonFileReader(string fileName);

		public static CommonFileReader commonFileReader;

		public static string LoadTextFileToString(string filePath)
		{
			if (commonFileReader != null)
			{
				return commonFileReader(filePath);
			}
			return null;
		}

		public static List<int> GetIntListFromString(string str)
		{
			List<int> list = new List<int>();
			if (string.IsNullOrEmpty(str))
			{
				return list;
			}
			string[] array = str.Split(","[0]);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(int.Parse(array[i]));
			}
			return list;
		}

		public static List<float> GetFloatListFromString(string str)
		{
			List<float> list = new List<float>();
			if (string.IsNullOrEmpty(str))
			{
				return list;
			}
			string[] array = str.Split(","[0]);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(float.Parse(array[i]));
			}
			return list;
		}

		public static List<string> GetStringListFromString(string str, char[] seperator = null)
		{
			if (seperator == null)
			{
				seperator = new char[1] { ',' };
			}
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(str))
			{
				return list;
			}
			string[] array = str.Split(seperator);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
			return list;
		}
	}
}
