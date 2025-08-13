using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class ContinuousUVScrollAnimation : MonoBehaviour
	{
		[Header("Scroll Speed")]
		public float speed_X;

		public float speed_Y;

		[Header("Drag the material here instead of in renderer")]
		public Material[] materials;

		private bool _prepared;

		private Material[] _materials;

		private float _offsetX;

		private float _offsetY;

		private void Awake()
		{
			Preparation();
		}

		private void OnValidate()
		{
			Preparation();
		}

		private void Preparation()
		{
			if (materials.Length == 0)
			{
				_prepared = false;
				return;
			}
			_materials = new Material[materials.Length];
			for (int i = 0; i < _materials.Length; i++)
			{
				_materials[i] = new Material(materials[i]);
				_materials[i].hideFlags = HideFlags.DontSave;
			}
			GetComponent<Renderer>().materials = _materials;
			_prepared = true;
		}

		public void Update()
		{
			if (_prepared)
			{
				float deltaTime = Time.deltaTime;
				_offsetX += speed_X * deltaTime;
				_offsetY += speed_Y * deltaTime;
				for (int i = 0; i < _materials.Length; i++)
				{
					_materials[i].SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
				}
			}
		}

		private void OnDestroy()
		{
			if (_materials != null)
			{
				for (int i = 0; i < _materials.Length; i++)
				{
					Object.DestroyImmediate(_materials[i]);
				}
			}
		}
	}
}
