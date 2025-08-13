namespace MoleMole.Config
{
	public class HimekoWeapoonAttach : ConfigWeaponAttach
	{
		public const string SWORD_PATH = "Sword";

		public const string SWORD_ATTACH_POINT = "WeaponRightHand";

		public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
		{
			return WeaponAttach.HimekoAttachHandler;
		}

		public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
		{
			return WeaponAttach.HimekoRuntimeAttachHandler;
		}

		public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
		{
			return WeaponAttach.HimekoDetachHandler;
		}
	}
}
