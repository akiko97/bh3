using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class ResetAnimatorTriggers : Action
	{
		public List<string> triggerNamesList;

		private Animator _animator;

		public override void OnAwake()
		{
			_animator = GetComponent<Animator>();
		}

		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < triggerNamesList.Count; i++)
			{
				_animator.ResetTrigger(triggerNamesList[i]);
			}
			return TaskStatus.Success;
		}
	}
}
