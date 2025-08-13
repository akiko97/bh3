using System;
using UnityEngine;

namespace MoleMole.Config
{
	public class TrailData
	{
		public bool clockwise = true;

		public int points = 5;

		public float totalPolar = (float)Math.PI * 2f;

		public float startPolarOffset;

		public float radiusX = 1f;

		public float radiusY = 1f;

		public float liftY = 1f;

		public Vector3 rotate = Vector3.zero;

		public Vector3 position = Vector3.zero;

		public TrailData()
		{
			clockwise = true;
			points = 5;
			totalPolar = (float)Math.PI * 2f;
			startPolarOffset = 0f;
			radiusX = 1f;
			radiusY = 1f;
			liftY = 1f;
			rotate = Vector3.zero;
			position = Vector3.zero;
		}

		public TrailData(bool clockwise, int points, float totalPolar, float startPolarOffset, float radiusX, float radiusY, float liftY, Vector3 rotate, Vector3 position)
		{
			this.clockwise = clockwise;
			this.points = points;
			this.totalPolar = totalPolar;
			this.startPolarOffset = startPolarOffset;
			this.radiusX = radiusX;
			this.radiusY = radiusY;
			this.liftY = liftY;
			this.rotate = rotate;
			this.position = position;
		}
	}
}
