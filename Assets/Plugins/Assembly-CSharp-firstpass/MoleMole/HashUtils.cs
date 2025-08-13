using System;

namespace MoleMole
{
	public static class HashUtils
	{
		private static float[] _floatArr = new float[1];

		public static void TryHashObject(object obj, ref int lastHash)
		{
			IHashable hashable = obj as IHashable;
			if (hashable != null)
			{
				hashable.ObjectContentHashOnto(ref lastHash);
			}
		}

		public static void ContentHashOnto(int value, ref int lastHash)
		{
			lastHash ^= value;
		}

		public static void ContentHashOnto(float value, ref int lastHash)
		{
			_floatArr[0] = value;
			lastHash ^= Buffer.GetByte(_floatArr, 0);
			lastHash ^= Buffer.GetByte(_floatArr, 1) << 8;
			lastHash ^= Buffer.GetByte(_floatArr, 2) << 16;
			lastHash ^= Buffer.GetByte(_floatArr, 3) << 24;
		}

		public static void ContentHashOnto(string value, ref int lastHash)
		{
			if (value != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
					lastHash ^= (int)((uint)c << i % 2);
				}
			}
		}

		public static void ContentHashOnto(bool value, ref int lastHash)
		{
			lastHash ^= (value ? 1 : 0);
		}

		public static void ContentHashOnto(IHashable value, ref int lastHash)
		{
			value.ObjectContentHashOnto(ref lastHash);
		}

		public static void ContentHashOntoFallback(object obj, ref int lastHash)
		{
			if (obj is int)
			{
				ContentHashOnto((int)obj, ref lastHash);
			}
			else if (obj is bool)
			{
				ContentHashOnto((bool)obj, ref lastHash);
			}
			else if (obj is float)
			{
				ContentHashOnto((float)obj, ref lastHash);
			}
			else if (obj is string)
			{
				ContentHashOnto((string)obj, ref lastHash);
			}
			else if (obj is IHashable)
			{
				ContentHashOnto((IHashable)obj, ref lastHash);
			}
		}
	}
}
