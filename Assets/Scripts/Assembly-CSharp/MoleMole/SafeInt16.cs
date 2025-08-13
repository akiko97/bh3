using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeInt16 : IComparable, IComparable<SafeInt16>, IEquatable<SafeInt16>
	{
		[SerializeField]
		private short _value;

		public short EncryptedValue
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

		public short Value
		{
			get
			{
				return (short)(((_value & 0xFF00) ^ (_value << 8)) + ((_value & 0xFF) ^ 0xCF));
			}
			set
			{
				_value = (short)((((ushort)value & 0xFF00) ^ ((((ushort)value & 0xFF) ^ 0xCF) << 8)) + (((ushort)value & 0xFF) ^ 0xCF));
			}
		}

		public SafeInt16(short value)
		{
			_value = (short)((((ushort)value & 0xFF00) ^ ((((ushort)value & 0xFF) ^ 0xCF) << 8)) + (((ushort)value & 0xFF) ^ 0xCF));
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
			if (obj != null && obj is SafeInt16)
			{
				return _value == ((SafeInt16)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeInt16)
			{
				return Value.CompareTo(((SafeInt16)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeInt16 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeInt16 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeInt16(short v)
		{
			return new SafeInt16(v);
		}

		public static implicit operator short(SafeInt16 v)
		{
			return v.Value;
		}

		public static SafeInt16 operator ++(SafeInt16 a)
		{
			return new SafeInt16((short)(a.Value + 1));
		}

		public static SafeInt16 operator --(SafeInt16 a)
		{
			return new SafeInt16((short)(a.Value - 1));
		}
	}
}
