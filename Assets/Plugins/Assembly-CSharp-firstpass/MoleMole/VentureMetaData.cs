using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class VentureMetaData : IHashable
	{
		public class UnlockVentureItem
		{
			public readonly int level;

			public readonly int difficulty;

			public UnlockVentureItem(int level, int difficulty)
			{
				this.level = level;
				this.difficulty = difficulty;
			}

			public UnlockVentureItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				level = int.Parse(stringListFromString[0]);
				difficulty = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int ID;

		public readonly string name;

		public readonly int level;

		public readonly int difficulty;

		public readonly string iconPath;

		public readonly string desc;

		public readonly int staminaCost;

		public readonly int timeCost;

		public readonly int avatarMaxNum;

		public readonly int requestType1;

		public readonly int argument11;

		public readonly int argument12;

		public readonly int requestType2;

		public readonly int argument21;

		public readonly int argument22;

		public readonly int requestType3;

		public readonly int argument31;

		public readonly int argument32;

		public readonly int rewardId;

		public readonly int extraHcoinChange;

		public readonly int extraHcoinNum;

		public readonly List<int> rewardItemShowList;

		public readonly List<int> dropList;

		public VentureMetaData(int ID, string name, int level, int difficulty, string iconPath, string desc, int staminaCost, int timeCost, int avatarMaxNum, int requestType1, int argument11, int argument12, int requestType2, int argument21, int argument22, int requestType3, int argument31, int argument32, int rewardId, int extraHcoinChange, int extraHcoinNum, List<int> rewardItemShowList, List<int> dropList)
		{
			this.ID = ID;
			this.name = name;
			this.level = level;
			this.difficulty = difficulty;
			this.iconPath = iconPath;
			this.desc = desc;
			this.staminaCost = staminaCost;
			this.timeCost = timeCost;
			this.avatarMaxNum = avatarMaxNum;
			this.requestType1 = requestType1;
			this.argument11 = argument11;
			this.argument12 = argument12;
			this.requestType2 = requestType2;
			this.argument21 = argument21;
			this.argument22 = argument22;
			this.requestType3 = requestType3;
			this.argument31 = argument31;
			this.argument32 = argument32;
			this.rewardId = rewardId;
			this.extraHcoinChange = extraHcoinChange;
			this.extraHcoinNum = extraHcoinNum;
			this.rewardItemShowList = rewardItemShowList;
			this.dropList = dropList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(difficulty, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(desc, ref lastHash);
			HashUtils.ContentHashOnto(staminaCost, ref lastHash);
			HashUtils.ContentHashOnto(timeCost, ref lastHash);
			HashUtils.ContentHashOnto(avatarMaxNum, ref lastHash);
			HashUtils.ContentHashOnto(requestType1, ref lastHash);
			HashUtils.ContentHashOnto(argument11, ref lastHash);
			HashUtils.ContentHashOnto(argument12, ref lastHash);
			HashUtils.ContentHashOnto(requestType2, ref lastHash);
			HashUtils.ContentHashOnto(argument21, ref lastHash);
			HashUtils.ContentHashOnto(argument22, ref lastHash);
			HashUtils.ContentHashOnto(requestType3, ref lastHash);
			HashUtils.ContentHashOnto(argument31, ref lastHash);
			HashUtils.ContentHashOnto(argument32, ref lastHash);
			HashUtils.ContentHashOnto(rewardId, ref lastHash);
			HashUtils.ContentHashOnto(extraHcoinChange, ref lastHash);
			HashUtils.ContentHashOnto(extraHcoinNum, ref lastHash);
			if (rewardItemShowList != null)
			{
				foreach (int rewardItemShow in rewardItemShowList)
				{
					HashUtils.ContentHashOnto(rewardItemShow, ref lastHash);
				}
			}
			if (dropList == null)
			{
				return;
			}
			foreach (int drop in dropList)
			{
				HashUtils.ContentHashOnto(drop, ref lastHash);
			}
		}
	}
}
