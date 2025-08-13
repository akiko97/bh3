namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class StigmataAffixMetaData : IHashable
	{
		public readonly int affixID;

		public readonly string comment;

		public readonly string nameMono;

		public readonly string nameDual;

		public readonly int level;

		public readonly int propID;

		public readonly float PropParam1;

		public readonly float PropParam2;

		public readonly float PropParam3;

		public readonly int UIType;

		public readonly float UIValue;

		public readonly int UINature;

		public readonly int UIClass;

		public StigmataAffixMetaData(int affixID, string comment, string nameMono, string nameDual, int level, int propID, float PropParam1, float PropParam2, float PropParam3, int UIType, float UIValue, int UINature, int UIClass)
		{
			this.affixID = affixID;
			this.comment = comment;
			this.nameMono = nameMono;
			this.nameDual = nameDual;
			this.level = level;
			this.propID = propID;
			this.PropParam1 = PropParam1;
			this.PropParam2 = PropParam2;
			this.PropParam3 = PropParam3;
			this.UIType = UIType;
			this.UIValue = UIValue;
			this.UINature = UINature;
			this.UIClass = UIClass;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(affixID, ref lastHash);
			HashUtils.ContentHashOnto(comment, ref lastHash);
			HashUtils.ContentHashOnto(nameMono, ref lastHash);
			HashUtils.ContentHashOnto(nameDual, ref lastHash);
			HashUtils.ContentHashOnto(level, ref lastHash);
			HashUtils.ContentHashOnto(propID, ref lastHash);
			HashUtils.ContentHashOnto(PropParam1, ref lastHash);
			HashUtils.ContentHashOnto(PropParam2, ref lastHash);
			HashUtils.ContentHashOnto(PropParam3, ref lastHash);
			HashUtils.ContentHashOnto(UIType, ref lastHash);
			HashUtils.ContentHashOnto(UIValue, ref lastHash);
			HashUtils.ContentHashOnto(UINature, ref lastHash);
			HashUtils.ContentHashOnto(UIClass, ref lastHash);
		}
	}
}
