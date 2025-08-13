using System;
using PigeonCoopToolkit.Utillities;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public class StaticTrail : TrailRenderer_Base
	{
		private class ControlPoint
		{
			public Vector3 p;

			public Vector3 forward;
		}

		public int PointsBetweenControlPoints = 4;

		private CircularBuffer<ControlPoint> _controlPoints;

		protected float _timer;

		protected float _appearDuration;

		protected AnimationCurve _appearCurve;

		protected float _vanishDuration;

		protected AnimationCurve _vanishCurve;

		[NonSerialized]
		public float TimeScale = 1f;

		public bool IsActive { get; set; }

		protected override void Awake()
		{
			_t = base.transform;
			_emit = false;
			Material[] materials = TrailData.GetMaterialsContainer().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				string val = materials[i].GetTag("Distortion", false);
				materials[i] = new Material(materials[i]);
				materials[i].SetOverrideTag("Distortion", val);
				materials[i].name += "(Instance)";
			}
		}

		protected override void OnDestroy()
		{
			Material[] materials = TrailData.GetMaterialsContainer().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(materials[i]);
				}
				else
				{
					UnityEngine.Object.Destroy(materials[i]);
				}
				materials[i] = null;
			}
		}

		protected override void Update()
		{
		}

		public virtual void PlayAnimation(float deltaTime)
		{
			if (!IsActive)
			{
				return;
			}
			_timer += deltaTime;
			if (_timer < 0f)
			{
				return;
			}
			float num = _timer / _appearDuration;
			float num2 = 0f;
			if (num > 1f)
			{
				num = 1f;
				num2 = (_timer - _appearDuration) / _vanishDuration;
				if (num2 > 1f)
				{
					IsActive = false;
				}
			}
			num = _appearCurve.Evaluate(num);
			num2 = _vanishCurve.Evaluate(num2);
			float value = 0f;
			Vector3 vector = Vector3.zero;
			if (!TrailData.UseForwardOverride || !TrailData.ForwardOverrideRelative)
			{
				value = 1f;
				vector = ((!(Camera.main != null)) ? Vector3.forward : Camera.main.transform.forward);
				if (TrailData.UseForwardOverride)
				{
					vector = TrailData.ForwardOverride.normalized;
				}
				vector = _t.InverseTransformDirection(vector);
			}
			Material[] materials = TrailData.GetMaterialsContainer().materials;
			foreach (Material material in materials)
			{
				material.SetVector("_CamForward", vector);
				material.SetFloat("_IsUseCamForward", value);
				material.SetFloat("_AppearTime", num);
				material.SetFloat("_VanishTime", num2);
			}
		}

		public virtual void ResetAnimation(float appearDuration, AnimationCurve appearCurve, float vanishDuration, AnimationCurve vanishCurve)
		{
			IsActive = true;
			_timer = 0f;
			_appearDuration = appearDuration;
			_appearCurve = appearCurve;
			_vanishDuration = vanishDuration;
			_vanishCurve = vanishCurve;
			Matrix4x4 matrix = default(Matrix4x4);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					matrix[i, j] = TrailData.SizeOverLife.Evaluate((float)(i * 4 + j) / 15f);
				}
			}
			Vector4 zero = Vector4.zero;
			if (TrailData.ColorOverLife.alphaKeys.Length > 2)
			{
				zero.x = TrailData.ColorOverLife.alphaKeys[1].time;
				zero.y = TrailData.ColorOverLife.alphaKeys[2].time;
			}
			else
			{
				zero.x = 0f;
				zero.y = 1f;
			}
			Material[] materials = TrailData.GetMaterialsContainer().materials;
			foreach (Material material in materials)
			{
				material.SetFloat("_AppearTime", 0f);
				material.SetFloat("_VanishTime", 0f);
				material.SetMatrix("_SizeOverLife", matrix);
				material.SetVector("_AlphaOverLife", zero);
			}
		}

		protected override void LateUpdate()
		{
			if (_activeTrail != null)
			{
				Material[] materials = TrailData.GetMaterialsContainer().materials;
				foreach (Material material in materials)
				{
					Graphics.DrawMesh(_activeTrail.Mesh, _t.localToWorldMatrix, material, base.gameObject.layer);
				}
			}
		}

		protected override void OnStartEmit()
		{
		}

		public virtual void Init(Vector3[] controlPoints)
		{
			if (_activeTrail != null)
			{
				_activeTrail.Dispose();
			}
			_activeTrail = new PCTrail(controlPoints.Length + (controlPoints.Length - 1) * PointsBetweenControlPoints);
			_controlPoints = new CircularBuffer<ControlPoint>(controlPoints.Length);
			for (int i = 0; i < controlPoints.Length - 1; i++)
			{
				AddControlPoint(controlPoints[i]);
			}
			if (controlPoints.Length > 0)
			{
				AddControlPoint(controlPoints[controlPoints.Length - 1], true);
			}
			GenerateTrail(_activeTrail);
			GenerateMesh(_activeTrail);
		}

		protected virtual void AddControlPoint(Vector3 position, bool isEndPoint = false)
		{
			if (!isEndPoint)
			{
				for (int i = 0; i < PointsBetweenControlPoints; i++)
				{
					AddPoint(new PCTrailPoint(), position);
				}
			}
			AddPoint(new PCTrailPoint(), position);
			ControlPoint controlPoint = new ControlPoint();
			controlPoint.p = position;
			ControlPoint controlPoint2 = controlPoint;
			if (TrailData.UseForwardOverride)
			{
				controlPoint2.forward = TrailData.ForwardOverride.normalized;
			}
			_controlPoints.Add(controlPoint2);
		}

		protected new void AddPoint(PCTrailPoint newPoint, Vector3 pos)
		{
			if (_activeTrail != null)
			{
				newPoint.Position = pos;
				newPoint.PointNumber = ((_activeTrail.Points.Count != 0) ? (_activeTrail.Points[_activeTrail.Points.Count - 1].PointNumber + 1) : 0);
				InitialiseNewPoint(newPoint);
				newPoint.SetDistanceFromStart((_activeTrail.Points.Count != 0) ? (_activeTrail.Points[_activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(_activeTrail.Points[_activeTrail.Points.Count - 1].Position, pos)) : 0f);
				if (TrailData.UseForwardOverride)
				{
					newPoint.Forward = TrailData.ForwardOverride.normalized;
				}
				_activeTrail.Points.Add(newPoint);
			}
		}

		protected virtual void GenerateTrail(PCTrail trail)
		{
			int num = 0;
			for (int i = 0; i < _controlPoints.Count; i++)
			{
				trail.Points[num].Position = _controlPoints[i].p;
				if (TrailData.UseForwardOverride)
				{
					trail.Points[num].Forward = _controlPoints[i].forward;
				}
				num++;
				if (i >= _controlPoints.Count - 1)
				{
					continue;
				}
				float num2 = Vector3.Distance(_controlPoints[i].p, _controlPoints[i + 1].p) / 2f;
				Vector3 curveStartHandle = ((i != 0) ? (_controlPoints[i].p + (_controlPoints[i + 1].p - _controlPoints[i - 1].p).normalized * num2) : (_controlPoints[i].p + (_controlPoints[i + 1].p - _controlPoints[i].p).normalized * num2));
				int num3 = i + 1;
				Vector3 curveEndHandle = ((num3 != _controlPoints.Count - 1) ? (_controlPoints[num3].p + (_controlPoints[num3 - 1].p - _controlPoints[num3 + 1].p).normalized * num2) : (_controlPoints[num3].p + (_controlPoints[num3 - 1].p - _controlPoints[num3].p).normalized * num2));
				PCTrailPoint pCTrailPoint = trail.Points[num - 1];
				PCTrailPoint pCTrailPoint2 = trail.Points[num - 1 + PointsBetweenControlPoints + 1];
				for (int j = 0; j < PointsBetweenControlPoints; j++)
				{
					float t = ((float)j + 1f) / ((float)PointsBetweenControlPoints + 1f);
					trail.Points[num].Position = GetPointAlongCurve(_controlPoints[i].p, curveStartHandle, _controlPoints[i + 1].p, curveEndHandle, t, 0.3f);
					trail.Points[num].SetTimeActive(Mathf.Lerp(pCTrailPoint.TimeActive(), pCTrailPoint2.TimeActive(), t));
					if (TrailData.UseForwardOverride)
					{
						trail.Points[num].Forward = Vector3.Lerp(pCTrailPoint.Forward, pCTrailPoint2.Forward, t);
					}
					num++;
				}
			}
			float num4 = 0f;
			for (int k = 1; k < trail.Points.Count; k++)
			{
				num4 += Vector3.Distance(trail.Points[k - 1].Position, trail.Points[k].Position);
				trail.Points[k].SetDistanceFromStart(num4);
			}
		}

		private void GenerateMesh(PCTrail trail)
		{
			trail.Mesh.Clear(false);
			Vector3 rhs = ((!(Camera.main != null)) ? Vector3.forward : Camera.main.transform.forward);
			if (TrailData.UseForwardOverride)
			{
				rhs = TrailData.ForwardOverride.normalized;
			}
			trail.activePointCount = trail.Points.Count;
			if (trail.activePointCount < 2)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < trail.Points.Count; i++)
			{
				PCTrailPoint pCTrailPoint = trail.Points[i];
				if (!(pCTrailPoint.TimeActive() > TrailData.Lifetime))
				{
					if (TrailData.UseForwardOverride && TrailData.ForwardOverrideRelative)
					{
						rhs = pCTrailPoint.Forward;
					}
					Vector3 zero = Vector3.zero;
					zero = ((i < trail.Points.Count - 1) ? ((!TrailData.UseForwardOverride || !TrailData.ForwardOverrideRelative) ? (trail.Points[i + 1].Position - pCTrailPoint.Position).normalized : Vector3.Cross((trail.Points[i + 1].Position - pCTrailPoint.Position).normalized, rhs).normalized) : ((!TrailData.UseForwardOverride || !TrailData.ForwardOverrideRelative) ? (pCTrailPoint.Position - trail.Points[i - 1].Position).normalized : Vector3.Cross((pCTrailPoint.Position - trail.Points[i - 1].Position).normalized, rhs).normalized));
					trail.verticies[num] = pCTrailPoint.Position;
					trail.normals[num] = zero * StretchUpRatio;
					if (clockSize)
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 1f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 0f);
					}
					trail.colors[num] = Color.white;
					num++;
					trail.verticies[num] = pCTrailPoint.Position;
					trail.normals[num] = -zero * StretchDownRatio;
					if (clockSize)
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 0f);
					}
					else
					{
						trail.uvs[num] = new Vector2(pCTrailPoint.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 1f);
					}
					trail.colors[num] = Color.white;
					num++;
				}
			}
			Vector2 vector = trail.verticies[num - 1];
			Vector3 vector2 = trail.normals[num - 1];
			for (int j = num; j < trail.verticies.Length; j++)
			{
				trail.verticies[j] = vector;
				trail.normals[j] = vector2;
			}
			int num2 = 0;
			for (int k = 0; k < 2 * (trail.activePointCount - 1); k++)
			{
				if (k % 2 == 0)
				{
					trail.indicies[num2] = k;
					num2++;
					trail.indicies[num2] = k + 1;
					num2++;
					trail.indicies[num2] = k + 2;
				}
				else
				{
					trail.indicies[num2] = k + 2;
					num2++;
					trail.indicies[num2] = k + 1;
					num2++;
					trail.indicies[num2] = k;
				}
				num2++;
			}
			int num3 = trail.indicies[num2 - 1];
			for (int l = num2; l < trail.indicies.Length; l++)
			{
				trail.indicies[l] = num3;
			}
			trail.Mesh.vertices = trail.verticies;
			trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
			trail.Mesh.uv = trail.uvs;
			trail.Mesh.normals = trail.normals;
			trail.Mesh.colors = trail.colors;
		}

		public Vector3 GetPointAlongCurve(Vector3 curveStart, Vector3 curveStartHandle, Vector3 curveEnd, Vector3 curveEndHandle, float t, float crease)
		{
			float num = 1f - t;
			float num2 = Mathf.Pow(num, 3f);
			float num3 = Mathf.Pow(num, 2f);
			float num4 = 1f - crease;
			return (num2 * curveStart * num4 + 3f * num3 * t * curveStartHandle * crease + 3f * num * Mathf.Pow(t, 2f) * curveEndHandle * crease + Mathf.Pow(t, 3f) * curveEnd * num4) / (num2 * num4 + 3f * num3 * t * crease + 3f * num * Mathf.Pow(t, 2f) * crease + Mathf.Pow(t, 3f) * num4);
		}

		public Vector3 GetPointAlongTrail(float t)
		{
			CircularBuffer<PCTrailPoint> points = _activeTrail.Points;
			if (points.Count == 0)
			{
				return Vector3.zero;
			}
			if (points.Count < 2)
			{
				return _t.TransformPoint(points[0].Position);
			}
			t = Mathf.Clamp01(t);
			float num = points[points.Count - 1].GetDistanceFromStart() * t;
			Vector3 position = points[points.Count - 1].Position;
			for (int i = 1; i < points.Count; i++)
			{
				if (points[i].GetDistanceFromStart() > num)
				{
					float t2 = (num - points[i - 1].GetDistanceFromStart()) / (points[i].GetDistanceFromStart() - points[i - 1].GetDistanceFromStart());
					position = Vector3.Lerp(points[i - 1].Position, points[i].Position, t2);
					break;
				}
			}
			return _t.TransformPoint(position);
		}

		protected override int GetMaxNumberOfPoints()
		{
			throw new NotImplementedException();
		}
	}
}
