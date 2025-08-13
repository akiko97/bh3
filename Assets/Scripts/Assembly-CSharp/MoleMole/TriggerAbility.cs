using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Ability/TriggerAbility")]
	public class TriggerAbility : Action
	{
		public string abilityName;

		public SharedFloat abilityArgument;

		public bool isRandom;

		public float randomMin;

		public float randomMax;

		public SharedFloat ratio;

		private BaseMonoEntity _entity;

		public override void OnAwake()
		{
			_entity = GetComponent<BaseMonoEntity>();
		}

		public override TaskStatus OnUpdate()
		{
			EvtAbilityStart evtAbilityStart = new EvtAbilityStart(_entity.GetRuntimeID());
			evtAbilityStart.abilityName = abilityName;
			float num = 1f;
			if (ratio.IsShared)
			{
				num = ratio.Value;
			}
			if (isRandom)
			{
				evtAbilityStart.abilityArgument = Random.Range(randomMin, randomMax) * num;
			}
			else
			{
				evtAbilityStart.abilityArgument = abilityArgument.Value * num;
			}
			Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			return TaskStatus.Success;
		}
	}
}
