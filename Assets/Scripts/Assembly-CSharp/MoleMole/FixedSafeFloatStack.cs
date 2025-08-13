using System;
using UnityEngine;

namespace MoleMole
{
	public class FixedSafeFloatStack : FixedStack<SafeFloat>
	{
		private const int DEFAULT_FLOAT_STACK_CAPACITY = 12;

		private SafeFloat _value = 0f;

		private Action _updateValue;

		private float _floor;

		private float _ceiling;

		public override SafeFloat value
		{
			get
			{
				return _value;
			}
		}

		public FixedSafeFloatStack(float initial, FixedFloatStack.StackMethod valueType, float floor, float ceiling, Action<SafeFloat, int, SafeFloat, int> onChanged = null)
			: base(12, onChanged)
		{
			switch (valueType)
			{
			case FixedFloatStack.StackMethod.Top:
				_checkAnyValueChange = false;
				_updateValue = UpdateTop;
				break;
			case FixedFloatStack.StackMethod.Sum:
				_checkAnyValueChange = true;
				_updateValue = UpdateSummed;
				break;
			case FixedFloatStack.StackMethod.Multiplied:
				_checkAnyValueChange = true;
				_updateValue = UpdateMultiplied;
				break;
			case FixedFloatStack.StackMethod.OneMinusMultiplied:
				_checkAnyValueChange = true;
				initial = 1f - initial;
				_updateValue = UpdateOneMinusMultiplied;
				break;
			}
			_floor = floor;
			_ceiling = ceiling;
			base.onChanged = (Action<SafeFloat, int, SafeFloat, int>)Delegate.Combine(base.onChanged, new Action<SafeFloat, int, SafeFloat, int>(SelfOnChanged));
			Push(initial, true);
		}

		public static FixedSafeFloatStack CreateDefault(float initValue, FixedFloatStack.StackMethod stackMethod, float floor, float ceiling, Action<SafeFloat, int, SafeFloat, int> onChanged = null)
		{
			return new FixedSafeFloatStack(initValue, stackMethod, floor, ceiling, onChanged);
		}

		private void UpdateSummed()
		{
			_value = 0f;
			for (int i = 0; i < _stack.Length; i++)
			{
				if (_occupied[i])
				{
					_value = (float)_value + (float)_stack[i];
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
					_value = (float)_value * (float)_stack[i];
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
					_value = (float)_value * (1f - (float)_stack[i]);
				}
			}
		}

		private void SelfOnChanged(SafeFloat oldValue, int oldIx, SafeFloat newValue, int newIx)
		{
			_updateValue();
			_value = Mathf.Clamp(_value, _floor, _ceiling);
		}
	}
}
