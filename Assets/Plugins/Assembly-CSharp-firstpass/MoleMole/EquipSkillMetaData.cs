namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EquipSkillMetaData : IHashable
	{
		public readonly int ID;

		public readonly string skillName;

		public readonly string skillDisplay;

		public readonly string skillIconPath;

		public EquipSkillMetaData(int ID, string skillName, string skillDisplay, string skillIconPath)
		{
			this.ID = ID;
			this.skillName = skillName;
			this.skillDisplay = skillDisplay;
			this.skillIconPath = skillIconPath;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(skillName, ref lastHash);
			HashUtils.ContentHashOnto(skillDisplay, ref lastHash);
			HashUtils.ContentHashOnto(skillIconPath, ref lastHash);
		}
	}
}
