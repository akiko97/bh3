using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class CanMonsterAttackInCamera : Action
	{
		private const float CAMERA_CHECK_FOV_OFFSET = -5f;

		private const float CAMERA_CHECK_NEAR_OFFSET = 5f;

		private const float CAMERA_CHECK_FAR_OFFSET = 0f;

		public SharedInt avatarBeAttackNum;

		public SharedInt avatarBeAttackMaxNum;

		public SharedFloat AttackCD;

		public SharedFloat resetAttackCDPos;

		public SharedFloat resetAttackCDTime;

		private IAIEntity _aiEntity;

		public override void OnAwake()
		{
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoEntity component = GetComponent<BaseMonoMonster>();
			_aiEntity = (BaseMonoMonster)component;
			if (_aiEntity.GetProperty("AI_IgnoreMaxAttackNumChance") > 0f)
			{
				return TaskStatus.Success;
			}
			if (Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisibleInCustomOffset(component, -5f, 5f, 0f))
			{
				return CheckCanAttackWithMaxNum();
			}
			if (Singleton<CameraManager>.Instance.GetMainCamera().GetVisibleMonstersCountWithOffset(-5f, 5f, 0f) < avatarBeAttackMaxNum.Value)
			{
				return CheckCanAttackWithMaxNum();
			}
			if (Random.value < resetAttackCDPos.Value)
			{
				AttackCD.Value = resetAttackCDTime.Value;
				return TaskStatus.Failure;
			}
			return CheckCanAttackWithMaxNum();
		}

		public TaskStatus CheckCanAttackWithMaxNum()
		{
			if (avatarBeAttackNum.Value >= avatarBeAttackMaxNum.Value)
			{
				AttackCD.Value = resetAttackCDTime.Value;
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}
	}
}
