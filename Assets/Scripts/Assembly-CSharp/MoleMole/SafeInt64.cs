using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeInt64 : IComparable, IComparable<SafeInt64>, IEquatable<SafeInt64>
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

		public long Value
		{
			get
			{
				return ((_value & -4294967296L) ^ (_value << 32)) + ((_value & 0xFFFFFFFFu) ^ 0xCAFECAFEu);
			}
			set
			{
				_value = ((value & -4294967296L) ^ (((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu);
			}
		}

		public SafeInt64(long value)
		{
			_value = ((value & -4294967296L) ^ (((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((value & 0xFFFFFFFFu) ^ 0xCAFECAFEu);
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
			if (obj != null && obj is SafeInt64)
			{
				return _value == ((SafeInt64)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeInt64)
			{
				return Value.CompareTo(((SafeInt64)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeInt64 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeInt64 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeInt64(long v)
		{
			return new SafeInt64(v);
		}

		public static implicit operator long(SafeInt64 v)
		{
			return v.Value;
		}

		public static SafeInt64 operator ++(SafeInt64 a)
		{
			return new SafeInt64(a.Value + 1);
		}

		public static SafeInt64 operator --(SafeInt64 a)
		{
			return new SafeInt64(a.Value - 1);
		}
	}
}
