using UnityEngine;

namespace MoleMole
{
	public class Paster : MonoBehaviour
	{
		private class QuadPasterMesh
		{
			private Vector3 dir;

			private Vector3[] offsets = new Vector3[4];

			private Vector2[] uvs = new Vector2[4];

			private int[] tris;

			public QuadPasterMesh(float aspect, float size)
			{
				dir = Vector3.forward;
				float num = size * aspect;
				offsets[0] = new Vector3(0f - num, size);
				uvs[0] = new Vector2(0f, 1f);
				offsets[1] = new Vector3(num, size);
				uvs[1] = new Vector2(1f, 1f);
				offsets[2] = new Vector3(0f - num, 0f - size);
				uvs[2] = new Vector2(0f, 0f);
				offsets[3] = new Vector3(num, 0f - size);
				uvs[3] = new Vector2(1f, 0f);
				tris = new int[6] { 0, 3, 2, 0, 1, 3 };
			}

			public void Transform(Transform tranform)
			{
				dir = tranform.TransformDirection(dir);
				for (int i = 0; i < 4; i++)
				{
					offsets[i] = tranform.TransformPoint(offsets[i]);
					offsets[i] -= tranform.position;
				}
			}

			private Vector3 ProjectPoint(Vector3 point)
			{
				return point - dir * point.y / dir.y;
			}

			public void ProjectorHorizontal()
			{
				for (int i = 0; i < 4; i++)
				{
					offsets[i] = ProjectPoint(offsets[i]);
				}
			}

			public Mesh getMesh()
			{
				Mesh mesh = new Mesh();
				mesh.vertices = offsets;
				mesh.uv = uvs;
				mesh.triangles = tris;
				return mesh;
			}
		}

		protected const float MIN_RATIO = 0.4f;

		protected const float MAX_RATIO = 1f;

		protected const float MIN_HEIGHT = 1f;

		protected const float MAX_HEIGHT = 4f;

		public float AspectRatio = 1f;

		public float Size = 1f;

		public float FalloffStartDistance = 1f;

		public float FalloffEndDistance = 3f;

		public Material Material;

		public int LayerMask = 131072;

		protected Transform _pasterTrsf;

		protected Material _material;

		private Transform _trsf;

		private float _groundHeight;

		private QuadPasterMesh _pasterMesh;

		private GameObject _pasterObj;

		public Material PasterMaterial
		{
			get
			{
				return _material;
			}
		}

		protected virtual void Start()
		{
			_trsf = base.transform;
			_pasterMesh = new QuadPasterMesh(AspectRatio, 1f);
			_pasterMesh.Transform(_trsf);
			_pasterMesh.ProjectorHorizontal();
			_pasterObj = new GameObject();
			_pasterObj.name = "Paster";
			MeshFilter meshFilter = _pasterObj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = _pasterMesh.getMesh();
			MeshRenderer meshRenderer = _pasterObj.AddComponent<MeshRenderer>();
			meshRenderer.material = Material;
			_material = meshRenderer.material;
			_pasterTrsf = _pasterObj.transform;
			_pasterTrsf.SetParent(_trsf, false);
			_pasterTrsf.localPosition = Vector3.zero;
			_pasterTrsf.rotation = Quaternion.identity;
			CalcGroundHeight();
		}

		private void OnEnable()
		{
			if (!(_trsf == null))
			{
				CalcGroundHeight();
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(_material);
		}

		protected virtual void Update()
		{
			float num = _trsf.position.y - _groundHeight;
			_pasterTrsf.position = _trsf.position - _trsf.forward * num / _trsf.forward.y;
			_pasterTrsf.localScale = Vector3.one * Size;
			float value = Mathf.Clamp01(1f - (num - FalloffStartDistance) / FalloffEndDistance);
			_material.SetFloat(InLevelData.SHADER_FALLOFF, value);
		}

		private void CalcGroundHeight()
		{
			_groundHeight = 0f;
		}
	}
}
