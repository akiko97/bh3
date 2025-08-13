using FullInspector;

namespace MoleMole
{
	public class MaxUltimateSkillTriggeredChallenge : BaseLevelChallenge
	{
		public readonly int targetUltimateSkillAmount;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private int _tempUltimateSkillAmount;

		private static string ULTIMATE_SKILL_NAME = "SKL02";

		public MaxUltimateSkillTriggeredChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempUltimateSkillAmount = 0;
			targetUltimateSkillAmount = _metaData.paramList[0];
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override string GetProcessMsg()
		{
			if (IsFinished())
			{
				return "Succ";
			}
			return string.Format("[{0}/{1}]", _tempUltimateSkillAmount, targetUltimateSkillAmount);
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
			if (actor == null || !(actor is AvatarActor) || evt.skillID != ULTIMATE_SKILL_NAME)
			{
				return false;
			}
			_tempUltimateSkillAmount++;
			if (_tempUltimateSkillAmount >= targetUltimateSkillAmount)
			{
				Finish();
			}
			return true;
		}

		private void Finish()
		{
			_finished = true;
			OnDecided();
		}
	}
}
