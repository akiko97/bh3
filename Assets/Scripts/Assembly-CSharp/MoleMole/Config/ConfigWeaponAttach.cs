using System;

namespace MoleMole.Config
{
	public abstract class ConfigWeaponAttach
	{
		[NonSerialized]
		public string PrefabPath;

		public abstract WeaponAttach.WeaponAttachHandler GetAttachHandler();

		public abstract WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler();

		public abstract WeaponAttach.WeaponDetachHandler GetDetachHandler();
	}
}
