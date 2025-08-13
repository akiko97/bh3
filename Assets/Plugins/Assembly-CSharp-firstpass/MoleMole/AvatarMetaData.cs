using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarMetaData : IHashable
	{
		public readonly int avatarID;

		public readonly int classID;

		public readonly string fullName;

		public readonly string shortName;

		public readonly string desc;

		public readonly string avatarRegistryKey;

		public readonly List<int> weaponBaseTypeList;

		public readonly int unlockStar;

		public readonly List<int> skillList;

		public readonly int attribute;

		public readonly int initialWeapon;

		public readonly int showSkill;

		public readonly int avatarCardID;

		public readonly int avatarFragmentID;

		public readonly int ultraSkillID;

		public readonly int captainSkillID;

		public readonly float SPRatio;

		public readonly float SKL01SP;

		public readonly float SKL01SPNeed;

		public readonly int SKL01Charges;

		public readonly float SKL01CD;

		public readonly float SKL02SP;

		public readonly float SKL02SPNeed;

		public readonly int SKL02Charges;

		public readonly float SKL02CD;

		public readonly float SKL03SP;

		public readonly float SKL03SPNeed;

		public readonly int SKL03Charges;

		public readonly float SKL03CD;

		public readonly int levelTutorialID;

		public AvatarMetaData(int avatarID, int classID, string fullName, string shortName, string desc, string avatarRegistryKey, List<int> weaponBaseTypeList, int unlockStar, List<int> skillList, int attribute, int initialWeapon, int showSkill, int avatarCardID, int avatarFragmentID, int ultraSkillID, int captainSkillID, float SPRatio, float SKL01SP, float SKL01SPNeed, int SKL01Charges, float SKL01CD, float SKL02SP, float SKL02SPNeed, int SKL02Charges, float SKL02CD, float SKL03SP, float SKL03SPNeed, int SKL03Charges, float SKL03CD, int levelTutorialID)
		{
			this.avatarID = avatarID;
			this.classID = classID;
			this.fullName = fullName;
			this.shortName = shortName;
			this.desc = desc;
			this.avatarRegistryKey = avatarRegistryKey;
			this.weaponBaseTypeList = weaponBaseTypeList;
			this.unlockStar = unlockStar;
			this.skillList = skillList;
			this.attribute = attribute;
			this.initialWeapon = initialWeapon;
			this.showSkill = showSkill;
			this.avatarCardID = avatarCardID;
			this.avatarFragmentID = avatarFragmentID;
			this.ultraSkillID = ultraSkillID;
			this.captainSkillID = captainSkillID;
			this.SPRatio = SPRatio;
			this.SKL01SP = SKL01SP;
			this.SKL01SPNeed = SKL01SPNeed;
			this.SKL01Charges = SKL01Charges;
			this.SKL01CD = SKL01CD;
			this.SKL02SP = SKL02SP;
			this.SKL02SPNeed = SKL02SPNeed;
			this.SKL02Charges = SKL02Charges;
			this.SKL02CD = SKL02CD;
			this.SKL03SP = SKL03SP;
			this.SKL03SPNeed = SKL03SPNeed;
			this.SKL03Charges = SKL03Charges;
			this.SKL03CD = SKL03CD;
			this.levelTutorialID = levelTutorialID;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(avatarID, ref lastHash);
			HashUtils.ContentHashOnto(classID, ref lastHash);
			HashUtils.ContentHashOnto(fullName, ref lastHash);
			HashUtils.ContentHashOnto(shortName, ref lastHash);
			HashUtils.ContentHashOnto(desc, ref lastHash);
			HashUtils.ContentHashOnto(avatarRegistryKey, ref lastHash);
			if (weaponBaseTypeList != null)
			{
				foreach (int weaponBaseType in weaponBaseTypeList)
				{
					HashUtils.ContentHashOnto(weaponBaseType, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(unlockStar, ref lastHash);
			if (skillList != null)
			{
				foreach (int skill in skillList)
				{
					HashUtils.ContentHashOnto(skill, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(attribute, ref lastHash);
			HashUtils.ContentHashOnto(initialWeapon, ref lastHash);
			HashUtils.ContentHashOnto(showSkill, ref lastHash);
			HashUtils.ContentHashOnto(avatarCardID, ref lastHash);
			HashUtils.ContentHashOnto(avatarFragmentID, ref lastHash);
			HashUtils.ContentHashOnto(ultraSkillID, ref lastHash);
			HashUtils.ContentHashOnto(captainSkillID, ref lastHash);
			HashUtils.ContentHashOnto(SPRatio, ref lastHash);
			HashUtils.ContentHashOnto(SKL01SP, ref lastHash);
			HashUtils.ContentHashOnto(SKL01SPNeed, ref lastHash);
			HashUtils.ContentHashOnto(SKL01Charges, ref lastHash);
			HashUtils.ContentHashOnto(SKL01CD, ref lastHash);
			HashUtils.ContentHashOnto(SKL02SP, ref lastHash);
			HashUtils.ContentHashOnto(SKL02SPNeed, ref lastHash);
			HashUtils.ContentHashOnto(SKL02Charges, ref lastHash);
			HashUtils.ContentHashOnto(SKL02CD, ref lastHash);
			HashUtils.ContentHashOnto(SKL03SP, ref lastHash);
			HashUtils.ContentHashOnto(SKL03SPNeed, ref lastHash);
			HashUtils.ContentHashOnto(SKL03Charges, ref lastHash);
			HashUtils.ContentHashOnto(SKL03CD, ref lastHash);
			HashUtils.ContentHashOnto(levelTutorialID, ref lastHash);
		}
	}
}
