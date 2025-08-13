using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AvatarMirrorActor : AvatarActor
	{
		public BaseMonoAvatar owner;

		public AvatarActor ownerActor;

		public void InitFromAvatarActor(AvatarActor avatarActor, float hpRatioOfParent)
		{
			ownerID = avatarActor.runtimeID;
			owner = avatarActor.avatar;
			ownerActor = avatarActor;
			level = ownerActor.level;
			attack = ownerActor.attack;
			critical = ownerActor.critical;
			defense = ownerActor.defense;
			HP = (maxHP = hpRatioOfParent * (float)avatarActor.maxHP);
			avatarDataItem = avatarActor.avatarDataItem.Clone();
			_isOnStage = true;
			avatar.DisableShadow();
			Physics.IgnoreCollision(owner.transform.GetComponent<CapsuleCollider>(), avatar.transform.GetComponent<CapsuleCollider>());
			avatar.transform.position = owner.transform.position;
		}

		public override void PostInit()
		{
			ActorAbilityPlugin.PostInitAbilityActorPlugin(this);
			abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.RemoveAll;
		}

		public override bool OnEventWithPlugins(BaseEvent evt)
		{
			if (evt is EvtAvatarSwapInEnd || evt is EvtAvatarSwapOutStart)
			{
				return false;
			}
			return base.OnEventWithPlugins(evt);
		}

		public override void Kill(uint killerID, string killerAnimEventID, KillEffect killEffect)
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID, killerID, killerAnimEventID));
		}
	}
}
