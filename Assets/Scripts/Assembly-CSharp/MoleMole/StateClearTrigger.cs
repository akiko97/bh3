using UnityEngine;

namespace MoleMole
{
	public class StateClearTrigger : StateMachineBehaviour
	{
		[Header("Target trigger")]
		public string target;

		public bool clearOnEnter;

		public bool clearOnExit;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (clearOnEnter)
			{
				animator.ResetTrigger(target);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (clearOnExit)
			{
				animator.ResetTrigger(target);
			}
		}
	}
}
