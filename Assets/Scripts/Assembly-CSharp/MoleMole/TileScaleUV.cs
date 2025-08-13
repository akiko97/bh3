using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class TileScaleUV : MonoBehaviour
	{
		[Header("Target Renderer")]
		public Renderer targetRenderer;

		private Material[] _materials;

		private void Awake()
		{
			if (Application.isPlaying)
			{
				_materials = targetRenderer.materials;
			}
			else
			{
				_materials = targetRenderer.sharedMaterials;
			}
			SyncMaterialTiling();
		}

		private void SyncMaterialTiling()
		{
			for (int i = 0; i < _materials.Length; i++)
			{
				_materials[i].SetTextureScale("_MainTex", new Vector2(base.transform.localScale.x, 1f));
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				if (base.transform.hasChanged)
				{
					SyncMaterialTiling();
				}
			}
			else
			{
				SyncMaterialTiling();
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
