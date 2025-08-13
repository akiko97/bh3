using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskIcon("{SkinColor}LogIcon.png")]
	[TaskDescription("Log is a simple task which will output the specified text and return success. It can be used for debugging.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=16")]
	public class Log : Action
	{
		[Tooltip("Text to output to the log")]
		public SharedString text;

		[Tooltip("Is this text an error?")]
		public SharedBool logError;

		public override TaskStatus OnUpdate()
		{
			if (logError.Value)
			{
				Debug.LogError(text);
			}
			else
			{
				Debug.Log(text);
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			text = string.Empty;
			logError = false;
		}
	}
}
