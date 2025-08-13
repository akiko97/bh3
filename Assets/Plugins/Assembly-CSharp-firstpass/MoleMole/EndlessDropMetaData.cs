using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EndlessDropMetaData : IHashable
	{
		public readonly int group;

		public readonly int level;

		public readonly List<int> dropList;

		public readonly List<int> dropDisplayList;

		public readonly List<int> firstDropList;

		public readonly List<int> firstDropDisplayList;

		public EndlessDropMetaData(int group, int level, List<int> dropList, List<int> dropDisplayList, List<int> firstDropList, List<int> firstDropDisplayList)
		{
			this.group = group;
			this.level = level;
			this.dropList = dropList;
			this.dropDisplayList = dropDisplayList;
			this.firstDropList = firstDropList;
			this.firstDropDisplayList = firstDropDisplayList;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(group, ref lastHash);
			HashUtils.ContentHashOnto(level, ref lastHash);
			if (dropList != null)
			{
				foreach (int drop in dropList)
				{
					HashUtils.ContentHashOnto(drop, ref lastHash);
				}
			}
			if (dropDisplayList != null)
			{
				foreach (int dropDisplay in dropDisplayList)
				{
					HashUtils.ContentHashOnto(dropDisplay, ref lastHash);
				}
			}
			if (firstDropList != null)
			{
				foreach (int firstDrop in firstDropList)
				{
					HashUtils.ContentHashOnto(firstDrop, ref lastHash);
				}
			}
			if (firstDropDisplayList == null)
			{
				return;
			}
			foreach (int firstDropDisplay in firstDropDisplayList)
			{
				HashUtils.ContentHashOnto(firstDropDisplay, ref lastHash);
			}
		}
	}
}
