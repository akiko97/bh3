using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	public class GlobalVariables : ScriptableObject, IVariableSource
	{
		private static GlobalVariables instance;

		[SerializeField]
		private List<SharedVariable> mVariables;

		private Dictionary<string, int> mSharedVariableIndex;

		[SerializeField]
		private VariableSerializationData mVariableData;

		public static GlobalVariables Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("BehaviorDesignerGlobalVariables", typeof(GlobalVariables)) as GlobalVariables;
					if (instance != null)
					{
						instance.CheckForSerialization(false);
					}
				}
				return instance;
			}
		}

		public List<SharedVariable> Variables
		{
			get
			{
				return mVariables;
			}
			set
			{
				mVariables = value;
				UpdateVariablesIndex();
			}
		}

		public VariableSerializationData VariableData
		{
			get
			{
				return mVariableData;
			}
			set
			{
				mVariableData = value;
			}
		}

		public void CheckForSerialization(bool force)
		{
			if (force || mVariables == null || (mVariables.Count > 0 && mVariables[0] == null))
			{
				if (VariableData != null && !string.IsNullOrEmpty(VariableData.JSONSerialization))
				{
					DeserializeJSON.Load(VariableData.JSONSerialization, this);
				}
				else
				{
					BinaryDeserialization.Load(this);
				}
			}
		}

		public SharedVariable GetVariable(string name)
		{
			if (name == null)
			{
				return null;
			}
			CheckForSerialization(false);
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
			CheckForSerialization(false);
			return mVariables;
		}

		public void SetVariable(string name, SharedVariable sharedVariable)
		{
			CheckForSerialization(false);
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

		public void SetVariableValue(string name, object value)
		{
			SharedVariable variable = GetVariable(name);
			if (variable != null)
			{
				variable.SetValue(value);
				variable.ValueChanged();
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
	}
}
