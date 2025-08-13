using FullInspector;

namespace MoleMole.Config
{
	public class ConfigAvatarAnimEvent : ConfigEntityAnimEvent
	{
		[InspectorNullable]
		public ConfigAvatarPhysicsProperty PhysicsProperty;

		[InspectorNullable]
		public ConfigAvatarCameraAction CameraAction;

		[InspectorNullable]
		public ConfigLastKillCameraAnimation LastKillCameraAnimation;

		[InspectorNullable]
		public ConfigWitchTimeResume WitchTimeResume;

		[InspectorNullable]
		public ConfigMissionSpecificKill MissionSpecificKill;
	}
}
