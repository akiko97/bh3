using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class DiscreteUVScrollAnimation : MonoBehaviour
	{
		public float playbackSpeed = 1f;

		public int tiles_X = 1;

		public int tiles_Y = 1;

		[Header("Map the normalized uv square to a block with X*Y tiles in the texture")]
		public int screen_X = 1;

		public int screen_Y = 1;

		[Header("Original offsets (tile as unit)")]
		public int origOffset_X;

		public int origOffset_Y;

		[Header("Scroll speed (tile as unit)")]
		public int speed_X;

		public int speed_Y;

		[Header("Drag the material here instead of in renderer")]
		public Material material;

		private float _step_X;

		private float _step_Y;

		private float _offset_X;

		private float _offset_Y;

		private Material _material;

		private void Awake()
		{
			Preparation();
		}

		private void OnEnable()
		{
			Preparation();
		}

		private void OnDisable()
		{
		}

		private void Preparation()
		{
			if (material == null)
			{
				base.enabled = false;
				return;
			}
			_material = new Material(material);
			_material.hideFlags = HideFlags.DontSave;
			GetComponent<Renderer>().material = _material;
			_step_X = 1f / (float)tiles_X;
			_step_Y = 1f / (float)tiles_Y;
			_material.SetTextureScale("_MainTex", new Vector2(_step_X * (float)screen_X, _step_Y * (float)screen_Y));
		}

		public void Update()
		{
			float deltaTime = Time.deltaTime;
			_offset_X += (float)speed_X * deltaTime;
			_offset_Y += (float)speed_Y * deltaTime;
			Vector2 offset = new Vector2
			{
				x = (float)(int)_offset_X * _step_X,
				y = (float)(int)_offset_Y * _step_Y
			};
			_material.SetTextureScale("_MainTex", new Vector2(_step_X * (float)screen_X, _step_Y * (float)screen_Y));
			_material.SetTextureOffset("_MainTex", offset);
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				Object.DestroyImmediate(_material);
			}
		}
	}
}
