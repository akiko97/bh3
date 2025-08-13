using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MoleMole
{
	public class OpeUtil
	{
		public class ApkCommentInfo
		{
			public bool isMihoyoComment;

			public string cps;

			public string checksum;
		}

		private const string SIGN_KEY = "1Sdfl0D98jc983BJG8O8fba";

		private const byte COMMENT_SALT = 204;

		private const string COMMENT_MAGIC_CODE = "BINLAO";

		private static string[] RESERVED_WORDS = new string[8] { "_version", "_account_uid", "_account_token", "_uid", "_nickname", "_level", "_vip_point", "_hcoin" };

		public static string ConvertEventUrl(string sourceUrl)
		{
			if (string.IsNullOrEmpty(sourceUrl))
			{
				return sourceUrl;
			}
			string baseUrl;
			Dictionary<string, string> paramDict;
			ParseUrl(sourceUrl, out baseUrl, out paramDict);
			SetupReservedParam(ref paramDict);
			paramDict["_time"] = Miscs.GetTimeStampFromDateTime(TimeUtil.Now).ToString();
			string strToEncode = GeneralUrlParamString(paramDict);
			string sourceStr = SecurityUtil.Base64Encoder(strToEncode);
			string text = SwapStr(sourceStr);
			string value = SecurityUtil.SHA256(text + "1Sdfl0D98jc983BJG8O8fba");
			return GeneralUrl(baseUrl, new Dictionary<string, string>
			{
				{ "auth_key", text },
				{ "sign", value }
			});
		}

		private static void SetupReservedParam(ref Dictionary<string, string> paramDict)
		{
			if (paramDict == null || paramDict.Count <= 0)
			{
				return;
			}
			int i = 0;
			for (int num = RESERVED_WORDS.Length; i < num; i++)
			{
				if (!paramDict.ContainsKey(RESERVED_WORDS[i]))
				{
					continue;
				}
				paramDict[RESERVED_WORDS[i]] = string.Empty;
				switch (RESERVED_WORDS[i])
				{
				case "_version":
					if (Singleton<NetworkManager>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<NetworkManager>.Instance.GetGameVersion();
					}
					break;
				case "_account_uid":
					if (Singleton<AccountManager>.Instance != null && Singleton<MiHoYoGameData>.Instance != null)
					{
						string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
						paramDict[RESERVED_WORDS[i]] = (string.IsNullOrEmpty(accountUid) ? Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.uid : accountUid);
					}
					break;
				case "_account_token":
					if (Singleton<AccountManager>.Instance != null && Singleton<MiHoYoGameData>.Instance != null)
					{
						string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
						paramDict[RESERVED_WORDS[i]] = (string.IsNullOrEmpty(accountToken) ? Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.token : accountToken);
					}
					break;
				case "_uid":
					if (Singleton<PlayerModule>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
					}
					break;
				case "_nickname":
					if (Singleton<PlayerModule>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<PlayerModule>.Instance.playerData.NickNameText;
					}
					break;
				case "_level":
					if (Singleton<PlayerModule>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<PlayerModule>.Instance.playerData.teamLevel.ToString();
					}
					break;
				case "_vip_point":
					if (Singleton<ShopWelfareModule>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<ShopWelfareModule>.Instance.totalPayHCoin.ToString();
					}
					break;
				case "_hcoin":
					if (Singleton<PlayerModule>.Instance != null)
					{
						paramDict[RESERVED_WORDS[i]] = Singleton<PlayerModule>.Instance.playerData.hcoin.ToString();
					}
					break;
				}
			}
		}

		public static void ParseUrl(string url, out string baseUrl, out Dictionary<string, string> paramDict)
		{
			baseUrl = string.Empty;
			paramDict = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			int num = url.IndexOf('?');
			if (num == -1)
			{
				baseUrl = url;
				return;
			}
			baseUrl = url.Substring(0, num);
			if (num == url.Length - 1)
			{
				return;
			}
			string input = url.Substring(num + 1);
			Regex regex = new Regex("(^|&)?([\\w]+)=([^&]+)(&|$)?");
			MatchCollection matchCollection = regex.Matches(input);
			foreach (Match item in matchCollection)
			{
				paramDict[item.Result("$2")] = item.Result("$3");
			}
		}

		private static string GeneralUrlParamString(Dictionary<string, string> paramDict)
		{
			if (paramDict == null || paramDict.Count <= 0)
			{
				return string.Empty;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> item in paramDict)
			{
				list.Add(item.Key + "=" + WWW.EscapeURL(item.Value));
			}
			return string.Join("&", list.ToArray());
		}

		public static string GeneralUrl(string baseUrl, Dictionary<string, string> paramDict)
		{
			return baseUrl + "?" + GeneralUrlParamString(paramDict);
		}

		private static string SwapStr(string sourceStr)
		{
			if (string.IsNullOrEmpty(sourceStr))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(sourceStr);
			int length = sourceStr.Length;
			for (int i = 0; i < length - 1; i += 2)
			{
				stringBuilder[i] = sourceStr[i + 1];
				stringBuilder[i + 1] = sourceStr[i];
			}
			return stringBuilder.ToString();
		}

		public static ApkCommentInfo GetApkComment()
		{
			ApkCommentInfo apkCommentInfo = new ApkCommentInfo();
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			string text = androidJavaObject.Call<string>("getPackageResourcePath", new object[0]);
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.miHoYo.ApkCommentReader", text, "BINLAO");
			if (androidJavaObject2 == null)
			{
				return apkCommentInfo;
			}
			byte[] array = androidJavaObject2.Call<byte[]>("getApkComment", new object[0]);
			apkCommentInfo.isMihoyoComment = androidJavaObject2.Call<bool>("isMihoyoComment", new object[0]);
			if (array != null && array.Length > 0)
			{
				if (apkCommentInfo.isMihoyoComment)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (byte)(0xCC ^ array[i]);
					}
					apkCommentInfo.cps = Encoding.Default.GetString(array);
				}
				else
				{
					apkCommentInfo.checksum = SecurityUtil.Md5(array);
				}
			}
			return apkCommentInfo;
		}
	}
}
