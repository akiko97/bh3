using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[DisallowMultipleComponent]
	public class MonoTreeLeaves : MonoBehaviour
	{
		private class MeshTransformPair
		{
			public Mesh mesh;

			public Transform trsf;

			public MeshTransformPair(Mesh mesh, Transform trsf)
			{
				this.mesh = mesh;
				this.trsf = trsf;
			}
		}

		private static readonly string TRUNK_PREFIX = "Trunk";

		public float ZOffsetStep = 0.001f;

		private Vector3[] _originalLocalPositions;

		private void Start()
		{
			List<MeshTransformPair> list = new List<MeshTransformPair>();
			MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				if (!meshFilter.name.StartsWith(TRUNK_PREFIX))
				{
					list.Add(new MeshTransformPair(meshFilter.mesh, meshFilter.transform));
				}
			}
			foreach (MeshTransformPair item in list)
			{
				SetBilloardOffset(item.mesh, item.trsf);
			}
		}

		private void SetBilloardOffset(Mesh mesh, Transform trsf)
		{
			Vector3[] vertices = mesh.vertices;
			if (vertices.Length != 0)
			{
				mesh.RecalculateNormals();
				Vector3 rhs = trsf.TransformDirection(mesh.normals[0]);
				Vector3 normalized = Vector3.Cross(Vector3.up, rhs).normalized;
				Vector2[] array = new Vector2[vertices.Length];
				for (int i = 0; i < vertices.Length; i++)
				{
					Vector3 lhs = trsf.TransformPoint(vertices[i]) - trsf.position;
					array[i] = new Vector2(Vector3.Dot(lhs, normalized), lhs.y);
					vertices[i] = Vector3.zero;
				}
				mesh.vertices = vertices;
				mesh.uv2 = array;
			}
		}
	}
}
