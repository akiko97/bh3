using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	public class UpdateStereoMaterial : MonoBehaviour
	{
		public Camera _camera;

		public MeshRenderer _renderer;

		private int _cameraPositionId;

		private void Awake()
		{
			_cameraPositionId = Shader.PropertyToID("_cameraPosition");
			if (_camera == null)
			{
				Debug.LogWarning("[AVProVideo] No camera set for UpdateStereoMaterial component. If you are rendering in stereo then it is recommended to set this.");
			}
		}

		private void LateUpdate()
		{
			Camera camera = _camera;
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (_renderer == null)
			{
				_renderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (camera != null && _renderer != null)
			{
				_renderer.material.SetVector(_cameraPositionId, camera.transform.position);
			}
		}
	}
}
