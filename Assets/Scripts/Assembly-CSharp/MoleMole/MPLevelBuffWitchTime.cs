namespace MoleMole
{
	public class MPLevelBuffWitchTime : LevelBuffWitchTime
	{
		public MPLevelBuffWitchTime(MPLevelActor mpLevelActor)
			: base(mpLevelActor)
		{
		}

		private bool IsPvP()
		{
			return Singleton<LevelManager>.Instance.gameMode is NetworkedMP_PvPTest_GameMode;
		}

		public override bool Refresh(float duration, LevelBuffSide side, uint ownerID, bool enteringTimeSlow, bool useMaxDuration, bool notStartEffect)
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				if (ownerID != base.ownerID && Singleton<LevelManager>.Instance.gameMode.IsEnemy(ownerID, base.ownerID))
				{
					SwitchSide(enteringTimeSlow, duration, LevelBuffSide.FromAvatar, ownerID, notStartEffect);
					return true;
				}
				ExtendDuration(duration, enteringTimeSlow, useMaxDuration);
				return false;
			}
			return base.Refresh(duration, side, ownerID, enteringTimeSlow, useMaxDuration, notStartEffect);
		}

		protected override void ApplyWitchTimeSlowedBySide()
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(ownerID);
				BaseAbilityActor[] enemyActorsOf = Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<BaseAbilityActor>(actor);
				for (int i = 0; i < enemyActorsOf.Length; i++)
				{
					ApplyWitchTimeEffect(enemyActorsOf[i].runtimeID);
				}
			}
			else
			{
				base.ApplyWitchTimeSlowedBySide();
			}
		}

		public override void ApplyWitchTimeSlowedBySideWithRuntimeID(uint runtimeID)
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				ApplyWitchTimeEffect(runtimeID);
			}
			else
			{
				base.ApplyWitchTimeSlowedBySideWithRuntimeID(runtimeID);
			}
		}

		protected override void PushRenderingDataBySide()
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				AvatarIdentity identity = Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(ownerID);
				if (identity.isAuthority)
				{
					PushBlueRenderingData();
				}
				else
				{
					PushRedRenderingData();
				}
			}
			else
			{
				base.PushRenderingDataBySide();
			}
		}

		protected override void RemoveWitchTimeSlowedBySide()
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(ownerID);
				BaseAbilityActor[] enemyActorsOf = Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<BaseAbilityActor>(actor);
				for (int i = 0; i < enemyActorsOf.Length; i++)
				{
					RemoveWitchTimeEffect(enemyActorsOf[i].runtimeID);
				}
			}
			else
			{
				base.RemoveWitchTimeSlowedBySide();
			}
		}

		protected override void ActStartParticleEffect()
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				if (!_notStartEffect)
				{
					AvatarIdentity identity = Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(ownerID);
					if (identity.isAuthority)
					{
						ActBlueOpenEffect();
					}
					else
					{
						ActRedOpenEffect();
					}
				}
			}
			else
			{
				base.ActStartParticleEffect();
			}
		}

		protected override void ActStopParticleEffect()
		{
			if (IsPvP() || levelBuffSide != LevelBuffSide.FromAvatar)
			{
				AvatarIdentity identity = Singleton<MPManager>.Instance.GetIdentity<AvatarIdentity>(ownerID);
				if (identity.isAuthority)
				{
					ActBlueCloseEffect();
				}
				else
				{
					ActRedCloseEffect();
				}
			}
			else
			{
				base.ActStopParticleEffect();
			}
		}
	}
}
