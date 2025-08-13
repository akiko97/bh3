namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BattleTypeMetaData : IHashable
	{
		public readonly int battleTypeId;

		public readonly string iconPath;

		public readonly string colorCode;

		public BattleTypeMetaData(int battleTypeId, string iconPath, string colorCode)
		{
			this.battleTypeId = battleTypeId;
			this.iconPath = iconPath;
			this.colorCode = colorCode;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(battleTypeId, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(colorCode, ref lastHash);
		}
	}
}
