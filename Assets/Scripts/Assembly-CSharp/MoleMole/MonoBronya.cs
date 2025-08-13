using UnityEngine;

namespace MoleMole
{
	public class MonoBronya : BaseMonoAvatar
	{
		public override void Awake()
		{
			base.Awake();
		}

		public void SetMCVisible(bool visible)
		{
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				if (renderer.gameObject.name.StartsWith("MC"))
				{
					renderer.enabled = visible;
				}
			}
		}
	}
}
