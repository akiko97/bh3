using System;
using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelChallengeHelperPlugin : BaseActorPlugin
	{
		[HideInInspector]
		public readonly LevelActor levelActor;

		public List<BaseLevelChallenge> challengeList;

		public LevelChallengeHelperPlugin(LevelActor levelActor)
		{
			this.levelActor = levelActor;
			challengeList = new List<BaseLevelChallenge>();
			List<int> trackChallengeIds = Singleton<LevelScoreManager>.Instance.trackChallengeIds;
			if (trackChallengeIds == null)
			{
				return;
			}
			foreach (int item in trackChallengeIds)
			{
				challengeList.Add(GetChallengeById(item));
			}
		}

		private BaseLevelChallenge GetChallengeById(int challengeId)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected I4, but got Unknown
			LevelChallengeMetaData levelChallengeMetaDataByKey = LevelChallengeMetaDataReader.GetLevelChallengeMetaDataByKey(challengeId);
			StageChallengeType val = (StageChallengeType)levelChallengeMetaDataByKey.conditionId;
			switch ((int)val - 1)
			{
			case 0:
				return new LimitAvatarChallege(this, levelChallengeMetaDataByKey);
			case 1:
				return new LimitAvatarKilledChallenge(this, levelChallengeMetaDataByKey);
			case 2:
				return new MaxComboChallenge(this, levelChallengeMetaDataByKey);
			case 3:
				return new LimitBeHitChallenge(this, levelChallengeMetaDataByKey);
			case 4:
				return new LimitLevelTimeChallenge(this, levelChallengeMetaDataByKey);
			case 5:
				return new LimitWitchTimeTriggeredChallenge(this, levelChallengeMetaDataByKey);
			case 6:
				return new MaxDamageInWitchTimeChallenge(this, levelChallengeMetaDataByKey);
			case 7:
				return new MaxUltimateSkillTriggeredChallenge(this, levelChallengeMetaDataByKey);
			case 8:
				return new MaxDamageInCertainTimeChallenge(this, levelChallengeMetaDataByKey);
			case 9:
				return new MaxUltimateSkillTriggeredByDistinctAvatarChallenge(this, levelChallengeMetaDataByKey);
			case 10:
				return new LimitBeHitDownChallenge(this, levelChallengeMetaDataByKey);
			case 11:
				return new MaxAvatarClassInTeamChallenge(this, levelChallengeMetaDataByKey);
			case 12:
				return new MaxMonsterKilledChallenge(this, levelChallengeMetaDataByKey);
			case 13:
				return new MaxAvatarNatureInTeamChallenge(this, levelChallengeMetaDataByKey);
			case 14:
				return new LimitTotalDamageTakenChallenge(this, levelChallengeMetaDataByKey);
			case 15:
				return new HelperAvatarAliveChallenge(this, levelChallengeMetaDataByKey);
			case 16:
				return new LimitTrapTriggeredChallenge(this, levelChallengeMetaDataByKey);
			case 17:
				return new MaxBoxOpenedChallenge(this, levelChallengeMetaDataByKey);
			case 18:
				return new MaxDebuffAddChallenge(this, levelChallengeMetaDataByKey);
			case 19:
				return new LimitBeAddedDebuffChallenge(this, levelChallengeMetaDataByKey);
			case 20:
				return new MaxAllDamageInWitchTimeChallenge(this, levelChallengeMetaDataByKey);
			case 21:
				return new MaxMonsterKilledBySwitchInAttackChallenge(this, levelChallengeMetaDataByKey);
			case 22:
				return new MaxMonsterKilledInWitchTimeChallenge(this, levelChallengeMetaDataByKey);
			case 23:
				return new MonsterLastKilledBySwitchInAttackChallenge(this, levelChallengeMetaDataByKey);
			case 24:
				return new MaxMonsterKilledInCertainTimeChallenge(this, levelChallengeMetaDataByKey);
			case 25:
				return new MaxMonsterHitAirChallenge(this, levelChallengeMetaDataByKey);
			case 26:
			case 27:
			case 28:
				return new SpecialLevelTimeChallenge(this, levelChallengeMetaDataByKey);
			case 29:
				return new MaxMonsterKilledByBranchAttackChallenge(this, levelChallengeMetaDataByKey);
			case 30:
				return new MaxQTETriggeredChallenge(this, levelChallengeMetaDataByKey);
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public override void OnAdded()
		{
			foreach (BaseLevelChallenge challenge in challengeList)
			{
				challenge.OnAdded();
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelChallenge challenge in challengeList)
			{
				if (challenge.active)
				{
					flag |= challenge.OnEvent(evt);
				}
			}
			return flag;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelChallenge challenge in challengeList)
			{
				if (challenge.active)
				{
					flag |= challenge.OnPostEvent(evt);
				}
			}
			return flag;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelChallenge challenge in challengeList)
			{
				if (challenge.active)
				{
					flag |= challenge.ListenEvent(evt);
				}
			}
			return flag;
		}

		public override void Core()
		{
			for (int i = 0; i < challengeList.Count; i++)
			{
				BaseLevelChallenge baseLevelChallenge = challengeList[i];
				if (baseLevelChallenge.active)
				{
					baseLevelChallenge.Core();
				}
			}
		}
	}
}
