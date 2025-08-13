using UnityEngine;

namespace MoleMole
{
	public class StateRebindAttachPoint : StateMachineBehaviour
	{
		[Header("Rebind AttachPoint to another attach point")]
		public string AttachPoint;

		[Header("Other Attach Point")]
		public string OtherAttachPoint;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BaseMonoAnimatorEntity component = animator.GetComponent<BaseMonoAnimatorEntity>();
			component.RebindAttachPoint(AttachPoint, OtherAttachPoint);
		}
	}
}
