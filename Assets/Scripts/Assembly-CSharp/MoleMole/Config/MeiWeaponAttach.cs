namespace MoleMole.Config
{
	public class MeiWeaponAttach : ConfigWeaponAttach
	{
		public const string SHORT_PART_PATH = "ShortSword";

		public const string LONG_PART_PATH = "LongSword";

		public const string SHORT_LEFT_ATTACH_POINT = "WeaponLeftHand";

		public const string SHORT_RIGHT_ATTACH_POINT = "WeaponRightHand";

		public const string LONG_RIGHT_ATTACH_POINT = "WeaponRightHand";

		public string WeaponEffectPattern;

		public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
		{
			return WeaponAttach.MeiAttachHandler;
		}

		public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
		{
			return WeaponAttach.MeiRuntimeAttachHandler;
		}

		public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
		{
			return WeaponAttach.MeiDetachHandler;
		}
	}
}
