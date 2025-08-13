using System;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class SharedSafeFloat : SharedFloat
	{
		[SerializeField]
		private SafeFloat mSafeValue = 0f;

		public override float Value
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
			mSafeValue = (float)value;
		}

		public override string ToString()
		{
			return mSafeValue.ToString();
		}

		public static implicit operator SharedSafeFloat(float value)
		{
			SharedSafeFloat sharedSafeFloat = new SharedSafeFloat();
			sharedSafeFloat.mSafeValue = value;
			return sharedSafeFloat;
		}
	}
}
