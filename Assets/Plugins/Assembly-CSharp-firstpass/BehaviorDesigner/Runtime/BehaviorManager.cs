using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[AddComponentMenu("Behavior Designer/Behavior Manager")]
	public class BehaviorManager : MonoBehaviour
	{
		public enum ExecutionsPerTickType
		{
			NoDuplicates = 0,
			Count = 1
		}

		public class BehaviorTree
		{
			public class ConditionalReevaluate
			{
				public int index;

				public TaskStatus taskStatus;

				public int compositeIndex = -1;

				public int stackIndex = -1;

				public void Initialize(int i, TaskStatus status, int stack, int composite)
				{
					index = i;
					taskStatus = status;
					stackIndex = stack;
					compositeIndex = composite;
				}
			}

			public List<Task> taskList = new List<Task>();

			public List<int> parentIndex = new List<int>();

			public List<List<int>> childrenIndex = new List<List<int>>();

			public List<int> relativeChildIndex = new List<int>();

			public List<Stack<int>> activeStack = new List<Stack<int>>();

			public List<TaskStatus> nonInstantTaskStatus = new List<TaskStatus>();

			public List<int> interruptionIndex = new List<int>();

			public List<ConditionalReevaluate> conditionalReevaluate = new List<ConditionalReevaluate>();

			public Dictionary<int, ConditionalReevaluate> conditionalReevaluateMap = new Dictionary<int, ConditionalReevaluate>();

			public List<int> parentReevaluate = new List<int>();

			public List<int> parentCompositeIndex = new List<int>();

			public List<List<int>> childConditionalIndex = new List<List<int>>();

			public int executionCount;

			public Behavior behavior;

			public void Initialize(Behavior b)
			{
				behavior = b;
				for (int num = childrenIndex.Count - 1; num > -1; num--)
				{
					ObjectPool.Return(childrenIndex[num]);
				}
				for (int num2 = activeStack.Count - 1; num2 > -1; num2--)
				{
					ObjectPool.Return(activeStack[num2]);
				}
				for (int num3 = childConditionalIndex.Count - 1; num3 > -1; num3--)
				{
					ObjectPool.Return(childConditionalIndex[num3]);
				}
				taskList.Clear();
				parentIndex.Clear();
				childrenIndex.Clear();
				relativeChildIndex.Clear();
				activeStack.Clear();
				nonInstantTaskStatus.Clear();
				interruptionIndex.Clear();
				conditionalReevaluate.Clear();
				conditionalReevaluateMap.Clear();
				parentReevaluate.Clear();
				parentCompositeIndex.Clear();
				childConditionalIndex.Clear();
			}
		}

		public enum ThirdPartyObjectType
		{
			PlayMaker = 0,
			uScript = 1,
			DialogueSystem = 2,
			uSequencer = 3,
			ICode = 4
		}

		public class ThirdPartyTask
		{
			private Task task;

			private ThirdPartyObjectType thirdPartyObjectType;

			public Task Task
			{
				get
				{
					return task;
				}
				set
				{
					task = value;
				}
			}

			public ThirdPartyObjectType ThirdPartyObjectType
			{
				get
				{
					return thirdPartyObjectType;
				}
			}

			public void Initialize(Task t, ThirdPartyObjectType objectType)
			{
				task = t;
				thirdPartyObjectType = objectType;
			}
		}

		public class ThirdPartyTaskComparer : IEqualityComparer<ThirdPartyTask>
		{
			public bool Equals(ThirdPartyTask a, ThirdPartyTask b)
			{
				if (object.ReferenceEquals(a, null))
				{
					return false;
				}
				if (object.ReferenceEquals(b, null))
				{
					return false;
				}
				return a.Task.Equals(b.Task);
			}

			public int GetHashCode(ThirdPartyTask obj)
			{
				return (obj != null) ? obj.Task.GetHashCode() : 0;
			}
		}

		public class TaskAddData
		{
			public class InheritedFieldValue
			{
				private object value;

				private int depth;

				public object Value
				{
					get
					{
						return value;
					}
				}

				public int Depth
				{
					get
					{
						return depth;
					}
				}

				public void Initialize(object v, int d)
				{
					value = v;
					depth = d;
				}
			}

			public bool fromExternalTask;

			public ParentTask parentTask;

			public int parentIndex = -1;

			public int depth;

			public int compositeParentIndex = -1;

			public Vector2 offset;

			public Dictionary<string, InheritedFieldValue> inheritedFields;

			public int errorTask = -1;

			public string errorTaskName = string.Empty;

			public void Initialize()
			{
				if (inheritedFields != null)
				{
					foreach (KeyValuePair<string, InheritedFieldValue> inheritedField in inheritedFields)
					{
						ObjectPool.Return(inheritedField);
					}
				}
				ObjectPool.Return(inheritedFields);
				fromExternalTask = false;
				parentTask = null;
				parentIndex = -1;
				depth = 0;
				compositeParentIndex = -1;
				inheritedFields = null;
			}
		}

		public delegate void TaskBreakpointHandler();

		public static BehaviorManager instance;

		[SerializeField]
		private UpdateIntervalType updateInterval;

		[SerializeField]
		private float updateIntervalSeconds;

		[SerializeField]
		private ExecutionsPerTickType executionsPerTick;

		[SerializeField]
		private int maxTaskExecutionsPerTick = 100;

		private WaitForSeconds updateWait;

		private List<BehaviorTree> behaviorTrees = new List<BehaviorTree>();

		private Dictionary<Behavior, BehaviorTree> pausedBehaviorTrees = new Dictionary<Behavior, BehaviorTree>();

		private Dictionary<Behavior, BehaviorTree> behaviorTreeMap = new Dictionary<Behavior, BehaviorTree>();

		private List<int> conditionalParentIndexes = new List<int>();

		private Dictionary<object, ThirdPartyTask> objectTaskMap = new Dictionary<object, ThirdPartyTask>();

		private Dictionary<ThirdPartyTask, object> taskObjectMap = new Dictionary<ThirdPartyTask, object>(new ThirdPartyTaskComparer());

		private ThirdPartyTask thirdPartyTaskCompare = new ThirdPartyTask();

		private static MethodInfo playMakerStopMethod;

		private static MethodInfo uScriptStopMethod;

		private static MethodInfo dialogueSystemStopMethod;

		private static MethodInfo uSequencerStopMethod;

		private static MethodInfo iCodeStopMethod;

		private static object[] invokeParameters;

		private bool atBreakpoint;

		private bool dirty;

		public UpdateIntervalType UpdateInterval
		{
			get
			{
				return updateInterval;
			}
			set
			{
				updateInterval = value;
				UpdateIntervalChanged();
			}
		}

		public float UpdateIntervalSeconds
		{
			get
			{
				return updateIntervalSeconds;
			}
			set
			{
				updateIntervalSeconds = value;
				UpdateIntervalChanged();
			}
		}

		public ExecutionsPerTickType ExecutionsPerTick
		{
			get
			{
				return executionsPerTick;
			}
			set
			{
				executionsPerTick = value;
			}
		}

		public int MaxTaskExecutionsPerTick
		{
			get
			{
				return maxTaskExecutionsPerTick;
			}
			set
			{
				maxTaskExecutionsPerTick = value;
			}
		}

		public List<BehaviorTree> BehaviorTrees
		{
			get
			{
				return behaviorTrees;
			}
		}

		private static MethodInfo PlayMakerStopMethod
		{
			get
			{
				if (playMakerStopMethod == null)
				{
					playMakerStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_PlayMaker").GetMethod("StopPlayMaker");
				}
				return playMakerStopMethod;
			}
		}

		private static MethodInfo UScriptStopMethod
		{
			get
			{
				if (uScriptStopMethod == null)
				{
					uScriptStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_uScript").GetMethod("StopuScript");
				}
				return uScriptStopMethod;
			}
		}

		private static MethodInfo DialogueSystemStopMethod
		{
			get
			{
				if (dialogueSystemStopMethod == null)
				{
					dialogueSystemStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_DialogueSystem").GetMethod("StopDialogueSystem");
				}
				return dialogueSystemStopMethod;
			}
		}

		private static MethodInfo USequencerStopMethod
		{
			get
			{
				if (uSequencerStopMethod == null)
				{
					uSequencerStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_uSequencer").GetMethod("StopuSequencer");
				}
				return uSequencerStopMethod;
			}
		}

		private static MethodInfo ICodeStopMethod
		{
			get
			{
				if (iCodeStopMethod == null)
				{
					iCodeStopMethod = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.BehaviorManager_ICode").GetMethod("StopICode");
				}
				return iCodeStopMethod;
			}
		}

		public bool AtBreakpoint
		{
			get
			{
				return atBreakpoint;
			}
			set
			{
				atBreakpoint = value;
			}
		}

		public bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
			}
		}

		public event TaskBreakpointHandler onTaskBreakpoint;

		public void Awake()
		{
			instance = this;
			UpdateIntervalChanged();
		}

		private void UpdateIntervalChanged()
		{
			if (updateInterval == UpdateIntervalType.EveryFrame || updateInterval == UpdateIntervalType.SpecifySeconds)
			{
				base.enabled = true;
			}
			else
			{
				base.enabled = false;
			}
			foreach (BehaviorTree behaviorTree in behaviorTrees)
			{
				behaviorTree.behavior.UpdateInterval = updateInterval;
			}
		}

		public void OnDestroy()
		{
			for (int num = behaviorTrees.Count - 1; num > -1; num--)
			{
				DisableBehavior(behaviorTrees[num].behavior);
			}
		}

		public void OnApplicationQuit()
		{
			for (int num = behaviorTrees.Count - 1; num > -1; num--)
			{
				DisableBehavior(behaviorTrees[num].behavior);
			}
		}

		public void EnableBehavior(Behavior behavior)
		{
			if (IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree value;
			if (pausedBehaviorTrees.TryGetValue(behavior, out value))
			{
				behaviorTrees.Add(value);
				pausedBehaviorTrees.Remove(behavior);
				behavior.ExecutionStatus = TaskStatus.Running;
				for (int i = 0; i < value.taskList.Count; i++)
				{
					value.taskList[i].OnPause(false);
				}
				return;
			}
			TaskAddData taskAddData = ObjectPool.Get<TaskAddData>();
			taskAddData.Initialize();
			behavior.CheckForSerialization();
			Task rootTask = behavior.GetBehaviorSource().RootTask;
			if (rootTask == null)
			{
				Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains no root task. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
				return;
			}
			value = ObjectPool.Get<BehaviorTree>();
			value.Initialize(behavior);
			value.parentIndex.Add(-1);
			value.relativeChildIndex.Add(-1);
			value.parentCompositeIndex.Add(-1);
			bool hasExternalBehavior = behavior.ExternalBehavior != null;
			int num = AddToTaskList(value, rootTask, ref hasExternalBehavior, taskAddData);
			if (num < 0)
			{
				value = null;
				switch (num)
				{
				case -1:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a parent task ({2} (index {3})) with no children. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name, taskAddData.errorTaskName, taskAddData.errorTask));
					break;
				case -2:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" cannot find the referenced external task. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
					break;
				case -3:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a null task (referenced from parent task {2} (index {3})). This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name, taskAddData.errorTaskName, taskAddData.errorTask));
					break;
				case -4:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains multiple external behavior trees at the root task or as a child of a parent task which cannot contain so many children (such as a decorator task). This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name));
					break;
				case -5:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a Behavior Tree Reference task ({2} (index {3})) that which has an element with a null value in the externalBehaviors array. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name, taskAddData.errorTaskName, taskAddData.errorTask));
					break;
				case -6:
					Debug.LogError(string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a root task which is disabled. This behavior will be disabled.", behavior.GetBehaviorSource().behaviorName, behavior.gameObject.name, taskAddData.errorTaskName, taskAddData.errorTask));
					break;
				}
				return;
			}
			dirty = true;
			if (behavior.ExternalBehavior != null)
			{
				behavior.GetBehaviorSource().EntryTask = behavior.ExternalBehavior.BehaviorSource.EntryTask;
			}
			behavior.GetBehaviorSource().RootTask = value.taskList[0];
			if (behavior.ResetValuesOnRestart)
			{
				behavior.SaveResetValues();
			}
			Stack<int> stack = ObjectPool.Get<Stack<int>>();
			stack.Clear();
			value.activeStack.Add(stack);
			value.interruptionIndex.Add(-1);
			value.nonInstantTaskStatus.Add(TaskStatus.Inactive);
			if (value.behavior.LogTaskChanges)
			{
				for (int j = 0; j < value.taskList.Count; j++)
				{
					Debug.Log(string.Format("{0}: Task {1} ({2}, index {3}) {4}", RoundedTime(), value.taskList[j].FriendlyName, value.taskList[j].GetType(), j, value.taskList[j].GetHashCode()));
				}
			}
			for (int k = 0; k < value.taskList.Count; k++)
			{
				value.taskList[k].OnAwake();
			}
			behaviorTrees.Add(value);
			behaviorTreeMap.Add(behavior, value);
			if (!value.taskList[0].NodeData.Disabled)
			{
				value.behavior.OnBehaviorStarted();
				behavior.ExecutionStatus = TaskStatus.Running;
				PushTask(value, 0, 0);
			}
		}

		private int AddToTaskList(BehaviorTree behaviorTree, Task task, ref bool hasExternalBehavior, TaskAddData data)
		{
			if (task == null)
			{
				return -3;
			}
			task.GameObject = behaviorTree.behavior.gameObject;
			task.Transform = behaviorTree.behavior.transform;
			task.Owner = behaviorTree.behavior;
			if (task is BehaviorReference)
			{
				BehaviorSource[] array = null;
				BehaviorReference behaviorReference = task as BehaviorReference;
				if (behaviorReference == null)
				{
					return -2;
				}
				ExternalBehavior[] array2 = null;
				if ((array2 = behaviorReference.GetExternalBehaviors()) == null)
				{
					return -2;
				}
				array = new BehaviorSource[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] == null)
					{
						data.errorTask = behaviorTree.taskList.Count;
						data.errorTaskName = (string.IsNullOrEmpty(task.FriendlyName) ? task.GetType().ToString() : task.FriendlyName);
						return -5;
					}
					array[i] = array2[i].BehaviorSource;
					array[i].Owner = array2[i];
				}
				if (array == null)
				{
					return -2;
				}
				ParentTask parentTask = data.parentTask;
				int parentIndex = data.parentIndex;
				int compositeParentIndex = data.compositeParentIndex;
				Vector2 offset = (data.offset = task.NodeData.Offset);
				data.depth++;
				for (int j = 0; j < array.Length; j++)
				{
					BehaviorSource behaviorSource = ObjectPool.Get<BehaviorSource>();
					behaviorSource.Initialize(array[j].Owner);
					array[j].CheckForSerialization(true, behaviorSource);
					Task rootTask = behaviorSource.RootTask;
					if (rootTask != null)
					{
						if (rootTask is ParentTask)
						{
							rootTask.NodeData.Collapsed = (task as BehaviorReference).collapsed;
						}
						if (behaviorReference.variables != null)
						{
							for (int k = 0; k < behaviorReference.variables.Length; k++)
							{
								if (data.inheritedFields == null)
								{
									data.inheritedFields = ObjectPool.Get<Dictionary<string, TaskAddData.InheritedFieldValue>>();
									data.inheritedFields.Clear();
								}
								if (!data.inheritedFields.ContainsKey(behaviorReference.variables[k].Value.name))
								{
									TaskAddData.InheritedFieldValue inheritedFieldValue = ObjectPool.Get<TaskAddData.InheritedFieldValue>();
									inheritedFieldValue.Initialize(behaviorReference.variables[k].Value, data.depth);
									data.inheritedFields.Add(behaviorReference.variables[k].Value.name, inheritedFieldValue);
								}
							}
						}
						if (behaviorSource.Variables != null)
						{
							for (int l = 0; l < behaviorSource.Variables.Count; l++)
							{
								if (behaviorReference.variables != null)
								{
									bool flag = false;
									for (int m = 0; m < behaviorReference.variables.Length; m++)
									{
										if (behaviorSource.Variables[l].Name.Equals(behaviorReference.variables[m].Value.name))
										{
											flag = true;
											break;
										}
									}
									if (flag)
									{
										continue;
									}
								}
								SharedVariable sharedVariable = null;
								if ((sharedVariable = behaviorTree.behavior.GetVariable(behaviorSource.Variables[l].Name)) == null)
								{
									sharedVariable = behaviorSource.Variables[l];
									behaviorTree.behavior.SetVariable(sharedVariable.Name, sharedVariable);
								}
								if (data.inheritedFields == null)
								{
									data.inheritedFields = ObjectPool.Get<Dictionary<string, TaskAddData.InheritedFieldValue>>();
									data.inheritedFields.Clear();
								}
								if (!data.inheritedFields.ContainsKey(sharedVariable.Name))
								{
									TaskAddData.InheritedFieldValue inheritedFieldValue2 = ObjectPool.Get<TaskAddData.InheritedFieldValue>();
									inheritedFieldValue2.Initialize(sharedVariable, data.depth);
									data.inheritedFields.Add(sharedVariable.Name, inheritedFieldValue2);
								}
							}
						}
						ObjectPool.Return(behaviorSource);
						FieldInfo[] allFields = TaskUtility.GetAllFields(task.GetType());
						for (int n = 0; n < allFields.Length; n++)
						{
							if (!TaskUtility.HasAttribute(allFields[n], typeof(InheritedFieldAttribute)))
							{
								continue;
							}
							if (data.inheritedFields == null)
							{
								data.inheritedFields = ObjectPool.Get<Dictionary<string, TaskAddData.InheritedFieldValue>>();
								data.inheritedFields.Clear();
							}
							if (data.inheritedFields.ContainsKey(allFields[n].Name))
							{
								continue;
							}
							TaskAddData.InheritedFieldValue inheritedFieldValue3 = ObjectPool.Get<TaskAddData.InheritedFieldValue>();
							if (allFields[n].FieldType.IsSubclassOf(typeof(SharedVariable)))
							{
								SharedVariable sharedVariable2 = allFields[n].GetValue(task) as SharedVariable;
								if (sharedVariable2.IsShared)
								{
									SharedVariable variable = behaviorTree.behavior.GetVariable(sharedVariable2.Name);
									inheritedFieldValue3.Initialize(variable, data.depth);
									data.inheritedFields.Add(allFields[n].Name, inheritedFieldValue3);
								}
								else
								{
									inheritedFieldValue3.Initialize(sharedVariable2, data.depth);
									data.inheritedFields.Add(allFields[n].Name, inheritedFieldValue3);
								}
							}
							else
							{
								inheritedFieldValue3.Initialize(allFields[n].GetValue(task), data.depth);
								data.inheritedFields.Add(allFields[n].Name, inheritedFieldValue3);
							}
						}
						if (j > 0)
						{
							data.parentTask = parentTask;
							data.parentIndex = parentIndex;
							data.compositeParentIndex = compositeParentIndex;
							data.offset = offset;
							if (data.parentTask == null || j >= data.parentTask.MaxChildren())
							{
								return -4;
							}
							behaviorTree.parentIndex.Add(data.parentIndex);
							behaviorTree.relativeChildIndex.Add(data.parentTask.Children.Count);
							behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
							behaviorTree.childrenIndex[data.parentIndex].Add(behaviorTree.taskList.Count);
							data.parentTask.AddChild(rootTask, data.parentTask.Children.Count);
						}
						hasExternalBehavior = true;
						bool fromExternalTask = data.fromExternalTask;
						data.fromExternalTask = true;
						int num = 0;
						if ((num = AddToTaskList(behaviorTree, rootTask, ref hasExternalBehavior, data)) < 0)
						{
							return num;
						}
						data.fromExternalTask = fromExternalTask;
						continue;
					}
					ObjectPool.Return(behaviorSource);
					return -2;
				}
				if (data.inheritedFields != null)
				{
					Dictionary<string, TaskAddData.InheritedFieldValue> dictionary = ObjectPool.Get<Dictionary<string, TaskAddData.InheritedFieldValue>>();
					dictionary.Clear();
					foreach (KeyValuePair<string, TaskAddData.InheritedFieldValue> inheritedField in data.inheritedFields)
					{
						if (inheritedField.Value.Depth != data.depth)
						{
							dictionary.Add(inheritedField.Key, inheritedField.Value);
						}
					}
					ObjectPool.Return(data.inheritedFields);
					data.inheritedFields = dictionary;
				}
				data.depth--;
			}
			else
			{
				if (behaviorTree.taskList.Count == 0 && task.NodeData.Disabled)
				{
					return -6;
				}
				task.ReferenceID = behaviorTree.taskList.Count;
				behaviorTree.taskList.Add(task);
				if (data.inheritedFields != null)
				{
					SetInheritedFields(data, task);
				}
				if (data.fromExternalTask)
				{
					if (data.parentTask == null)
					{
						task.NodeData.Offset = behaviorTree.behavior.GetBehaviorSource().RootTask.NodeData.Offset;
					}
					else
					{
						int index = behaviorTree.relativeChildIndex[behaviorTree.relativeChildIndex.Count - 1];
						data.parentTask.ReplaceAddChild(task, index);
						if (data.offset != Vector2.zero)
						{
							task.NodeData.Offset = data.offset;
							data.offset = Vector2.zero;
						}
					}
				}
				if (task is ParentTask)
				{
					ParentTask parentTask2 = task as ParentTask;
					if (parentTask2.Children == null || parentTask2.Children.Count == 0)
					{
						data.errorTask = behaviorTree.taskList.Count - 1;
						data.errorTaskName = (string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].GetType().ToString() : behaviorTree.taskList[data.errorTask].FriendlyName);
						return -1;
					}
					int num2 = behaviorTree.taskList.Count - 1;
					List<int> list = ObjectPool.Get<List<int>>();
					list.Clear();
					behaviorTree.childrenIndex.Add(list);
					list = ObjectPool.Get<List<int>>();
					list.Clear();
					behaviorTree.childConditionalIndex.Add(list);
					int count = parentTask2.Children.Count;
					for (int num3 = 0; num3 < count; num3++)
					{
						behaviorTree.parentIndex.Add(num2);
						behaviorTree.relativeChildIndex.Add(num3);
						behaviorTree.childrenIndex[num2].Add(behaviorTree.taskList.Count);
						data.parentTask = task as ParentTask;
						data.parentIndex = num2;
						if (task is Composite)
						{
							data.compositeParentIndex = num2;
						}
						behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
						int num4;
						if ((num4 = AddToTaskList(behaviorTree, parentTask2.Children[num3], ref hasExternalBehavior, data)) < 0)
						{
							if (num4 == -3)
							{
								data.errorTask = num2;
								data.errorTaskName = (string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].GetType().ToString() : behaviorTree.taskList[data.errorTask].FriendlyName);
							}
							return num4;
						}
					}
				}
				else
				{
					behaviorTree.childrenIndex.Add(null);
					behaviorTree.childConditionalIndex.Add(null);
					if (task is Conditional)
					{
						int num5 = behaviorTree.taskList.Count - 1;
						int num6 = behaviorTree.parentCompositeIndex[num5];
						if (num6 != -1)
						{
							behaviorTree.childConditionalIndex[num6].Add(num5);
						}
					}
				}
			}
			return 0;
		}

		private void SetInheritedFields(TaskAddData data, object obj)
		{
			if (obj == null || object.Equals(obj, null))
			{
				return;
			}
			FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
			for (int i = 0; i < allFields.Length; i++)
			{
				object value = allFields[i].GetValue(obj);
				TaskAddData.InheritedFieldValue value2;
				if (data.inheritedFields.TryGetValue(allFields[i].Name, out value2) && TaskUtility.HasAttribute(allFields[i], typeof(InheritedFieldAttribute)))
				{
					allFields[i].SetValue(obj, value2.Value);
				}
				else if (value is SharedVariable)
				{
					SharedVariable sharedVariable = value as SharedVariable;
					if (!string.IsNullOrEmpty(sharedVariable.Name) && data.inheritedFields.TryGetValue(sharedVariable.Name, out value2))
					{
						object value3 = value2.Value;
						if (value3 is NamedVariable)
						{
							NamedVariable namedVariable = value3 as NamedVariable;
							if (namedVariable.name.Equals(sharedVariable.Name) && (allFields[i].FieldType.Equals(typeof(SharedVariable)) || namedVariable.type.Equals(allFields[i].FieldType.Name)))
							{
								if (namedVariable.value.IsShared)
								{
									allFields[i].SetValue(obj, namedVariable.value);
								}
								else
								{
									sharedVariable.SetValue(namedVariable.value.GetValue());
									sharedVariable.IsShared = false;
								}
							}
						}
						else if (allFields[i].FieldType.Equals(typeof(SharedVariable)) || value3.GetType().Equals(allFields[i].FieldType))
						{
							allFields[i].SetValue(obj, value3);
						}
					}
				}
				if (allFields[i].FieldType.IsClass)
				{
					SetInheritedFields(data, value);
				}
			}
		}

		public void DisableBehavior(Behavior behavior)
		{
			DisableBehavior(behavior, false);
		}

		public void DisableBehavior(Behavior behavior, bool paused)
		{
			if (!IsBehaviorEnabled(behavior) || !behaviorTreeMap.ContainsKey(behavior))
			{
				if (!pausedBehaviorTrees.ContainsKey(behavior) || paused)
				{
					return;
				}
				EnableBehavior(behavior);
			}
			if (behavior.LogTaskChanges)
			{
				Debug.Log(string.Format("{0}: {1} {2}", RoundedTime(), (!paused) ? "Disabling" : "Pausing", behavior.ToString()));
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			if (paused)
			{
				if (!pausedBehaviorTrees.ContainsKey(behavior))
				{
					pausedBehaviorTrees.Add(behavior, behaviorTree);
					behavior.ExecutionStatus = TaskStatus.Inactive;
					for (int i = 0; i < behaviorTree.taskList.Count; i++)
					{
						behaviorTree.taskList[i].OnPause(true);
					}
					behaviorTrees.Remove(behaviorTree);
				}
				return;
			}
			TaskStatus status = TaskStatus.Success;
			for (int num = behaviorTree.activeStack.Count - 1; num > -1; num--)
			{
				while (behaviorTree.activeStack[num].Count > 0)
				{
					int count = behaviorTree.activeStack[num].Count;
					PopTask(behaviorTree, behaviorTree.activeStack[num].Peek(), num, ref status, true, false);
					if (count == 1)
					{
						break;
					}
				}
			}
			RemoveChildConditionalReevaluate(behaviorTree, -1);
			for (int j = 0; j < behaviorTree.taskList.Count; j++)
			{
				behaviorTree.taskList[j].OnBehaviorComplete();
			}
			behavior.ExecutionStatus = status;
			behavior.OnBehaviorEnded();
			behaviorTreeMap.Remove(behavior);
			behaviorTrees.Remove(behaviorTree);
			ObjectPool.Return(behaviorTree);
		}

		public void RestartBehavior(Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			TaskStatus status = TaskStatus.Success;
			for (int num = behaviorTree.activeStack.Count - 1; num > -1; num--)
			{
				while (behaviorTree.activeStack[num].Count > 0)
				{
					int count = behaviorTree.activeStack[num].Count;
					PopTask(behaviorTree, behaviorTree.activeStack[num].Peek(), num, ref status, true, false);
					if (count == 1)
					{
						break;
					}
				}
			}
			Restart(behaviorTree);
		}

		public bool IsBehaviorEnabled(Behavior behavior)
		{
			return behaviorTreeMap != null && behaviorTreeMap.Count > 0 && behavior != null && behavior.ExecutionStatus == TaskStatus.Running;
		}

		public void Update()
		{
			Tick();
		}

		private IEnumerator CoroutineUpdate()
		{
			while (true)
			{
				Tick();
				yield return updateWait;
			}
		}

		public void Tick()
		{
			for (int i = 0; i < behaviorTrees.Count; i++)
			{
				Tick(behaviorTrees[i]);
			}
		}

		public void Tick(Behavior behavior)
		{
			if (!(behavior == null) && IsBehaviorEnabled(behavior))
			{
				Tick(behaviorTreeMap[behavior]);
			}
		}

		private void Tick(BehaviorTree behaviorTree)
		{
			float time = Time.time;
			bool flag = false;
			if (behaviorTree.behavior.UpdateInterval == UpdateIntervalType.EveryFrame)
			{
				flag = true;
			}
			else if (behaviorTree.behavior.UpdateInterval == UpdateIntervalType.Manual)
			{
				flag = false;
			}
			else if (behaviorTree.behavior.UpdateInterval == UpdateIntervalType.SpecifySeconds && (behaviorTree.behavior.LastUpdateTime == 0f || behaviorTree.behavior.UpdateIntervalSeconds + behaviorTree.behavior.LastUpdateTime <= time))
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if (behaviorTree == null || behaviorTree.behavior == null || behaviorTree.behavior.gameObject == null)
			{
				Debug.LogWarning("behaviorTree is not valid, we should disable behavior outside");
			}
			else
			{
				if (!behaviorTree.behavior.gameObject.activeInHierarchy)
				{
					return;
				}
				behaviorTree.behavior.LastUpdateTime = time;
				behaviorTree.executionCount = 0;
				ReevaluateParentTasks(behaviorTree);
				ReevaluateConditionalTasks(behaviorTree);
				for (int num = behaviorTree.activeStack.Count - 1; num > -1; num--)
				{
					TaskStatus status = TaskStatus.Inactive;
					int num2;
					if (num < behaviorTree.interruptionIndex.Count && (num2 = behaviorTree.interruptionIndex[num]) != -1)
					{
						behaviorTree.interruptionIndex[num] = -1;
						while (behaviorTree.activeStack[num].Peek() != num2)
						{
							int count = behaviorTree.activeStack[num].Count;
							PopTask(behaviorTree, behaviorTree.activeStack[num].Peek(), num, ref status, true);
							if (count == 1)
							{
								break;
							}
						}
						if (num < behaviorTree.activeStack.Count && behaviorTree.activeStack[num].Count > 0 && behaviorTree.taskList[num2] == behaviorTree.taskList[behaviorTree.activeStack[num].Peek()])
						{
							if (behaviorTree.taskList[num2] is ParentTask)
							{
								status = (behaviorTree.taskList[num2] as ParentTask).OverrideStatus();
							}
							PopTask(behaviorTree, num2, num, ref status, true);
						}
					}
					int num3 = -1;
					while (status != TaskStatus.Running && num < behaviorTree.activeStack.Count && behaviorTree.activeStack[num].Count > 0)
					{
						int num4 = behaviorTree.activeStack[num].Peek();
						if ((num < behaviorTree.activeStack.Count && behaviorTree.activeStack[num].Count > 0 && num3 == behaviorTree.activeStack[num].Peek()) || !IsBehaviorEnabled(behaviorTree.behavior))
						{
							break;
						}
						num3 = num4;
						status = RunTask(behaviorTree, num4, num, status);
					}
				}
			}
		}

		private void ReevaluateConditionalTasks(BehaviorTree behaviorTree)
		{
			for (int num = behaviorTree.conditionalReevaluate.Count - 1; num > -1; num--)
			{
				if (behaviorTree.conditionalReevaluate[num].compositeIndex != -1)
				{
					int index = behaviorTree.conditionalReevaluate[num].index;
					TaskStatus taskStatus = behaviorTree.taskList[index].OnUpdate();
					if (taskStatus != behaviorTree.conditionalReevaluate[num].taskStatus)
					{
						if (behaviorTree.behavior.LogTaskChanges)
						{
							int num2 = behaviorTree.parentCompositeIndex[index];
							MonoBehaviour.print(string.Format("{0}: {1}: Conditional abort with task {2} ({3}, index {4}) because of conditional task {5} ({6}, index {7}) with status {8}", RoundedTime(), behaviorTree.behavior.ToString(), behaviorTree.taskList[num2].FriendlyName, behaviorTree.taskList[num2].GetType(), num2, behaviorTree.taskList[index].FriendlyName, behaviorTree.taskList[index].GetType(), index, taskStatus));
						}
						int compositeIndex = behaviorTree.conditionalReevaluate[num].compositeIndex;
						for (int num3 = behaviorTree.activeStack.Count - 1; num3 > -1; num3--)
						{
							if (behaviorTree.activeStack[num3].Count > 0)
							{
								int num4 = behaviorTree.activeStack[num3].Peek();
								int num5 = FindLCA(behaviorTree, index, num4);
								if (IsChild(behaviorTree, num5, compositeIndex))
								{
									int count = behaviorTree.activeStack.Count;
									while (num4 != -1 && num4 != num5 && behaviorTree.activeStack.Count == count)
									{
										TaskStatus status = TaskStatus.Failure;
										PopTask(behaviorTree, num4, num3, ref status, false);
										num4 = behaviorTree.parentIndex[num4];
									}
								}
							}
						}
						for (int num6 = behaviorTree.conditionalReevaluate.Count - 1; num6 > num - 1; num6--)
						{
							BehaviorTree.ConditionalReevaluate conditionalReevaluate = behaviorTree.conditionalReevaluate[num6];
							if (FindLCA(behaviorTree, compositeIndex, conditionalReevaluate.index) == compositeIndex)
							{
								behaviorTree.taskList[behaviorTree.conditionalReevaluate[num6].index].NodeData.IsReevaluating = false;
								ObjectPool.Return(behaviorTree.conditionalReevaluate[num6]);
								behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[num6].index);
								behaviorTree.conditionalReevaluate.RemoveAt(num6);
							}
						}
						Composite composite = behaviorTree.taskList[behaviorTree.parentCompositeIndex[index]] as Composite;
						for (int num7 = num - 1; num7 > -1; num7--)
						{
							BehaviorTree.ConditionalReevaluate conditionalReevaluate2 = behaviorTree.conditionalReevaluate[num7];
							if (composite.AbortType == AbortType.LowerPriority && behaviorTree.parentCompositeIndex[conditionalReevaluate2.index] == behaviorTree.parentCompositeIndex[index])
							{
								behaviorTree.taskList[behaviorTree.conditionalReevaluate[num7].index].NodeData.IsReevaluating = false;
								ObjectPool.Return(behaviorTree.conditionalReevaluate[num7]);
								behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[num7].index);
								behaviorTree.conditionalReevaluate.RemoveAt(num7);
								num--;
							}
							else if (behaviorTree.parentCompositeIndex[conditionalReevaluate2.index] == behaviorTree.parentCompositeIndex[index])
							{
								for (int i = 0; i < behaviorTree.childrenIndex[compositeIndex].Count; i++)
								{
									if (IsParentTask(behaviorTree, behaviorTree.childrenIndex[compositeIndex][i], conditionalReevaluate2.index))
									{
										int num8 = behaviorTree.childrenIndex[compositeIndex][i];
										while (!(behaviorTree.taskList[num8] is Composite) && behaviorTree.childrenIndex[num8] != null)
										{
											num8 = behaviorTree.childrenIndex[num8][0];
										}
										if (behaviorTree.taskList[num8] is Composite)
										{
											conditionalReevaluate2.compositeIndex = num8;
										}
										break;
									}
								}
							}
						}
						conditionalParentIndexes.Clear();
						for (int num9 = behaviorTree.parentIndex[index]; num9 != compositeIndex; num9 = behaviorTree.parentIndex[num9])
						{
							conditionalParentIndexes.Add(num9);
						}
						if (conditionalParentIndexes.Count == 0)
						{
							conditionalParentIndexes.Add(behaviorTree.parentIndex[index]);
						}
						ParentTask parentTask = behaviorTree.taskList[compositeIndex] as ParentTask;
						parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[conditionalParentIndexes[conditionalParentIndexes.Count - 1]]);
						for (int num10 = conditionalParentIndexes.Count - 1; num10 > -1; num10--)
						{
							parentTask = behaviorTree.taskList[conditionalParentIndexes[num10]] as ParentTask;
							if (num10 == 0)
							{
								parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[index]);
							}
							else
							{
								parentTask.OnConditionalAbort(behaviorTree.relativeChildIndex[conditionalParentIndexes[num10 - 1]]);
							}
						}
						behaviorTree.taskList[index].NodeData.InterruptTime = Time.realtimeSinceStartup;
					}
				}
			}
		}

		private void ReevaluateParentTasks(BehaviorTree behaviorTree)
		{
			for (int num = behaviorTree.parentReevaluate.Count - 1; num > -1; num--)
			{
				int num2 = behaviorTree.parentReevaluate[num];
				if (behaviorTree.taskList[num2] is Decorator)
				{
					if (behaviorTree.taskList[num2].OnUpdate() == TaskStatus.Failure)
					{
						Interrupt(behaviorTree.behavior, behaviorTree.taskList[num2]);
					}
				}
				else if (behaviorTree.taskList[num2] is Composite)
				{
					ParentTask parentTask = behaviorTree.taskList[num2] as ParentTask;
					if (parentTask.OnReevaluationStarted())
					{
						int stackIndex = 0;
						TaskStatus status = RunParentTask(behaviorTree, num2, ref stackIndex, TaskStatus.Inactive);
						parentTask.OnReevaluationEnded(status);
					}
				}
			}
		}

		private TaskStatus RunTask(BehaviorTree behaviorTree, int taskIndex, int stackIndex, TaskStatus previousStatus)
		{
			Task task = behaviorTree.taskList[taskIndex];
			if (task == null)
			{
				return previousStatus;
			}
			if (task.NodeData.Disabled)
			{
				if (behaviorTree.behavior.LogTaskChanges)
				{
					MonoBehaviour.print(string.Format("{0}: {1}: Skip task {2} ({3}, index {4}) at stack index {5} (task disabled)", RoundedTime(), behaviorTree.behavior.ToString(), behaviorTree.taskList[taskIndex].FriendlyName, behaviorTree.taskList[taskIndex].GetType(), taskIndex, stackIndex));
				}
				if (behaviorTree.parentIndex[taskIndex] != -1)
				{
					ParentTask parentTask = behaviorTree.taskList[behaviorTree.parentIndex[taskIndex]] as ParentTask;
					if (!parentTask.CanRunParallelChildren())
					{
						parentTask.OnChildExecuted(TaskStatus.Inactive);
					}
					else
					{
						parentTask.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], TaskStatus.Inactive);
					}
				}
				return previousStatus;
			}
			TaskStatus status = previousStatus;
			if (!task.IsInstant && (behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Failure || behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Success))
			{
				status = behaviorTree.nonInstantTaskStatus[stackIndex];
				PopTask(behaviorTree, taskIndex, stackIndex, ref status, true);
				return status;
			}
			PushTask(behaviorTree, taskIndex, stackIndex);
			if (atBreakpoint)
			{
				return TaskStatus.Running;
			}
			if (task is ParentTask)
			{
				ParentTask parentTask2 = task as ParentTask;
				status = RunParentTask(behaviorTree, taskIndex, ref stackIndex, status);
				status = parentTask2.OverrideStatus(status);
			}
			else
			{
				status = task.OnUpdate();
			}
			if (status != TaskStatus.Running)
			{
				if (task.IsInstant)
				{
					PopTask(behaviorTree, taskIndex, stackIndex, ref status, true);
				}
				else
				{
					behaviorTree.nonInstantTaskStatus[stackIndex] = status;
				}
			}
			return status;
		}

		private TaskStatus RunParentTask(BehaviorTree behaviorTree, int taskIndex, ref int stackIndex, TaskStatus status)
		{
			ParentTask parentTask = behaviorTree.taskList[taskIndex] as ParentTask;
			if (!parentTask.CanRunParallelChildren() || parentTask.OverrideStatus(TaskStatus.Running) != TaskStatus.Running)
			{
				TaskStatus taskStatus = TaskStatus.Inactive;
				int num = stackIndex;
				int num2 = -1;
				while (parentTask.CanExecute() && (taskStatus != TaskStatus.Running || parentTask.CanRunParallelChildren()) && IsBehaviorEnabled(behaviorTree.behavior))
				{
					List<int> list = behaviorTree.childrenIndex[taskIndex];
					int num3 = parentTask.CurrentChildIndex();
					if ((executionsPerTick == ExecutionsPerTickType.NoDuplicates && num3 == num2) || (executionsPerTick == ExecutionsPerTickType.Count && behaviorTree.executionCount >= maxTaskExecutionsPerTick))
					{
						if (executionsPerTick == ExecutionsPerTickType.Count)
						{
							Debug.LogWarning(string.Format("{0}: {1}: More than the specified number of task executions per tick ({2}) have executed, returning early.", RoundedTime(), behaviorTree.behavior.ToString(), maxTaskExecutionsPerTick));
						}
						status = TaskStatus.Running;
						break;
					}
					num2 = num3;
					if (parentTask.CanRunParallelChildren())
					{
						behaviorTree.activeStack.Add(ObjectPool.Get<Stack<int>>());
						behaviorTree.interruptionIndex.Add(-1);
						behaviorTree.nonInstantTaskStatus.Add(TaskStatus.Inactive);
						stackIndex = behaviorTree.activeStack.Count - 1;
						parentTask.OnChildStarted(num3);
					}
					else
					{
						parentTask.OnChildStarted();
					}
					status = (taskStatus = RunTask(behaviorTree, list[num3], stackIndex, status));
				}
				stackIndex = num;
			}
			return status;
		}

		private void PushTask(BehaviorTree behaviorTree, int taskIndex, int stackIndex)
		{
			if (!IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count)
			{
				return;
			}
			Stack<int> stack = behaviorTree.activeStack[stackIndex];
			if (stack.Count != 0 && stack.Peek() == taskIndex)
			{
				return;
			}
			stack.Push(taskIndex);
			behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Running;
			behaviorTree.executionCount++;
			Task task = behaviorTree.taskList[taskIndex];
			task.NodeData.PushTime = Time.realtimeSinceStartup;
			task.NodeData.ExecutionStatus = TaskStatus.Running;
			if (task.NodeData.IsBreakpoint)
			{
				atBreakpoint = true;
				if (this.onTaskBreakpoint != null)
				{
					this.onTaskBreakpoint();
				}
			}
			if (behaviorTree.behavior.LogTaskChanges)
			{
				MonoBehaviour.print(string.Format("{0}: {1}: Push task {2} ({3}, index {4}) at stack index {5}", RoundedTime(), behaviorTree.behavior.ToString(), task.FriendlyName, task.GetType(), taskIndex, stackIndex));
			}
			task.OnStart();
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.CanReevaluate())
				{
					behaviorTree.parentReevaluate.Add(taskIndex);
				}
			}
		}

		private void PopTask(BehaviorTree behaviorTree, int taskIndex, int stackIndex, ref TaskStatus status, bool popChildren)
		{
			PopTask(behaviorTree, taskIndex, stackIndex, ref status, popChildren, true);
		}

		private void PopTask(BehaviorTree behaviorTree, int taskIndex, int stackIndex, ref TaskStatus status, bool popChildren, bool notifyOnEmptyStack)
		{
			if (!IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count || behaviorTree.activeStack[stackIndex].Count == 0 || taskIndex != behaviorTree.activeStack[stackIndex].Peek())
			{
				return;
			}
			behaviorTree.activeStack[stackIndex].Pop();
			behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Inactive;
			StopThirdPartyTask(behaviorTree, taskIndex);
			Task task = behaviorTree.taskList[taskIndex];
			task.OnEnd();
			int num = behaviorTree.parentIndex[taskIndex];
			task.NodeData.PushTime = -1f;
			task.NodeData.PopTime = Time.realtimeSinceStartup;
			task.NodeData.ExecutionStatus = status;
			if (behaviorTree.behavior.LogTaskChanges)
			{
				MonoBehaviour.print(string.Format("{0}: {1}: Pop task {2} ({3}, index {4}) at stack index {5} with status {6}", RoundedTime(), behaviorTree.behavior.ToString(), task.FriendlyName, task.GetType(), taskIndex, stackIndex, status));
			}
			if (num != -1)
			{
				if (task is Conditional)
				{
					int num2 = behaviorTree.parentCompositeIndex[taskIndex];
					if (num2 != -1)
					{
						Composite composite = behaviorTree.taskList[num2] as Composite;
						if (composite.AbortType != AbortType.None)
						{
							BehaviorTree.ConditionalReevaluate value;
							if (behaviorTree.conditionalReevaluateMap.TryGetValue(taskIndex, out value))
							{
								value.compositeIndex = -1;
								task.NodeData.IsReevaluating = false;
							}
							else
							{
								BehaviorTree.ConditionalReevaluate conditionalReevaluate = ObjectPool.Get<BehaviorTree.ConditionalReevaluate>();
								conditionalReevaluate.Initialize(taskIndex, status, stackIndex, (composite.AbortType == AbortType.LowerPriority) ? (-1) : num2);
								behaviorTree.conditionalReevaluate.Add(conditionalReevaluate);
								behaviorTree.conditionalReevaluateMap.Add(taskIndex, conditionalReevaluate);
								task.NodeData.IsReevaluating = composite.AbortType == AbortType.Self || composite.AbortType == AbortType.Both;
							}
						}
					}
				}
				ParentTask parentTask = behaviorTree.taskList[num] as ParentTask;
				if (!parentTask.CanRunParallelChildren())
				{
					parentTask.OnChildExecuted(status);
					status = parentTask.Decorate(status);
				}
				else
				{
					parentTask.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], status);
				}
			}
			if (task is ParentTask)
			{
				ParentTask parentTask2 = task as ParentTask;
				if (parentTask2.CanReevaluate())
				{
					for (int num3 = behaviorTree.parentReevaluate.Count - 1; num3 > -1; num3--)
					{
						if (behaviorTree.parentReevaluate[num3] == taskIndex)
						{
							behaviorTree.parentReevaluate.RemoveAt(num3);
							break;
						}
					}
				}
				if (parentTask2 is Composite)
				{
					Composite composite2 = parentTask2 as Composite;
					if (composite2.AbortType == AbortType.Self || composite2.AbortType == AbortType.None || behaviorTree.activeStack[stackIndex].Count == 0)
					{
						RemoveChildConditionalReevaluate(behaviorTree, taskIndex);
					}
					else if (composite2.AbortType == AbortType.LowerPriority || composite2.AbortType == AbortType.Both)
					{
						for (int i = 0; i < behaviorTree.childConditionalIndex[taskIndex].Count; i++)
						{
							int num4 = behaviorTree.childConditionalIndex[taskIndex][i];
							BehaviorTree.ConditionalReevaluate value2;
							if (behaviorTree.conditionalReevaluateMap.TryGetValue(num4, out value2))
							{
								value2.compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
								behaviorTree.taskList[num4].NodeData.IsReevaluating = true;
							}
						}
						for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
						{
							if (behaviorTree.conditionalReevaluate[j].compositeIndex == taskIndex)
							{
								behaviorTree.conditionalReevaluate[j].compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
							}
						}
					}
				}
			}
			if (popChildren)
			{
				for (int num5 = behaviorTree.activeStack.Count - 1; num5 > stackIndex; num5--)
				{
					if (behaviorTree.activeStack[num5].Count > 0 && IsParentTask(behaviorTree, taskIndex, behaviorTree.activeStack[num5].Peek()))
					{
						TaskStatus status2 = TaskStatus.Failure;
						for (int num6 = behaviorTree.activeStack[num5].Count; num6 > 0; num6--)
						{
							PopTask(behaviorTree, behaviorTree.activeStack[num5].Peek(), num5, ref status2, false, notifyOnEmptyStack);
						}
					}
				}
			}
			if (behaviorTree.activeStack[stackIndex].Count != 0)
			{
				return;
			}
			if (stackIndex == 0)
			{
				if (notifyOnEmptyStack)
				{
					if (behaviorTree.behavior.RestartWhenComplete)
					{
						Restart(behaviorTree);
					}
					else
					{
						DisableBehavior(behaviorTree.behavior);
						behaviorTree.behavior.ExecutionStatus = status;
					}
				}
				status = TaskStatus.Inactive;
			}
			else
			{
				RemoveStack(behaviorTree, stackIndex);
				status = TaskStatus.Running;
			}
		}

		private void RemoveChildConditionalReevaluate(BehaviorTree behaviorTree, int compositeIndex)
		{
			for (int num = behaviorTree.conditionalReevaluate.Count - 1; num > -1; num--)
			{
				if (behaviorTree.conditionalReevaluate[num].compositeIndex == compositeIndex)
				{
					ObjectPool.Return(behaviorTree.conditionalReevaluate[num]);
					int index = behaviorTree.conditionalReevaluate[num].index;
					behaviorTree.conditionalReevaluateMap.Remove(index);
					behaviorTree.conditionalReevaluate.RemoveAt(num);
					behaviorTree.taskList[index].NodeData.IsReevaluating = false;
				}
			}
		}

		private void Restart(BehaviorTree behaviorTree)
		{
			if (behaviorTree.behavior.LogTaskChanges)
			{
				Debug.Log(string.Format("{0}: Restarting {1}", RoundedTime(), behaviorTree.behavior.ToString()));
			}
			RemoveChildConditionalReevaluate(behaviorTree, -1);
			if (behaviorTree.behavior.ResetValuesOnRestart)
			{
				behaviorTree.behavior.SaveResetValues();
			}
			for (int i = 0; i < behaviorTree.taskList.Count; i++)
			{
				behaviorTree.taskList[i].OnBehaviorRestart();
			}
			behaviorTree.behavior.OnBehaviorRestarted();
			PushTask(behaviorTree, 0, 0);
		}

		private bool IsParentTask(BehaviorTree behaviorTree, int possibleParent, int possibleChild)
		{
			int num = 0;
			for (int num2 = possibleChild; num2 != -1; num2 = num)
			{
				num = behaviorTree.parentIndex[num2];
				if (num == possibleParent)
				{
					return true;
				}
			}
			return false;
		}

		public void Interrupt(Behavior behavior, Task task)
		{
			Interrupt(behavior, task, task);
		}

		public void Interrupt(Behavior behavior, Task task, Task interruptionTask)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			int num = -1;
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.taskList.Count; i++)
			{
				if (behaviorTree.taskList[i].ReferenceID == task.ReferenceID)
				{
					num = i;
					break;
				}
			}
			if (num <= -1)
			{
				return;
			}
			for (int j = 0; j < behaviorTree.activeStack.Count; j++)
			{
				if (behaviorTree.activeStack[j].Count <= 0)
				{
					continue;
				}
				for (int num2 = behaviorTree.activeStack[j].Peek(); num2 != -1; num2 = behaviorTree.parentIndex[num2])
				{
					if (num2 == num)
					{
						behaviorTree.interruptionIndex[j] = num;
						if (behavior.LogTaskChanges)
						{
							Debug.Log(string.Format("{0}: {1}: Interrupt task {2} ({3}) with index {4} at stack index {5}", RoundedTime(), behaviorTree.behavior.ToString(), task.FriendlyName, task.GetType().ToString(), num, j));
						}
						interruptionTask.NodeData.InterruptTime = Time.realtimeSinceStartup;
						break;
					}
				}
			}
		}

		public void StopThirdPartyTask(BehaviorTree behaviorTree, int taskIndex)
		{
			thirdPartyTaskCompare.Task = behaviorTree.taskList[taskIndex];
			object value;
			if (taskObjectMap.TryGetValue(thirdPartyTaskCompare, out value))
			{
				ThirdPartyObjectType thirdPartyObjectType = objectTaskMap[value].ThirdPartyObjectType;
				if (invokeParameters == null)
				{
					invokeParameters = new object[1];
				}
				invokeParameters[0] = behaviorTree.taskList[taskIndex];
				switch (thirdPartyObjectType)
				{
				case ThirdPartyObjectType.PlayMaker:
					PlayMakerStopMethod.Invoke(null, invokeParameters);
					break;
				case ThirdPartyObjectType.uScript:
					UScriptStopMethod.Invoke(null, invokeParameters);
					break;
				case ThirdPartyObjectType.DialogueSystem:
					DialogueSystemStopMethod.Invoke(null, invokeParameters);
					break;
				case ThirdPartyObjectType.uSequencer:
					USequencerStopMethod.Invoke(null, invokeParameters);
					break;
				case ThirdPartyObjectType.ICode:
					ICodeStopMethod.Invoke(null, invokeParameters);
					break;
				}
				RemoveActiveThirdPartyTask(behaviorTree.taskList[taskIndex]);
			}
		}

		public void RemoveActiveThirdPartyTask(Task task)
		{
			thirdPartyTaskCompare.Task = task;
			object value;
			if (taskObjectMap.TryGetValue(thirdPartyTaskCompare, out value))
			{
				ObjectPool.Return(value);
				taskObjectMap.Remove(thirdPartyTaskCompare);
				objectTaskMap.Remove(value);
			}
		}

		private void RemoveStack(BehaviorTree behaviorTree, int stackIndex)
		{
			Stack<int> stack = behaviorTree.activeStack[stackIndex];
			stack.Clear();
			ObjectPool.Return(stack);
			behaviorTree.activeStack.RemoveAt(stackIndex);
			behaviorTree.interruptionIndex.RemoveAt(stackIndex);
			behaviorTree.nonInstantTaskStatus.RemoveAt(stackIndex);
		}

		private int FindLCA(BehaviorTree behaviorTree, int taskIndex1, int taskIndex2)
		{
			HashSet<int> hashSet = ObjectPool.Get<HashSet<int>>();
			hashSet.Clear();
			int num;
			for (num = taskIndex1; num != -1; num = behaviorTree.parentIndex[num])
			{
				hashSet.Add(num);
			}
			num = taskIndex2;
			while (!hashSet.Contains(num))
			{
				num = behaviorTree.parentIndex[num];
			}
			return num;
		}

		private bool IsChild(BehaviorTree behaviorTree, int taskIndex1, int taskIndex2)
		{
			for (int num = taskIndex1; num != -1; num = behaviorTree.parentIndex[num])
			{
				if (num == taskIndex2)
				{
					return true;
				}
			}
			return false;
		}

		public List<Task> GetActiveTasks(Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return null;
			}
			List<Task> list = new List<Task>();
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				Task task = behaviorTree.taskList[behaviorTree.activeStack[i].Peek()];
				if (task is BehaviorDesigner.Runtime.Tasks.Action)
				{
					list.Add(task);
				}
			}
			return list;
		}

		public void BehaviorOnCollisionEnter(Collision collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionEnter(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionEnter(collision);
				}
			}
		}

		public void BehaviorOnCollisionExit(Collision collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionExit(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionExit(collision);
				}
			}
		}

		public void BehaviorOnCollisionStay(Collision collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionStay(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionStay(collision);
				}
			}
		}

		public void BehaviorOnTriggerEnter(Collider other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerEnter(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnTriggerEnter(other);
				}
			}
		}

		public void BehaviorOnTriggerExit(Collider other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerExit(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnTriggerExit(other);
				}
			}
		}

		public void BehaviorOnTriggerStay(Collider other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerStay(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnTriggerStay(other);
				}
			}
		}

		public void BehaviorOnCollisionEnter2D(Collision2D collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionEnter2D(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionEnter2D(collision);
				}
			}
		}

		public void BehaviorOnCollisionExit2D(Collision2D collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionExit2D(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionExit2D(collision);
				}
			}
		}

		public void BehaviorOnCollisionStay2D(Collision2D collision, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnCollisionStay2D(collision);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnCollisionStay2D(collision);
				}
			}
		}

		public void BehaviorOnTriggerEnter2D(Collider2D other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerEnter2D(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnTriggerEnter2D(other);
				}
			}
		}

		public void BehaviorOnTriggerExit2D(Collider2D other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerExit2D(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				int num = behaviorTree.conditionalReevaluate[j].index;
				if (!behaviorTree.taskList[num].NodeData.Disabled)
				{
					behaviorTree.taskList[num].OnTriggerExit2D(other);
				}
			}
		}

		public void BehaviorOnTriggerStay2D(Collider2D other, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerStay2D(other);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				if (behaviorTree.activeStack[j].Count != 0)
				{
					int num = behaviorTree.conditionalReevaluate[j].index;
					if (!behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnTriggerStay2D(other);
					}
				}
			}
		}

		public void BehaviorOnControllerColliderHit(ControllerColliderHit hit, Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			for (int i = 0; i < behaviorTree.activeStack.Count; i++)
			{
				if (behaviorTree.activeStack[i].Count != 0)
				{
					int num = behaviorTree.activeStack[i].Peek();
					while (num != -1 && !behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnControllerColliderHit(hit);
						num = behaviorTree.parentIndex[num];
					}
				}
			}
			for (int j = 0; j < behaviorTree.conditionalReevaluate.Count; j++)
			{
				if (behaviorTree.activeStack[j].Count != 0)
				{
					int num = behaviorTree.conditionalReevaluate[j].index;
					if (!behaviorTree.taskList[num].NodeData.Disabled)
					{
						behaviorTree.taskList[num].OnControllerColliderHit(hit);
					}
				}
			}
		}

		public bool MapObjectToTask(object objectKey, Task task, ThirdPartyObjectType objectType)
		{
			if (objectTaskMap.ContainsKey(objectKey))
			{
				string arg = string.Empty;
				switch (objectType)
				{
				case ThirdPartyObjectType.PlayMaker:
					arg = "PlayMaker FSM";
					break;
				case ThirdPartyObjectType.uScript:
					arg = "uScript Graph";
					break;
				case ThirdPartyObjectType.DialogueSystem:
					arg = "Dialogue System";
					break;
				case ThirdPartyObjectType.uSequencer:
					arg = "uSequencer sequence";
					break;
				case ThirdPartyObjectType.ICode:
					arg = "ICode state machine";
					break;
				}
				Debug.LogError(string.Format("Only one behavior can be mapped to the same instance of the {0}.", arg));
				return false;
			}
			ThirdPartyTask thirdPartyTask = ObjectPool.Get<ThirdPartyTask>();
			thirdPartyTask.Initialize(task, objectType);
			objectTaskMap.Add(objectKey, thirdPartyTask);
			taskObjectMap.Add(thirdPartyTask, objectKey);
			return true;
		}

		public Task TaskForObject(object objectKey)
		{
			ThirdPartyTask value;
			if (!objectTaskMap.TryGetValue(objectKey, out value))
			{
				return null;
			}
			return value.Task;
		}

		private decimal RoundedTime()
		{
			return Math.Round((decimal)Time.time, 5, MidpointRounding.AwayFromZero);
		}

		public List<Task> GetTaskList(Behavior behavior)
		{
			if (!IsBehaviorEnabled(behavior))
			{
				return null;
			}
			BehaviorTree behaviorTree = behaviorTreeMap[behavior];
			return behaviorTree.taskList;
		}
	}
}
