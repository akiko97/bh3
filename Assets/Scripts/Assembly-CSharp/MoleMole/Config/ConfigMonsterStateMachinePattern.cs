using FullInspector;

namespace MoleMole.Config
{
	public class ConfigMonsterStateMachinePattern : ConfigEntityStateMachinePattern
	{
		public string AIMode;

		public float ThrowAnimDefenceRatio;

		[InspectorNullable]
		public string ThrowDieEffectPattern;

		[InspectorNullable]
		public string FastDieEffectPattern;

		public float FastDieAnimationWaitDuration = 0.5f;

		[InspectorNullable]
		public string ThrowUpNamedState;

		public float ThrowUpNamedStateRetreatStopNormalizedTime = 1f;

		[InspectorNullable]
		public string ThrowBlowNamedState;

		[InspectorNullable]
		public string ThrowBlowDieNamedState;

		[InspectorNullable]
		public string ThrowBlowAirNamedState;

		public float ThrowBlowAirNamedStateRetreatStopNormalizedTime = 1f;

		public float RetreatToVelocityScaleRatio = 0.02f;

		public float RetreatBlowVelocityRatio = 1f;

		public float HeavyRetreatThreshold = 25f;

		public bool UseRandomLeftRightHitEffectAsNormal;

		public bool UseBackHitAngleCheck;

		public float BackHitDegreeThreshold = 60f;

		public bool UseLeftRightHitAngleCheck;

		public float LeftRightHitAngleRange = 60f;

		public bool UseStandByWalkSteer;

		public float WalkSteerTimeThreshold = 0.3f;

		public string WalkSteerAnimatorStateName;

		public bool KeepHitboxStanding;

		public float KeepHitboxStandingMinHeight = 0.7f;

		public bool UseAbsMoveSpeed;

		public ConfigEntityAttackEffect BeHitEffect;

		public ConfigEntityAttackEffect BeHitEffectMid;

		public ConfigEntityAttackEffect BeHitEffectBig;

		public string DieAnimEventID;
	}
}
