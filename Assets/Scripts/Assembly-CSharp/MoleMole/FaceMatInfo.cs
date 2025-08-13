using UnityEngine;

namespace MoleMole
{
	public struct FaceMatInfo
	{
		public Texture2D texture;

		public Vector2 tile;

		public Vector2 offset;

		public bool valid
		{
			get
			{
				return texture != null;
			}
		}
	}
}
