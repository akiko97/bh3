using UnityEngine;

namespace MoleMole
{
	public class MonoEndlessSettlementAnimation : MonoBehaviour
	{
		public void OnAnimationEnd()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessSettlementAnimationEnd));
		}
	}
}
