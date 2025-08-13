namespace MoleMole
{
	public static class SafeTypeUtil
	{
		public static byte EncryptBool(bool value)
		{
			return (byte)((value ? 1 : 0) ^ 0xCF);
		}

		public static bool DecryptBool(byte value)
		{
			return (value ^ 0xCF) != 0;
		}

		public static byte EncryptInt8(sbyte value)
		{
			return (byte)(value ^ 0xCF);
		}

		public static sbyte DecryptInt8(byte value)
		{
			return (sbyte)(value ^ 0xCF);
		}

		public static byte EncryptUInt8(byte value)
		{
			return (byte)(value ^ 0xCF);
		}

		public static byte DecryptUInt8(byte value)
		{
			return (byte)(value ^ 0xCF);
		}

		public unsafe static ushort EncryptInt16(short value)
		{
			byte* ptr = (byte*)(&value);
			*ptr ^= 0xCF;
			byte* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			return (ushort)value;
		}

		public unsafe static short DecryptInt16(ushort value)
		{
			byte* ptr = (byte*)(&value);
			byte* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			*ptr ^= 0xCF;
			return (short)value;
		}

		public unsafe static ushort EncryptUInt16(ushort value)
		{
			byte* ptr = (byte*)(&value);
			*ptr ^= 0xCF;
			byte* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			return value;
		}

		public unsafe static ushort DecryptUInt16(ushort value)
		{
			byte* ptr = (byte*)(&value);
			byte* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			*ptr ^= 0xCF;
			return value;
		}

		public unsafe static uint EncryptInt32(int value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			return (uint)value;
		}

		public unsafe static int DecryptInt32(uint value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			*ptr ^= 0xCAFE;
			return (int)value;
		}

		public unsafe static uint EncryptUInt32(uint value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			return value;
		}

		public unsafe static uint DecryptUInt32(uint value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			*ptr ^= 0xCAFE;
			return value;
		}

		public unsafe static ulong EncryptInt64(long value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= 0xCAFE;
			ushort* intPtr2 = ptr + 2;
			*intPtr2 ^= *ptr;
			ushort* intPtr3 = ptr + 3;
			*intPtr3 ^= ptr[1];
			return (ulong)value;
		}

		public unsafe static long DecryptInt64(ulong value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 2;
			*intPtr ^= *ptr;
			ushort* intPtr2 = ptr + 3;
			*intPtr2 ^= ptr[1];
			*ptr ^= 0xCAFE;
			ushort* intPtr3 = ptr + 1;
			*intPtr3 ^= 0xCAFE;
			return (long)value;
		}

		public unsafe static ulong EncryptUInt64(ulong value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= 0xCAFE;
			ushort* intPtr2 = ptr + 2;
			*intPtr2 ^= *ptr;
			ushort* intPtr3 = ptr + 3;
			*intPtr3 ^= ptr[1];
			return value;
		}

		public unsafe static ulong DecryptUInt64(ulong value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 2;
			*intPtr ^= *ptr;
			ushort* intPtr2 = ptr + 3;
			*intPtr2 ^= ptr[1];
			*ptr ^= 0xCAFE;
			ushort* intPtr3 = ptr + 1;
			*intPtr3 ^= 0xCAFE;
			return value;
		}

		public unsafe static uint EncryptFloat(float value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			return *(uint*)(&value);
		}

		public unsafe static float DecryptFloat(uint value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 1;
			*intPtr ^= *ptr;
			*ptr ^= 0xCAFE;
			return *(float*)(&value);
		}

		public unsafe static ulong EncryptDouble(double value)
		{
			ushort* ptr = (ushort*)(&value);
			*ptr ^= 0xCAFE;
			ushort* intPtr = ptr + 1;
			*intPtr ^= 0xCAFE;
			ushort* intPtr2 = ptr + 2;
			*intPtr2 ^= *ptr;
			ushort* intPtr3 = ptr + 3;
			*intPtr3 ^= ptr[1];
			return *(ulong*)(&value);
		}

		public unsafe static double DecryptDouble(ulong value)
		{
			ushort* ptr = (ushort*)(&value);
			ushort* intPtr = ptr + 2;
			*intPtr ^= *ptr;
			ushort* intPtr2 = ptr + 3;
			*intPtr2 ^= ptr[1];
			*ptr ^= 0xCAFE;
			ushort* intPtr3 = ptr + 1;
			*intPtr3 ^= 0xCAFE;
			return *(double*)(&value);
		}
	}
}
