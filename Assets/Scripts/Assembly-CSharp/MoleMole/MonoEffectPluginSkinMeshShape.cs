using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginSkinMeshShape : BaseMonoEffectPlugin
	{
		[Header("Skin mesh attach point")]
		public string skinMeshAttachPoint;

		[Header("Target particle system")]
		public ParticleSystem targetParticleSystem;

		private ParticleSystem.ShapeModule _shapeModule;

		public override void Setup()
		{
			base.Setup();
			_shapeModule = targetParticleSystem.shape;
		}

		public void SetupSkinmesh(BaseMonoEntity entity)
		{
			Transform attachPoint = entity.GetAttachPoint(skinMeshAttachPoint);
			SkinnedMeshRenderer component = attachPoint.GetComponent<SkinnedMeshRenderer>();
			if (!(component == null))
			{
				_shapeModule.skinnedMeshRenderer = component;
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}
	}
}
