using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class EndlessActivityDataItem : ActivityDataItemBase
	{
		private static EndlessActivityDataItem _instance;

		private EndlessActivityDataItem()
		{
			_status = Status.InProgress;
		}

		public static EndlessActivityDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new EndlessActivityDataItem();
			}
			return _instance;
		}

		public override int GetActivityID()
		{
			return 9001;
		}

		public override ActivityType GetActivityType()
		{
			return (ActivityType)0;
		}

		public override string GetActitityTitle()
		{
			return "Endless";
		}

		public override string GetActivityDescription()
		{
			return "Endless";
		}

		public override string GetActivityEnterImgPath()
		{
			return "SpriteOutput/ChapterCover/Event/BookEventEndless";
		}

		public override List<int> GetLevelIDList()
		{
			List<int> list = new List<int>();
			list.Add(1);
			return list;
		}

		public override int GetMinPlayerLevelLimit()
		{
			return Singleton<PlayerModule>.Instance.playerData.endlessMinPlayerLevel;
		}

		public int GetEndlessMaxProgress()
		{
			return Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress;
		}

		public override Status GetStatus()
		{
			_status = ((Singleton<PlayerModule>.Instance.playerData.teamLevel >= GetMinPlayerLevelLimit()) ? Status.InProgress : Status.Locked);
			return _status;
		}

		public override void InitStatusOnPacket()
		{
			_status = Status.WaitToStart;
		}
	}
}
