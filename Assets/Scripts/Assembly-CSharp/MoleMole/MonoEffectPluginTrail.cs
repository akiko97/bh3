using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginTrail : BaseMonoEffectPlugin
	{
		public Transform TrailRendererTransform;

		public Transform AniAnchorTransform;

		public Vector3[] FramePosList;

		protected int _curFrame;

		protected override void Awake()
		{
			base.Awake();
			_curFrame = 0;
			AniAnchorTransform.gameObject.SetActive(false);
		}

		public override void Setup()
		{
			AniAnchorTransform.gameObject.SetActive(true);
			_curFrame = 0;
		}

		protected virtual void Update()
		{
			if (_curFrame < FramePosList.Length)
			{
				AniAnchorTransform.localPosition = FramePosList[_curFrame];
				TrailRendererTransform.localPosition = AniAnchorTransform.localPosition;
				_curFrame++;
			}
		}

		public override bool IsToBeRemove()
		{
			return null == TrailRendererTransform || _curFrame >= FramePosList.Length;
		}

		public override void SetDestroy()
		{
			if (TrailRendererTransform != null)
			{
				Object.Destroy(TrailRendererTransform.gameObject);
			}
		}
	}
}
