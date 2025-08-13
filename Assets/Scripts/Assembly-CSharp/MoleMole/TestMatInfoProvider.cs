using UnityEngine;

namespace MoleMole
{
	public class TestMatInfoProvider : MonoBehaviour, IFaceMatInfoProvider
	{
		public Texture2D[] textures;

		public int capacity
		{
			get
			{
				return textures.Length;
			}
		}

		public FaceMatInfo GetFaceMatInfo(int index)
		{
			FaceMatInfo result = default(FaceMatInfo);
			if (index >= 0 && index < textures.Length)
			{
				result.texture = textures[index];
				result.tile = new Vector2(1f, 1f);
				result.offset = Vector2.zero;
			}
			return result;
		}

		public string[] GetMatInfoNames()
		{
			string[] array = new string[textures.Length];
			int i = 0;
			for (int num = textures.Length; i < num; i++)
			{
				array[i] = textures[i].name;
			}
			return array;
		}
	}
}
