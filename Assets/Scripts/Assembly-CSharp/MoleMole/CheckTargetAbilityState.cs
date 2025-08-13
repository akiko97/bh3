using System;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Monster")]
	[TaskDescription("Check whether target in is any ability state, e.g. stun, paralyze, etc.")]
	public class CheckTargetAbilityState : BehaviorDesigner.Runtime.Tasks.Action
	{
		public string state;

		private IAIEntity _aiEntity;

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			_aiEntity = (BaseMonoMonster)component;
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = _aiEntity.AttackTarget as BaseMonoAnimatorEntity;
			if (baseMonoAnimatorEntity == null)
			{
				return TaskStatus.Failure;
			}
			AbilityState targetState = (AbilityState)(int)Enum.Parse(typeof(AbilityState), state);
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(baseMonoAnimatorEntity.GetRuntimeID());
			if (actor != null && actor.abilityState.ContainsState(targetState))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
