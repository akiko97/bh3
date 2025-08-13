using UnityEngine;

namespace MoleMole
{
	public class StateBaseLayer : StateMachineBehaviour
	{
		private BaseMonoAnimatorEntity _entity;

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_entity == null)
			{
				_entity = animator.GetComponent<BaseMonoAnimatorEntity>();
			}
			if (layerIndex == 0)
			{
				_entity.AddFrameExitedAnimatorStates(stateInfo);
			}
		}
	}
}
