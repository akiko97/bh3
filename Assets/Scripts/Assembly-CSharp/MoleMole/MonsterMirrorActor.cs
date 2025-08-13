using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonsterMirrorActor : MonsterActor
	{
		public BaseMonoMonster owner;

		public MonsterActor ownerActor;

		public void InitFromMonsterActor(MonsterActor monsterActor, float hpRatioOfParent)
		{
			owner = monsterActor.monster;
			ownerActor = monsterActor;
			level = ownerActor.level;
			attack = ownerActor.attack;
			defense = ownerActor.defense;
			HP = (maxHP = hpRatioOfParent * (float)monsterActor.maxHP);
			monster.DisableShadow();
			Physics.IgnoreCollision(owner.transform.GetComponent<Collider>(), monster.transform.GetComponent<Collider>());
			monster.transform.position = owner.transform.position;
		}

		public override void PostInit()
		{
			base.PostInit();
			abilityPlugin.muteEvents = true;
		}

		public override void Kill(uint killerID, string animEventID, KillEffect killEffect)
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID, killerID, animEventID));
		}
	}
}
