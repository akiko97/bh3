namespace MoleMole.Config
{
	public class FukaWeapoonAttach : ConfigWeaponAttach
	{
		public const string TAIL_PATH = "Tail";

		public const string TAIL_ATTACH_POINT = "WeaponTail";

		public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
		{
			return WeaponAttach.FukaAttachHandler;
		}

		public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
		{
			return WeaponAttach.FukaRuntimeAttachHandler;
		}

		public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
		{
			return WeaponAttach.FukaDetachHandler;
		}
	}
}
