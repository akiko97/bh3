using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginBullet : BaseMonoEffectPlugin
	{
		[Header("Max Distance")]
		public float maxDistance;

		[Header("Effect Velocity")]
		public float velocity;

		[Header("Collision Layer Mask")]
		public LayerMask mask = (1 << InLevelData.MONSTER_HITBOX_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER);

		[Header("Empty path means start at the most outer transform")]
		public string startTargetPath;

		[Header("Follow rotation")]
		public bool followRotation;

		private float _travelDistance;

		private RaycastHit _castHit;

		protected override void Awake()
		{
			base.Awake();
		}

		public void SetupStartParentTarget(Transform parent)
		{
			Transform transform = parent.Find(startTargetPath);
			base.transform.position = transform.position + base.transform.TransformDirection(_effect.OffsetVec3);
			if (followRotation)
			{
				base.transform.rotation = transform.rotation;
			}
		}

		public override void Setup()
		{
			_travelDistance = maxDistance;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out _castHit, maxDistance, mask))
			{
				_travelDistance = _castHit.distance;
			}
		}

		private void Update()
		{
			float num = velocity * _effect.TimeScale;
			base.transform.Translate(new Vector3(0f, 0f, num), Space.Self);
			_travelDistance -= num;
		}

		public override bool IsToBeRemove()
		{
			return _travelDistance <= 0f;
		}

		public override void SetDestroy()
		{
			_travelDistance = 0f;
		}
	}
}
