using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeChar : IComparable, IComparable<SafeChar>, IEquatable<SafeChar>
	{
		[SerializeField]
		private char _value;

		public char EncryptedValue
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

		public char Value
		{
			get
			{
				return (char)(((uint)(_value & 0xFF00) ^ ((uint)_value << 8)) + (uint)((_value & 0xFF) ^ 0xCF));
			}
			set
			{
				_value = (char)(((value & 0xFF00) ^ (((value & 0xFF) ^ 0xCF) << 8)) + ((value & 0xFF) ^ 0xCF));
			}
		}

		public SafeChar(char value)
		{
			_value = (char)(((value & 0xFF00) ^ (((value & 0xFF) ^ 0xCF) << 8)) + ((value & 0xFF) ^ 0xCF));
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
			if (obj != null && obj is SafeChar)
			{
				return _value == ((SafeChar)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeChar)
			{
				return Value.CompareTo(((SafeChar)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeChar other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeChar other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeChar(char v)
		{
			return new SafeChar(v);
		}

		public static implicit operator char(SafeChar v)
		{
			return v.Value;
		}

		public static SafeChar operator ++(SafeChar a)
		{
			return new SafeChar((char)(a.Value + 1));
		}

		public static SafeChar operator --(SafeChar a)
		{
			return new SafeChar((char)(a.Value - 1));
		}
	}
}
