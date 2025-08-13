namespace MoleMole.Config
{
	public class HitExplodeTracingBulletMixinArgument : IndexedConfig<HitExplodeTracingBulletMixinArgument>, IMixinArgument
	{
		public string BulletName;

		public string[] RandomBulletNames;

		public float XZAngleOffset;

		public DynamicFloat BulletSpeed;

		public override int CompareTo(HitExplodeTracingBulletMixinArgument other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = IndexedConfig.Compare(BulletName, other.BulletName);
			if (num != 0)
			{
				return num;
			}
			num = XZAngleOffset.CompareTo(other.XZAngleOffset);
			if (num != 0)
			{
				return num;
			}
			num = IndexedConfig.Compare(RandomBulletNames, other.RandomBulletNames);
			if (num != 0)
			{
				return num;
			}
			return IndexedConfig.Compare(BulletSpeed, other.BulletSpeed);
		}

		public override int ContentHash()
		{
			int lastHash = 0;
			HashUtils.ContentHashOnto(BulletName, ref lastHash);
			HashUtils.ContentHashOnto(XZAngleOffset, ref lastHash);
			if (RandomBulletNames != null)
			{
				for (int i = 0; i < RandomBulletNames.Length; i++)
				{
					HashUtils.ContentHashOnto(RandomBulletNames[i], ref lastHash);
				}
			}
			if (BulletSpeed != null)
			{
				HashUtils.ContentHashOnto(BulletSpeed.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BulletSpeed.dynamicKey, ref lastHash);
				HashUtils.ContentHashOnto(BulletSpeed.fixedValue, ref lastHash);
			}
			return 0;
		}
	}
}
