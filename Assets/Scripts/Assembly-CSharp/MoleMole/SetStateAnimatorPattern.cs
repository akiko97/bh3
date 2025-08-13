using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class SetStateAnimatorPattern : Action
	{
		public string stateName;

		public string animatorEventPatternName;

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoMonster)
			{
				BaseMonoMonster baseMonoMonster = component as BaseMonoMonster;
				baseMonoMonster.SetSoleAnimatorEventPattern(Animator.StringToHash(stateName), animatorEventPatternName);
			}
			return TaskStatus.Success;
		}
	}
}
