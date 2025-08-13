using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeBool : IComparable, IComparable<SafeBool>, IEquatable<SafeBool>
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

		public bool Value
		{
			get
			{
				return (sbyte)(_value ^ 0xCF) != 0;
			}
			set
			{
				_value = (sbyte)((value ? ((_value + 1) | 1) : 0) ^ 0xCF);
			}
		}

		public SafeBool(bool value)
		{
			_value = (sbyte)((value ? 1 : 0) ^ 0xCF);
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
			if (obj != null && obj is SafeBool)
			{
				return Value == ((SafeBool)obj).Value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeBool)
			{
				return Value.CompareTo(((SafeBool)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeBool other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeBool other)
		{
			return Value == other.Value;
		}

		public static implicit operator SafeBool(bool v)
		{
			return new SafeBool(v);
		}

		public static implicit operator bool(SafeBool v)
		{
			return v.Value;
		}
	}
}
