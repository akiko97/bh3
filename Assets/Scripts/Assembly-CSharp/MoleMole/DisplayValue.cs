using System;

namespace MoleMole
{
	public class DisplayValue<T> : IDisposable where T : IComparable
	{
		private T _floor;

		private T _ceiling;

		private T _value;

		private Action<T, T> _changeDelegate;

		public T value
		{
			get
			{
				return _value;
			}
		}

		public DisplayValue(T floor, T ceiling, T init)
		{
			_floor = floor;
			_ceiling = ceiling;
			_value = init;
		}

		public void Pub(T newValue)
		{
			T arg = _value;
			_value = newValue;
			if (_changeDelegate != null)
			{
				_changeDelegate(arg, newValue);
			}
		}

		public void SubAttach(Action<T, T> changeCallback, ref T curValue, ref T floor, ref T ceiling)
		{
			_changeDelegate = (Action<T, T>)Delegate.Combine(_changeDelegate, changeCallback);
			curValue = _value;
			floor = _floor;
			ceiling = _ceiling;
		}

		public void SubDetach(Action<T, T> changeCalback)
		{
			_changeDelegate = (Action<T, T>)Delegate.Remove(_changeDelegate, changeCalback);
		}

		public void Dispose()
		{
			_changeDelegate = null;
		}
	}
}
