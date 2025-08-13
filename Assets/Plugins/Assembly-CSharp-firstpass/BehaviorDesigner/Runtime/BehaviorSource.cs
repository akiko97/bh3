using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class BehaviorSource : IVariableSource
	{
		public string behaviorName = "Behavior";

		public string behaviorDescription = string.Empty;

		private int behaviorID = -1;

		private Task mEntryTask;

		private Task mRootTask;

		private List<Task> mDetachedTasks;

		private List<SharedVariable> mVariables;

		private Dictionary<string, int> mSharedVariableIndex;

		[NonSerialized]
		private bool mHasSerialized;

		[SerializeField]
		private TaskSerializationData mTaskData;

		[SerializeField]
		private IBehavior mOwner;

		public int BehaviorID
		{
			get
			{
				return behaviorID;
			}
			set
			{
				behaviorID = value;
			}
		}

		public Task EntryTask
		{
			get
			{
				return mEntryTask;
			}
			set
			{
				mEntryTask = value;
			}
		}

		public Task RootTask
		{
			get
			{
				return mRootTask;
			}
			set
			{
				mRootTask = value;
			}
		}

		public List<Task> DetachedTasks
		{
			get
			{
				return mDetachedTasks;
			}
			set
			{
				mDetachedTasks = value;
			}
		}

		public List<SharedVariable> Variables
		{
			get
			{
				CheckForSerialization(false);
				return mVariables;
			}
			set
			{
				mVariables = value;
				UpdateVariablesIndex();
			}
		}

		public bool HasSerialized
		{
			get
			{
				return mHasSerialized;
			}
			set
			{
				mHasSerialized = value;
			}
		}

		public TaskSerializationData TaskData
		{
			get
			{
				return mTaskData;
			}
			set
			{
				mTaskData = value;
			}
		}

		public IBehavior Owner
		{
			get
			{
				return mOwner;
			}
			set
			{
				mOwner = value;
			}
		}

		public BehaviorSource()
		{
		}

		public BehaviorSource(IBehavior owner)
		{
			Initialize(owner);
		}

		public void Initialize(IBehavior owner)
		{
			mOwner = owner;
		}

		public void Save(Task entryTask, Task rootTask, List<Task> detachedTasks)
		{
			mEntryTask = entryTask;
			mRootTask = rootTask;
			mDetachedTasks = detachedTasks;
		}

		public void Load(out Task entryTask, out Task rootTask, out List<Task> detachedTasks)
		{
			entryTask = mEntryTask;
			rootTask = mRootTask;
			detachedTasks = mDetachedTasks;
		}

		public bool CheckForSerialization(bool force, BehaviorSource behaviorSource = null)
		{
			if (!((behaviorSource == null) ? HasSerialized : behaviorSource.HasSerialized) || force)
			{
				if (behaviorSource != null)
				{
					behaviorSource.HasSerialized = true;
				}
				else
				{
					HasSerialized = true;
				}
				if (mTaskData != null && !string.IsNullOrEmpty(mTaskData.JSONSerialization))
				{
					DeserializeJSON.Load(mTaskData, (behaviorSource != null) ? behaviorSource : this);
				}
				else
				{
					BinaryDeserialization.Load(mTaskData, (behaviorSource != null) ? behaviorSource : this);
				}
				return true;
			}
			return false;
		}

		public SharedVariable GetVariable(string name)
		{
			if (name == null)
			{
				return null;
			}
			if (mVariables != null)
			{
				if (mSharedVariableIndex == null || mSharedVariableIndex.Count != mVariables.Count)
				{
					UpdateVariablesIndex();
				}
				int value;
				if (mSharedVariableIndex.TryGetValue(name, out value))
				{
					return mVariables[value];
				}
			}
			return null;
		}

		public List<SharedVariable> GetAllVariables()
		{
			return mVariables;
		}

		public void SetVariable(string name, SharedVariable sharedVariable)
		{
			if (mVariables == null)
			{
				mVariables = new List<SharedVariable>();
			}
			else if (mSharedVariableIndex == null)
			{
				UpdateVariablesIndex();
			}
			sharedVariable.Name = name;
			int value;
			if (mSharedVariableIndex != null && mSharedVariableIndex.TryGetValue(name, out value))
			{
				SharedVariable sharedVariable2 = mVariables[value];
				if (!sharedVariable2.GetType().Equals(typeof(SharedVariable)) && !sharedVariable2.GetType().Equals(sharedVariable.GetType()))
				{
					Debug.LogError(string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", name, sharedVariable2.GetType(), sharedVariable.GetType()));
				}
				else
				{
					sharedVariable2.SetValue(sharedVariable.GetValue());
				}
			}
			else
			{
				mVariables.Add(sharedVariable);
				UpdateVariablesIndex();
			}
		}

		public void UpdateVariableName(SharedVariable sharedVariable, string name)
		{
			CheckForSerialization(false);
			sharedVariable.Name = name;
			UpdateVariablesIndex();
		}

		public void SetAllVariables(List<SharedVariable> variables)
		{
			mVariables = variables;
		}

		private void UpdateVariablesIndex()
		{
			if (mVariables == null)
			{
				if (mSharedVariableIndex != null)
				{
					mSharedVariableIndex = null;
				}
				return;
			}
			if (mSharedVariableIndex == null)
			{
				mSharedVariableIndex = new Dictionary<string, int>(mVariables.Count);
			}
			else
			{
				mSharedVariableIndex.Clear();
			}
			for (int i = 0; i < mVariables.Count; i++)
			{
				if (mVariables[i] != null)
				{
					mSharedVariableIndex.Add(mVariables[i].Name, i);
				}
			}
		}

		public override string ToString()
		{
			if (mOwner == null)
			{
				return behaviorName;
			}
			return string.Format("{0} - {1}", Owner.GetOwnerName(), behaviorName);
		}
	}
}
