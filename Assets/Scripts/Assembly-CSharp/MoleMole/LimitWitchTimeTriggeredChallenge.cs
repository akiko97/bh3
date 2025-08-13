using FullInspector;

namespace MoleMole
{
	public class LimitWitchTimeTriggeredChallenge : BaseLevelChallenge
	{
		public readonly int targetNum;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _witchTimeTiggered;

		public LimitWitchTimeTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			targetNum = _metaData.paramList[0];
			_witchTimeTiggered = 0;
		}

		public override bool IsFinished()
		{
			_finished = _witchTimeTiggered >= targetNum;
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return string.Format("[{0}/{1}]", _witchTimeTiggered, targetNum);
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtSkillStart>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtSkillStart>(_helper.levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtSkillStart)
			{
				return ListenSkillStart((EvtSkillStart)evt);
			}
			return false;
		}

		private bool ListenSkillStart(EvtSkillStart evt)
		{
			BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.targetID);
			if (actor is AvatarActor && (actor as AvatarActor).config.Skills.ContainsKey(evt.skillID) && (actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag != null)
			{
				for (int i = 0; i < (actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag.Length; i++)
				{
					if ((actor as AvatarActor).config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Evade)
					{
						_witchTimeTiggered++;
						break;
					}
				}
			}
			if (_witchTimeTiggered >= targetNum)
			{
				Finish();
			}
			return false;
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}
	}
}
