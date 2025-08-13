using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginFollowLocalAvatar : BaseMonoEffectPlugin
	{
		[Header("Follow rotation")]
		public bool FollowRotation;

		private void FollowPosition()
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (!(baseMonoAvatar == null))
			{
				Transform transform = baseMonoAvatar.transform;
				base.transform.position = transform.position + Vector3.Scale(base.transform.TransformDirection(_effect.OffsetVec3), _effect.transform.localScale);
				if (FollowRotation)
				{
					base.transform.rotation = transform.rotation;
				}
			}
		}

		public void LateUpdate()
		{
			if (!IsToBeRemove())
			{
				FollowPosition();
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
