using System;
using System.Collections.Generic;

namespace MoleMole
{
	public class BaseModule
	{
		public virtual bool OnPacket(NetPacketV1 packet)
		{
			return false;
		}

		protected void UpdateField<T>(bool isSpecified, ref T field, T newValue, Action<T, T> callback = null)
		{
			if (isSpecified)
			{
				UpdateField(ref field, newValue, callback);
			}
		}

		protected void UpdateField<T>(ref T field, T newValue, Action<T, T> callback = null)
		{
			T arg = field;
			field = newValue;
			if (callback != null)
			{
				callback(arg, newValue);
			}
		}

		protected List<int> ConvertList(List<uint> fromList)
		{
			List<int> list = new List<int>();
			foreach (uint from in fromList)
			{
				list.Add((int)from);
			}
			return list;
		}
	}
}
