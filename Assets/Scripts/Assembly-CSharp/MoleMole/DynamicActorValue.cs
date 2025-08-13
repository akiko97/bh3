using System;

namespace MoleMole
{
	public class DynamicActorValue<T> : IDisposable where T : IComparable
	{
		private T _value;

		private Action<T, T> _changeDelegate;

		public T Value
		{
			get
			{
				return _value;
			}
		}

		public DynamicActorValue(T value)
		{
			_value = value;
		}

		public void Pub(T newValue)
		{
			T value = _value;
			_value = newValue;
			if (_changeDelegate != null)
			{
				_changeDelegate(value, newValue);
			}
		}

		public void SubAttach(Action<T, T> changeCallback, ref T curValue)
		{
			_changeDelegate = (Action<T, T>)Delegate.Combine(_changeDelegate, changeCallback);
			curValue = _value;
		}

		public void SubDetach(Action<T, T> changeCallback)
		{
			_changeDelegate = (Action<T, T>)Delegate.Remove(_changeDelegate, changeCallback);
		}

		public void Dispose()
		{
			_changeDelegate = null;
		}
	}
}
