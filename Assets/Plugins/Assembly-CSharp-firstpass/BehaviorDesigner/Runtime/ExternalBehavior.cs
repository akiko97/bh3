using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public abstract class ExternalBehavior : ScriptableObject, IBehavior
	{
		[SerializeField]
		private BehaviorSource mBehaviorSource;

		public BehaviorSource BehaviorSource
		{
			get
			{
				return mBehaviorSource;
			}
			set
			{
				mBehaviorSource = value;
			}
		}

		public BehaviorSource GetBehaviorSource()
		{
			return mBehaviorSource;
		}

		public void SetBehaviorSource(BehaviorSource behaviorSource)
		{
			mBehaviorSource = behaviorSource;
		}

		public UnityEngine.Object GetObject()
		{
			return this;
		}

		public string GetOwnerName()
		{
			return base.name;
		}

		public SharedVariable GetVariable(string name)
		{
			mBehaviorSource.CheckForSerialization(false);
			return mBehaviorSource.GetVariable(name);
		}

		public void SetVariable(string name, SharedVariable item)
		{
			mBehaviorSource.CheckForSerialization(false);
			mBehaviorSource.SetVariable(name, item);
		}

		public void SetVariableValue(string name, object value)
		{
			SharedVariable variable = GetVariable(name);
			if (variable != null)
			{
				variable.SetValue(value);
				variable.ValueChanged();
			}
		}

		 int IBehavior.GetInstanceID()
		{
			return GetInstanceID();
		}
	}
}
