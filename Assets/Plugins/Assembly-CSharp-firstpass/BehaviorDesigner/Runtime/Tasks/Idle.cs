namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns a TaskStatus of running. Will only stop when interrupted or a conditional abort is triggered.")]
	[TaskIcon("{SkinColor}IdleIcon.png")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=112")]
	public class Idle : Action
	{
		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Running;
		}
	}
}
