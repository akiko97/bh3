using System;
using FullInspector;

namespace MoleMole.Config
{
	public class ConfigAvatarSkill
	{
		public string[] AnimatorStateNames;

		public string AnimatorEventPattern;

		public AvatarSkillType SkillType;

		[NonSerialized]
		public float SPCost;

		[NonSerialized]
		public float SPNeed;

		public DynamicFloat SPCostDelta = DynamicFloat.ZERO;

		[NonSerialized]
		public float SkillCD;

		public DynamicFloat SkillCDDelta = DynamicFloat.ZERO;

		public bool CanHold;

		public bool MuteHighlighted;

		[NonSerialized]
		public int ChargesCount;

		public DynamicInt ChargesCountDelta = DynamicInt.ZERO;

		public bool HaveBranch;

		public bool IsInstantTrigger;

		public string InstantTriggerEvent;

		public bool ForceMuteSteer;

		public float BranchHighlightNormalizedTimeStart;

		public float BranchHighlightNormalizedTimeStop;

		public float AnimDefenceRatio;

		public float AnimDefenceNormalizedTimeStart;

		public float AnimDefenceNormalizedTimeStop = 1f;

		public float ComboTimerPauseNormalizedTimeStart;

		public float ComboTimerPauseNormalizedTimeStop;

		public string LastKillCameraAnimation;

		public float AttackNormalizedTimeStart;

		public float AttackNormalizedTimeStop;

		public SkillEnterSetting EnterSteer;

		[InspectorNullable]
		public SkillEnterSteerOption EnterSteerOption;

		public bool HighSpeedMovement;

		public float MassRatio = 1f;

		public bool NeedClearEffect;

		public bool MuteCameraControl;

		public ReviveSkillCDAction ReviveCDAction;

		[InspectorNullable]
		public AttackResult.AttackCategoryTag[] SkillCategoryTag;
	}
}
