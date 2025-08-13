using FlatBuffers;
using MoleMole.MPProtocol;
using UnityEngine;

namespace MoleMole
{
	public static class MPMiscs
	{
		public const float POSITION_MINIMAL_SYNC_DIFF = 0.1f;

		public const float ANGLE_MINIMAL_SYNC_DIFF = 0.5f;

		public static Vector3 UNITINIALIZED = new Vector3(float.MinValue, float.MinValue, float.MinValue);

		public static Vector3 XZAnglesToForward(float degrees)
		{
			return new Vector3(Mathf.Cos(degrees), 0f, Mathf.Sin(degrees));
		}

		public static float ForwardToXZAngles(Vector3 forward)
		{
			return Mathf.Atan2(forward.z, forward.x);
		}

		public static bool NeedSyncTransform(float v2sqr, float angleDiff)
		{
			if (v2sqr > 0.1f || Mathf.Abs(angleDiff) > 0.5f)
			{
				return true;
			}
			return false;
		}

		public static Vector3 Convert(MPVector2_XZ table)
		{
			return new Vector3(table.X, 0f, table.Z);
		}

		public static StringOffset CreateString(FlatBufferBuilder builder, string str)
		{
			return (str != null) ? builder.CreateString(str) : default(StringOffset);
		}
	}
}
