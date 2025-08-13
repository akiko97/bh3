namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MaterialExpBonusMetaData : IHashable
	{
		public readonly int materialId;

		public readonly float weaponExpBonus;

		public readonly float stigmataExpBonus;

		public MaterialExpBonusMetaData(int materialId, float weaponExpBonus, float stigmataExpBonus)
		{
			this.materialId = materialId;
			this.weaponExpBonus = weaponExpBonus;
			this.stigmataExpBonus = stigmataExpBonus;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(materialId, ref lastHash);
			HashUtils.ContentHashOnto(weaponExpBonus, ref lastHash);
			HashUtils.ContentHashOnto(stigmataExpBonus, ref lastHash);
		}
	}
}
