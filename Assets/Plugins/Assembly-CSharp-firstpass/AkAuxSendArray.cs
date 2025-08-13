using System;
using System.Runtime.InteropServices;

public class AkAuxSendArray
{
	public IntPtr m_Buffer;

	private IntPtr m_Current;

	private uint m_MaxCount;

	public uint m_Count;

	public AkAuxSendArray(uint in_Count)
	{
		m_Buffer = Marshal.AllocHGlobal((int)(in_Count * 8));
		m_Current = m_Buffer;
		m_MaxCount = in_Count;
		m_Count = 0u;
	}

	~AkAuxSendArray()
	{
		Marshal.FreeHGlobal(m_Buffer);
		m_Buffer = IntPtr.Zero;
	}

	public void Reset()
	{
		m_Current = m_Buffer;
		m_Count = 0u;
	}

	public void Add(uint in_EnvID, float in_fValue)
	{
		if (m_Count >= m_MaxCount)
		{
			Resize(m_Count * 2);
		}
		Marshal.WriteInt32(m_Current, (int)in_EnvID);
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		m_Count++;
	}

	public void Resize(uint in_size)
	{
		if (in_size <= m_Count)
		{
			m_Count = in_size;
			return;
		}
		m_MaxCount = in_size;
		IntPtr intPtr = Marshal.AllocHGlobal((int)(m_MaxCount * 8));
		IntPtr ptr = m_Buffer;
		m_Current = intPtr;
		for (int i = 0; i < m_Count; i++)
		{
			Marshal.WriteInt32(m_Current, Marshal.ReadInt32(ptr));
			m_Current = (IntPtr)(m_Current.ToInt64() + 4);
			ptr = (IntPtr)(ptr.ToInt64() + 4);
			Marshal.WriteInt32(m_Current, Marshal.ReadInt32(ptr));
			m_Current = (IntPtr)(m_Current.ToInt64() + 4);
			ptr = (IntPtr)(ptr.ToInt64() + 4);
		}
		Marshal.FreeHGlobal(m_Buffer);
		m_Buffer = intPtr;
	}

	public void Remove(uint in_EnvID)
	{
		IntPtr ptr = m_Buffer;
		for (int i = 0; i < m_Count; i++)
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				IntPtr ptr2 = (IntPtr)(m_Buffer.ToInt64() + (m_Count - 1) * 8);
				Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
				ptr = (IntPtr)(ptr.ToInt64() + 4);
				ptr2 = (IntPtr)(ptr2.ToInt64() + 4);
				Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
				m_Count--;
				break;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4 + 4);
		}
	}

	public bool Contains(uint in_EnvID)
	{
		IntPtr ptr = m_Buffer;
		for (int i = 0; i < m_Count; i++)
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				return true;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4 + 4);
		}
		return false;
	}

	public int OffsetOf(uint in_EnvID)
	{
		int result = -1;
		IntPtr ptr = m_Buffer;
		for (int i = 0; i < m_Count; i++)
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				result = ptr.ToInt32() - m_Buffer.ToInt32();
				break;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4 + 4);
		}
		return result;
	}

	public void RemoveAt(int in_offset)
	{
		IntPtr ptr = (IntPtr)(m_Buffer.ToInt64() + in_offset);
		IntPtr ptr2 = (IntPtr)(m_Buffer.ToInt64() + (m_Count - 1) * 8);
		Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		ptr2 = (IntPtr)(ptr2.ToInt64() + 4);
		Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
		m_Count--;
	}

	public void ReplaceAt(int in_offset, uint in_EnvID, float in_fValue)
	{
		IntPtr ptr = (IntPtr)(m_Buffer.ToInt64() + in_offset);
		Marshal.WriteInt32(ptr, (int)in_EnvID);
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		Marshal.WriteInt32(ptr, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
	}
}
