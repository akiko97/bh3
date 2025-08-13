using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class NetworkedMP_PvPTest_GameMode : NetworkedMP_Default_GameMode
	{
		private List<uint>[] _peerGroups;

		public NetworkedMP_PvPTest_GameMode()
		{
			_peerGroups = new List<uint>[7];
			for (int i = 0; i < _peerGroups.Length; i++)
			{
				_peerGroups[i] = new List<uint>();
			}
		}

		public override void RegisterRuntimeID(uint runtimeID)
		{
			if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
			{
				uint num = Singleton<RuntimeIDManager>.Instance.ParsePeerID(runtimeID);
				_peerGroups[num].Add(runtimeID);
			}
		}

		public override void DestroyRuntimeID(uint runtimeID)
		{
			if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
			{
				uint num = Singleton<RuntimeIDManager>.Instance.ParsePeerID(runtimeID);
				_peerGroups[num].Remove(runtimeID);
			}
		}

		public override bool IsEnemy(uint fromID, uint toID)
		{
			if (!Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(fromID))
			{
				BaseActor actor = Singleton<EventManager>.Instance.GetActor(fromID);
				if (actor == null)
				{
					return false;
				}
				fromID = actor.ownerID;
			}
			if (!Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(toID))
			{
				BaseActor actor2 = Singleton<EventManager>.Instance.GetActor(toID);
				if (actor2 == null)
				{
					return false;
				}
				toID = actor2.ownerID;
			}
			return Singleton<RuntimeIDManager>.Instance.ParsePeerID(fromID) != Singleton<RuntimeIDManager>.Instance.ParsePeerID(toID);
		}

		public override T[] GetEnemyActorsOf<T>(BaseActor actor)
		{
			uint num = Singleton<RuntimeIDManager>.Instance.ParsePeerID(actor.runtimeID);
			List<T> list = new List<T>();
			for (int i = 1; i < _peerGroups.Length; i++)
			{
				if (i == num)
				{
					continue;
				}
				List<uint> list2 = _peerGroups[i];
				for (int j = 0; j < list2.Count; j++)
				{
					for (int k = 0; k < list2.Count; k++)
					{
						T actor2 = Singleton<EventManager>.Instance.GetActor<T>(list2[k]);
						if (actor2 != null)
						{
							list.Add(actor2);
						}
					}
				}
			}
			return list.ToArray();
		}

		public override T[] GetAlliedActorsOf<T>(BaseActor actor)
		{
			uint num = Singleton<RuntimeIDManager>.Instance.ParsePeerID(actor.runtimeID);
			List<T> list = new List<T>();
			List<uint> list2 = _peerGroups[num];
			for (int i = 0; i < list2.Count; i++)
			{
				T actor2 = Singleton<EventManager>.Instance.GetActor<T>(list2[i]);
				if (actor2 != null)
				{
					list.Add(actor2);
				}
			}
			return list.ToArray();
		}

		public override LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID))
			{
			case 3:
				return (1 << InLevelData.AVATAR_HITBOX_LAYER) | (1 << InLevelData.MONSTER_HITBOX_LAYER) | (1 << InLevelData.PROP_HITBOX_LAYER);
			case 4:
				return (1 << InLevelData.AVATAR_HITBOX_LAYER) | (1 << InLevelData.MONSTER_HITBOX_LAYER);
			case 7:
				return (1 << InLevelData.AVATAR_HITBOX_LAYER) | (1 << InLevelData.MONSTER_HITBOX_LAYER);
			default:
				return 0;
			}
		}

		public override LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
		{
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID))
			{
			case 3:
				return (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER) | (1 << InLevelData.PROP_LAYER);
			case 4:
				return (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER);
			case 7:
				return (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER);
			default:
				return 0;
			}
		}

		public override LayerMask GetAttackPatternDefaultLayerMask(uint runtimeID)
		{
			return (1 << InLevelData.MONSTER_HITBOX_LAYER) | (1 << InLevelData.AVATAR_HITBOX_LAYER) | (1 << InLevelData.PROP_HITBOX_LAYER);
		}
	}
}
