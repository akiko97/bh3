using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class LevelTutorialMetaData : IHashable
	{
		public readonly int tutorialId;

		public readonly int levelId;

		public readonly List<float> paramList;

		public readonly List<string> diaplayTarget;

		public readonly string lua;

		public LevelTutorialMetaData(int tutorialId, int levelId, List<float> paramList, List<string> diaplayTarget, string lua)
		{
			this.tutorialId = tutorialId;
			this.levelId = levelId;
			this.paramList = paramList;
			this.diaplayTarget = diaplayTarget;
			this.lua = lua;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(tutorialId, ref lastHash);
			HashUtils.ContentHashOnto(levelId, ref lastHash);
			if (paramList != null)
			{
				foreach (float param in paramList)
				{
					float value = param;
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (diaplayTarget != null)
			{
				foreach (string item in diaplayTarget)
				{
					HashUtils.ContentHashOnto(item, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(lua, ref lastHash);
		}
	}
}
