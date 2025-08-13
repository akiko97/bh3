using UnityEngine;

namespace MoleMole
{
	public class MonoAvatarFaceControl : MonoBehaviour
	{
		public Renderer faceRenderer;

		public FaceTextureItem[] faceTextureItems;

		public FaceFrame[] faceFrames;

		public int defaultFaceFrameIndex;

		private Texture2D _currentTexture;

		private int _currentFaceIndex = -1;

		public bool useUpdateFaceIndex;

		public float targetFaceIndex;

		private void Start()
		{
			SetFace(defaultFaceFrameIndex);
		}

		private void Update()
		{
			if (useUpdateFaceIndex && _currentFaceIndex != (int)targetFaceIndex)
			{
				SetFace((int)targetFaceIndex);
			}
		}

		public int GetFaceCount()
		{
			return faceFrames.Length;
		}

		public bool SetFace(int frameIndex)
		{
			if (frameIndex == _currentFaceIndex)
			{
				return true;
			}
			if (faceRenderer == null)
			{
				Debug.LogError("[GalTouch] face renderer not set : " + base.gameObject.name);
				return false;
			}
			if (frameIndex < 0 || frameIndex >= faceFrames.Length)
			{
				Debug.LogError(string.Format("[GalTouch] face frame index({0}) out of range : {1}", frameIndex.ToString(), base.gameObject.name));
				return false;
			}
			FaceFrame faceFrame = faceFrames[frameIndex];
			if (faceFrame.textureItemIndex < 0 || faceFrame.textureItemIndex >= faceTextureItems.Length)
			{
				Debug.LogError(string.Format("[GalTouch] face texture item index({0}) out of range : {1}", faceFrame.textureItemIndex.ToString(), base.gameObject.name));
				return false;
			}
			FaceTextureItem faceTextureItem = faceTextureItems[faceFrame.textureItemIndex];
			if (faceFrame.frameIndex < 0 || faceFrame.frameIndex >= faceTextureItem.row * faceTextureItem.row)
			{
				Debug.LogError("[GalTouch] face texture frame index out of range : " + base.gameObject.name);
				return false;
			}
			if (faceTextureItem.row <= 0)
			{
				Debug.LogError("[GalTouch] texture item row illegal : " + base.gameObject.name);
				return false;
			}
			SetFaceTextureAndUV(faceTextureItem.texture, faceTextureItem.row, faceFrame.frameIndex);
			_currentFaceIndex = frameIndex;
			return true;
		}

		private void SetFaceTextureAndUV(Texture2D texture, int row, int index)
		{
			Material material = faceRenderer.material;
			if (_currentTexture != texture)
			{
				_currentTexture = texture;
				material.mainTexture = texture;
			}
			int num = index / row;
			int num2 = index % row;
			float num3 = 1f / (float)row;
			material.mainTextureScale = new Vector2(num3, num3);
			material.mainTextureOffset = new Vector2((float)num * num3, (float)num2 * num3);
		}
	}
}
