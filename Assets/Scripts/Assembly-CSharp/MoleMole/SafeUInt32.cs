using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeUInt32 : IComparable, IComparable<SafeUInt32>, IEquatable<SafeUInt32>
	{
		[SerializeField]
		private int _value;

		public int EncryptedValue
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

		public uint Value
		{
			get
			{
				return (uint)(((_value & 0xFFFF0000u) ^ (_value << 16)) + ((_value & 0xFFFF) ^ 0xCAFE));
			}
			set
			{
				_value = (int)(((value & 0xFFFF0000u) ^ (((value & 0xFFFF) ^ 0xCAFE) << 16)) + ((value & 0xFFFF) ^ 0xCAFE));
			}
		}

		public SafeUInt32(uint value)
		{
			_value = (int)(((value & 0xFFFF0000u) ^ (((value & 0xFFFF) ^ 0xCAFE) << 16)) + ((value & 0xFFFF) ^ 0xCAFE));
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
			if (obj != null && obj is SafeUInt32)
			{
				return _value == ((SafeUInt32)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeUInt32)
			{
				return Value.CompareTo(((SafeUInt32)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeUInt32 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeUInt32 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeUInt32(uint v)
		{
			return new SafeUInt32(v);
		}

		public static implicit operator uint(SafeUInt32 v)
		{
			return v.Value;
		}

		public static explicit operator int(SafeUInt32 v)
		{
			return (int)v.Value;
		}

		public static explicit operator float(SafeUInt32 v)
		{
			return v.Value;
		}

		public static explicit operator double(SafeUInt32 v)
		{
			return v.Value;
		}

		public static SafeUInt32 operator ++(SafeUInt32 a)
		{
			return new SafeUInt32(a.Value + 1);
		}

		public static SafeUInt32 operator --(SafeUInt32 a)
		{
			return new SafeUInt32(a.Value - 1);
		}
	}
}
