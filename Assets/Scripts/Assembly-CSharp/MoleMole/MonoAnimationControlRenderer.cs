using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoAnimationControlRenderer : MonoBehaviour
	{
		[Header("Target Renderers")]
		public Renderer[] targetRenderers = new Renderer[0];

		[Header("Keyed Vector Property, leave key string empty to not take effect")]
		public string vectorPropertyKey;

		public Vector4 keyedVector;

		private int _vectorPropertyID;

		[Header("Keyed Vector Property #2, leave key string empty to not take effect")]
		public string vectorPropertyKey2;

		public Vector4 keyedVector2;

		private int _vectorPropertyID2;

		[Header("Keyed float Property, leave key string empty to not take effect")]
		public string floatPropertyKey;

		public float keyedFloat;

		private int _floatPropertyID;

		[Header("Keyed float Property, leave key string empty to not take effect")]
		public string floatPropertyKey2;

		public float keyedFloat2;

		private int _floatPropertyID2;

		[Header("Keyed float Property, leave key string empty to not take effect")]
		public string floatPropertyKey3;

		public float keyedFloat3;

		private int _floatPropertyID3;

		[Header("Keyed color Property, leave key string empty to not take effect")]
		public string colorPropertyKey;

		public Color keyedColor;

		private int _colorPropertyID;

		private MaterialPropertyBlock _block;

		private void Awake()
		{
			SetPropertyIDs();
			_block = new MaterialPropertyBlock();
		}

		private void SetPropertyIDs()
		{
			if (!string.IsNullOrEmpty(vectorPropertyKey))
			{
				_vectorPropertyID = Shader.PropertyToID(vectorPropertyKey);
			}
			if (!string.IsNullOrEmpty(vectorPropertyKey2))
			{
				_vectorPropertyID2 = Shader.PropertyToID(vectorPropertyKey2);
			}
			if (!string.IsNullOrEmpty(floatPropertyKey))
			{
				_floatPropertyID = Shader.PropertyToID(floatPropertyKey);
			}
			if (!string.IsNullOrEmpty(floatPropertyKey2))
			{
				_floatPropertyID2 = Shader.PropertyToID(floatPropertyKey2);
			}
			if (!string.IsNullOrEmpty(floatPropertyKey3))
			{
				_floatPropertyID3 = Shader.PropertyToID(floatPropertyKey3);
			}
			if (!string.IsNullOrEmpty(colorPropertyKey))
			{
				_colorPropertyID = Shader.PropertyToID(colorPropertyKey);
			}
		}

		private void SyncTargetRenderers()
		{
			for (int i = 0; i < targetRenderers.Length; i++)
			{
				Renderer renderer = targetRenderers[i];
				renderer.GetPropertyBlock(_block);
				if (_vectorPropertyID != 0)
				{
					_block.SetVector(_vectorPropertyID, keyedVector);
				}
				if (_vectorPropertyID2 != 0)
				{
					_block.SetVector(_vectorPropertyID2, keyedVector2);
				}
				if (_floatPropertyID != 0)
				{
					_block.SetFloat(_floatPropertyID, keyedFloat);
				}
				if (_floatPropertyID2 != 0)
				{
					_block.SetFloat(_floatPropertyID2, keyedFloat2);
				}
				if (_floatPropertyID3 != 0)
				{
					_block.SetFloat(_floatPropertyID3, keyedFloat3);
				}
				if (_colorPropertyID != 0)
				{
					_block.SetColor(_colorPropertyID, keyedColor);
				}
				renderer.SetPropertyBlock(_block);
			}
		}

		private void Update()
		{
			SyncTargetRenderers();
		}
	}
}
