namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarSkillMetaData : IHashable
	{
		public readonly int skillId;

		public readonly string avatarName;

		public readonly string name;

		public readonly string info;

		public readonly int showOrder;

		public readonly int unlockLv;

		public readonly int unlockStar;

		public readonly int canTry;

		public readonly string skillStep;

		public readonly string iconPath;

		public readonly string buttonName;

		public readonly float paramBase_1;

		public readonly char paramLogic_1;

		public readonly int paramSubID_1;

		public readonly int paramSubIndex_1;

		public readonly float paramBase_2;

		public readonly char paramLogic_2;

		public readonly int paramSubID_2;

		public readonly int paramSubIndex_2;

		public readonly float paramBase_3;

		public readonly char paramLogic_3;

		public readonly int paramSubID_3;

		public readonly int paramSubIndex_3;

		public readonly string iconPathInLevel;

		public AvatarSkillMetaData(int skillId, string avatarName, string name, string info, int showOrder, int unlockLv, int unlockStar, int canTry, string skillStep, string iconPath, string buttonName, float paramBase_1, char paramLogic_1, int paramSubID_1, int paramSubIndex_1, float paramBase_2, char paramLogic_2, int paramSubID_2, int paramSubIndex_2, float paramBase_3, char paramLogic_3, int paramSubID_3, int paramSubIndex_3, string iconPathInLevel)
		{
			this.skillId = skillId;
			this.avatarName = avatarName;
			this.name = name;
			this.info = info;
			this.showOrder = showOrder;
			this.unlockLv = unlockLv;
			this.unlockStar = unlockStar;
			this.canTry = canTry;
			this.skillStep = skillStep;
			this.iconPath = iconPath;
			this.buttonName = buttonName;
			this.paramBase_1 = paramBase_1;
			this.paramLogic_1 = paramLogic_1;
			this.paramSubID_1 = paramSubID_1;
			this.paramSubIndex_1 = paramSubIndex_1;
			this.paramBase_2 = paramBase_2;
			this.paramLogic_2 = paramLogic_2;
			this.paramSubID_2 = paramSubID_2;
			this.paramSubIndex_2 = paramSubIndex_2;
			this.paramBase_3 = paramBase_3;
			this.paramLogic_3 = paramLogic_3;
			this.paramSubID_3 = paramSubID_3;
			this.paramSubIndex_3 = paramSubIndex_3;
			this.iconPathInLevel = iconPathInLevel;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(skillId, ref lastHash);
			HashUtils.ContentHashOnto(avatarName, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(info, ref lastHash);
			HashUtils.ContentHashOnto(showOrder, ref lastHash);
			HashUtils.ContentHashOnto(unlockLv, ref lastHash);
			HashUtils.ContentHashOnto(unlockStar, ref lastHash);
			HashUtils.ContentHashOnto(canTry, ref lastHash);
			HashUtils.ContentHashOnto(skillStep, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(buttonName, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_1, ref lastHash);
			HashUtils.ContentHashOnto(paramLogic_1, ref lastHash);
			HashUtils.ContentHashOnto(paramSubID_1, ref lastHash);
			HashUtils.ContentHashOnto(paramSubIndex_1, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_2, ref lastHash);
			HashUtils.ContentHashOnto(paramLogic_2, ref lastHash);
			HashUtils.ContentHashOnto(paramSubID_2, ref lastHash);
			HashUtils.ContentHashOnto(paramSubIndex_2, ref lastHash);
			HashUtils.ContentHashOnto(paramBase_3, ref lastHash);
			HashUtils.ContentHashOnto(paramLogic_3, ref lastHash);
			HashUtils.ContentHashOnto(paramSubID_3, ref lastHash);
			HashUtils.ContentHashOnto(paramSubIndex_3, ref lastHash);
			HashUtils.ContentHashOnto(iconPathInLevel, ref lastHash);
		}
	}
}
