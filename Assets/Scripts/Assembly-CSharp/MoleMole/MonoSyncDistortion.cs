using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoSyncDistortion : MonoBehaviour
	{
		private const string DISTORTION_MATERIAL_NAME = "DistortionNormal";

		private const string DISTORTION_INTENSITY_NAME = "_DistortionIntensity";

		private const string DISTORTION_SPTRANSITION_NAME = "_SPTransition";

		private bool needSync;

		private MaterialPropertyBlock _sourceMaterial;

		private List<KeyValuePair<MeshRenderer, MaterialPropertyBlock>> _targetMaterials;

		private void Start()
		{
			_targetMaterials = new List<KeyValuePair<MeshRenderer, MaterialPropertyBlock>>();
			foreach (Transform item in base.transform)
			{
				if (item.GetComponent<MeshRenderer>() != null)
				{
					needSync = true;
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					item.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
					_targetMaterials.Add(new KeyValuePair<MeshRenderer, MaterialPropertyBlock>(item.GetComponent<MeshRenderer>(), materialPropertyBlock));
				}
			}
			_sourceMaterial = new MaterialPropertyBlock();
			base.transform.GetComponent<MeshRenderer>().GetPropertyBlock(_sourceMaterial);
		}

		private void Update()
		{
			if (!needSync)
			{
				return;
			}
			base.transform.GetComponent<MeshRenderer>().GetPropertyBlock(_sourceMaterial);
			foreach (KeyValuePair<MeshRenderer, MaterialPropertyBlock> targetMaterial in _targetMaterials)
			{
				targetMaterial.Value.SetFloat("_DistortionIntensity", _sourceMaterial.GetFloat("_DistortionIntensity"));
				targetMaterial.Value.SetFloat("_SPTransition", _sourceMaterial.GetFloat("_SPTransition"));
				if (targetMaterial.Key != null)
				{
					targetMaterial.Key.SetPropertyBlock(targetMaterial.Value);
				}
			}
		}
	}
}
