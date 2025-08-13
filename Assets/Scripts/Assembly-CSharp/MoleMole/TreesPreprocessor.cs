using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class TreesPreprocessor
	{
		private GameObject _trunksObj;

		private GameObject _leavesRootObj;

		private Mesh _trunksMesh;

		private Transform _trunksTransform;

		private Mesh[] _leafMeshes;

		private Transform[] _leafTransforms;

		private int[][] _truncVrtxIds;

		private int[][] _rootVrtxIds;

		private int[][] _branchVrtxIds;

		public TreesPreprocessor(GameObject trunksObj, GameObject leavesRootObj)
		{
			_trunksObj = trunksObj;
			_leavesRootObj = leavesRootObj;
		}

		public void Process()
		{
			if (CollectMesh(false))
			{
				DistinguishRootsAndBranches();
				SetTrunks();
				SetLeaves();
			}
		}

		private bool CollectMesh(bool collectShared)
		{
			if (_trunksObj == null || _leavesRootObj == null)
			{
				return false;
			}
			MeshFilter component = _trunksObj.GetComponent<MeshFilter>();
			if (collectShared)
			{
				_trunksMesh = component.sharedMesh;
			}
			else
			{
				_trunksMesh = component.mesh;
			}
			_trunksTransform = component.transform;
			List<Mesh> list = new List<Mesh>();
			List<Transform> list2 = new List<Transform>();
			MeshFilter[] componentsInChildren = _leavesRootObj.GetComponentsInChildren<MeshFilter>();
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				if (meshFilter.name.StartsWith("Leaf"))
				{
					if (collectShared)
					{
						list.Add(meshFilter.sharedMesh);
					}
					else
					{
						list.Add(meshFilter.mesh);
					}
					list2.Add(meshFilter.transform);
				}
			}
			_leafMeshes = list.ToArray();
			_leafTransforms = list2.ToArray();
			return true;
		}

		private void GetConnectedGraph(int id, List<int>[] edges, bool[] accTable, List<int> vertices)
		{
			if (accTable[id])
			{
				return;
			}
			vertices.Add(id);
			accTable[id] = true;
			foreach (int item in edges[id])
			{
				GetConnectedGraph(item, edges, accTable, vertices);
			}
		}

		private int[][] SplitObjs(Mesh mesh)
		{
			List<int[]> list = new List<int[]>();
			bool[] array = new bool[mesh.vertexCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = false;
			}
			List<int>[] array2 = new List<int>[mesh.vertexCount];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = new List<int>();
			}
			int[] triangles = mesh.triangles;
			for (int k = 0; k < triangles.Length / 3; k++)
			{
				for (int l = 0; l < 3; l++)
				{
					for (int m = 0; m < 3; m++)
					{
						if (l != m)
						{
							array2[triangles[k * 3 + l]].Add(triangles[k * 3 + m]);
							array2[triangles[k * 3 + m]].Add(triangles[k * 3 + l]);
						}
					}
				}
			}
			Vector3[] vertices = mesh.vertices;
			for (int n = 0; n < vertices.Length; n++)
			{
				for (int num = 0; num < vertices.Length; num++)
				{
					if ((vertices[n] - vertices[num]).magnitude < float.Epsilon)
					{
						array2[n].Add(num);
					}
				}
			}
			while (true)
			{
				int num2;
				for (num2 = 0; num2 < array.Length && array[num2]; num2++)
				{
				}
				if (num2 == array.Length)
				{
					break;
				}
				List<int> list2 = new List<int>();
				GetConnectedGraph(num2, array2, array, list2);
				list.Add(list2.ToArray());
			}
			return list.ToArray();
		}

		private Bounds GetBounds(Vector3[] vertices, int[] vids)
		{
			Vector3 vector = Vector3.one * float.MaxValue;
			Vector3 vector2 = Vector3.one * float.MinValue;
			foreach (int num in vids)
			{
				vector = Vector3.Min(vector, vertices[num]);
				vector2 = Vector3.Max(vector2, vertices[num]);
			}
			return new Bounds((vector + vector2) * 0.5f, vector2 - vector);
		}

		private void DistinguishRootsAndBranches()
		{
			_truncVrtxIds = SplitObjs(_trunksMesh);
			Vector3[] vertices = _trunksMesh.vertices;
			List<int[]> list = new List<int[]>();
			List<int[]> list2 = new List<int[]>();
			int[][] truncVrtxIds = _truncVrtxIds;
			foreach (int[] array in truncVrtxIds)
			{
				Bounds bounds = GetBounds(vertices, array);
				if ((double)(bounds.size.y / bounds.size.magnitude) > 0.94)
				{
					list.Add(array);
				}
				else
				{
					list2.Add(array);
				}
			}
			_rootVrtxIds = list.ToArray();
			_branchVrtxIds = list2.ToArray();
		}

		private Vector3 GetLowestVertex(Vector3[] vertices, int[] vids)
		{
			Vector3 result = vertices[vids[0]];
			foreach (int num in vids)
			{
				if (result.y > vertices[num].y)
				{
					result = vertices[num];
				}
			}
			return result;
		}

		private void SetTrunks()
		{
			Vector3[] vertices = _trunksMesh.vertices;
			Vector4[] array = new Vector4[_trunksMesh.vertexCount];
			int[][] rootVrtxIds = _rootVrtxIds;
			foreach (int[] array2 in rootVrtxIds)
			{
				Vector3 lowestVertex = GetLowestVertex(vertices, array2);
				Vector4 vector = new Vector4(lowestVertex.x, lowestVertex.y, lowestVertex.z, 0f);
				int[] array3 = array2;
				foreach (int num in array3)
				{
					array[num] = vector;
				}
			}
			int[][] branchVrtxIds = _branchVrtxIds;
			foreach (int[] array4 in branchVrtxIds)
			{
				Vector3 lowestVertex2 = GetLowestVertex(vertices, array4);
				Vector4 vector2 = new Vector4(lowestVertex2.x, lowestVertex2.y, lowestVertex2.z, 1f);
				int[] array5 = array4;
				foreach (int num2 in array5)
				{
					array[num2] = vector2;
				}
			}
			_trunksMesh.tangents = array;
		}

		private void SetLeafRoot(Mesh mesh, Transform trsf)
		{
			Vector3 zero = Vector3.zero;
			Vector3[] vertices = mesh.vertices;
			Vector3[] array = vertices;
			foreach (Vector3 vector in array)
			{
				zero += vector;
			}
			zero /= (float)vertices.Length;
			zero = trsf.TransformPoint(zero);
			float[] array2 = new float[_branchVrtxIds.Length];
			Vector3[] vertices2 = _trunksMesh.vertices;
			for (int j = 0; j < vertices2.Length; j++)
			{
				vertices2[j] = _trunksTransform.TransformPoint(vertices2[j]);
			}
			int num = 0;
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k] = 0f;
				int[] array3 = _branchVrtxIds[k];
				for (int l = 0; l < array3.Length; l++)
				{
					array2[k] += 1f / Mathf.Pow((zero - vertices2[array3[l]]).magnitude + 0.1f, 2f);
				}
				array2[k] /= array3.Length;
				if (array2[num] < array2[k])
				{
					num = k;
				}
			}
			Vector3 lowestVertex = GetLowestVertex(vertices2, _branchVrtxIds[num]);
			lowestVertex = trsf.InverseTransformVector(lowestVertex);
			Vector4[] array4 = new Vector4[mesh.vertexCount];
			for (int m = 0; m < array4.Length; m++)
			{
				array4[m] = new Vector4(lowestVertex.x, lowestVertex.y, lowestVertex.z, 0f);
			}
			mesh.tangents = array4;
		}

		private void SetLeaves()
		{
			for (int i = 0; i < _leafMeshes.Length; i++)
			{
				SetLeafRoot(_leafMeshes[i], _leafTransforms[i]);
			}
		}
	}
}
