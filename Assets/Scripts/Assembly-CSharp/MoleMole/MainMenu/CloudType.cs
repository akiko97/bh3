using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class CloudType
	{
		public string Name;

		public bool Enable = true;

		public int[] MaterialIds;

		public float EmitRate;

		public int EmitCount;

		[Range(0f, 1f)]
		public float Size;

		[Range(0f, 1f)]
		public float SizeDeviation;

		[Range(0f, 1f)]
		public float MeanX = 0.5f;

		[Range(0f, 1f)]
		public float StdDevX = 0.5f;

		[Range(0f, 1f)]
		public float MeanY = 0.5f;

		[Range(0f, 1f)]
		public float StdDevY = 0.5f;

		private float _RemainEmitCount;

		public int GetEmitCount(float deltaTime)
		{
			_RemainEmitCount += EmitRate * (1f + (UnityEngine.Random.value * 2f - 1f) * 0.2f) * deltaTime;
			int num = Mathf.FloorToInt(_RemainEmitCount);
			_RemainEmitCount -= num;
			return num;
		}

		public float GetRandomSize()
		{
			return Size * (1f + (UnityEngine.Random.value * 2f - 1f) * SizeDeviation);
		}

		public Vector2 GetRandomPosition()
		{
			float num;
			do
			{
				num = GaussianRandom.Val(MeanX, StdDevX);
			}
			while (num < 0f || num > 1f);
			float num2;
			do
			{
				num2 = GaussianRandom.Val(MeanY, StdDevY);
			}
			while (num2 < 0f || num2 > 1f);
			return new Vector2(num, num2);
		}
	}
}
