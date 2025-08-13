using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/PostFX")]
	public class PostFX : PostFXWithResScale
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			internalBufferSize = InternalBufferSizeEnum.SIZE_128;
			CameraResScale = CAMERA_RES_SCALE.RES_100;
		}
	}
}
