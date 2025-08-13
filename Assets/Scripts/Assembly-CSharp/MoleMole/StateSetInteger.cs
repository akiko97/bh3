using UnityEngine;

namespace MoleMole
{
	public class StateSetInteger : StateMachineBehaviour
	{
		[Header("Set the value of an integer parameter.")]
		public string Name;

		public int Value;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetInteger(Name, Value);
		}
	}
}
