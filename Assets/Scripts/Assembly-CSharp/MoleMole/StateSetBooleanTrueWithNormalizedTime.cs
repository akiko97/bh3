using UnityEngine;

namespace MoleMole
{
	public class StateSetBooleanTrueWithNormalizedTime : StateMachineBehaviour
	{
		[Header("Target's value will be set to true Between normalizedTimeBegin and normalizedTimeEnd, else set false")]
		public float normalizedTimeBegin;

		public float normalizedTimeEnd = 1f;

		public string target;

		public bool resetOnExit = true;

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= normalizedTimeBegin && stateInfo.normalizedTime <= normalizedTimeEnd)
			{
				animator.SetBool(target, true);
			}
			else
			{
				animator.SetBool(target, false);
			}
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
