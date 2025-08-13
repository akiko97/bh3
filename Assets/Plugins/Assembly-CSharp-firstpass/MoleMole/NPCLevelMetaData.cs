namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class NPCLevelMetaData : IHashable
	{
		public readonly int HardLevel;

		public readonly float HPRatio;

		public readonly float ATKRatio;

		public readonly float DEFRatio;

		public readonly float ElementalResistRatio;

		public NPCLevelMetaData(int HardLevel, float HPRatio, float ATKRatio, float DEFRatio, float ElementalResistRatio)
		{
			this.HardLevel = HardLevel;
			this.HPRatio = HPRatio;
			this.ATKRatio = ATKRatio;
			this.DEFRatio = DEFRatio;
			this.ElementalResistRatio = ElementalResistRatio;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(HardLevel, ref lastHash);
			HashUtils.ContentHashOnto(HPRatio, ref lastHash);
			HashUtils.ContentHashOnto(ATKRatio, ref lastHash);
			HashUtils.ContentHashOnto(DEFRatio, ref lastHash);
			HashUtils.ContentHashOnto(ElementalResistRatio, ref lastHash);
		}
	}
}
