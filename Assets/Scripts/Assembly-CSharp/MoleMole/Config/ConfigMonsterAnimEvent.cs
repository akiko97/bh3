using FullInspector;

namespace MoleMole.Config
{
	public class ConfigMonsterAnimEvent : ConfigEntityAnimEvent
	{
		[InspectorNullable]
		public ConfigMonsterAttackHint AttackHint;

		[InspectorNullable]
		public ConfigEntityPhysicsProperty PhysicsProperty;
	}
}
