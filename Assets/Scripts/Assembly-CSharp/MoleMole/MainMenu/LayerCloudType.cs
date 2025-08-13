using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class LayerCloudType
	{
		public string Name;

		public bool Enable = true;

		public int[] MaterialIds;

		public float EmitRate;

		[Range(0f, 1f)]
		public float Size;

		[Range(0f, 1f)]
		public float SizeDeviation;

		[Range(0f, 1f)]
		public float MeanY = 0.5f;

		[Range(0f, 1f)]
		public float StdDevY = 0.5f;

		[Range(0f, 1f)]
		public float Gap;

		[Range(0f, 1f)]
		public float GapDeviation;

		[Range(0f, 1f)]
		public float InterleavedOffset;

		private float _remainEmitCount;

		private bool _isOdd = true;

		public bool IsEmit(float deltaTime)
		{
			_remainEmitCount += EmitRate * (1f + (UnityEngine.Random.value * 2f - 1f) * 0.2f) * deltaTime;
			if (_remainEmitCount > 1f)
			{
				_remainEmitCount -= Mathf.FloorToInt(_remainEmitCount);
				return true;
			}
			return false;
		}

		public float GetRandomSize()
		{
			return Size * (1f + (UnityEngine.Random.value * 2f - 1f) * SizeDeviation);
		}

		public float GetRandomGap()
		{
			return Gap * (1f + (UnityEngine.Random.value * 2f - 1f) * GapDeviation);
		}

		public float GetRandomPositionY()
		{
			float num;
			do
			{
				num = GaussianRandom.Val(MeanY, StdDevY);
			}
			while (num < 0f || num > 1f);
			return num;
		}

		public bool IsOdd()
		{
			_isOdd = !_isOdd;
			return !_isOdd;
		}
	}
}
