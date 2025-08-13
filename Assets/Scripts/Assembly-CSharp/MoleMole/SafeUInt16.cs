using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeUInt16 : IComparable, IComparable<SafeUInt16>, IEquatable<SafeUInt16>
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

		public ushort Value
		{
			get
			{
				return (ushort)(((_value & 0xFF00) ^ (_value << 8)) + ((_value & 0xFF) ^ 0xCF));
			}
			set
			{
				_value = (short)(((value & 0xFF00) ^ (((value & 0xFF) ^ 0xCF) << 8)) + ((value & 0xFF) ^ 0xCF));
			}
		}

		public SafeUInt16(ushort value)
		{
			_value = (short)(((value & 0xFF00) ^ (((value & 0xFF) ^ 0xCF) << 8)) + ((value & 0xFF) ^ 0xCF));
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
			if (obj != null && obj is SafeUInt16)
			{
				return _value == ((SafeUInt16)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeUInt16)
			{
				return Value.CompareTo(((SafeUInt16)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeUInt16 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeUInt16 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeUInt16(ushort v)
		{
			return new SafeUInt16(v);
		}

		public static implicit operator ushort(SafeUInt16 v)
		{
			return v.Value;
		}

		public static SafeUInt16 operator ++(SafeUInt16 a)
		{
			return new SafeUInt16((ushort)(a.Value + 1));
		}

		public static SafeUInt16 operator --(SafeUInt16 a)
		{
			return new SafeUInt16((ushort)(a.Value - 1));
		}
	}
}
