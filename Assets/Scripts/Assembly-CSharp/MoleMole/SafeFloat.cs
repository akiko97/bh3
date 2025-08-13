using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public struct SafeFloat : IComparable, IComparable<SafeFloat>, IEquatable<SafeFloat>
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

		public unsafe float Value
		{
			get
			{
				long num = ((_value & 0xFFFF0000u) ^ (_value << 16)) + ((_value & 0xFFFF) ^ 0xCAFE);
				return *(float*)(&num);
			}
			set
			{
				uint num = *(uint*)(&value);
				_value = (int)(((num & 0xFFFF0000u) ^ (((num & 0xFFFF) ^ 0xCAFE) << 16)) + ((num & 0xFFFF) ^ 0xCAFE));
			}
		}

		public unsafe SafeFloat(float value)
		{
			uint num = *(uint*)(&value);
			_value = (int)(((num & 0xFFFF0000u) ^ (((num & 0xFFFF) ^ 0xCAFE) << 16)) + ((num & 0xFFFF) ^ 0xCAFE));
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
			if (obj != null && obj is SafeFloat)
			{
				return _value == ((SafeFloat)obj)._value;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is SafeFloat)
			{
				return Value.CompareTo(((SafeFloat)obj).Value);
			}
			throw new ArgumentException("Invalid type!");
		}

		public int CompareTo(SafeFloat other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(SafeFloat other)
		{
			return _value == other._value;
		}

		public static implicit operator SafeFloat(float v)
		{
			return new SafeFloat(v);
		}

		public static implicit operator float(SafeFloat v)
		{
			return v.Value;
		}

		public static explicit operator double(SafeFloat v)
		{
			return v.Value;
		}

		public static explicit operator int(SafeFloat v)
		{
			return (int)v.Value;
		}

		public static explicit operator uint(SafeFloat v)
		{
			return (uint)v.Value;
		}

		public static SafeFloat operator ++(SafeFloat a)
		{
			return new SafeFloat(a.Value + 1f);
		}

		public static SafeFloat operator --(SafeFloat a)
		{
			return new SafeFloat(a.Value - 1f);
		}
	}
}
