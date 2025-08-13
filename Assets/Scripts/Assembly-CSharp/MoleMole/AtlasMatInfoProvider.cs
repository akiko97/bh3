using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class AtlasMatInfoProvider : BaseScriptableObject, IFaceMatInfoProvider
	{
		public string basePath;

		public AtlasItem[] items;

		private List<Texture2D> _texturesToUnload = new List<Texture2D>();

		private int _refCnt;

		public int capacity
		{
			get
			{
				return items.Length;
			}
		}

		public FaceMatInfo GetFaceMatInfo(int index)
		{
			if (index < 0 || index >= items.Length)
			{
				return default(FaceMatInfo);
			}
			FaceMatInfo result = default(FaceMatInfo);
			Texture2D texture2D = Resources.Load<Texture2D>(string.Format("{0}/{1}", basePath, items[index].textureName));
			if (!_texturesToUnload.Contains(texture2D))
			{
				_texturesToUnload.Add(texture2D);
			}
			result.texture = texture2D;
			CalcTileAndOffset(items[index].rect, out result.tile, out result.offset);
			return result;
		}

		private void CalcTileAndOffset(Vector4 rect, out Vector2 tile, out Vector2 offset)
		{
			tile = new Vector2(rect.z, rect.w);
			offset = new Vector2(rect.x, rect.y);
		}

		public string[] GetMatInfoNames()
		{
			string[] array = new string[items.Length];
			int i = 0;
			for (int num = items.Length; i < num; i++)
			{
				array[i] = items[i].name;
			}
			return array;
		}

		private void ClearTextureCache()
		{
			int i = 0;
			for (int count = _texturesToUnload.Count; i < count; i++)
			{
				Resources.UnloadAsset(_texturesToUnload[i]);
			}
			_texturesToUnload.Clear();
		}

		public void RetainReference()
		{
			_refCnt++;
		}

		public bool ReleaseReference()
		{
			_refCnt--;
			if (_refCnt <= 0)
			{
				ClearTextureCache();
				return true;
			}
			return false;
		}
	}
}
