using UnityEngine;

namespace MoleMole
{
	public class MonoCabinOverViewAnimation : MonoBehaviour
	{
		public Animation _animation;

		public void AnimationStop()
		{
			_animation.Stop();
		}

		public void AnimationStart()
		{
			_animation.Play();
		}
	}
}
