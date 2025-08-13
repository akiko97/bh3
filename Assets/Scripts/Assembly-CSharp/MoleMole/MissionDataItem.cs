using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class MissionDataItem
	{
		public int id;

		public MissionStatus status;

		public int progress;

		public bool beginTimeSpecified;

		public int beginTime;

		public bool endTimeSpecified;

		public int endTime;

		public MissionData metaData;

		private static Dictionary<MissionStatus, int> _statusPriority = new Dictionary<MissionStatus, int>
		{
			{
				(MissionStatus)5,
				1
			},
			{
				(MissionStatus)2,
				2
			},
			{
				(MissionStatus)3,
				3
			},
			{
				(MissionStatus)1,
				2
			},
			{
				(MissionStatus)4,
				1
			}
		};

		private static Dictionary<int, int> _typePriority = new Dictionary<int, int>
		{
			{ 2, 1 },
			{ 1, 3 },
			{ 3, 2 },
			{ 4, 4 }
		};

		public MissionDataItem(Mission mission)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			id = (int)mission.mission_id;
			metaData = MissionDataReader.GetMissionDataByKey(id);
			status = mission.status;
			progress = (int)mission.progress;
			beginTimeSpecified = mission.begin_timeSpecified;
			beginTime = (int)mission.begin_time;
			endTimeSpecified = mission.end_timeSpecified;
			endTime = (int)mission.end_time;
		}

		public void UpdateFromMission(Mission mission)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			status = mission.status;
			progress = (int)mission.progress;
			beginTimeSpecified = mission.begin_timeSpecified;
			beginTime = (int)mission.begin_time;
			endTimeSpecified = mission.end_timeSpecified;
			endTime = (int)mission.end_time;
		}

		public bool IsMissionEqual(Mission mission)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return status == mission.status && progress == (int)mission.progress && (!mission.begin_timeSpecified || beginTime == (int)mission.begin_time) && (!mission.end_timeSpecified || endTime == (int)mission.end_time);
		}

		public static int CompareToMission(MissionDataItem lobj, MissionDataItem robj)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			int num = _statusPriority[lobj.status];
			int num2 = _statusPriority[robj.status];
			if (num != num2)
			{
				return num2 - num;
			}
			num = _typePriority[lobj.metaData.type];
			num2 = _typePriority[robj.metaData.type];
			if (num != num2)
			{
				return num2 - num;
			}
			return lobj.id - robj.id;
		}
	}
}
