using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class GeneralLocalDataItem
	{
		public class AccountData
		{
			public string uid;

			public string token;

			public string ext;

			public string name;

			public string email;

			public string mobile;

			public bool isEmailVerify;

			public bool isRealNameVerify;

			public string GetAccountName()
			{
				string[] array = new string[3] { name, email, mobile };
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						return array[i];
					}
				}
				return string.Empty;
			}
		}

		[SerializeField]
		private AccountData _account;

		[SerializeField]
		private int _lastLoginUserId;

		[SerializeField]
		private string _lastLoginAccountName;

		[SerializeField]
		private string _userLocalDataVersionId;

		[SerializeField]
		private ConfigGraphicsPersonalSetting _personalGraphicsSetting;

		[SerializeField]
		private ConfigAudioSetting _personalAudioSetting;

		[SerializeField]
		private Dictionary<Type, int> _uiStatistics;

		public AccountData Account
		{
			get
			{
				if (_account == null)
				{
					_account = new AccountData();
				}
				return _account;
			}
			set
			{
				_account = value;
			}
		}

		public int LastLoginUserId
		{
			get
			{
				return _lastLoginUserId;
			}
			set
			{
				_lastLoginUserId = value;
			}
		}

		public string UserLocalDataVersion
		{
			get
			{
				if (string.IsNullOrEmpty(_userLocalDataVersionId))
				{
					_userLocalDataVersionId = LocalDataVersion.version;
				}
				return _userLocalDataVersionId;
			}
			set
			{
				_userLocalDataVersionId = value;
			}
		}

		public string LastLoginAccountName
		{
			get
			{
				return _lastLoginAccountName;
			}
			set
			{
				_lastLoginAccountName = value;
			}
		}

		public ConfigGraphicsPersonalSetting PersonalGraphicsSetting
		{
			get
			{
				return _personalGraphicsSetting;
			}
			set
			{
				_personalGraphicsSetting = value;
			}
		}

		public ConfigAudioSetting PersonalAudioSetting
		{
			get
			{
				return _personalAudioSetting;
			}
			set
			{
				_personalAudioSetting = value;
			}
		}

		public GeneralLocalDataItem()
		{
			_account = new AccountData();
			_personalGraphicsSetting = new ConfigGraphicsPersonalSetting();
			_personalAudioSetting = new ConfigAudioSetting();
			_lastLoginUserId = 0;
			_uiStatistics = new Dictionary<Type, int>();
		}

		public void ClearLastLoginUser()
		{
			LastLoginAccountName = _account.GetAccountName();
			_account = new AccountData();
			_lastLoginUserId = 0;
		}

		public void AddContextShowCount(BaseContext context)
		{
			if (MiscData.Config != null && MiscData.Config.CollectUIStatistics && (context.uiType == UIType.Page || context.uiType == UIType.Dialog || context.uiType == UIType.SpecialDialog))
			{
				int value = 0;
				_uiStatistics.TryGetValue(context.GetType(), out value);
				_uiStatistics[context.GetType()] = value + 1;
				Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			}
		}

		public void ReportUIStatistics()
		{
			if (MiscData.Config != null && MiscData.Config.CollectUIStatistics && _uiStatistics != null && _uiStatistics.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("UI Statistics:");
				foreach (KeyValuePair<Type, int> item in _uiStatistics.OrderByDescending((KeyValuePair<Type, int> p) => p.Value))
				{
					stringBuilder.AppendLine(string.Format("{0}={1}", item.Key.Name, item.Value));
				}
				Singleton<QAManager>.GetInstance().SendMessageToSever(stringBuilder.ToString(), string.Empty);
			}
			if (_uiStatistics != null && _uiStatistics.Count > 0)
			{
				_uiStatistics.Clear();
				Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
			}
		}
	}
}
