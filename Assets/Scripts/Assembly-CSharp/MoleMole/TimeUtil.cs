using System;

namespace MoleMole
{
	public static class TimeUtil
	{
		private static bool debugTime = false;

		private static DateTime _testNow = DateTime.Parse("2016-03-03 12:00:00");

		private static int _dayTimeOffset;

		private static DateTime _lastCheckTime;

		private static TimeSpan _serverTimeSpan = TimeSpan.Zero;

		public static DateTime Now
		{
			get
			{
				if (debugTime)
				{
					return _testNow;
				}
				return DateTime.Now + _serverTimeSpan;
			}
		}

		public static DateTime DailyUpdateTime
		{
			get
			{
				return _lastCheckTime.Date.AddSeconds(_dayTimeOffset).AddDays(1.0);
			}
		}

		public static void SetDayTimeOffset(int seconds)
		{
			_dayTimeOffset = seconds;
			_lastCheckTime = Now;
		}

		public static void SetServerCurTime(uint timestamp)
		{
			DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(timestamp);
			_serverTimeSpan = dateTimeFromTimeStamp - Now;
		}

		public static bool AcrossDailyUpdateTime()
		{
			DateTime dateTime = _lastCheckTime.Date.AddSeconds(_dayTimeOffset).AddDays(1.0);
			return Now >= dateTime;
		}

		public static string GetRemainTime(TimeSpan remainTime)
		{
			if (remainTime.Days > 0)
			{
				return GetFormatTimeSplit(remainTime.Days) + ":" + GetFormatTimeSplit(remainTime.Hours) + ":" + GetFormatTimeSplit(remainTime.Minutes);
			}
			return GetFormatTimeSplit(remainTime.Hours) + ":" + GetFormatTimeSplit(remainTime.Minutes);
		}

		private static string GetFormatTimeSplit(int num)
		{
			if (num > 9)
			{
				return num.ToString();
			}
			return "0" + num;
		}
	}
}
