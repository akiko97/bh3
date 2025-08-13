using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class OriginalSPGameMode : IGameMode
	{
		public virtual void HandleLocalPlayerAvatarDie(BaseMonoAvatar diedAvatar)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Invalid comparison between Unknown and I4
			BaseMonoAvatar firstAliveAvatar = Singleton<AvatarManager>.Instance.GetFirstAliveAvatar();
			if (firstAliveAvatar != null)
			{
				diedAvatar.gameObject.SetActive(false);
				Singleton<LevelManager>.Instance.levelActor.SwapLocalAvatar(diedAvatar.GetRuntimeID(), firstAliveAvatar.GetRuntimeID());
				if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
				{
					Singleton<LevelManager>.Instance.levelActor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig("Level_AvatarReviveInvincible"), firstAliveAvatar.GetRuntimeID(), 2f);
				}
				return;
			}
			Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_On");
			if ((int)Singleton<LevelScoreManager>.Instance.LevelType == 4)
			{
				Singleton<LevelManager>.Instance.SetPause(false);
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseAllDead));
				Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(true);
			}
			else if (Singleton<LevelDesignManager>.Instance.state == LevelDesignManager.LDState.Running)
			{
				Singleton<LevelManager>.Instance.SetPause(true);
				Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(Singleton<AvatarManager>.Instance.GetTeamLeader().GetRuntimeID(), diedAvatar.XZPosition, true));
				Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(true);
			}
		}

		public virtual bool ShouldAttackPatternSendBeingHit(uint beHitEntityID)
		{
			return false;
		}

		public virtual LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID)
		{
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID))
			{
			case 3:
				return (1 << InLevelData.MONSTER_HITBOX_LAYER) | (1 << InLevelData.PROP_HITBOX_LAYER);
			case 4:
				return 1 << InLevelData.AVATAR_HITBOX_LAYER;
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public virtual void RegisterRuntimeID(uint runtimeID)
		{
		}

		public virtual void DestroyRuntimeID(uint runtimeID)
		{
		}

		private ushort GetEnemyCategory(ushort category)
		{
			ushort result = 0;
			switch (category)
			{
			case 3:
				result = 4;
				break;
			case 4:
				result = 3;
				break;
			case 1:
				result = 1;
				break;
			}
			return result;
		}

		public virtual T[] GetEnemyActorsOf<T>(BaseActor actor) where T : BaseActor
		{
			return Singleton<EventManager>.Instance.GetActorByCategory<T>(GetEnemyCategory(Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID)));
		}

		public virtual T[] GetAlliedActorsOf<T>(BaseActor actor) where T : BaseActor
		{
			return Singleton<EventManager>.Instance.GetActorByCategory<T>(Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID));
		}

		public virtual LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			ushort num;
			ushort category;
			if (ownerID == 562036737 || Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7)
			{
				num = 4;
				category = 3;
			}
			else
			{
				num = Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID);
				category = GetEnemyCategory(num);
			}
			LayerMask layerMask;
			switch (targetting)
			{
			case MixinTargetting.None:
				layerMask = 0;
				break;
			case MixinTargetting.Allied:
				layerMask = InLevelData.GetLayerMask(num);
				break;
			case MixinTargetting.Enemy:
				layerMask = InLevelData.GetLayerMask(category);
				if (num == 3)
				{
					layerMask = (int)layerMask | (1 << (InLevelData.PROP_LAYER & 0x1F));
				}
				break;
			case MixinTargetting.All:
				layerMask = (int)InLevelData.GetLayerMask(num) | (int)InLevelData.GetLayerMask(category);
				if (num == 3)
				{
					layerMask = (int)layerMask | (1 << (InLevelData.PROP_LAYER & 0x1F));
				}
				break;
			default:
				layerMask = 0;
				break;
			}
			return layerMask;
		}

		public virtual LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			ushort num;
			ushort category;
			if (ownerID == 562036737 || Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7)
			{
				num = 4;
				category = 3;
			}
			else
			{
				num = Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID);
				category = GetEnemyCategory(num);
			}
			if (ownerID == 562036737 || Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 7)
			{
				num = 4;
				category = 3;
			}
			LayerMask layerMask;
			switch (targetting)
			{
			case MixinTargetting.None:
				layerMask = 0;
				break;
			case MixinTargetting.Allied:
				layerMask = InLevelData.GetHitboxLayerMask(num);
				break;
			case MixinTargetting.Enemy:
				layerMask = InLevelData.GetHitboxLayerMask(category);
				if (num == 3)
				{
					layerMask = (int)layerMask | (1 << (InLevelData.PROP_HITBOX_LAYER & 0x1F));
				}
				break;
			case MixinTargetting.All:
				layerMask = (int)InLevelData.GetHitboxLayerMask(num) | (int)InLevelData.GetHitboxLayerMask(category);
				if (num == 3)
				{
					layerMask = (int)layerMask | (1 << (InLevelData.PROP_HITBOX_LAYER & 0x1F));
				}
				break;
			default:
				layerMask = 0;
				break;
			}
			return layerMask;
		}

		public virtual bool IsEnemy(uint fromID, uint toID)
		{
			return true;
		}
	}
}
