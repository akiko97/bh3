using System;
using UnityEngine;

namespace MoleMole
{
	public class FixedFloatStack : FixedStack<float>
	{
		public enum StackMethod
		{
			Top = 0,
			Sum = 1,
			Multiplied = 2,
			OneMinusMultiplied = 3
		}

		private const int DEFAULT_FLOAT_STACK_CAPACITY = 12;

		private float _value;

		private Action _updateValue;

		private float _floor;

		private float _ceiling;

		public override float value
		{
			get
			{
				return _value;
			}
		}

		public FixedFloatStack(float initial, StackMethod valueType, float floor, float ceiling, Action<float, int, float, int> onChanged = null)
			: base(12, onChanged)
		{
			switch (valueType)
			{
			case StackMethod.Top:
				_checkAnyValueChange = false;
				_updateValue = UpdateTop;
				break;
			case StackMethod.Sum:
				_checkAnyValueChange = true;
				_updateValue = UpdateSummed;
				break;
			case StackMethod.Multiplied:
				_checkAnyValueChange = true;
				_updateValue = UpdateMultiplied;
				break;
			case StackMethod.OneMinusMultiplied:
				_checkAnyValueChange = true;
				initial = 1f - initial;
				_updateValue = UpdateOneMinusMultiplied;
				break;
			}
			_floor = floor;
			_ceiling = ceiling;
			base.onChanged = (Action<float, int, float, int>)Delegate.Combine(base.onChanged, new Action<float, int, float, int>(SelfOnChanged));
			Push(initial, true);
		}

		public static FixedFloatStack CreateDefault(float initValue, StackMethod stackMethod, float floor, float ceiling, Action<float, int, float, int> onChanged = null)
		{
			return new FixedFloatStack(initValue, stackMethod, floor, ceiling, onChanged);
		}

		private void UpdateSummed()
		{
			_value = 0f;
			for (int i = 0; i < _stack.Length; i++)
			{
				if (_occupied[i])
				{
					_value += _stack[i];
				}
			}
		}

		private void UpdateMultiplied()
		{
			_value = 1f;
			for (int i = 0; i < _stack.Length; i++)
			{
				if (_occupied[i])
				{
					_value *= _stack[i];
				}
			}
		}

		private void UpdateTop()
		{
			_value = base.value;
		}

		private void UpdateOneMinusMultiplied()
		{
			_value = 1f;
			for (int i = 0; i < _stack.Length; i++)
			{
				if (_occupied[i])
				{
					_value *= 1f - _stack[i];
				}
			}
		}

		private void SelfOnChanged(float oldValue, int oldIx, float newValue, int newIx)
		{
			_updateValue();
			_value = Mathf.Clamp(_value, _floor, _ceiling);
		}
	}
}
