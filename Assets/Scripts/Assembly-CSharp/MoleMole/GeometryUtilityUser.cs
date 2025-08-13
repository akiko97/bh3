using System;
using UnityEngine;

namespace MoleMole
{
	public static class GeometryUtilityUser
	{
		private enum EPlaneSide
		{
			Left = 0,
			Right = 1,
			Bottom = 2,
			Top = 3,
			Near = 4,
			Far = 5
		}

		private static float[] RootVector = new float[4];

		private static float[] ComVector = new float[4];

		public static void CalculateFrustumPlanes(Camera InCamera, ref Plane[] OutPlanes)
		{
			Matrix4x4 projectionMatrix = InCamera.projectionMatrix;
			Matrix4x4 worldToCameraMatrix = InCamera.worldToCameraMatrix;
			Matrix4x4 matrix4x = projectionMatrix * worldToCameraMatrix;
			RootVector[0] = matrix4x[3, 0];
			RootVector[1] = matrix4x[3, 1];
			RootVector[2] = matrix4x[3, 2];
			RootVector[3] = matrix4x[3, 3];
			ComVector[0] = matrix4x[0, 0];
			ComVector[1] = matrix4x[0, 1];
			ComVector[2] = matrix4x[0, 2];
			ComVector[3] = matrix4x[0, 3];
			CalcPlane(ref OutPlanes[0], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
			CalcPlane(ref OutPlanes[1], 0f - ComVector[0] + RootVector[0], 0f - ComVector[1] + RootVector[1], 0f - ComVector[2] + RootVector[2], 0f - ComVector[3] + RootVector[3]);
			ComVector[0] = matrix4x[1, 0];
			ComVector[1] = matrix4x[1, 1];
			ComVector[2] = matrix4x[1, 2];
			ComVector[3] = matrix4x[1, 3];
			CalcPlane(ref OutPlanes[2], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
			CalcPlane(ref OutPlanes[3], 0f - ComVector[0] + RootVector[0], 0f - ComVector[1] + RootVector[1], 0f - ComVector[2] + RootVector[2], 0f - ComVector[3] + RootVector[3]);
			ComVector[0] = matrix4x[2, 0];
			ComVector[1] = matrix4x[2, 1];
			ComVector[2] = matrix4x[2, 2];
			ComVector[3] = matrix4x[2, 3];
			CalcPlane(ref OutPlanes[4], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
			CalcPlane(ref OutPlanes[5], 0f - ComVector[0] + RootVector[0], 0f - ComVector[1] + RootVector[1], 0f - ComVector[2] + RootVector[2], 0f - ComVector[3] + RootVector[3]);
		}

		private static void CalcPlane(ref Plane InPlane, float InA, float InB, float InC, float InDistance)
		{
			Vector3 vector = new Vector3(InA, InB, InC);
			float num = 1f / (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
			InPlane.normal = new Vector3(vector.x * num, vector.y * num, vector.z * num);
			InPlane.distance = InDistance * num;
		}
	}
}
