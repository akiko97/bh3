using UnityEngine;

namespace MoleMole
{
	[SharedBetweenAnimators]
	public class StateAvatarRunBS : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
			component.RunBSStart();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
			component.RunBSStop();
		}
	}
}
