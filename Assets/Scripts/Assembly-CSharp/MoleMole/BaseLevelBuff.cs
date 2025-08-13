using UnityEngine;

namespace MoleMole
{
	public abstract class BaseLevelBuff : BaseActorPlugin
	{
		[HideInInspector]
		public LevelActor levelActor;

		public LevelBuffType levelBuffType;

		public LevelBuffSide levelBuffSide;

		public bool isActive;

		public uint ownerID;

		public bool muteUpdateDuration;
	}
}
