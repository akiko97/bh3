using UnityEngine;

namespace MoleMole
{
	public class FacePartControl
	{
		private IFaceMatInfoProvider _faceMatInfoProvider;

		private Renderer _facePartRenderer;

		private FaceMatInfo _originFaceMatInfo = default(FaceMatInfo);

		private string[] _frameNames;

		public void Init(IFaceMatInfoProvider provider, Renderer part)
		{
			_faceMatInfoProvider = provider;
			_facePartRenderer = part;
			_originFaceMatInfo = new FaceMatInfo
			{
				texture = (part.material.mainTexture as Texture2D),
				tile = part.material.mainTextureScale,
				offset = part.material.mainTextureOffset
			};
			string[] matInfoNames = provider.GetMatInfoNames();
			_frameNames = new string[matInfoNames.Length + 1];
			_frameNames[0] = "origin";
			int i = 0;
			for (int num = matInfoNames.Length; i < num; i++)
			{
				_frameNames[i + 1] = matInfoNames[i];
			}
		}

		public void SetFacePartIndex(int index)
		{
			if (_faceMatInfoProvider != null && !(_facePartRenderer == null))
			{
				FaceMatInfo faceMatInfo = ((index != 0) ? _faceMatInfoProvider.GetFaceMatInfo(index - 1) : _originFaceMatInfo);
				if (faceMatInfo.valid)
				{
					_facePartRenderer.material.mainTexture = faceMatInfo.texture;
					_facePartRenderer.material.mainTextureScale = faceMatInfo.tile;
					_facePartRenderer.material.mainTextureOffset = faceMatInfo.offset;
				}
			}
		}

		public int GetMaxIndex()
		{
			return _faceMatInfoProvider.capacity;
		}

		public void Reset()
		{
			_facePartRenderer.material.mainTexture = _originFaceMatInfo.texture;
			_facePartRenderer.material.mainTextureScale = _originFaceMatInfo.tile;
			_facePartRenderer.material.mainTextureOffset = _originFaceMatInfo.offset;
		}

		public string[] GetFrameNames()
		{
			return _frameNames;
		}
	}
}
