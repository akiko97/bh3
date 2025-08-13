using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MoleMole
{
	public class LocalizationGeneralLogic
	{
		private static Dictionary<string, string> _textMap;

		public static void InitOnGameStart()
		{
			_textMap = new Dictionary<string, string>();
			TextMapGameStartMetaDataReader.LoadFromFile();
			List<TextMapMetaData> itemList = TextMapGameStartMetaDataReader.GetItemList();
			foreach (TextMapMetaData item in itemList)
			{
				_textMap.Add(item.ID, item.Text);
			}
		}

		public static void InitOnDataAssetReady()
		{
			List<TextMapMetaData> itemList = TextMapMetaDataReader.GetItemList();
			foreach (TextMapMetaData item in itemList)
			{
				_textMap[item.ID] = item.Text;
			}
		}

		public static string GetText(string textID, params object[] replaceParams)
		{
			string textFromTextMap = GetTextFromTextMap(textID);
			return CompileMyDefinedPattern(textFromTextMap, replaceParams);
		}

		public static string GetTextWithParamArray<T>(string textID, T[] replaceParams)
		{
			string textFromTextMap = GetTextFromTextMap(textID);
			return CompileMyDefinedPattern(textFromTextMap, replaceParams);
		}

		public static string GetText(string textID, Color color, params object[] replaceParams)
		{
			string textFromTextMap = GetTextFromTextMap(textID);
			textFromTextMap = PreInsertRichTextCode(textFromTextMap, color);
			return CompileMyDefinedPattern(textFromTextMap, replaceParams);
		}

		public static string GetTextWithParamArray<T>(string textID, Color color, T[] replaceParams)
		{
			string textFromTextMap = GetTextFromTextMap(textID);
			textFromTextMap = PreInsertRichTextCode(textFromTextMap, color);
			return CompileMyDefinedPattern(textFromTextMap, replaceParams);
		}

		public static string GetNetworkErrCodeOutput(object retcode, params object[] replaceParams)
		{
			string input = retcode.GetType().ToString();
			Regex regex = new Regex("proto\\.(.*)\\+Retcode");
			if (!regex.IsMatch(input))
			{
				return string.Empty;
			}
			string errType = regex.Match(input).Groups[1].Value.Trim();
			NetworkErrCodeMetaData networkErrCodeMetaDataByKey = NetworkErrCodeMetaDataReader.GetNetworkErrCodeMetaDataByKey(errType, retcode.ToString());
			string empty = string.Empty;
			if (networkErrCodeMetaDataByKey != null)
			{
				return GetText(networkErrCodeMetaDataByKey.textMapID, replaceParams);
			}
			return retcode.ToString();
		}

		private static string GetTextFromTextMap(string textID)
		{
			string key = textID.Trim();
			if (!_textMap.ContainsKey(key))
			{
				return string.Empty;
			}
			return _textMap[key];
		}

		private static string PreInsertRichTextCode(string text, Color color)
		{
			string text2 = text;
			string pattern = "(#\\d\\[.+?\\]%?|#\\d%?)";
			foreach (Match item in Regex.Matches(text, pattern))
			{
				string value = item.Groups[1].Value;
				string arg = ColorUtility.ToHtmlStringRGBA(color);
				text2 = text2.Replace(value, string.Format("<color=#{0}>{1}</color>", arg, value));
			}
			return text2;
		}

		private static string CompileMyDefinedPattern<T>(string text, T[] replaceParams)
		{
			string text2 = text;
			text2 = text2.Replace("\\n", Environment.NewLine);
			for (int i = 0; i < replaceParams.Length; i++)
			{
				string text3 = "#" + (i + 1);
				if (text2.Contains(text3))
				{
					Regex regex = new Regex(text3 + "\\[f(\\d+)\\]");
					Regex regex2 = new Regex(text3 + "\\[f(\\d+)\\]%");
					if (text2.Contains(text3 + "[i]%"))
					{
						string newValue = string.Format("{0:P0}", float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
						text2 = text2.Replace(text3 + "[i]%", newValue);
					}
					else if (regex2.IsMatch(text2))
					{
						string format = "{0:P" + regex2.Match(text2).Groups[1].Value + "}";
						string replacement = string.Format(format, float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
						text2 = regex2.Replace(text2, replacement);
					}
					else if (text2.Contains(text3 + "%"))
					{
						string newValue2 = string.Format("{0:P0}", float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
						text2 = text2.Replace(text3 + "%", newValue2);
					}
					else if (regex.IsMatch(text2))
					{
						string format2 = "{0:N" + regex.Match(text2).Groups[1].Value + "}";
						text2 = regex.Replace(text2, string.Format(format2, replaceParams[i]));
					}
					else if (text2.Contains(text3 + "[i]"))
					{
						text2 = text2.Replace(text3 + "[i]", string.Format("{0:N0}", Mathf.Floor(float.Parse(replaceParams[i].ToString()))));
					}
					else if (text2.Contains("<color=" + text3))
					{
						text2 = text2.Replace("<color=#", "<color=");
						text2 = text2.Replace(text3, replaceParams[i].ToString());
						text2 = text2.Replace("<color=", "<color=#");
					}
					else
					{
						text2 = text2.Replace(text3, replaceParams[i].ToString());
					}
				}
			}
			return text2;
		}
	}
}
