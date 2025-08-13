using UnityEngine;

namespace MoleMole
{
	public class AvatarControlData
	{
		public static AvatarControlData emptyControlData = new AvatarControlData();

		public bool hasSteer;

		public Vector3 steerDirection;

		public float lerpRatio;

		public bool hasOrderMove;

		public bool orderMove;

		public bool hasSetAttackTarget;

		public BaseMonoEntity attackTarget;

		public bool useAttack;

		public bool useHoldAttack;

		public bool[] useSkills = new bool[4];

		public bool hasAnyControl;

		public void FrameReset()
		{
			hasAnyControl = false;
			hasSteer = false;
			hasOrderMove = false;
			hasSetAttackTarget = false;
			useAttack = false;
			useHoldAttack = false;
			for (int i = 0; i < useSkills.Length; i++)
			{
				useSkills[i] = false;
			}
		}

		public AvatarControlData CopyFrom(AvatarControlData controlData)
		{
			hasSteer = controlData.hasSteer;
			steerDirection = controlData.steerDirection;
			lerpRatio = controlData.lerpRatio;
			hasOrderMove = controlData.hasOrderMove;
			orderMove = controlData.orderMove;
			hasSetAttackTarget = controlData.hasSetAttackTarget;
			attackTarget = controlData.attackTarget;
			useAttack = controlData.useAttack;
			useHoldAttack = controlData.useHoldAttack;
			hasAnyControl = controlData.hasAnyControl;
			controlData.useSkills.CopyTo(useSkills, 0);
			return null;
		}
	}
}
