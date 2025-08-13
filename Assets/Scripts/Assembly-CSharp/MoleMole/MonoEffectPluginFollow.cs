using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginFollow : BaseMonoEffectPlugin
	{
		[Header("Empty path means the most outer transform.")]
		public string FollowTargetPath;

		[Header("Is NOT using attach point, treat Follow Target Path as transform path.")]
		public bool IsNotAttachPoint;

		[Header("Follow rotation")]
		public bool FollowRotation;

		[Header("Follow Target's ***root***'s Y axis rotation")]
		public bool FollowYRotation;

		[Header("Don't destory when follow target becomes null")]
		public bool NoFollowDestory;

		[Header("Only do follow on first frame")]
		public bool OnlyFirstFrame;

		[Header("Follow by init pos")]
		public bool FollowByInitPos;

		[Header("Activate Game Objectg On Start")]
		public GameObject ActivateOnStart;

		private Vector3 _additionalOffset = Vector3.zero;

		private Transform _followTarget;

		private float _initRotationYOffset;

		private bool _firstFrameUpdated;

		protected override void Awake()
		{
			base.Awake();
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(false);
			}
		}

		public override void Setup()
		{
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(true);
			}
		}

		public void SetFollowParentTarget(Transform parent)
		{
			if (string.IsNullOrEmpty(FollowTargetPath) || IsNotAttachPoint)
			{
				_followTarget = parent.Find(FollowTargetPath);
			}
			else
			{
				_followTarget = parent.GetComponent<BaseMonoEntity>().GetAttachPoint(FollowTargetPath);
			}
			if (FollowByInitPos)
			{
				_additionalOffset = _followTarget.InverseTransformDirection(base.transform.position - _followTarget.position);
			}
			_initRotationYOffset = _followTarget.root.eulerAngles.y - base.transform.rotation.eulerAngles.y;
			FollowPosition();
			_firstFrameUpdated = false;
		}

		private void FollowPosition()
		{
			if (FollowRotation)
			{
				base.transform.rotation = _followTarget.rotation;
			}
			if (FollowYRotation)
			{
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, _followTarget.root.eulerAngles.y - _initRotationYOffset, base.transform.rotation.eulerAngles.z);
			}
			base.transform.position = _followTarget.position + Vector3.Scale(base.transform.TransformDirection(_effect.OffsetVec3), _effect.transform.localScale) + _followTarget.TransformDirection(_additionalOffset);
		}

		public void LateUpdate()
		{
			if (!OnlyFirstFrame || !_firstFrameUpdated)
			{
				_firstFrameUpdated = true;
				if (_followTarget != null && !IsToBeRemove())
				{
					FollowPosition();
				}
			}
		}

		public override bool IsToBeRemove()
		{
			if (NoFollowDestory)
			{
				return false;
			}
			return _followTarget == null;
		}

		public override void SetDestroy()
		{
		}

		private void OnDisable()
		{
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(false);
			}
		}
	}
}
