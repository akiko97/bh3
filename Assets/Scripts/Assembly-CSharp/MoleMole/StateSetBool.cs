using UnityEngine;

namespace MoleMole
{
	public class StateSetBool : StateMachineBehaviour
	{
		[Header("Target Bool")]
		public string target;

		public bool setOnEnter;

		public bool enterValue;

		public bool setOnExit;

		public bool exitValue;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (setOnEnter)
			{
				animator.SetBool(Animator.StringToHash(target), enterValue);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (setOnExit)
			{
				animator.SetBool(Animator.StringToHash(target), exitValue);
			}
		}
	}
}
