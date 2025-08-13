using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class SetCameraTarget : MonoBehaviour
	{
		public enum DepthBitEnum
		{
			bit_0 = 0,
			bit_8 = 8,
			bit_16 = 0x10
		}

		public enum DownSampleEnum
		{
			down_1x = 1,
			down_2x = 2,
			down_4x = 4
		}

		public RenderTextureFormat TargetFormat;

		public DepthBitEnum DepthBit = DepthBitEnum.bit_16;

		public DownSampleEnum DownSample = DownSampleEnum.down_1x;

		public string TargetNameInShader;

		private RenderTexture _target;

		private void Start()
		{
			Camera component = GetComponent<Camera>();
			_target = new RenderTexture(Screen.width / (int)DownSample, Screen.height / (int)DownSample, 16, TargetFormat);
			component.SetTargetBuffers(_target.colorBuffer, _target.depthBuffer);
			component.targetTexture = _target;
			Shader.SetGlobalTexture(TargetNameInShader, _target);
		}
	}
}
