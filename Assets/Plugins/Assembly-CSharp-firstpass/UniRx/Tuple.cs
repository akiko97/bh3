using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public static class Tuple
	{
		public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8));
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
		}

		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
		{
			return new Tuple<T1, T2, T3>(item1, item2, item3);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1> Create<T1>(T1 item1)
		{
			return new Tuple<T1>(item1);
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		private T7 item7;

		private TRest rest;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}

		public T5 Item5
		{
			get
			{
				return item5;
			}
		}

		public T6 Item6
		{
			get
			{
				return item6;
			}
		}

		public T7 Item7
		{
			get
			{
				return item7;
			}
		}

		public TRest Rest
		{
			get
			{
				return rest;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
			this.item7 = item7;
			this.rest = rest;
			if (!(rest is ITuple))
			{
				throw new ArgumentException("rest", "The last element of an eight element tuple must be a Tuple.");
			}
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item6, tuple.item6);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item7, tuple.item7);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(rest, tuple.rest);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6) && comparer.Equals(item7, tuple.item7) && comparer.Equals(rest, tuple.rest);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4);
			hashCode = ((hashCode << 5) + hashCode) ^ hashCode2;
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6);
			int hashCode3 = comparer.GetHashCode(item7);
			hashCode3 = ((hashCode3 << 5) + hashCode3) ^ comparer.GetHashCode(rest);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ hashCode3;
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", item1, item2, item3, item4, item5, item6, item7, ((ITuple)(object)rest).ToString());
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public Tuple(T1 item1)
		{
			this.item1 = item1;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1> tuple = other as Tuple<T1>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			return comparer.Compare(item1, tuple.item1);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1> tuple = other as Tuple<T1>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return comparer.GetHashCode(item1);
		}

		string ITuple.ToString()
		{
			return string.Format("{0}", item1);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public Tuple(T1 item1, T2 item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item2, tuple.item2);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}", item1, item2);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3> tuple = other as Tuple<T1, T2, T3>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item3, tuple.item3);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3> tuple = other as Tuple<T1, T2, T3>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item3);
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}", item1, item2, item3);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3, T4> tuple = other as Tuple<T1, T2, T3, T4>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item4, tuple.item4);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3, T4> tuple = other as Tuple<T1, T2, T3, T4>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4);
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}", item1, item2, item3, item4);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}

		public T5 Item5
		{
			get
			{
				return item5;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5> tuple = other as Tuple<T1, T2, T3, T4, T5>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item5, tuple.item5);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5> tuple = other as Tuple<T1, T2, T3, T4, T5>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4);
			hashCode = ((hashCode << 5) + hashCode) ^ hashCode2;
			return ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item5);
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}", item1, item2, item3, item4, item5);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}

		public T5 Item5
		{
			get
			{
				return item5;
			}
		}

		public T6 Item6
		{
			get
			{
				return item6;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6> tuple = other as Tuple<T1, T2, T3, T4, T5, T6>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item6, tuple.item6);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6> tuple = other as Tuple<T1, T2, T3, T4, T5, T6>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4);
			hashCode = ((hashCode << 5) + hashCode) ^ hashCode2;
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6);
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", item1, item2, item3, item4, item5, item6);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IComparable, IStructuralEquatable, IStructuralComparable, ITuple
	{
		private T1 item1;

		private T2 item2;

		private T3 item3;

		private T4 item4;

		private T5 item5;

		private T6 item6;

		private T7 item7;

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}

		public T5 Item5
		{
			get
			{
				return item5;
			}
		}

		public T6 Item6
		{
			get
			{
				return item6;
			}
		}

		public T7 Item7
		{
			get
			{
				return item7;
			}
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
			this.item7 = item7;
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IStructuralComparable)this).CompareTo(obj, (IComparer)Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
			if (tuple == null)
			{
				if (other == null)
				{
					return 1;
				}
				throw new ArgumentException("other");
			}
			int num = comparer.Compare(item1, tuple.item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item2, tuple.item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item3, tuple.item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item4, tuple.item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item5, tuple.item5);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(item6, tuple.item6);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(item7, tuple.item7);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
			if (tuple == null)
			{
				return false;
			}
			return comparer.Equals(item1, tuple.item1) && comparer.Equals(item2, tuple.item2) && comparer.Equals(item3, tuple.item3) && comparer.Equals(item4, tuple.item4) && comparer.Equals(item5, tuple.item5) && comparer.Equals(item6, tuple.item6) && comparer.Equals(item7, tuple.item7);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			int hashCode = comparer.GetHashCode(item1);
			hashCode = ((hashCode << 5) + hashCode) ^ comparer.GetHashCode(item2);
			int hashCode2 = comparer.GetHashCode(item3);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item4);
			hashCode = ((hashCode << 5) + hashCode) ^ hashCode2;
			hashCode2 = comparer.GetHashCode(item5);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item6);
			hashCode2 = ((hashCode2 << 5) + hashCode2) ^ comparer.GetHashCode(item7);
			return ((hashCode << 5) + hashCode) ^ hashCode2;
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", item1, item2, item3, item4, item5, item6, item7);
		}

		public override bool Equals(object obj)
		{
			return ((IStructuralEquatable)this).Equals(obj, (IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode((IEqualityComparer)EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}
	}
}
