using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public abstract class ActivityDataItemBase
	{
		public enum Status
		{
			Unavailable = 0,
			Over = 1,
			Locked = 2,
			WaitToStart = 3,
			InProgress = 4
		}

		public DateTime beginTime;

		public DateTime endTime;

		protected Status _status;

		public abstract int GetActivityID();

		public abstract string GetActitityTitle();

		public abstract string GetActivityDescription();

		public abstract string GetActivityEnterImgPath();

		public abstract List<int> GetLevelIDList();

		public abstract int GetMinPlayerLevelLimit();

		public abstract ActivityType GetActivityType();

		public abstract Status GetStatus();

		public abstract void InitStatusOnPacket();
	}
}
