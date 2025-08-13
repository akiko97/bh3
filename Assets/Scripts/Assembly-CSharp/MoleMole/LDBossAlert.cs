using UnityEngine;

namespace MoleMole
{
	public class LDBossAlert : BaseLDEvent
	{
		private const float WAIT_DURATION = 2f;

		private EntityTimer _timer;

		public LDBossAlert()
		{
			_timer = new EntityTimer(2f);
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (baseMonoAvatar == null)
			{
				Done();
				return;
			}
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("InLevel_BossAlert", baseMonoAvatar.XZPosition, baseMonoAvatar.FaceDirection, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			_timer.Reset(true);
		}

		public override void Core()
		{
			_timer.Core(1f);
			if (_timer.isTimeUp)
			{
				PlayBossBornSound();
				Done();
			}
		}

		private void PlayBossBornSound()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			MonoEntityAudio component = localAvatar.GetComponent<MonoEntityAudio>();
			if (component != null)
			{
				component.PostBossBorn();
			}
		}
	}
}
