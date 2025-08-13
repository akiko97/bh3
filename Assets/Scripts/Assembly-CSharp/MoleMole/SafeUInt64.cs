using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeUInt64 : IComparable, IComparable<SafeUInt64>, IEquatable<SafeUInt64>
	{
		[SerializeField]
		private long _value;

		public long EncryptedValue
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public ulong Value
		{
			get
			{
				return (ulong)(((_value & -4294967296L) ^ (_value << 32)) + ((_value & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
			}
			set
			{
				_value = (long)(((value & 0xFFFFFFFF00000000uL) ^ (((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
			}
		}

		public SafeUInt64(ulong value)
		{
			_value = (long)(((value & 0xFFFFFFFF00000000uL) ^ (((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is SafeUInt64)
			{
				return _value == ((SafeUInt64)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeUInt64)
			{
				return Value.CompareTo(((SafeUInt64)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeUInt64 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeUInt64 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeUInt64(ulong v)
		{
			return new SafeUInt64(v);
		}

		public static implicit operator ulong(SafeUInt64 v)
		{
			return v.Value;
		}

		public static SafeUInt64 operator ++(SafeUInt64 a)
		{
			return new SafeUInt64(a.Value + 1);
		}

		public static SafeUInt64 operator --(SafeUInt64 a)
		{
			return new SafeUInt64(a.Value - 1);
		}
	}
}
