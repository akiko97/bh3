using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Ability/CheckHasAbility")]
	public class CheckHasAbility : Action
	{
		public string abilityName;

		private BaseAbilityActor _actor;

		public override void OnStart()
		{
			BaseMonoEntity component = GetComponent<BaseMonoEntity>();
			_actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(component.GetRuntimeID());
		}

		public override TaskStatus OnUpdate()
		{
			if (_actor == null || _actor.abilityPlugin == null)
			{
				return TaskStatus.Failure;
			}
			if (_actor.abilityPlugin.HasAbility(abilityName))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
