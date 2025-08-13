using System;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseAvatarInputController : BaseAvatarController
	{
		protected uint _controlType;

		public BaseAvatarInputController(uint controllerType, BaseMonoEntity avatar)
			: base(avatar)
		{
			_controlType = 1u;
		}

		public override void SetActive(bool isActive)
		{
			base.active = isActive;
		}

		private void InitInputStick()
		{
		}

		public override void Core()
		{
		}

		public void TryHold(string skillName)
		{
			switch (skillName)
			{
			case "ATK":
				TryHoldAttack();
				break;
			}
		}

		public void TryUseSkill(string skillName)
		{
			switch (skillName)
			{
			case "ATK":
				TryAttack();
				break;
			case "SKL01":
				TryUseSkill(1);
				break;
			case "SKL02":
				TryUseSkill(2);
				break;
			case "SKL_WEAPON":
				TryUseSkill(3);
				break;
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public void TryMove(bool isMoving, float angle)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			Vector3 xZPosition = base.avatar.XZPosition;
			Vector3 vector;
			if (mainCamera.followState.followAvatarAndBossState.active)
			{
				vector = mainCamera.transform.forward;
			}
			else if (mainCamera.followState.followAvatarAndCrowdState.active)
			{
				vector = mainCamera.transform.forward;
			}
			else
			{
				Vector3 xZPosition2 = mainCamera.XZPosition;
				vector = xZPosition - xZPosition2;
			}
			vector.y = 0f;
			vector.Normalize();
			Vector3 dir = Quaternion.AngleAxis(0f - angle, Vector3.up) * vector;
			dir.Normalize();
			TryOrderMove(isMoving);
			if (isMoving)
			{
				TrySteer(dir, base.avatar.config.StateMachinePattern.ChangeDirLerpRatioForMove);
			}
		}
	}
}
