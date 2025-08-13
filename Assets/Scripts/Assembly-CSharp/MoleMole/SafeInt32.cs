using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeInt32 : IComparable, IComparable<SafeInt32>, IEquatable<SafeInt32>
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

		public int Value
		{
			get
			{
				return (int)(((_value & 0xFFFF0000u) ^ (_value << 16)) + ((_value & 0xFFFF) ^ 0xCAFE));
			}
			set
			{
				_value = ((value & -65536) ^ (((value & 0xFFFF) ^ 0xCAFE) << 16)) + ((value & 0xFFFF) ^ 0xCAFE);
			}
		}

		public SafeInt32(int value)
		{
			_value = ((value & -65536) ^ (((value & 0xFFFF) ^ 0xCAFE) << 16)) + ((value & 0xFFFF) ^ 0xCAFE);
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
			if (obj != null && obj is SafeInt32)
			{
				return _value == ((SafeInt32)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeInt32)
			{
				return Value.CompareTo(((SafeInt32)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeInt32 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeInt32 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeInt32(int v)
		{
			return new SafeInt32(v);
		}

		public static implicit operator int(SafeInt32 v)
		{
			return v.Value;
		}

		public static explicit operator uint(SafeInt32 v)
		{
			return (uint)v.Value;
		}

		public static explicit operator float(SafeInt32 v)
		{
			return v.Value;
		}

		public static explicit operator double(SafeInt32 v)
		{
			return v.Value;
		}

		public static SafeInt32 operator ++(SafeInt32 a)
		{
			return new SafeInt32(a.Value + 1);
		}

		public static SafeInt32 operator --(SafeInt32 a)
		{
			return new SafeInt32(a.Value - 1);
		}
	}
}
