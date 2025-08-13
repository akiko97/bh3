using UnityEngine;

namespace MoleMole
{
	public class StateAvatarDefaultEmpty : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
			component.transform.localScale = Vector3.zero;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
			component.transform.localScale = Vector3.one;
		}
	}
}
