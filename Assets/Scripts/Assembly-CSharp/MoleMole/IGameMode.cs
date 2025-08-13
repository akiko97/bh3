using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public interface IGameMode
	{
		void HandleLocalPlayerAvatarDie(BaseMonoAvatar diedAvatar);

		bool ShouldAttackPatternSendBeingHit(uint beHitEntityID);

		LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID);

		void RegisterRuntimeID(uint runtimeID);

		void DestroyRuntimeID(uint runtimeID);

		bool IsEnemy(uint fromID, uint toID);

		T[] GetEnemyActorsOf<T>(BaseActor actor) where T : BaseActor;

		T[] GetAlliedActorsOf<T>(BaseActor actor) where T : BaseActor;

		LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting);

		LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting);
	}
}
