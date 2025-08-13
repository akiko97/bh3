using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class SyncWithSharedEntity : Action
	{
		private const float TELEPORT_DISTANCE = 1f;

		public SharedEntity targetEntity;

		public SharedFloat offsetRadius;

		public SharedFloat offsetAngle;

		public bool syncOneFrame;

		public SharedFloat teleportTimeSpan;

		public bool syncWithAttackTarget;

		private bool isTeleporting;

		private float teleportTimer;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override TaskStatus OnUpdate()
		{
			if (targetEntity.Value != null)
			{
				BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
				if (syncWithAttackTarget && (targetEntity.Value as BaseMonoMonster).GetAttackTarget() == null)
				{
					return TaskStatus.Failure;
				}
				Vector3 targetPostion = GetTargetPostion();
				if (isTeleporting)
				{
					if (teleportTimeSpan.Value > 0f)
					{
						if (teleportTimer >= teleportTimeSpan.Value)
						{
							isTeleporting = false;
						}
					}
					else if (component.gameObject.activeSelf)
					{
						component.FireEffect("Monster_TeleportTo_Small");
						isTeleporting = false;
					}
				}
				if (Vector3.Distance(component.XZPosition, targetPostion) > 1f)
				{
					if (teleportTimeSpan.Value > 0f)
					{
						if (!isTeleporting)
						{
							teleportTimer = 0f;
							isTeleporting = true;
						}
					}
					else if (component.gameObject.activeSelf)
					{
						component.FireEffect("Monster_TeleportFrom_Small");
						isTeleporting = true;
					}
				}
				if (syncWithAttackTarget)
				{
					component.transform.forward = (targetPostion - component.transform.position).normalized;
				}
				else
				{
					component.transform.forward = targetEntity.Value.transform.forward;
				}
				if (isTeleporting && teleportTimeSpan.Value > 0f)
				{
					teleportTimer += Time.deltaTime;
					component.transform.position = Vector3.Lerp(component.transform.position, targetPostion, teleportTimer / teleportTimeSpan.Value);
				}
				else
				{
					component.transform.position = targetPostion;
				}
				component.SetLocomotionBool("IsMove", (targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionBool("IsMove"));
				component.SetLocomotionBool("IsMoveHorizontal", (targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionBool("IsMoveHorizontal"));
				component.SetLocomotionFloat("MoveSpeed", (targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionFloat("MoveSpeed"));
				component.SetLocomotionFloat("AbsMoveSpeed", (targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionFloat("AbsMoveSpeed"));
				if (syncOneFrame && !isTeleporting)
				{
					return TaskStatus.Success;
				}
				return TaskStatus.Running;
			}
			return TaskStatus.Failure;
		}

		private Vector3 GetTargetPostion()
		{
			Vector3 result;
			Vector3 vector;
			if (syncWithAttackTarget)
			{
				result = (targetEntity.Value as BaseMonoMonster).GetAttackTarget().XZPosition;
				vector = -targetEntity.Value.transform.forward;
			}
			else
			{
				result = targetEntity.Value.XZPosition;
				vector = targetEntity.Value.transform.forward;
			}
			if (offsetRadius.Value != 0f)
			{
				Vector3 vector2 = Quaternion.Euler(0f, offsetAngle.Value, 0f) * vector;
				result += vector2 * offsetRadius.Value;
			}
			bool flag = false;
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			Vector3 vector3 = new Vector3(component.transform.position.x, 0.1f, component.transform.position.z);
			Vector3 vector4 = new Vector3(result.x, 0.1f, result.z);
			RaycastHit hitInfo;
			if (Physics.Linecast(vector3, vector4, out hitInfo, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
			{
				flag = true;
			}
			if (flag)
			{
				Vector3 point = hitInfo.point;
				Vector3 normalized = (vector3 - vector4).normalized;
				float num = 0.1f;
				Vector3 vector5 = point + normalized * num;
				result = new Vector3(vector5.x, result.y, vector5.z);
			}
			return result;
		}
	}
}
