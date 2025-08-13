using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class UVAnimation : MonoBehaviour
	{
		public float scrollX;

		public float scrollY;

		public float speed2Ratio = 1f;

		public Transform referenceTransform;

		public Vector2 referenceDisplacementPerCycle = Vector2.one;

		[Range(0f, 1f)]
		public float referenceStartPhaseX;

		[Range(0f, 1f)]
		public float referenceStartPhaseY;

		public int materialId;

		public int materialId2 = -1;

		public string TexName;

		private Material _material;

		private Material _material2;

		public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public bool useCurve;

		public float curveSpeed = 1f;

		public float curveScaler = 1f;

		private Vector3 _referenceStartPos;

		private void Start()
		{
			Preparation();
		}

		private void Preparation()
		{
			_material = GetComponent<Renderer>().materials[materialId];
			if (materialId2 >= 0)
			{
				_material2 = GetComponent<Renderer>().materials[materialId2];
			}
			frameOverTime.preWrapMode = WrapMode.Loop;
			frameOverTime.postWrapMode = WrapMode.Loop;
			if (referenceTransform != null)
			{
				_referenceStartPos = referenceTransform.position;
			}
		}

		public void Update()
		{
			Vector2 vector;
			if (referenceTransform == null)
			{
				vector = ((!useCurve) ? (Vector2.one * Time.time) : (Vector2.one * frameOverTime.Evaluate(Time.time * curveSpeed) * curveScaler));
			}
			else
			{
				Vector2 vector2 = default(Vector2);
				Vector3 vector3 = referenceTransform.position - _referenceStartPos;
				vector2.x = vector3.x / referenceDisplacementPerCycle.x + referenceStartPhaseX;
				vector2.y = vector3.y / referenceDisplacementPerCycle.y + referenceStartPhaseY;
				if (useCurve)
				{
					vector = default(Vector2);
					vector = new Vector2(frameOverTime.Evaluate(vector2.x * curveSpeed) * curveScaler, frameOverTime.Evaluate(vector2.y * curveSpeed) * curveScaler);
				}
				else
				{
					vector = vector2;
				}
			}
			if (string.IsNullOrEmpty(TexName))
			{
				_material.SetTextureOffset("_MainTex", new Vector2(vector.x * scrollX % 1f, vector.y * scrollY % 1f));
				if (materialId2 >= 0)
				{
					_material2.SetTextureOffset("_DistortionTex", new Vector2(vector.x * scrollX * speed2Ratio % 1f, vector.y * scrollY * speed2Ratio % 1f));
				}
			}
			else
			{
				_material.SetTextureOffset(TexName, new Vector2(vector.x * scrollX % 1f, vector.y * scrollY % 1f));
				if (materialId2 >= 0)
				{
					_material2.SetTextureOffset("_DistortionTex", new Vector2(vector.x * scrollX * speed2Ratio % 1f, vector.y * scrollY * speed2Ratio % 1f));
				}
			}
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				Object.DestroyImmediate(_material);
			}
		}
	}
}
