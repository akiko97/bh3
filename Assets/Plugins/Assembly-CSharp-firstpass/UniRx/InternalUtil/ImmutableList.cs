using System;

namespace UniRx.InternalUtil
{
	public class ImmutableList<T>
	{
		private T[] data;

		public T[] Data
		{
			get
			{
				return data;
			}
		}

		public ImmutableList()
		{
			data = new T[0];
		}

		public ImmutableList(T[] data)
		{
			this.data = data;
		}

		public ImmutableList<T> Add(T value)
		{
			T[] array = new T[data.Length + 1];
			Array.Copy(data, array, data.Length);
			array[data.Length] = value;
			return new ImmutableList<T>(array);
		}

		public ImmutableList<T> Remove(T value)
		{
			int num = IndexOf(value);
			if (num < 0)
			{
				return this;
			}
			T[] destinationArray = new T[data.Length - 1];
			Array.Copy(data, 0, destinationArray, 0, num);
			Array.Copy(data, num + 1, destinationArray, num, data.Length - num - 1);
			return new ImmutableList<T>(destinationArray);
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (object.Equals(data[i], value))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
