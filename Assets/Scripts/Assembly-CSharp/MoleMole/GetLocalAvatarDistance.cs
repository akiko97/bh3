using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class GetLocalAvatarDistance : Action
	{
		public SharedFloat Distance;

		public override TaskStatus OnUpdate()
		{
			Vector3 xZPosition = GetComponent<BaseMonoAvatar>().XZPosition;
			Vector3 xZPosition2 = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
			Distance.SetValue(Vector3.Distance(xZPosition, xZPosition2));
			return TaskStatus.Success;
		}
	}
}
