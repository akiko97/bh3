using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
	public sealed class CompositeDisposable : IDisposable, IEnumerable, ICollection<IDisposable>, IEnumerable<IDisposable>, ICancelable
	{
		private const int SHRINK_THRESHOLD = 64;

		private readonly object _gate = new object();

		private bool _disposed;

		private List<IDisposable> _disposables;

		private int _count;

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return _disposed;
			}
		}

		public CompositeDisposable()
		{
			_disposables = new List<IDisposable>();
		}

		public CompositeDisposable(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			_disposables = new List<IDisposable>(capacity);
		}

		public CompositeDisposable(params IDisposable[] disposables)
		{
			if (disposables == null)
			{
				throw new ArgumentNullException("disposables");
			}
			_disposables = new List<IDisposable>(disposables);
			_count = _disposables.Count;
		}

		public CompositeDisposable(IEnumerable<IDisposable> disposables)
		{
			if (disposables == null)
			{
				throw new ArgumentNullException("disposables");
			}
			_disposables = new List<IDisposable>(disposables);
			_count = _disposables.Count;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(IDisposable item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = false;
			lock (_gate)
			{
				flag = _disposed;
				if (!_disposed)
				{
					_disposables.Add(item);
					_count++;
				}
			}
			if (flag)
			{
				item.Dispose();
			}
		}

		public bool Remove(IDisposable item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			bool flag = false;
			lock (_gate)
			{
				if (!_disposed)
				{
					int num = _disposables.IndexOf(item);
					if (num >= 0)
					{
						flag = true;
						_disposables[num] = null;
						_count--;
						if (_disposables.Capacity > 64 && _count < _disposables.Capacity / 2)
						{
							List<IDisposable> disposables = _disposables;
							_disposables = new List<IDisposable>(_disposables.Capacity / 2);
							foreach (IDisposable item2 in disposables)
							{
								if (item2 != null)
								{
									_disposables.Add(item2);
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				item.Dispose();
			}
			return flag;
		}

		public void Dispose()
		{
			IDisposable[] array = null;
			lock (_gate)
			{
				if (!_disposed)
				{
					_disposed = true;
					array = _disposables.ToArray();
					_disposables.Clear();
					_count = 0;
				}
			}
			if (array == null)
			{
				return;
			}
			IDisposable[] array2 = array;
			foreach (IDisposable disposable in array2)
			{
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Clear()
		{
			IDisposable[] array = null;
			lock (_gate)
			{
				array = _disposables.ToArray();
				_disposables.Clear();
				_count = 0;
			}
			IDisposable[] array2 = array;
			foreach (IDisposable disposable in array2)
			{
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public bool Contains(IDisposable item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			lock (_gate)
			{
				return _disposables.Contains(item);
			}
		}

		public void CopyTo(IDisposable[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex >= array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			lock (_gate)
			{
				List<IDisposable> list = new List<IDisposable>();
				foreach (IDisposable item in list)
				{
					if (item != null)
					{
						list.Add(item);
					}
				}
				Array.Copy(list.ToArray(), 0, array, arrayIndex, array.Length - arrayIndex);
			}
		}

		public IEnumerator<IDisposable> GetEnumerator()
		{
			List<IDisposable> list = new List<IDisposable>();
			lock (_gate)
			{
				foreach (IDisposable disposable in _disposables)
				{
					if (disposable != null)
					{
						list.Add(disposable);
					}
				}
			}
			return list.GetEnumerator();
		}
	}
}
