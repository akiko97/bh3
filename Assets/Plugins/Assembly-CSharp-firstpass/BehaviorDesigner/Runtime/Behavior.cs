using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public abstract class Behavior : MonoBehaviour, IBehavior
	{
		public enum GizmoViewMode
		{
			Running = 0,
			Always = 1,
			Selected = 2,
			Never = 3
		}

		public delegate void BehaviorHandler();

		[NonSerialized]
		public UpdateIntervalType UpdateInterval;

		[NonSerialized]
		public float UpdateIntervalSeconds;

		[NonSerialized]
		public float LastUpdateTime;

		[NonSerialized]
		public bool DisableBehaviorWhenMonoDisabled = true;

		[NonSerialized]
		public bool TryEnableBehaviorWhenMonoEnabled = true;

		[SerializeField]
		private bool startWhenEnabled = true;

		[SerializeField]
		private bool pauseWhenDisabled;

		[SerializeField]
		private bool restartWhenComplete;

		[SerializeField]
		private bool logTaskChanges;

		[SerializeField]
		private int group;

		[SerializeField]
		private bool resetValuesOnRestart;

		[SerializeField]
		private ExternalBehavior externalBehavior;

		private bool hasInheritedVariables;

		[SerializeField]
		private BehaviorSource mBehaviorSource;

		private bool isPaused;

		private TaskStatus executionStatus;

		private bool initialized;

		private List<Dictionary<string, object>> defaultValues;

		private Dictionary<string, object> defaultVariableValues;

		private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines;

		private Dictionary<Type, Dictionary<string, Delegate>> eventTable;

		[NonSerialized]
		public bool selected;

		[NonSerialized]
		public GizmoViewMode gizmoViewMode;

		[NonSerialized]
		public bool showBehaviorDesignerGizmo = true;

		public bool StartWhenEnabled
		{
			get
			{
				return startWhenEnabled;
			}
			set
			{
				startWhenEnabled = value;
			}
		}

		public bool PauseWhenDisabled
		{
			get
			{
				return pauseWhenDisabled;
			}
			set
			{
				pauseWhenDisabled = value;
			}
		}

		public bool RestartWhenComplete
		{
			get
			{
				return restartWhenComplete;
			}
			set
			{
				restartWhenComplete = value;
			}
		}

		public bool LogTaskChanges
		{
			get
			{
				return logTaskChanges;
			}
			set
			{
				logTaskChanges = value;
			}
		}

		public int Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public bool ResetValuesOnRestart
		{
			get
			{
				return resetValuesOnRestart;
			}
			set
			{
				resetValuesOnRestart = value;
			}
		}

		public ExternalBehavior ExternalBehavior
		{
			get
			{
				return externalBehavior;
			}
			set
			{
				if (externalBehavior != value)
				{
					if (BehaviorManager.instance != null)
					{
						BehaviorManager.instance.DisableBehavior(this);
					}
					mBehaviorSource.HasSerialized = false;
				}
				externalBehavior = value;
				if (startWhenEnabled)
				{
					EnableBehavior();
				}
			}
		}

		public bool HasInheritedVariables
		{
			get
			{
				return hasInheritedVariables;
			}
			set
			{
				hasInheritedVariables = value;
			}
		}

		public string BehaviorName
		{
			get
			{
				return mBehaviorSource.behaviorName;
			}
			set
			{
				mBehaviorSource.behaviorName = value;
			}
		}

		public string BehaviorDescription
		{
			get
			{
				return mBehaviorSource.behaviorDescription;
			}
			set
			{
				mBehaviorSource.behaviorDescription = value;
			}
		}

		public TaskStatus ExecutionStatus
		{
			get
			{
				return executionStatus;
			}
			set
			{
				executionStatus = value;
			}
		}

		public event BehaviorHandler OnBehaviorStart;

		public event BehaviorHandler OnBehaviorRestart;

		public event BehaviorHandler OnBehaviorEnd;

		public Behavior()
		{
			mBehaviorSource = new BehaviorSource(this);
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
			return base.gameObject.name;
		}

		public void OnDrawGizmosSelected()
		{
			if (showBehaviorDesignerGizmo)
			{
				Gizmos.DrawIcon(base.transform.position, "Behavior Designer Scene Icon.png");
			}
		}

		public void Start()
		{
			if (startWhenEnabled)
			{
				EnableBehavior();
			}
			initialized = true;
		}

		private bool TaskContainsMethod(string methodName, Task task)
		{
			if (task == null)
			{
				return false;
			}
			MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null && task.GetType().Equals(method.DeclaringType))
			{
				return true;
			}
			if (task is ParentTask)
			{
				ParentTask parentTask = task as ParentTask;
				if (parentTask.Children != null)
				{
					for (int i = 0; i < parentTask.Children.Count; i++)
					{
						if (TaskContainsMethod(methodName, parentTask.Children[i]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void EnableBehavior()
		{
			CreateBehaviorManager();
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.EnableBehavior(this);
			}
		}

		public void DisableBehavior()
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, pauseWhenDisabled);
				isPaused = pauseWhenDisabled;
			}
		}

		public void DisableBehavior(bool pause)
		{
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.DisableBehavior(this, pause);
				isPaused = pause;
			}
		}

		public void OnEnable()
		{
			if (TryEnableBehaviorWhenMonoEnabled)
			{
				if (BehaviorManager.instance != null && isPaused)
				{
					BehaviorManager.instance.EnableBehavior(this);
					isPaused = false;
				}
				else if (startWhenEnabled && initialized)
				{
					EnableBehavior();
				}
			}
		}

		public void OnDisable()
		{
			if (DisableBehaviorWhenMonoDisabled)
			{
				DisableBehavior();
			}
		}

		public SharedVariable GetVariable(string name)
		{
			CheckForSerialization();
			return mBehaviorSource.GetVariable(name);
		}

		public void SetVariable(string name, SharedVariable item)
		{
			CheckForSerialization();
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

		public List<SharedVariable> GetAllVariables()
		{
			CheckForSerialization();
			return mBehaviorSource.GetAllVariables();
		}

		public void CheckForSerialization()
		{
			if (externalBehavior != null)
			{
				List<SharedVariable> list = null;
				bool force = false;
				if (!hasInheritedVariables)
				{
					mBehaviorSource.CheckForSerialization(false);
					list = mBehaviorSource.GetAllVariables();
					hasInheritedVariables = true;
					force = true;
				}
				externalBehavior.BehaviorSource.Owner = ExternalBehavior;
				externalBehavior.BehaviorSource.CheckForSerialization(force, GetBehaviorSource());
				if (list == null)
				{
					return;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						mBehaviorSource.SetVariable(list[i].Name, list[i]);
					}
				}
			}
			else
			{
				mBehaviorSource.CheckForSerialization(false);
			}
		}

		public void OnDrawGizmos()
		{
			if (gizmoViewMode == GizmoViewMode.Never || (gizmoViewMode == GizmoViewMode.Selected && !selected) || (gizmoViewMode != GizmoViewMode.Running && gizmoViewMode != GizmoViewMode.Always && (!Application.isPlaying || ExecutionStatus != TaskStatus.Running) && Application.isPlaying))
			{
				return;
			}
			CheckForSerialization();
			OnDrawGizmos(mBehaviorSource.RootTask);
			List<Task> detachedTasks = mBehaviorSource.DetachedTasks;
			if (detachedTasks != null)
			{
				for (int i = 0; i < detachedTasks.Count; i++)
				{
					OnDrawGizmos(detachedTasks[i]);
				}
			}
		}

		private void OnDrawGizmos(Task task)
		{
			if (task == null || (gizmoViewMode == GizmoViewMode.Running && !task.NodeData.IsReevaluating && (task.NodeData.IsReevaluating || task.NodeData.ExecutionStatus != TaskStatus.Running)))
			{
				return;
			}
			task.OnDrawGizmos();
			if (!(task is ParentTask))
			{
				return;
			}
			ParentTask parentTask = task as ParentTask;
			if (parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					OnDrawGizmos(parentTask.Children[i]);
				}
			}
		}

		public T FindTask<T>() where T : Task
		{
			return FindTask<T>(mBehaviorSource.RootTask);
		}

		private T FindTask<T>(Task task) where T : Task
		{
			if (task.GetType().Equals(typeof(T)))
			{
				return (T)task;
			}
			ParentTask parentTask;
			if ((parentTask = task as ParentTask) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					T val = (T)null;
					if ((val = FindTask<T>(parentTask.Children[i])) != null)
					{
						return val;
					}
				}
			}
			return (T)null;
		}

		public List<T> FindTasks<T>() where T : Task
		{
			List<T> taskList = new List<T>();
			FindTasks(mBehaviorSource.RootTask, ref taskList);
			return taskList;
		}

		private void FindTasks<T>(Task task, ref List<T> taskList) where T : Task
		{
			if (typeof(T).IsAssignableFrom(task.GetType()))
			{
				taskList.Add((T)task);
			}
			ParentTask parentTask;
			if ((parentTask = task as ParentTask) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					FindTasks(parentTask.Children[i], ref taskList);
				}
			}
		}

		public Task FindTaskWithName(string taskName)
		{
			return FindTaskWithName(taskName, mBehaviorSource.RootTask);
		}

		private Task FindTaskWithName(string taskName, Task task)
		{
			if (task.FriendlyName.Equals(taskName))
			{
				return task;
			}
			ParentTask parentTask;
			if ((parentTask = task as ParentTask) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					Task task2 = null;
					if ((task2 = FindTaskWithName(taskName, parentTask.Children[i])) != null)
					{
						return task2;
					}
				}
			}
			return null;
		}

		public List<Task> FindTasksWithName(string taskName)
		{
			List<Task> taskList = new List<Task>();
			FindTasksWithName(taskName, mBehaviorSource.RootTask, ref taskList);
			return taskList;
		}

		private void FindTasksWithName(string taskName, Task task, ref List<Task> taskList)
		{
			if (task.FriendlyName.Equals(taskName))
			{
				taskList.Add(task);
			}
			ParentTask parentTask;
			if ((parentTask = task as ParentTask) != null && parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					FindTasksWithName(taskName, parentTask.Children[i], ref taskList);
				}
			}
		}

		public List<Task> GetActiveTasks()
		{
			if (BehaviorManager.instance == null)
			{
				return null;
			}
			return BehaviorManager.instance.GetActiveTasks(this);
		}

		public Coroutine StartTaskCoroutine(Task task, string methodName)
		{
			MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
				return null;
			}
			if (activeTaskCoroutines == null)
			{
				activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
			}
			TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[0]), methodName);
			if (activeTaskCoroutines.ContainsKey(methodName))
			{
				List<TaskCoroutine> list = activeTaskCoroutines[methodName];
				list.Add(taskCoroutine);
				activeTaskCoroutines[methodName] = list;
			}
			else
			{
				List<TaskCoroutine> list2 = new List<TaskCoroutine>();
				list2.Add(taskCoroutine);
				activeTaskCoroutines.Add(methodName, list2);
			}
			return taskCoroutine.Coroutine;
		}

		public Coroutine StartTaskCoroutine(Task task, string methodName, object value)
		{
			MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
				return null;
			}
			if (activeTaskCoroutines == null)
			{
				activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
			}
			TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[1] { value }), methodName);
			if (activeTaskCoroutines.ContainsKey(methodName))
			{
				List<TaskCoroutine> list = activeTaskCoroutines[methodName];
				list.Add(taskCoroutine);
				activeTaskCoroutines[methodName] = list;
			}
			else
			{
				List<TaskCoroutine> list2 = new List<TaskCoroutine>();
				list2.Add(taskCoroutine);
				activeTaskCoroutines.Add(methodName, list2);
			}
			return taskCoroutine.Coroutine;
		}

		public void StopTaskCoroutine(string methodName)
		{
			if (activeTaskCoroutines.ContainsKey(methodName))
			{
				List<TaskCoroutine> list = activeTaskCoroutines[methodName];
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Stop();
				}
			}
		}

		public void StopAllTaskCoroutines()
		{
			StopAllCoroutines();
			foreach (KeyValuePair<string, List<TaskCoroutine>> activeTaskCoroutine in activeTaskCoroutines)
			{
				List<TaskCoroutine> value = activeTaskCoroutine.Value;
				for (int i = 0; i < value.Count; i++)
				{
					value[i].Stop();
				}
			}
		}

		public void TaskCoroutineEnded(TaskCoroutine taskCoroutine, string coroutineName)
		{
			if (activeTaskCoroutines.ContainsKey(coroutineName))
			{
				List<TaskCoroutine> list = activeTaskCoroutines[coroutineName];
				if (list.Count == 1)
				{
					activeTaskCoroutines.Remove(coroutineName);
					return;
				}
				list.Remove(taskCoroutine);
				activeTaskCoroutines[coroutineName] = list;
			}
		}

		public void OnBehaviorStarted()
		{
			if (this.OnBehaviorStart != null)
			{
				this.OnBehaviorStart();
			}
		}

		public void OnBehaviorRestarted()
		{
			if (this.OnBehaviorRestart != null)
			{
				this.OnBehaviorRestart();
			}
		}

		public void OnBehaviorEnded()
		{
			if (this.OnBehaviorEnd != null)
			{
				this.OnBehaviorEnd();
			}
		}

		private void RegisterEvent(string name, Delegate handler)
		{
			if (eventTable == null)
			{
				eventTable = new Dictionary<Type, Dictionary<string, Delegate>>();
			}
			Dictionary<string, Delegate> value;
			if (!eventTable.TryGetValue(handler.GetType(), out value))
			{
				value = new Dictionary<string, Delegate>();
				eventTable.Add(handler.GetType(), value);
			}
			Delegate value2;
			if (value.TryGetValue(name, out value2))
			{
				value[name] = Delegate.Combine(value2, handler);
			}
			else
			{
				value.Add(name, handler);
			}
		}

		public void RegisterEvent(string name, System.Action handler)
		{
			RegisterEvent(name, (Delegate)handler);
		}

		public void RegisterEvent<T>(string name, Action<T> handler)
		{
			RegisterEvent(name, (Delegate)handler);
		}

		public void RegisterEvent<T, U>(string name, Action<T, U> handler)
		{
			RegisterEvent(name, (Delegate)handler);
		}

		public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			RegisterEvent(name, (Delegate)handler);
		}

		private Delegate GetDelegate(string name, Type type)
		{
			Dictionary<string, Delegate> value;
			Delegate value2;
			if (eventTable != null && eventTable.TryGetValue(type, out value) && value.TryGetValue(name, out value2))
			{
				return value2;
			}
			return null;
		}

		public void SendEvent(string name)
		{
			System.Action action = GetDelegate(name, typeof(System.Action)) as System.Action;
			if (action != null)
			{
				action();
			}
		}

		public void SendEvent<T>(string name, T arg1)
		{
			Action<T> action = GetDelegate(name, typeof(Action<T>)) as Action<T>;
			if (action != null)
			{
				action(arg1);
			}
		}

		public void SendEvent<T, U>(string name, T arg1, U arg2)
		{
			Action<T, U> action = GetDelegate(name, typeof(Action<T, U>)) as Action<T, U>;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3)
		{
			Action<T, U, V> action = GetDelegate(name, typeof(Action<T, U, V>)) as Action<T, U, V>;
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}

		private void UnregisterEvent(string name, Delegate handler)
		{
			Dictionary<string, Delegate> value;
			Delegate value2;
			if (eventTable != null && eventTable.TryGetValue(handler.GetType(), out value) && value.TryGetValue(name, out value2))
			{
				value[name] = Delegate.Remove(value2, handler);
			}
		}

		public void UnregisterEvent(string name, System.Action handler)
		{
			UnregisterEvent(name, (Delegate)handler);
		}

		public void UnregisterEvent<T>(string name, Action<T> handler)
		{
			UnregisterEvent(name, (Delegate)handler);
		}

		public void UnregisterEvent<T, U>(string name, Action<T, U> handler)
		{
			UnregisterEvent(name, (Delegate)handler);
		}

		public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler)
		{
			UnregisterEvent(name, (Delegate)handler);
		}

		public void SaveResetValues()
		{
			if (defaultValues == null)
			{
				defaultValues = new List<Dictionary<string, object>>();
				defaultVariableValues = new Dictionary<string, object>();
				SaveValues();
			}
			else
			{
				ResetValues();
			}
		}

		private void SaveValues()
		{
			List<SharedVariable> allVariables = mBehaviorSource.GetAllVariables();
			for (int i = 0; i < allVariables.Count; i++)
			{
				defaultVariableValues.Add(allVariables[i].Name, allVariables[i].GetValue());
			}
			SaveValue(mBehaviorSource.RootTask);
		}

		private void SaveValue(Task task)
		{
			if (task == null)
			{
				return;
			}
			FieldInfo[] publicFields = TaskUtility.GetPublicFields(task.GetType());
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int i = 0; i < publicFields.Length; i++)
			{
				object value = publicFields[i].GetValue(task);
				if (value is SharedVariable)
				{
					SharedVariable sharedVariable = value as SharedVariable;
					if (sharedVariable.IsGlobal || sharedVariable.IsShared)
					{
						continue;
					}
				}
				dictionary.Add(publicFields[i].Name, publicFields[i].GetValue(task));
			}
			defaultValues.Add(dictionary);
			if (!(task is ParentTask))
			{
				return;
			}
			ParentTask parentTask = task as ParentTask;
			if (parentTask.Children != null)
			{
				for (int j = 0; j < parentTask.Children.Count; j++)
				{
					SaveValue(parentTask.Children[j]);
				}
			}
		}

		private void ResetValues()
		{
			foreach (KeyValuePair<string, object> defaultVariableValue in defaultVariableValues)
			{
				SetVariableValue(defaultVariableValue.Key, defaultVariableValue.Value);
			}
			int index = 0;
			ResetValue(mBehaviorSource.RootTask, ref index);
		}

		private void ResetValue(Task task, ref int index)
		{
			if (task == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = defaultValues[index];
			index++;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				FieldInfo field = task.GetType().GetField(item.Key);
				if (field != null)
				{
					field.SetValue(task, item.Value);
				}
			}
			if (!(task is ParentTask))
			{
				return;
			}
			ParentTask parentTask = task as ParentTask;
			if (parentTask.Children != null)
			{
				for (int i = 0; i < parentTask.Children.Count; i++)
				{
					ResetValue(parentTask.Children[i], ref index);
				}
			}
		}

		public override string ToString()
		{
			return mBehaviorSource.ToString();
		}

		public static BehaviorManager CreateBehaviorManager()
		{
			if (BehaviorManager.instance == null && Application.isPlaying)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "Behavior Manager";
				return gameObject.AddComponent<BehaviorManager>();
			}
			return null;
		}

		int IBehavior.GetInstanceID()
		{
			return GetInstanceID();
		}
	}
}
