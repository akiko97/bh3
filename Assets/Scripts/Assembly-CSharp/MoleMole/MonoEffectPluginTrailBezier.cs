using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginTrailBezier : MonoEffectPluginTrail
	{
		private const int CURVE_DEGREE = 4;

		public int InterpolationSteps = 7;

		public float SmoothValue = 1f;

		private Vector3[] _points;

		private int _pointReady;

		protected override void Awake()
		{
			base.Awake();
			_points = new Vector3[4];
			_pointReady = 0;
		}

		public override void Setup()
		{
		}

		protected override void Update()
		{
			if (_curFrame < FramePosList.Length && !IsToBeRemove())
			{
				if (_pointReady < 4)
				{
					_pointReady++;
				}
				for (int i = 1; i < 4; i++)
				{
					_points[i - 1] = _points[i];
				}
				AniAnchorTransform.localPosition = FramePosList[_curFrame];
				_points[_points.Length - 1] = AniAnchorTransform.position;
				if (_pointReady >= 4)
				{
					DrawBezierCurve();
				}
				_curFrame++;
			}
		}

		private void DrawBezierCurve()
		{
			Vector3 c = Vector3.zero;
			Vector3 c2 = Vector3.zero;
			Bezier.GetControlPoint(_points[0], _points[1], _points[2], _points[3], SmoothValue, ref c, ref c2);
			float num = 1f / (float)InterpolationSteps;
			for (int i = 0; i < InterpolationSteps; i++)
			{
				TrailRendererTransform.position = Bezier.GetPoint(_points[1], c, c2, _points[2], (float)i * num);
			}
		}
	}
}
