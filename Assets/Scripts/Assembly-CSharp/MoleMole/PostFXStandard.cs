using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/PostFX")]
	[ExecuteInEditMode]
	public class PostFXStandard : PostFXBase
	{
		public LayerMask cullingMask;

		[ImageEffectTransformsToLDR]
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			DoPostProcess(source, destination);
		}
	}
}
