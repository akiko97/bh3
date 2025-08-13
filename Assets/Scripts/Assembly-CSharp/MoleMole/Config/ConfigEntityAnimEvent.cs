using FullInspector;

namespace MoleMole.Config
{
	public abstract class ConfigEntityAnimEvent
	{
		public string Predicate = "Always";

		public string Predicate2 = "Always";

		[InspectorNullable]
		public ConfigEntityAttackPattern AttackPattern;

		[InspectorNullable]
		public ConfigEntityAttackProperty AttackProperty;

		[InspectorNullable]
		public ConfigEntityCameraShake CameraShake;

		[InspectorNullable]
		public ConfigEntityAttackEffect AttackEffect;

		[InspectorNullable]
		public ConfigEntityTriggerAbility TriggerAbility;

		[InspectorNullable]
		public ConfigTriggerEffectPattern TriggerEffectPattern;

		[InspectorNullable]
		public ConfigTimeSlow TimeSlow;

		[InspectorNullable]
		public ConfigTintCamera TriggerTintCamera;
	}
}
