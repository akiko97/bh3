using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class Mono_RO_040 : BaseMonoRobot
	{
		protected override void OnCollisionEnter(Collision collision)
		{
			base.OnCollisionEnter(collision);
			if (InLevelData.AVATAR_LAYER == collision.gameObject.layer || InLevelData.MONSTER_LAYER == collision.gameObject.layer)
			{
				BaseMonoEntity baseMonoEntity = ((InLevelData.AVATAR_LAYER != collision.gameObject.layer) ? ((BaseMonoEntity)collision.gameObject.GetComponent<BaseMonoMonster>()) : ((BaseMonoEntity)collision.gameObject.GetComponent<BaseMonoAvatar>()));
				if (baseMonoEntity != null)
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtTouch(GetRuntimeID(), baseMonoEntity.GetRuntimeID()));
				}
			}
		}

		public override void SetDied(KillEffect killEffect)
		{
			base.SetDied(KillEffect.KillImmediately);
		}
	}
}
