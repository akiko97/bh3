using UnityEngine;

namespace MoleMole
{
	public class StateSetBooleanTrue : StateMachineBehaviour
	{
		[Header("Target's value will be set to true on enter.")]
		public string target;

		public bool resetOnExit = true;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(target, true);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (resetOnExit)
			{
				animator.SetBool(target, false);
			}
		}
	}
}
