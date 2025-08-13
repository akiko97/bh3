using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Paster))]
	public class ShadowPaster : MonoBehaviour
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

		private Paster _paster;

		private Camera _innerCamera;

		private Transform _lightForwardTransform;

		private Transform _rootNodeTransform;

		private RenderTextureWrapper _renderTexture;

		private void Start()
		{
			_innerCamera = GetComponentInChildren<Camera>();
			_innerCamera.enabled = true;
			_innerCamera.SetReplacementShader(Shader, string.Empty);
			_paster = GetComponent<Paster>();
			Material material = new Material(ProjectorShader);
			material.SetTexture("_FalloffTex", FallOffTex);
			_paster.Material = material;
			_renderTexture = GraphicsUtils.GetRenderTexture(size, size, 0, format);
			_renderTexture.BindToCamera(_innerCamera);
			_paster.Material.SetTexture("_Cookie", (RenderTexture)_renderTexture);
			_rootNodeTransform = GetComponentInParent<BaseMonoAnimatorEntity>().RootNode;
			SetTransform();
		}

		private void OnDestroy()
		{
			GraphicsUtils.ReleaseRenderTexture(_renderTexture);
		}

		private void Update()
		{
			SetTransform();
		}

		private void SetTransform()
		{
			if (_lightForwardTransform == null)
			{
				_lightForwardTransform = Singleton<StageManager>.Instance.GetStageEnv().lightForwardTransform;
			}
			_innerCamera.orthographicSize = Size;
			_paster.Size = Size;
			_paster.AspectRatio = _innerCamera.aspect;
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
