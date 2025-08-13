namespace MoleMole
{
	public class SimpleLRU<T> where T : class
	{
		private T[] _arr;

		public int count;

		public T this[int ix]
		{
			get
			{
				return _arr[ix];
			}
		}

		public SimpleLRU(int count)
		{
			_arr = new T[count];
			this.count = count;
		}

		public void Touch(T entry, out T outdated)
		{
			int num = -1;
			for (int i = 0; i < count; i++)
			{
				if (_arr[i] == null)
				{
					num = i;
					break;
				}
				if (_arr[i] == entry)
				{
					outdated = (T)null;
					for (int num2 = i; num2 >= 1; num2--)
					{
						_arr[num2] = _arr[num2 - 1];
					}
					_arr[0] = entry;
					return;
				}
			}
			if (num != -1)
			{
				_arr[num] = entry;
				outdated = (T)null;
				return;
			}
			outdated = _arr[count - 1];
			for (int num3 = count - 1; num3 >= 1; num3--)
			{
				_arr[num3] = _arr[num3 - 1];
			}
			_arr[0] = entry;
		}

		public void Clear()
		{
			for (int i = 0; i < count; i++)
			{
				_arr[i] = (T)null;
			}
		}

		public void MarkClear(int ix)
		{
			_arr[ix] = (T)null;
		}

		public void Rebuild()
		{
			T[] array = new T[count];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (_arr[i] != null)
				{
					array[num++] = _arr[i];
				}
			}
			_arr = array;
		}
	}
}
