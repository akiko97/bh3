using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ChapterMetaData : IHashable
	{
		public readonly int chapterId;

		public readonly string title;

		public readonly string chapterDetail;

		public readonly List<int> monsterList;

		public readonly int chapterType;

		public readonly string coverPicture;

		public ChapterMetaData(int chapterId, string title, string chapterDetail, List<int> monsterList, int chapterType, string coverPicture)
		{
			this.chapterId = chapterId;
			this.title = title;
			this.chapterDetail = chapterDetail;
			this.monsterList = monsterList;
			this.chapterType = chapterType;
			this.coverPicture = coverPicture;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(chapterId, ref lastHash);
			HashUtils.ContentHashOnto(title, ref lastHash);
			HashUtils.ContentHashOnto(chapterDetail, ref lastHash);
			if (monsterList != null)
			{
				foreach (int monster in monsterList)
				{
					HashUtils.ContentHashOnto(monster, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(chapterType, ref lastHash);
			HashUtils.ContentHashOnto(coverPicture, ref lastHash);
		}
	}
}
