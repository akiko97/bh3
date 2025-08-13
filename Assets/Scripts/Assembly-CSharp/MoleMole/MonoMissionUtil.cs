using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoMissionUtil : MonoBehaviour
	{
		private List<MissionDataItem> _previewList = new List<MissionDataItem>();

		public void Init()
		{
			_previewList.Clear();
		}

		public void AddPreviewMission(MissionDataItem mission)
		{
			_previewList.Add(mission);
		}

		private void Update()
		{
			bool flag = false;
			uint num = 0u;
			for (int num2 = _previewList.Count - 1; num2 >= 0; num2--)
			{
				MissionDataItem missionDataItem = _previewList[num2];
				DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint)missionDataItem.beginTime);
				if ((dateTimeFromTimeStamp - TimeUtil.Now).TotalSeconds <= (double)missionDataItem.metaData.PreviewTime)
				{
					flag = true;
					num = (uint)missionDataItem.id;
					_previewList.RemoveAt(num2);
				}
			}
			if (flag)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, num));
			}
		}
	}
}
