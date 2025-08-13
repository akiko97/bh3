using UnityEngine;

namespace MoleMole
{
	public static class MeshGenerator
	{
		public static Mesh Quad()
		{
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] array3 = new int[6];
			array[0] = new Vector3(-0.5f, -0.5f, 0f);
			array[1] = new Vector3(0.5f, -0.5f, 0f);
			array[2] = new Vector3(-0.5f, 0.5f, 0f);
			array[3] = new Vector3(0.5f, 0.5f, 0f);
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(1f, 1f);
			array3[0] = 0;
			array3[1] = 1;
			array3[2] = 2;
			array3[3] = 1;
			array3[4] = 3;
			array3[5] = 2;
			Mesh mesh = new Mesh();
			mesh.name = "BillboardQuad";
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			return mesh;
		}

		public static Mesh QuadFaceUp()
		{
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] array3 = new int[6];
			array[0] = new Vector3(-0.5f, 0f, -0.5f);
			array[1] = new Vector3(0.5f, 0f, -0.5f);
			array[2] = new Vector3(-0.5f, 0f, 0.5f);
			array[3] = new Vector3(0.5f, 0f, 0.5f);
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(1f, 1f);
			array3[0] = 0;
			array3[1] = 1;
			array3[2] = 2;
			array3[3] = 1;
			array3[4] = 3;
			array3[5] = 2;
			Mesh mesh = new Mesh();
			mesh.name = "BillboardQuad";
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			return mesh;
		}

		public static Mesh BillboardQuad()
		{
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] array3 = new int[6];
			for (int i = 0; i < 4; i++)
			{
				array[i] = Vector3.zero;
			}
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(1f, 1f);
			array3[0] = 0;
			array3[1] = 1;
			array3[2] = 2;
			array3[3] = 1;
			array3[4] = 3;
			array3[5] = 2;
			Mesh mesh = new Mesh();
			mesh.name = "BillboardQuad";
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			return mesh;
		}
	}
}
