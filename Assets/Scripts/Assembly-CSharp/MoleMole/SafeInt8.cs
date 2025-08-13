using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeInt8 : IComparable, IComparable<SafeInt8>, IEquatable<SafeInt8>
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

		public sbyte Value
		{
			get
			{
				return (sbyte)(_value ^ 0xCF);
			}
			set
			{
				_value = (sbyte)(value ^ 0xCF);
			}
		}

		public SafeInt8(sbyte value)
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
			if (obj != null && obj is SafeInt8)
			{
				return _value == ((SafeInt8)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeInt8)
			{
				return Value.CompareTo(((SafeInt8)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeInt8 other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeInt8 other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeInt8(sbyte v)
		{
			return new SafeInt8(v);
		}

		public static implicit operator sbyte(SafeInt8 v)
		{
			return v.Value;
		}

		public static SafeInt8 operator ++(SafeInt8 a)
		{
			return new SafeInt8((sbyte)(a.Value + 1));
		}

		public static SafeInt8 operator --(SafeInt8 a)
		{
			return new SafeInt8((sbyte)(a.Value - 1));
		}
	}
}
