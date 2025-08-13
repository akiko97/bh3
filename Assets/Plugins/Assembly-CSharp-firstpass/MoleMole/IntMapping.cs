using System.Collections.Generic;

namespace MoleMole
{
	public class IntMapping<T> where T : class
	{
		private static T[] EMPTY_T_ARR = new T[0];

		private T[] _intToValue;

		private Dictionary<T, int> _valueToInt;

		public int length
		{
			get
			{
				return _intToValue.Length - 1;
			}
		}

		public IntMapping()
			: this(EMPTY_T_ARR)
		{
		}

		public IntMapping(T[] arr)
		{
			_intToValue = new T[arr.Length + 1];
			_valueToInt = new Dictionary<T, int>(arr.Length);
			arr.CopyTo(_intToValue, 1);
			int num = 1;
			for (int i = 0; i < arr.Length; i++)
			{
				_valueToInt.Add(arr[i], num++);
			}
		}

		public int TryGet(T value)
		{
			if (value == null)
			{
				return 0;
			}
			int value2 = 0;
			_valueToInt.TryGetValue(value, out value2);
			return value2;
		}

		public T TryGet(int index)
		{
			if (index <= 0 && index >= _intToValue.Length)
			{
				return (T)null;
			}
			return Get(index);
		}

		public int Get(T value)
		{
			return _valueToInt[value];
		}

		public T Get(int index)
		{
			return _intToValue[index];
		}
	}
}
