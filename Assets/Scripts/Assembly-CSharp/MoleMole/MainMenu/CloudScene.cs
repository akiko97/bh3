using System;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class CloudScene
	{
		public string Name;

		public CloudType[] CloudTypes;

		public CompoundCloudType[] CompoundCloudTypes;

		public LayerCloudType[] LayerCloudTypes;

		public LightningType[] LightningTypes;

		public void Init(CloudEmitter cloudEmitter)
		{
			LightningType[] lightningTypes = LightningTypes;
			foreach (LightningType lightningType in lightningTypes)
			{
				lightningType.Init(cloudEmitter);
			}
		}
	}
}
