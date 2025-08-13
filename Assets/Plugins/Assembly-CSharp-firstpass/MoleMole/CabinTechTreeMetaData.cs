namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinTechTreeMetaData : IHashable
	{
		public readonly int ID;

		public readonly int Cabin;

		public readonly int X;

		public readonly int Y;

		public readonly int UnlockLevel;

		public readonly int UnlockAvatarID;

		public readonly int UnlockAvatarLevel;

		public readonly int PowerCost;

		public readonly int AbilityType;

		public readonly int Argument1;

		public readonly int Argument2;

		public readonly int Argument3;

		public readonly int ResetSCoin;

		public readonly string Icon;

		public readonly string Title;

		public readonly string Desc;

		public CabinTechTreeMetaData(int ID, int Cabin, int X, int Y, int UnlockLevel, int UnlockAvatarID, int UnlockAvatarLevel, int PowerCost, int AbilityType, int Argument1, int Argument2, int Argument3, int ResetSCoin, string Icon, string Title, string Desc)
		{
			this.ID = ID;
			this.Cabin = Cabin;
			this.X = X;
			this.Y = Y;
			this.UnlockLevel = UnlockLevel;
			this.UnlockAvatarID = UnlockAvatarID;
			this.UnlockAvatarLevel = UnlockAvatarLevel;
			this.PowerCost = PowerCost;
			this.AbilityType = AbilityType;
			this.Argument1 = Argument1;
			this.Argument2 = Argument2;
			this.Argument3 = Argument3;
			this.ResetSCoin = ResetSCoin;
			this.Icon = Icon;
			this.Title = Title;
			this.Desc = Desc;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(Cabin, ref lastHash);
			HashUtils.ContentHashOnto(X, ref lastHash);
			HashUtils.ContentHashOnto(Y, ref lastHash);
			HashUtils.ContentHashOnto(UnlockLevel, ref lastHash);
			HashUtils.ContentHashOnto(UnlockAvatarID, ref lastHash);
			HashUtils.ContentHashOnto(UnlockAvatarLevel, ref lastHash);
			HashUtils.ContentHashOnto(PowerCost, ref lastHash);
			HashUtils.ContentHashOnto(AbilityType, ref lastHash);
			HashUtils.ContentHashOnto(Argument1, ref lastHash);
			HashUtils.ContentHashOnto(Argument2, ref lastHash);
			HashUtils.ContentHashOnto(Argument3, ref lastHash);
			HashUtils.ContentHashOnto(ResetSCoin, ref lastHash);
			HashUtils.ContentHashOnto(Icon, ref lastHash);
			HashUtils.ContentHashOnto(Title, ref lastHash);
			HashUtils.ContentHashOnto(Desc, ref lastHash);
		}
	}
}
