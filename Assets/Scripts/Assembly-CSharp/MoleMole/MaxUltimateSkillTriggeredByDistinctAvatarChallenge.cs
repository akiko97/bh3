using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public class MaxUltimateSkillTriggeredByDistinctAvatarChallenge : BaseLevelChallenge
	{
		public readonly int targetUltimateSkillAmountByDistinctAvatar;

		[ShowInInspector]
		private bool _finished;

		[ShowInInspector]
		private List<uint> _tempTriggeredUltimateSkillAvatarIDs;

		private static string ULTIMATE_SKILL_NAME = "SKL02";

		public MaxUltimateSkillTriggeredByDistinctAvatarChallenge(LevelChallengeHelperPlugin helper, LevelChallengeMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_tempTriggeredUltimateSkillAvatarIDs = new List<uint>();
			targetUltimateSkillAmountByDistinctAvatar = _metaData.paramList[0];
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
			return string.Format("[{0}/{1}]", _tempTriggeredUltimateSkillAvatarIDs.Count, targetUltimateSkillAmountByDistinctAvatar);
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
			if (actor != null && actor is AvatarActor)
			{
				if (evt.skillID == ULTIMATE_SKILL_NAME && !_tempTriggeredUltimateSkillAvatarIDs.Contains(evt.targetID))
				{
					_tempTriggeredUltimateSkillAvatarIDs.Add(evt.targetID);
				}
				if (_tempTriggeredUltimateSkillAvatarIDs.Count >= targetUltimateSkillAmountByDistinctAvatar)
				{
					Finish();
				}
				return true;
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
