using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class SharedSafeInt : SharedInt
	{
		[SerializeField]
		private SafeInt32 mSafeValue = 0;

		public override int Value
		{
			get
			{
				return mSafeValue.Value;
			}
			set
			{
				bool flag = Value == value;
				mSafeValue = value;
				if (flag)
				{
					ValueChanged();
				}
			}
		}

		public override object GetValue()
		{
			return Value;
		}

		public override void SetValue(object value)
		{
			mSafeValue = (int)value;
		}

		public override string ToString()
		{
			return mSafeValue.ToString();
		}

		public static implicit operator SharedSafeInt(int value)
		{
			SharedSafeInt sharedSafeInt = new SharedSafeInt();
			sharedSafeInt.mSafeValue = value;
			return sharedSafeInt;
		}
	}
}
