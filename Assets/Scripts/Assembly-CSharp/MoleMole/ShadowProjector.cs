using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Projector))]
	public class ShadowProjector : MonoBehaviour
	{
		public float PitchAngle = 70f;

		[Header("Size of projector and camera")]
		public float Size = 1.2f;

		[Header("Distance from projector and camera to the avatar")]
		public float ProjectorDistance;

		public float CameraDistance = 2f;

		[Header("Shader used to draw shadow")]
		public Shader Shader;

		[Header("Setting for RenderTexture to which draw shadow")]
		public int size = 64;

		public RenderTextureFormat format = RenderTextureFormat.RGB565;

		[Header("Setting for projector")]
		public Shader ProjectorShader;

		public Texture2D FallOffTex;

		private Projector _projector;

		private Camera _innerCamera;

		private Transform _lightForwardTransform;

		private Transform _rootNodeTransform;

		private RenderTextureWrapper _renderTexture;

		private void Start()
		{
			_innerCamera = GetComponentInChildren<Camera>();
			_innerCamera.enabled = true;
			_innerCamera.SetReplacementShader(Shader, string.Empty);
			_renderTexture = GraphicsUtils.GetRenderTexture(size, size, 0, format);
			_innerCamera.targetTexture = _renderTexture;
			_projector = GetComponent<Projector>();
			Material material = new Material(ProjectorShader);
			material.SetTexture("_Cookie", (RenderTexture)_renderTexture);
			material.SetTexture("_FalloffTex", FallOffTex);
			_projector.material = material;
			_rootNodeTransform = GetComponentInParent<BaseMonoAnimatorEntity>().RootNode;
		}

		private void OnDestroy()
		{
			if (_renderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_renderTexture);
				_renderTexture = null;
			}
		}

		private void Update()
		{
			if (_lightForwardTransform == null)
			{
				_lightForwardTransform = Singleton<StageManager>.Instance.GetStageEnv().lightForwardTransform;
			}
			_innerCamera.orthographicSize = Size;
			_projector.orthographicSize = Size;
			base.transform.position = _lightForwardTransform.position;
			base.transform.rotation = _lightForwardTransform.rotation;
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles[0] = PitchAngle;
			base.transform.eulerAngles = eulerAngles;
			Vector3 position = _rootNodeTransform.position;
			base.transform.position = position - base.transform.forward * ProjectorDistance;
			_innerCamera.transform.position = position - _innerCamera.transform.forward * CameraDistance;
		}
	}
}
