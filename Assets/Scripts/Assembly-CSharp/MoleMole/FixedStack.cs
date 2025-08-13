using System;
using FullInspector;

namespace MoleMole
{
	public class FixedStack<T>
	{
		protected const int DEFAULT_CAPACITY = 6;

		protected T[] _stack;

		protected bool[] _occupied;

		protected int _pushTop;

		protected int _realTop;

		public Action<T, int, T, int> onChanged;

		protected bool _checkAnyValueChange;

		[ShowInInspector]
		public virtual T value
		{
			get
			{
				return _stack[_realTop];
			}
		}

		public FixedStack(int capacity, Action<T, int, T, int> onChanged = null)
		{
			_stack = new T[capacity];
			_occupied = new bool[capacity];
			_pushTop = -1;
			_realTop = -1;
			this.onChanged = onChanged;
		}

		public static FixedStack<T> CreateDefault(T initValue, Action<T, int, T, int> onChanged = null)
		{
			FixedStack<T> fixedStack = new FixedStack<T>(6, onChanged);
			fixedStack.Push(initValue, true);
			return fixedStack;
		}

		private void OnChanged(T oldValue, int oldStackIx, T newValue, int newStackIx)
		{
			if (onChanged != null)
			{
				onChanged(oldValue, oldStackIx, newValue, newStackIx);
			}
		}

		private void SeekPushPos(int startAt = 0)
		{
			for (int i = startAt; i < _stack.Length; i++)
			{
				if (!_occupied[i])
				{
					_pushTop = i;
					break;
				}
			}
		}

		private void SeekRealTop(bool silent = false)
		{
			int realTop = _realTop;
			T oldValue = ((realTop < 0) ? default(T) : _stack[realTop]);
			for (int num = _stack.Length - 1; num >= 0; num--)
			{
				if (_occupied[num])
				{
					_realTop = num;
					if (!silent && (_checkAnyValueChange || realTop != _realTop))
					{
						OnChanged(oldValue, realTop, _stack[_realTop], _realTop);
					}
					return;
				}
			}
			if (!silent || _checkAnyValueChange)
			{
				OnChanged(oldValue, realTop, default(T), -1);
			}
		}

		public int Push(T value, bool silent = false)
		{
			SeekPushPos();
			_stack[_pushTop] = value;
			_occupied[_pushTop] = true;
			SeekRealTop(silent);
			return _pushTop;
		}

		public void Push(int ix, T value, bool silent = false)
		{
			_stack[ix] = value;
			_occupied[ix] = true;
			SeekRealTop(silent);
		}

		public int PushAbove(int aboveIx, T value, bool silent = false)
		{
			SeekPushPos(aboveIx + 1);
			_stack[_pushTop] = value;
			_occupied[_pushTop] = true;
			SeekRealTop(silent);
			return _pushTop;
		}

		public void TryPop(int ix)
		{
			if (_occupied[ix])
			{
				Pop(ix);
			}
		}

		public void Pop(int ix)
		{
			_occupied[ix] = false;
			SeekPushPos();
			SeekRealTop();
		}

		public void Set(int ix, T value, bool silent = false)
		{
			T oldValue = _stack[ix];
			_stack[ix] = value;
			if (!silent && (_checkAnyValueChange || ix == _realTop))
			{
				OnChanged(oldValue, ix, value, ix);
			}
		}

		public T Get(int ix)
		{
			return _stack[ix];
		}

		public bool IsOccupied(int ix)
		{
			return _occupied[ix];
		}

		public int GetRealTopIndex()
		{
			return _realTop;
		}
	}
}
