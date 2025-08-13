using System;

namespace MoleMole
{
	internal class NetUtils
	{
		public static ushort HostToNetworkOrder(ushort num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			bytes[0] = (byte)((num >> 8) & 0xFF);
			bytes[1] = (byte)(num & 0xFF);
			return BitConverter.ToUInt16(bytes, 0);
		}

		public static uint HostToNetworkOrder(uint num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			bytes[0] = (byte)((num >> 24) & 0xFF);
			bytes[1] = (byte)((num >> 16) & 0xFF);
			bytes[2] = (byte)((num >> 8) & 0xFF);
			bytes[3] = (byte)(num & 0xFF);
			return BitConverter.ToUInt32(bytes, 0);
		}

		public static ulong HostToNetworkOrder(ulong num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			bytes[0] = (byte)((num >> 56) & 0xFF);
			bytes[1] = (byte)((num >> 48) & 0xFF);
			bytes[2] = (byte)((num >> 40) & 0xFF);
			bytes[3] = (byte)((num >> 32) & 0xFF);
			bytes[4] = (byte)((num >> 24) & 0xFF);
			bytes[5] = (byte)((num >> 16) & 0xFF);
			bytes[6] = (byte)((num >> 8) & 0xFF);
			bytes[7] = (byte)(num & 0xFF);
			return BitConverter.ToUInt64(bytes, 0);
		}

		public static ushort NetworkToHostOrder(ushort num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			ushort num2 = 0;
			num2 = bytes[0];
			return (ushort)((num2 << 8) | bytes[1]);
		}

		public static uint NetworkToHostOrder(uint num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			uint num2 = 0u;
			num2 = bytes[0];
			num2 = (num2 << 8) | bytes[1];
			num2 = (num2 << 8) | bytes[2];
			return (num2 << 8) | bytes[3];
		}

		public static ulong NetworkToHostOrder(ulong num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			ulong num2 = 0uL;
			num2 = bytes[0];
			num2 = (num2 << 8) | bytes[1];
			num2 = (num2 << 8) | bytes[2];
			num2 = (num2 << 8) | bytes[3];
			num2 = (num2 << 8) | bytes[4];
			num2 = (num2 << 8) | bytes[5];
			num2 = (num2 << 8) | bytes[6];
			return (num2 << 8) | bytes[7];
		}
	}
}
