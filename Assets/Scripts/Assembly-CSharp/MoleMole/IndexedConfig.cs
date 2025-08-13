using System;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public static class IndexedConfig
	{
		public static int Compare(string lhs, string rhs)
		{
			if (lhs == null && rhs == null)
			{
				return 0;
			}
			if (lhs == null)
			{
				return -1;
			}
			return lhs.CompareTo(rhs);
		}

		public static int Compare(string[] lhs, string[] rhs)
		{
			if (lhs == null && rhs == null)
			{
				return 0;
			}
			if (lhs == null)
			{
				return -1;
			}
			if (rhs == null)
			{
				return 1;
			}
			int num = lhs.Length.CompareTo(rhs.Length);
			if (num != 0)
			{
				return num;
			}
			int num2 = lhs.Length;
			for (int i = 0; i < num2; i++)
			{
				num = Compare(lhs[i], rhs[i]);
				if (num != 0)
				{
					return num;
				}
			}
			return num;
		}

		public static int Compare(DynamicFloat lhs, DynamicFloat rhs)
		{
			if (lhs == null && rhs == null)
			{
				return 0;
			}
			if (lhs == null)
			{
				return -1;
			}
			if (rhs == null)
			{
				return 1;
			}
			int num = lhs.isDynamic.CompareTo(rhs.isDynamic);
			if (num != 0)
			{
				return num;
			}
			num = Compare(lhs.dynamicKey, rhs.dynamicKey);
			if (num != 0)
			{
				return num;
			}
			num = lhs.fixedValue.CompareTo(rhs.fixedValue);
			if (num != 0)
			{
				return num;
			}
			return num;
		}
	}
	public abstract class IndexedConfig<T> : IComparable<T> where T : IndexedConfig<T>
	{
		private static List<T> _allInstancesList = new List<T>();

		public static IntMapping<T> Mapping = new IntMapping<T>();

		public IndexedConfig()
		{
			_allInstancesList.Add((T)this);
		}

		public static void InitializeMapping()
		{
			HashSet<T> hashSet = new HashSet<T>(_allInstancesList);
			for (int i = 1; i <= Mapping.length; i++)
			{
				hashSet.Add(Mapping.Get(i));
			}
			T[] array = new T[hashSet.Count];
			hashSet.CopyTo(array);
			Mapping = new IntMapping<T>(array);
			_allInstancesList.Clear();
		}

		public abstract int CompareTo(T other);

		public abstract int ContentHash();

		public override int GetHashCode()
		{
			return ContentHash();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(T))
			{
				return false;
			}
			return CompareTo((T)obj) == 0;
		}
	}
}
