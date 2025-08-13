using UnityEngine;

namespace MoleMole
{
	public class MonoDAKiana : BaseMonoDarkAvatar
	{
		public Renderer LeftWeapon;

		public Renderer RightWeapon;

		private void SetWeaponVisible(int show)
		{
			bool flag = show != 0;
			if (LeftWeapon != null)
			{
				LeftWeapon.enabled = flag;
			}
			if (RightWeapon != null)
			{
				RightWeapon.enabled = flag;
			}
		}
	}
}
