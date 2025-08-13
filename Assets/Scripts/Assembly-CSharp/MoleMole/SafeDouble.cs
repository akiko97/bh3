using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeDouble : IComparable, IComparable<SafeDouble>, IEquatable<SafeDouble>
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

		public unsafe double Value
		{
			get
			{
				ulong num = (ulong)(((_value & -4294967296L) ^ (_value << 32)) + ((_value & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
				return *(double*)(&num);
			}
			set
			{
				ulong num = *(ulong*)(&value);
				_value = (long)(((num & 0xFFFFFFFF00000000uL) ^ (((num & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((num & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
			}
		}

		public unsafe SafeDouble(double value)
		{
			ulong num = *(ulong*)(&value);
			_value = (long)(((num & 0xFFFFFFFF00000000uL) ^ (((num & 0xFFFFFFFFu) ^ 0xCAFECAFEu) << 32)) + ((num & 0xFFFFFFFFu) ^ 0xCAFECAFEu));
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
			if (obj != null && obj is SafeDouble)
			{
				return _value == ((SafeDouble)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeDouble)
			{
				return Value.CompareTo(((SafeDouble)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeDouble other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeDouble other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeDouble(double v)
		{
			return new SafeDouble(v);
		}

		public static implicit operator double(SafeDouble v)
		{
			return v.Value;
		}

		public static explicit operator float(SafeDouble v)
		{
			return (float)v.Value;
		}

		public static SafeDouble operator ++(SafeDouble a)
		{
			return new SafeDouble(a.Value + 1.0);
		}

		public static SafeDouble operator --(SafeDouble a)
		{
			return new SafeDouble(a.Value - 1.0);
		}
	}
}
