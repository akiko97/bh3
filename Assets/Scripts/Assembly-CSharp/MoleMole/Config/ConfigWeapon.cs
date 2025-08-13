using System;

namespace MoleMole.Config
{
	public class ConfigWeapon
	{
		public int WeaponID;

		[NonSerialized]
		public WeaponMetaData Meta;

		public EntityRoleName OwnerRole;

		public ConfigWeaponAttach Attach;

		public ConfigEntityWeaponAdditionalAbilityEntry[] AdditionalAbilities = ConfigEntityWeaponAdditionalAbilityEntry.EMPTY;

		public ConfigEffectOverlayEntry[] EffectOverlays = ConfigEffectOverlayEntry.EMPTY;

		public ConfigEffectOverlayEntry[] EffectOverrides = ConfigEffectOverlayEntry.EMPTY;
	}
}
