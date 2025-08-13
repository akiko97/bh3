using UnityEngine;

namespace MoleMole
{
	public sealed class Mono_UL_020 : BaseMonoUlysses
	{
		protected override void Update()
		{
			base.Update();
			Transform transform = hitbox.transform;
			transform.eulerAngles = Vector3.zero;
		}
	}
}
