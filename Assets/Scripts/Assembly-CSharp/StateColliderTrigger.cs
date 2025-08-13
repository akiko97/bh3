using MoleMole;
using UnityEngine;

public class StateColliderTrigger : StateMachineBehaviour
{
	public bool isActive;

	private bool lastActiveAnim;

	private bool lastActiveHit;

	private Collider animCollider;

	private Collider hitCollider;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animCollider = animator.GetComponent<Collider>();
		if (animCollider != null)
		{
			lastActiveAnim = animCollider.enabled;
			animCollider.enabled = isActive;
		}
		BaseMonoMonster component = animator.GetComponent<BaseMonoMonster>();
		if (component != null)
		{
			hitCollider = component.hitbox;
			if (hitCollider != null)
			{
				lastActiveHit = hitCollider.enabled;
				hitCollider.enabled = isActive;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animCollider != null)
		{
			animCollider.enabled = lastActiveAnim;
		}
		if (hitCollider != null)
		{
			hitCollider.enabled = lastActiveHit;
		}
	}
}
