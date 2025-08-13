using System;
using UnityEngine;

namespace MoleMole
{
	public static class Bezier
	{
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * p0 + 2f * num * t * p1 + t * t * p2;
		}

		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
		}

		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
		}

		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
		}

		public static void GetControlPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float smooth_value, ref Vector3 c1, ref Vector3 c2)
		{
			float x = p0.x;
			float y = p0.y;
			float z = p0.z;
			float x2 = p1.x;
			float y2 = p1.y;
			float z2 = p1.z;
			float x3 = p2.x;
			float y3 = p2.y;
			float z3 = p2.z;
			float x4 = p3.x;
			float y4 = p3.y;
			float z4 = p3.z;
			float num = (x + x2) / 2f;
			float num2 = (y + y2) / 2f;
			float num3 = (z + z2) / 2f;
			float num4 = (x2 + x3) / 2f;
			float num5 = (y2 + y3) / 2f;
			float num6 = (z2 + z3) / 2f;
			float num7 = (x3 + x4) / 2f;
			float num8 = (y3 + y4) / 2f;
			float num9 = (z3 + z4) / 2f;
			float num10 = (float)Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y) + (z2 - z) * (z2 - z));
			float num11 = (float)Math.Sqrt((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2) + (z3 - z2) * (z3 - z2));
			float num12 = (float)Math.Sqrt((x4 - x3) * (x4 - x3) + (y4 - y3) * (y4 - y3) + (z4 - z3) * (z4 - z3));
			float num13 = num10 / (num10 + num11);
			float num14 = num11 / (num11 + num12);
			float num15 = num + (num4 - num) * num13;
			float num16 = num2 + (num5 - num2) * num13;
			float num17 = num3 + (num6 - num3) * num13;
			float num18 = num4 + (num7 - num4) * num14;
			float num19 = num5 + (num8 - num5) * num14;
			float num20 = num6 + (num9 - num6) * num14;
			smooth_value = Mathf.Clamp01(smooth_value);
			float x5 = num15 + (num4 - num15) * smooth_value + x2 - num15;
			float y5 = num16 + (num5 - num16) * smooth_value + y2 - num16;
			float z5 = num17 + (num6 - num17) * smooth_value + z2 - num17;
			float x6 = num18 + (num4 - num18) * smooth_value + x3 - num18;
			float y6 = num19 + (num5 - num19) * smooth_value + y3 - num19;
			float z6 = num20 + (num6 - num20) * smooth_value + z3 - num20;
			c1 = new Vector3(x5, y5, z5);
			c2 = new Vector3(x6, y6, z6);
		}
	}
}
