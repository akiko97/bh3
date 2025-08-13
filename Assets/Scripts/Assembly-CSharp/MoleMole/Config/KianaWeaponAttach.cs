namespace MoleMole.Config
{
	public class KianaWeaponAttach : ConfigWeaponAttach
	{
		public const string LEFT_PART_PATH = "LeftPistol";

		public const string RIGHT_PART_PATH = "RightPistol";

		public const string LEFT_ATTACH_POINT = "WeaponLeftHand";

		public const string RIGHT_ATTACH_POINT = "WeaponRightHand";

		public const string AVATAR_LEFT_GUN_ATTACH_POINT_NAME = "LeftGunPoint";

		public const string LEFT_GUN_ATTACH_POINT_TRANSFORM = "LeftPistol/LeftGunPoint";

		public const string AVATAR_RIGHT_GUN_ATTACH_POINT_NAME = "RightGunPoint";

		public const string RIGHT_GUN_ATTACH_POINT_TRANSFORM = "RightPistol/RightGunPoint";

		public string WeaponEffectPattern;

		public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
		{
			return WeaponAttach.KianaAttachHandler;
		}

		public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
		{
			return WeaponAttach.KianaRuntimeAttachHandler;
		}

		public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
		{
			return WeaponAttach.KianaDetachHandler;
		}
	}
}
