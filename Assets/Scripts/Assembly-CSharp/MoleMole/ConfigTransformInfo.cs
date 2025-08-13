using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class ConfigTransformInfo
	{
		public List<float> Pos;

		public List<float> Euler;

		public Vector3 Position
		{
			get
			{
				return new Vector3(Pos[0], Pos[1], Pos[2]);
			}
		}

		public Vector3 EulerAngle
		{
			get
			{
				return new Vector3(Euler[0], Euler[1], Euler[2]);
			}
		}
	}
}
