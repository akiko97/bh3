using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=20")]
	[TaskIcon("{SkinColor}StartBehaviorTreeIcon.png")]
	[TaskDescription("Start a new behavior tree and return success after it has been started.")]
	public class StartBehaviorTree : Action
	{
		[Tooltip("The GameObject of the behavior tree that should be started. If null use the current behavior")]
		public SharedGameObject behaviorGameObject;

		[Tooltip("The group of the behavior tree that should be started")]
		public SharedInt group;

		[Tooltip("Should this task wait for the behavior tree to complete?")]
		public SharedBool waitForCompletion = false;

		[Tooltip("Should the variables be synchronized?")]
		public SharedBool synchronizeVariables;

		private bool behaviorComplete;

		private Behavior behavior;

		public override void OnStart()
		{
			Behavior[] components = GetDefaultGameObject(behaviorGameObject.Value).GetComponents<Behavior>();
			if (components.Length == 1)
			{
				behavior = components[0];
			}
			else if (components.Length > 1)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].Group == group.Value)
					{
						behavior = components[i];
						break;
					}
				}
				if (behavior == null)
				{
					behavior = components[0];
				}
			}
			if (behavior != null)
			{
				List<SharedVariable> allVariables = base.Owner.GetAllVariables();
				for (int j = 0; j < allVariables.Count; j++)
				{
					behavior.SetVariable(allVariables[j].Name, allVariables[j]);
				}
				behavior.EnableBehavior();
				if (waitForCompletion.Value)
				{
					behaviorComplete = false;
					behavior.OnBehaviorEnd += BehaviorEnded;
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (behavior == null)
			{
				return TaskStatus.Failure;
			}
			if (waitForCompletion.Value && !behaviorComplete)
			{
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}

		private void BehaviorEnded()
		{
			behaviorComplete = true;
		}

		public override void OnEnd()
		{
			if (behavior != null && waitForCompletion.Value)
			{
				behavior.OnBehaviorEnd -= BehaviorEnded;
			}
		}

		public override void OnReset()
		{
			behaviorGameObject = null;
			group = 0;
			waitForCompletion = false;
			synchronizeVariables = false;
		}
	}
}
