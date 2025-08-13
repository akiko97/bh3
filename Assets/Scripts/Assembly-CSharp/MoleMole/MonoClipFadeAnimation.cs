using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Animation))]
	public class MonoClipFadeAnimation : MonoAuxObject
	{
		[Header("Use animation to key this instead of transform.rotation for working constant tangent")]
		public Vector4 keyedClipPlane;

		private List<Material> _materialList;

		public void Awake()
		{
			_materialList = new List<Material>();
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				_materialList.Add(skinnedMeshRenderer.material);
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < _materialList.Count; i++)
			{
				_materialList[i].SetVector("_ClipPlane", keyedClipPlane);
			}
		}
	}
}
