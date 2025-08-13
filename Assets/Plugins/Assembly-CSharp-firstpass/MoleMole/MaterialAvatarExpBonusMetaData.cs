namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MaterialAvatarExpBonusMetaData : IHashable
	{
		public readonly int materialId;

		public readonly float biologyExpBonus;

		public readonly float psychoExpBonus;

		public readonly float mechanicExpBonus;

		public MaterialAvatarExpBonusMetaData(int materialId, float biologyExpBonus, float psychoExpBonus, float mechanicExpBonus)
		{
			this.materialId = materialId;
			this.biologyExpBonus = biologyExpBonus;
			this.psychoExpBonus = psychoExpBonus;
			this.mechanicExpBonus = mechanicExpBonus;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(materialId, ref lastHash);
			HashUtils.ContentHashOnto(biologyExpBonus, ref lastHash);
			HashUtils.ContentHashOnto(psychoExpBonus, ref lastHash);
			HashUtils.ContentHashOnto(mechanicExpBonus, ref lastHash);
		}
	}
}
