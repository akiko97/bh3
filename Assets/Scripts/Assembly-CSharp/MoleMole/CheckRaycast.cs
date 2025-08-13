using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Ability")]
	public class CheckRaycast : Action
	{
		public enum RaycastForwardType
		{
			BasedOnEntityForward = 0,
			BasedOnEntityAndTarget = 1
		}

		[BehaviorDesigner.Runtime.Tasks.Tooltip("If use target-oriented, the start forward of raycast will based on target and entity; otherwise, the forward of entity will be used.")]
		public RaycastForwardType ForwardType;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("The angle between raycast and start forward.")]
		public float Angle;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("The length of raycast.")]
		public float Distance;

		private BaseMonoAnimatorEntity _animatorEntity;

		private BaseMonoEntity _target;

		public override void OnAwake()
		{
			BaseMonoEntity component = GetComponent<BaseMonoEntity>();
			_animatorEntity = (BaseMonoAnimatorEntity)component;
			_target = GetComponent<BaseMonoAbilityEntity>().GetAttackTarget();
		}

		public override TaskStatus OnUpdate()
		{
			Vector3 vector = _animatorEntity.transform.forward;
			if (ForwardType == RaycastForwardType.BasedOnEntityAndTarget && _target != null)
			{
				vector = _target.transform.position - _animatorEntity.transform.position;
			}
			vector.y = 0f;
			vector = Vector3.Normalize(Quaternion.AngleAxis(Angle, Vector3.up) * vector);
			if (!Physics.Linecast(_animatorEntity.transform.position, _animatorEntity.transform.position + vector * Distance, 1 << InLevelData.STAGE_COLLIDER_LAYER))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
