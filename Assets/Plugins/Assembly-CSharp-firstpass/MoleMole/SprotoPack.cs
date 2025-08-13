using System.IO;

namespace MoleMole
{
	public class SprotoPack
	{
		private MemoryStream buffer;

		private byte[] tmp;

		public SprotoPack()
		{
			tmp = new byte[8];
		}

		private void write_ff(byte[] src, int offset, long pos, int n)
		{
			int num = (n + 7) & -8;
			long position = buffer.Position;
			buffer.Seek(pos, SeekOrigin.Begin);
			buffer.WriteByte(byte.MaxValue);
			buffer.WriteByte((byte)(num / 8 - 1));
			buffer.Write(src, offset, n);
			for (int i = 0; i < num - n; i++)
			{
				buffer.WriteByte(0);
			}
			buffer.Seek(position, SeekOrigin.Begin);
		}

		private int pack_seg(byte[] src, long offset, int ff_n)
		{
			byte b = 0;
			int num = 0;
			long position = buffer.Position;
			buffer.Seek(1L, SeekOrigin.Current);
			for (int i = 0; i < 8; i++)
			{
				if (src[offset + i] != 0)
				{
					num++;
					b |= (byte)(1 << i);
					buffer.WriteByte(src[offset + i]);
				}
			}
			if ((num == 7 || num == 6) && ff_n > 0)
			{
				num = 8;
			}
			if (num == 8)
			{
				if (ff_n > 0)
				{
					buffer.Seek(position, SeekOrigin.Begin);
					return 8;
				}
				buffer.Seek(position, SeekOrigin.Begin);
				return 10;
			}
			buffer.Seek(position, SeekOrigin.Begin);
			buffer.WriteByte(b);
			buffer.Seek(position, SeekOrigin.Begin);
			return num + 1;
		}

		public int pack(byte[] data, int len, byte[] outBuffer, int outBufferOffset)
		{
			clear();
			buffer = new MemoryStream(outBuffer);
			buffer.Position = outBufferOffset;
			byte[] array = null;
			int num = 0;
			long pos = 0L;
			int num2 = 0;
			byte[] array2 = data;
			int num3 = 0;
			int num5;
			for (int i = 0; i < len; buffer.Seek(num5, SeekOrigin.Current), i += 8)
			{
				num3 = i;
				int num4 = i + 8 - len;
				if (num4 > 0)
				{
					for (int j = 0; j < 8 - num4; j++)
					{
						tmp[j] = array2[i + j];
					}
					for (int k = 0; k < num4; k++)
					{
						tmp[7 - k] = 0;
					}
					array2 = tmp;
					num3 = 0;
				}
				num5 = pack_seg(array2, num3, num2);
				switch (num5)
				{
				case 10:
					array = array2;
					num = num3;
					pos = buffer.Position;
					num2 = 1;
					continue;
				case 8:
					if (num2 > 0)
					{
						num2++;
						if (num2 == 256)
						{
							write_ff(array, num, pos, 2048);
							num2 = 0;
						}
						continue;
					}
					break;
				}
				if (num2 > 0)
				{
					write_ff(array, num, pos, num2 * 8);
					num2 = 0;
				}
			}
			if (num2 == 1)
			{
				write_ff(array, num, pos, 8);
			}
			else if (num2 > 1)
			{
				int num6 = ((array != data) ? array.Length : len);
				write_ff(array, num, pos, num6 - num);
			}
			long num7 = (len + 2047) / 2048 * 2 + len + 2;
			if (num7 < buffer.Position)
			{
			}
			int result = (int)(buffer.Position - outBufferOffset);
			buffer.Dispose();
			return result;
		}

		public int unpack(byte[] data, int len, byte[] outBuffer, int outBufferOffset)
		{
			clear();
			buffer = new MemoryStream(outBuffer);
			buffer.Position = outBufferOffset;
			len = ((len != 0) ? len : data.Length);
			int num = len;
			while (num > 0)
			{
				byte b = data[len - num];
				num--;
				if (b == byte.MaxValue)
				{
					if (num < 0)
					{
					}
					int num2 = (data[len - num] + 1) * 8;
					if (num < num2 + 1)
					{
					}
					buffer.Write(data, len - num + 1, num2);
					num -= num2 + 1;
					continue;
				}
				for (int i = 0; i < 8; i++)
				{
					int num3 = (b >> i) & 1;
					if (num3 == 1)
					{
						if (num < 0)
						{
						}
						buffer.WriteByte(data[len - num]);
						num--;
					}
					else
					{
						buffer.WriteByte(0);
					}
				}
			}
			int result = (int)(buffer.Position - outBufferOffset);
			buffer.Dispose();
			return result;
		}

		private void clear()
		{
			for (int i = 0; i < tmp.Length; i++)
			{
				tmp[i] = 0;
			}
		}

		public int RoundUpTo8(int x)
		{
			int num = x & 7;
			return (num != 0) ? (x + 8 - num) : x;
		}
	}
}
