using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoLevelPanel : MonoBehaviour
	{
		public GameObject level;

		public void SetupView(List<LevelDataItem> levels, LevelBtnClickCallBack OnLevelClick, Dictionary<LevelDataItem, Transform> levelTransDict, WeekDayActivityDataItem activityData = null, int totalFinishChallengeNum = 0)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Invalid comparison between Unknown and I4
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Invalid comparison between Unknown and I4
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Invalid comparison between Unknown and I4
			if (activityData == null || activityData.GetStatus() == ActivityDataItemBase.Status.InProgress)
			{
				foreach (LevelDataItem level in levels)
				{
					Transform parent = base.transform.Find(level.SectionId.ToString());
					Transform transform = Object.Instantiate(this.level).transform;
					transform.SetParent(parent, false);
					int num = 1;
					StageType levelType = level.LevelType;
					if (((int)levelType == 2 || (int)levelType == 3) && activityData != null)
					{
						num = activityData.maxEnterTimes - activityData.enterTimes;
					}
					transform.GetComponent<MonoLevelView>().SetupView(level, num > 0, OnLevelClick, totalFinishChallengeNum);
					if ((int)level.LevelType == 1)
					{
						levelTransDict.Add(level, transform);
					}
				}
				return;
			}
			base.transform.Find("Icon/Desc").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ActivityStatusImgPath[(int)activityData.GetStatus()]);
		}
	}
}
