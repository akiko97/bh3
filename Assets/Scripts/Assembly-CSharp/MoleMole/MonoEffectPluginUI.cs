using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginUI : BaseMonoEffectPlugin
	{
		[Header("UI transform path, if empty use InitPos")]
		public string FollowTargetUIPath;

		public bool UseParentUIPos;

		public float depthInMainCamera;

		private Vector3 _initPosition;

		protected override void Awake()
		{
			base.Awake();
		}

		public override void Setup()
		{
			_initPosition = base.transform.position;
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public void LateUpdate()
		{
			Vector3 position = Vector3.one;
			if (!string.IsNullOrEmpty(FollowTargetUIPath))
			{
				BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
				if (sceneCanvas == null)
				{
					return;
				}
				Transform transform = sceneCanvas.transform.Find(FollowTargetUIPath);
				position = transform.position;
			}
			else if (UseParentUIPos)
			{
				Transform parent = base.transform.parent;
				if (parent != null && parent.gameObject.layer == LayerMask.NameToLayer("UI"))
				{
					position = parent.position;
				}
			}
			else
			{
				position = _initPosition;
			}
			Camera cameraComponent = Singleton<CameraManager>.Instance.GetMainCamera().cameraComponent;
			Camera component = Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.GetComponent<Camera>();
			Vector3 position2 = component.WorldToScreenPoint(position);
			position2.z = depthInMainCamera;
			_effect.gameObject.transform.position = cameraComponent.ScreenToWorldPoint(position2);
		}

		public override void SetDestroy()
		{
		}
	}
}
