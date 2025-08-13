using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	public abstract class SharedVariable
	{
		[SerializeField]
		private bool mIsShared;

		[SerializeField]
		private bool mIsGlobal;

		[SerializeField]
		private string mName;

		[SerializeField]
		private string mPropertyMapping;

		[SerializeField]
		private bool mNetworkSync;

		public bool IsShared
		{
			get
			{
				return mIsShared;
			}
			set
			{
				mIsShared = value;
			}
		}

		public bool IsGlobal
		{
			get
			{
				return mIsGlobal;
			}
			set
			{
				mIsGlobal = value;
			}
		}

		public string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}

		public string PropertyMapping
		{
			get
			{
				return mPropertyMapping;
			}
			set
			{
				mPropertyMapping = value;
			}
		}

		public bool NetworkSync
		{
			get
			{
				return mNetworkSync;
			}
			set
			{
				mNetworkSync = value;
			}
		}

		public bool IsNone
		{
			get
			{
				return mIsShared && string.IsNullOrEmpty(mName);
			}
		}

		public void ValueChanged()
		{
		}

		public virtual void InitializePropertyMapping(BehaviorSource behaviorSource)
		{
		}

		public abstract object GetValue();

		public abstract void SetValue(object value);
	}
	public abstract class SharedVariable<T> : SharedVariable
	{
		protected Func<T> mGetter;

		protected Action<T> mSetter;

		[SerializeField]
		protected T mValue;

		public virtual T Value
		{
			get
			{
				return (mGetter == null) ? mValue : mGetter();
			}
			set
			{
				bool flag = !object.Equals(Value, value);
				if (mSetter != null)
				{
					mSetter(value);
				}
				else
				{
					mValue = value;
				}
				if (flag)
				{
					ValueChanged();
				}
			}
		}

		public override void InitializePropertyMapping(BehaviorSource behaviorSource)
		{
			if (!Application.isPlaying || !(behaviorSource.Owner.GetObject() is Behavior))
			{
				return;
			}
			GameObject gameObject = (behaviorSource.Owner.GetObject() as Behavior).gameObject;
			if (string.IsNullOrEmpty(base.PropertyMapping))
			{
				return;
			}
			string[] array = base.PropertyMapping.Split('/');
			Component component = gameObject.GetComponent(TaskUtility.GetTypeWithinAssembly(array[0]));
			Type type = component.GetType();
			PropertyInfo property = type.GetProperty(array[1]);
			if (property != null)
			{
				MethodInfo getMethod = property.GetGetMethod();
				if (getMethod != null)
				{
					mGetter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), component, getMethod);
				}
				getMethod = property.GetSetMethod();
				if (getMethod != null)
				{
					mSetter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), component, getMethod);
				}
			}
		}

		public override object GetValue()
		{
			return Value;
		}

		public override void SetValue(object value)
		{
			if (mSetter != null)
			{
				mSetter((T)value);
			}
			else
			{
				mValue = (T)value;
			}
		}

		public void SetValue(T value)
		{
			if (mSetter != null)
			{
				mSetter(value);
			}
			else
			{
				mValue = value;
			}
		}
	}
}
