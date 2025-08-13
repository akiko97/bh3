namespace MoleMole
{
	public class UnlockUIDataReaderExtend
	{
		public static void LoadFromFileAndBuildMap()
		{
			UnlockUIDataReader.LoadFromFile();
		}

		public static bool UnLockByMission(int id)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I4
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Invalid comparison between Unknown and I4
			UnlockUIData unlockUIDataByKey = UnlockUIDataReader.GetUnlockUIDataByKey(id);
			if (unlockUIDataByKey.unlockByMission <= 0)
			{
				return true;
			}
			MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(unlockUIDataByKey.unlockByMission);
			if (missionDataItem == null)
			{
				return false;
			}
			if (((int)missionDataItem.status == 2 && unlockUIDataByKey.OnDoing > 0) || ((int)missionDataItem.status == 3 && unlockUIDataByKey.OnFinish > 0) || ((int)missionDataItem.status == 5 && unlockUIDataByKey.OnClose > 0))
			{
				return true;
			}
			return false;
		}

		public static bool UnlockByTutorial(int id)
		{
			UnlockUIData unlockUIDataByKey = UnlockUIDataReader.GetUnlockUIDataByKey(id);
			if (unlockUIDataByKey.unlockByTutorial <= 0)
			{
				return true;
			}
			return Singleton<TutorialModule>.Instance.IsTutorialIDFinish(unlockUIDataByKey.unlockByTutorial);
		}
	}
}
