using UnityEngine;

namespace MoleMole
{
	public class MonoHimeko : BaseMonoAvatar, IStaticHitBox
	{
		private static string HIMEKO_WEAPON_HIT_BOX_PATH = "Entities/Avatar/StaticCollider/HimekoWeaponHitBox";

		private MonoStaticHitboxDetect WeaponHitBox;

		public Transform weaponTransform;

		protected override void PostInit()
		{
			base.PostInit();
			GameObject gameObject = (GameObject)Object.Instantiate(Miscs.LoadResource<GameObject>(HIMEKO_WEAPON_HIT_BOX_PATH), weaponTransform.position, Quaternion.identity);
			WeaponHitBox = gameObject.GetComponent<MonoStaticHitboxDetect>();
			WeaponHitBox.Init(this, AttackPattern.GetLayerMask(this), weaponTransform);
		}

		public MonoStaticHitboxDetect GetStaticHitBox()
		{
			return WeaponHitBox;
		}
	}
}
