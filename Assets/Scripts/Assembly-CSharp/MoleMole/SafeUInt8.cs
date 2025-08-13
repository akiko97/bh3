using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeUInt8 : IComparable, IComparable<SafeUInt8>, IEquatable<SafeUInt8>
	{
		[SerializeField]
		private sbyte _value;

		public sbyte EncryptedValue
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

		public byte Value
		{
			get
			{
				return (byte)(_value ^ 0xCF);
			}
			set
			{
				_value = (sbyte)(value ^ 0xCF);
			}
		}

		public SafeUInt8(byte value)
		{
			_value = (sbyte)(value ^ 0xCF);
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
			if (obj != null && obj is SafeUInt8)
			{
				return _value == ((SafeUInt8)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeUInt8)
			{
				return Value.CompareTo(((SafeUInt8)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeUInt8 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeUInt8 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeUInt8(byte v)
		{
			return new SafeUInt8(v);
		}

		public static implicit operator byte(SafeUInt8 v)
		{
			return v.Value;
		}

		public static SafeUInt8 operator ++(SafeUInt8 a)
		{
			return new SafeUInt8((byte)(a.Value + 1));
		}

		public static SafeUInt8 operator --(SafeUInt8 a)
		{
			return new SafeUInt8((byte)(a.Value - 1));
		}
	}
}
