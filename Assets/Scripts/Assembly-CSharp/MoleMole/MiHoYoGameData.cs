using UnityEngine;

namespace MoleMole
{
	public class MiHoYoGameData
	{
		private const string GENERAL_DATA_KEY = "GENERAL_DATA";

		private GeneralLocalDataItem _generalData;

		private UserLocalDataItem _userLocalData;

		public UserLocalDataItem LocalData
		{
			get
			{
				CheckVersionAndClearIfNeed();
				if (_userLocalData == null)
				{
					string prefsKey = GetPrefsKey();
					if (!PlayerPrefs.HasKey(prefsKey))
					{
						InitUserLocalData();
					}
					else
					{
						_userLocalData = ConfigUtil.LoadJSONStrConfig<UserLocalDataItem>(PlayerPrefs.GetString(prefsKey));
						if (_userLocalData == null)
						{
							InitUserLocalData();
						}
					}
				}
				return _userLocalData;
			}
		}

		public GeneralLocalDataItem GeneralLocalData
		{
			get
			{
				CheckVersionAndClearIfNeed();
				if (_generalData == null)
				{
					if (!PlayerPrefs.HasKey("GENERAL_DATA"))
					{
						InitGeneralData();
					}
					else
					{
						_generalData = ConfigUtil.LoadJSONStrConfig<GeneralLocalDataItem>(PlayerPrefs.GetString("GENERAL_DATA"));
						if (_generalData == null)
						{
							InitGeneralData();
						}
					}
				}
				return _generalData;
			}
		}

		private MiHoYoGameData()
		{
			_userLocalData = null;
		}

		private void InitUserLocalData()
		{
			_userLocalData = new UserLocalDataItem();
			Save();
		}

		private void InitGeneralData()
		{
			_generalData = new GeneralLocalDataItem();
			SaveGeneralData();
		}

		public void Save()
		{
			CheckThreadSafe();
			string value = ConfigUtil.SaveJSONStrConfig(_userLocalData);
			PlayerPrefs.SetString(GetPrefsKey(), value);
		}

		public void SaveGeneralData()
		{
			CheckThreadSafe();
			string value = ConfigUtil.SaveJSONStrConfig(_generalData);
			PlayerPrefs.SetString("GENERAL_DATA", value);
		}

		public static void DeleteAllData()
		{
			PlayerPrefs.DeleteAll();
		}

		private string GetPrefsKey()
		{
			int userId = Singleton<PlayerModule>.Instance.playerData.userId;
			return "USD_" + userId;
		}

		private void CheckThreadSafe()
		{
			PlayerPrefs.HasKey("TestThread");
		}

		private void CheckVersionAndClearIfNeed()
		{
			if (_generalData != null && _generalData.UserLocalDataVersion != LocalDataVersion.version)
			{
				DeleteAllData();
				_generalData = new GeneralLocalDataItem();
				_generalData.UserLocalDataVersion = LocalDataVersion.version;
				SaveGeneralData();
			}
		}
	}
}
