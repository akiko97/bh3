using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoElevatorProjectiveLight : MonoBehaviour
	{
		[Header("Distance between projected object and the left of right edge of the projective light sheet (object space)")]
		public float projObjDist;

		[Header("Height of top of the projective obj")]
		public float projObjTopY;

		[Header("Height of bottom of the projective obj")]
		public float projObjBottomY;

		[Header("Distance between light source and the left or right edge of the projective light sheet (object space)")]
		public float lightSourceDist;

		[Header("Light source animation")]
		public float lightSourceHeigthStart;

		public float lightSourceHeigthEnd;

		[Range(0f, 60f)]
		public float lightSourceCycleTime;

		public AnimationCurve lightSourceMovementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[Range(0f, 1f)]
		public float lightSourceMovementPhase;

		public AnimationCurve lightStrenthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float lightStrenthCurveScale = 1f;

		[Header("True if light source at the right light sheet; false if at the right")]
		public bool isLightSourceAtRight;

		public bool pause;

		private MeshFilter _meshFilter;

		private Mesh _sharedMesh;

		private Mesh _mesh;

		private Vector3[] _verticesInTangentSpace;

		private Vector3[] _vertices;

		private Vector2[] _originaluvs;

		private Vector2[] _uvs;

		private float _pivotX;

		private Matrix4x4 _tbnMat = default(Matrix4x4);

		private Matrix4x4 _tbnMat_inv = default(Matrix4x4);

		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		private float _timer;

		private Vector2 _duvdxy;

		private List<List<int>> _groups;

		private float[] _posInProjectSpace;

		private void OnEnable()
		{
			_meshFilter = GetComponent<MeshFilter>();
			_mesh = _meshFilter.mesh;
			_vertices = _mesh.vertices;
			_originaluvs = _mesh.uv;
			_uvs = new Vector2[_originaluvs.Length];
			_renderer = GetComponent<Renderer>();
			_mpb = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_mpb);
			_verticesInTangentSpace = _mesh.vertices;
			Vector3 vector = _mesh.normals[0];
			Vector3 vector2 = new Vector3(0f, 1f, 0f);
			Vector3 vector3 = Vector3.Cross(vector2, vector);
			_tbnMat = default(Matrix4x4);
			_tbnMat.SetRow(0, vector3);
			_tbnMat.SetRow(1, vector2);
			_tbnMat.SetRow(2, vector);
			_tbnMat.m33 = 1f;
			_tbnMat_inv = _tbnMat.inverse;
			for (int i = 0; i < _mesh.vertexCount; i++)
			{
				_verticesInTangentSpace[i] = _tbnMat.MultiplyPoint(_verticesInTangentSpace[i]);
			}
			_groups = new List<List<int>>();
			List<float> list = new List<float>();
			List<float> list2 = new List<float>();
			float num = 0.01f;
			for (int j = 0; j < _mesh.vertexCount; j++)
			{
				bool flag = false;
				for (int k = 0; k < _groups.Count; k++)
				{
					List<int> list3 = _groups[k];
					if (Mathf.Abs(_verticesInTangentSpace[list3[0]].x - _verticesInTangentSpace[j].x) < num)
					{
						flag = true;
						list3.Add(j);
						list[k] = Mathf.Min(list[k], _verticesInTangentSpace[j].y);
						list2[k] = Mathf.Max(list2[k], _verticesInTangentSpace[j].y);
						break;
					}
				}
				if (!flag)
				{
					List<int> list4 = new List<int>();
					list4.Add(j);
					_groups.Add(list4);
					list.Add(_verticesInTangentSpace[j].y);
					list2.Add(_verticesInTangentSpace[j].y);
				}
			}
			_posInProjectSpace = new float[_mesh.vertexCount];
			for (int l = 0; l < _groups.Count; l++)
			{
				float num2 = list2[l] - list[l];
				List<int> list5 = _groups[l];
				foreach (int item in list5)
				{
					_posInProjectSpace[item] = (_verticesInTangentSpace[item].y - list[l]) / num2;
				}
			}
			float num3 = 0f;
			int num4 = -1;
			float num5 = 0f;
			int num6 = -1;
			for (int m = 1; m < _mesh.vertexCount - _groups.Count; m++)
			{
				Vector3 vector4 = _verticesInTangentSpace[m] - _verticesInTangentSpace[0];
				if (Mathf.Abs(vector4.x) > num3)
				{
					num3 = Mathf.Abs(vector4.x);
					num4 = m;
				}
				if (Mathf.Abs(vector4.y) > num5)
				{
					num5 = Mathf.Abs(vector4.y);
					num6 = m;
				}
			}
			_duvdxy = default(Vector2);
			_duvdxy.x = (_originaluvs[0].x - _originaluvs[num4].x) / (_verticesInTangentSpace[0].x - _verticesInTangentSpace[num4].x);
			_duvdxy.y = (_originaluvs[0].y - _originaluvs[num6].y) / (_verticesInTangentSpace[0].y - _verticesInTangentSpace[num6].y);
			for (int n = 0; n < _groups.Count; n++)
			{
				List<int> list6 = _groups[n];
				int num7 = list6[list6.Count - 1];
				int num8 = list6[list6.Count - 2];
				_originaluvs[num7].y = _originaluvs[num8].y + (_verticesInTangentSpace[num7].y - _verticesInTangentSpace[num8].y) * _duvdxy.y;
			}
			_pivotX = ((!isLightSourceAtRight) ? 99999 : (-99999));
			foreach (List<int> group in _groups)
			{
				int num9 = group[0];
				if (isLightSourceAtRight)
				{
					_pivotX = Mathf.Max(_pivotX, _verticesInTangentSpace[num9].x);
				}
				else
				{
					_pivotX = Mathf.Min(_pivotX, _verticesInTangentSpace[num9].x);
				}
			}
		}

		private void Release()
		{
			if (_mesh != null)
			{
				Object.Destroy(_mesh);
			}
		}

		private void OnDisable()
		{
			_meshFilter.mesh = _sharedMesh;
			Release();
		}

		private void OnDestroy()
		{
			Release();
		}

		private void Update()
		{
			float num = ((!isLightSourceAtRight) ? (_pivotX + projObjDist) : (_pivotX - projObjDist));
			float num2 = ((!isLightSourceAtRight) ? (_pivotX + lightSourceDist) : (_pivotX - lightSourceDist));
			_timer += Time.deltaTime;
			float num3 = _timer / lightSourceCycleTime;
			num3 += lightSourceMovementPhase;
			num3 -= (float)Mathf.FloorToInt(num3);
			float t = lightSourceMovementCurve.Evaluate(num3);
			float num4 = Mathf.Lerp(lightSourceHeigthStart, lightSourceHeigthEnd, t);
			float value = lightStrenthCurve.Evaluate(num3) * lightStrenthCurveScale;
			_mpb.SetFloat("_LightStrength", value);
			_renderer.SetPropertyBlock(_mpb);
			for (int i = 0; i < _groups.Count; i++)
			{
				List<int> list = _groups[i];
				float x = _verticesInTangentSpace[list[0]].x;
				float num5 = num4 + (projObjTopY - num4) * (x - num2) / (num - num2);
				float num6 = num4 + (projObjBottomY - num4) * (x - num2) / (num - num2);
				for (int j = 0; j < list.Count; j++)
				{
					int num7 = list[j];
					Vector3 vector = _verticesInTangentSpace[num7];
					vector.y = num6 + (num5 - num6) * _posInProjectSpace[num7];
					_vertices[num7] = _tbnMat_inv.MultiplyPoint(vector);
					Vector3 vector2 = vector - _verticesInTangentSpace[num7];
					_uvs[num7].x = _originaluvs[num7].x + vector2.x * _duvdxy.x;
					_uvs[num7].y = _originaluvs[num7].y + vector2.y * _duvdxy.y;
				}
			}
			if (!pause)
			{
				_mesh.vertices = _vertices;
				_mesh.uv = _uvs;
			}
		}
	}
}
