using UnityEngine;

public class GaussParamGenerator
{
	private float[] _weights;

	private Vector4[] _offsets;

	public float[] Weights
	{
		get
		{
			return _weights;
		}
	}

	public Vector4[] Offsets
	{
		get
		{
			return _offsets;
		}
	}

	public GaussParamGenerator(float s, float imageWidth)
	{
		CalcParams(s, imageWidth);
	}

	private float gaussian(float x, float s)
	{
		return Mathf.Exp((0f - s * x) * (s * x));
	}

	private float[] generateGaussianWeights(float s, out int width)
	{
		width = (int)(3f / s);
		int num = width * 2 + 1;
		float[] array = new float[num];
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			array[i] = gaussian((float)i - (float)width, s);
			num2 += array[i];
		}
		for (int j = 0; j < num; j++)
		{
			array[j] /= num2;
		}
		return array;
	}

	private void CalcParams(float s, float imageWidth)
	{
		int width;
		float[] array = generateGaussianWeights(s, out width);
		int num = 2 * width + 1;
		int num2 = Mathf.CeilToInt((float)num / 2f);
		_weights = new float[num2];
		_offsets = new Vector4[Mathf.CeilToInt((float)num2 / 2f)];
		for (int i = 0; i < _offsets.Length; i++)
		{
			_offsets[i] = Vector4.zero;
		}
		for (int j = 0; j < num2; j++)
		{
			float num3 = array[j * 2];
			float num4 = ((j * 2 + 1 <= num - 1) ? array[j * 2 + 1] : 0f);
			_weights[j] = num3 + num4;
			float num5 = num4 / (num3 + num4);
			float num6 = (float)(j * 2 - width) + num5;
			num6 /= imageWidth;
			_offsets[j / 2][j % 2 * 2] = num6;
			_offsets[j / 2][j % 2 * 2 + 1] = num6;
		}
	}
}
