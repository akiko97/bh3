using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MoleMole
{
	public class NetPacketV1
	{
		private uint head_magic_;

		private ushort packet_version_;

		private ushort client_version_;

		private uint time_;

		private uint user_id_;

		private uint user_ip_;

		private uint user_session_id_;

		private uint gateway_ip_;

		private ushort cmd_id_;

		private uint body_len_;

		private ushort sign_type_;

		private uint sign_;

		private MemoryStream body_;

		private uint tail_magic_;

		public NetPacketV1()
		{
			head_magic_ = NetUtils.HostToNetworkOrder(19088743u);
			packet_version_ = NetUtils.HostToNetworkOrder(1);
			client_version_ = 0;
			time_ = 0u;
			user_id_ = 0u;
			user_ip_ = 0u;
			user_session_id_ = 0u;
			gateway_ip_ = 0u;
			cmd_id_ = 0;
			body_len_ = 0u;
			sign_type_ = 0;
			sign_ = 0u;
			body_ = new MemoryStream();
			tail_magic_ = NetUtils.HostToNetworkOrder(2309737967u);
		}

		public uint getHeadMagic()
		{
			return NetUtils.NetworkToHostOrder(head_magic_);
		}

		public ushort getPacketVersion()
		{
			return NetUtils.NetworkToHostOrder(packet_version_);
		}

		public ushort getClientVersion()
		{
			return NetUtils.NetworkToHostOrder(client_version_);
		}

		public uint getTime()
		{
			return NetUtils.NetworkToHostOrder(time_);
		}

		public uint getUserId()
		{
			return NetUtils.NetworkToHostOrder(user_id_);
		}

		public ushort getCmdId()
		{
			return NetUtils.NetworkToHostOrder(cmd_id_);
		}

		public uint getBodyLen()
		{
			return NetUtils.NetworkToHostOrder(body_len_);
		}

		public ushort getSignType()
		{
			return NetUtils.NetworkToHostOrder(sign_type_);
		}

		public uint getSign()
		{
			return NetUtils.NetworkToHostOrder(sign_);
		}

		public T getData<T>()
		{
			if (body_.Length == 0L)
			{
				return default(T);
			}
			try
			{
				body_.Position = 0L;
				return (T)Singleton<NetworkManager>.Instance.serializer.Deserialize(body_, null, typeof(T));
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public uint getTailMagic()
		{
			return NetUtils.NetworkToHostOrder(tail_magic_);
		}

		public int getPacketLen()
		{
			return (int)(40 + getBodyLen() + 4);
		}

		public void setClientVersion(ushort client_version)
		{
			client_version_ = NetUtils.HostToNetworkOrder(client_version);
		}

		public void setTime(uint time)
		{
			time_ = NetUtils.HostToNetworkOrder(time);
		}

		public void setUserId(uint user_id)
		{
			user_id_ = NetUtils.HostToNetworkOrder(user_id);
		}

		public void setCmdId(ushort cmd_id)
		{
			cmd_id_ = NetUtils.HostToNetworkOrder(cmd_id);
		}

		private void setBodyLen()
		{
			body_len_ = NetUtils.HostToNetworkOrder((uint)body_.Length);
		}

		private void setSign()
		{
			sign_ = NetUtils.HostToNetworkOrder(Crc32Utils.crc32(body_));
		}

		public bool setData<T>(T data)
		{
			if (data == null)
			{
				return false;
			}
			try
			{
				body_.SetLength(0L);
				body_.Position = 0L;
				Singleton<NetworkManager>.Instance.serializer.Serialize(body_, data);
			}
			catch (Exception)
			{
				return false;
			}
			setBodyLen();
			setSign();
			return true;
		}

		public bool serialize(ref MemoryStream ms)
		{
			if (ms == null)
			{
				return false;
			}
			if (body_ == null)
			{
				return false;
			}
			ms.SetLength(0L);
			ms.Position = 0L;
			ms.Write(BitConverter.GetBytes(head_magic_), 0, Marshal.SizeOf(head_magic_));
			ms.Write(BitConverter.GetBytes(packet_version_), 0, Marshal.SizeOf(packet_version_));
			ms.Write(BitConverter.GetBytes(client_version_), 0, Marshal.SizeOf(client_version_));
			ms.Write(BitConverter.GetBytes(time_), 0, Marshal.SizeOf(time_));
			ms.Write(BitConverter.GetBytes(user_id_), 0, Marshal.SizeOf(user_id_));
			ms.Write(BitConverter.GetBytes(user_ip_), 0, Marshal.SizeOf(user_ip_));
			ms.Write(BitConverter.GetBytes(user_session_id_), 0, Marshal.SizeOf(user_session_id_));
			ms.Write(BitConverter.GetBytes(gateway_ip_), 0, Marshal.SizeOf(gateway_ip_));
			ms.Write(BitConverter.GetBytes(cmd_id_), 0, Marshal.SizeOf(cmd_id_));
			ms.Write(BitConverter.GetBytes(body_len_), 0, Marshal.SizeOf(body_len_));
			ms.Write(BitConverter.GetBytes(sign_type_), 0, Marshal.SizeOf(sign_type_));
			ms.Write(BitConverter.GetBytes(sign_), 0, Marshal.SizeOf(sign_));
			body_.WriteTo(ms);
			ms.Write(BitConverter.GetBytes(tail_magic_), 0, Marshal.SizeOf(tail_magic_));
			return true;
		}

		public PacketStatus deserialize(ref byte[] buf)
		{
			body_.SetLength(0L);
			body_.Position = 0L;
			if (buf == null)
			{
				return PacketStatus.PACKET_NOT_CORRECT;
			}
			if (buf.Length < 44)
			{
				return PacketStatus.PACKET_NOT_COMPLETE;
			}
			int num = 0;
			head_magic_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(head_magic_);
			packet_version_ = BitConverter.ToUInt16(buf, num);
			num += Marshal.SizeOf(packet_version_);
			client_version_ = BitConverter.ToUInt16(buf, num);
			num += Marshal.SizeOf(client_version_);
			time_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(time_);
			user_id_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(user_id_);
			user_ip_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(user_ip_);
			user_session_id_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(user_session_id_);
			gateway_ip_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(gateway_ip_);
			cmd_id_ = BitConverter.ToUInt16(buf, num);
			num += Marshal.SizeOf(cmd_id_);
			body_len_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(body_len_);
			sign_type_ = BitConverter.ToUInt16(buf, num);
			num += Marshal.SizeOf(sign_type_);
			sign_ = BitConverter.ToUInt32(buf, num);
			num += Marshal.SizeOf(sign_);
			if (getHeadMagic() != 19088743)
			{
				return PacketStatus.PACKET_NOT_CORRECT;
			}
			if (buf.Length < getPacketLen())
			{
				return PacketStatus.PACKET_NOT_COMPLETE;
			}
			tail_magic_ = BitConverter.ToUInt32(buf, num + (int)getBodyLen());
			if (getTailMagic() != 2309737967u)
			{
				return PacketStatus.PACKET_NOT_CORRECT;
			}
			body_.Write(buf, num, (int)getBodyLen());
			body_.Position = 0L;
			return PacketStatus.PACKET_CORRECT;
		}
	}
}
