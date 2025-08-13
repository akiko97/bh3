using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class LevelTutorialHelperPlugin : BaseActorPlugin
	{
		[HideInInspector]
		public readonly LevelActor levelActor;

		public List<BaseLevelTutorial> tutorialList;

		public LevelTutorialHelperPlugin(LevelActor levelActor)
		{
			this.levelActor = levelActor;
			tutorialList = new List<BaseLevelTutorial>();
			int levelId = Singleton<LevelScoreManager>.Instance.LevelId;
			List<int> unFinishedTutorialIDList = Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(levelId);
			if (unFinishedTutorialIDList == null)
			{
				return;
			}
			foreach (int item in unFinishedTutorialIDList)
			{
				tutorialList.Add(GetTutorialById(item));
			}
		}

		private BaseLevelTutorial GetTutorialById(int tutorialId)
		{
			LevelTutorialMetaData levelTutorialMetaDataByKey = LevelTutorialMetaDataReader.GetLevelTutorialMetaDataByKey(tutorialId);
			switch (levelTutorialMetaDataByKey.tutorialId)
			{
			case 10001:
				return new LevelTutorialPlayerTeaching(this, levelTutorialMetaDataByKey);
			case 10002:
				return new LevelTutorialUltraAttack(this, levelTutorialMetaDataByKey);
			case 10003:
				return new LevelTutorialBranchAttack(this, levelTutorialMetaDataByKey);
			case 10004:
				return new LevelTutorialEliteAttack(this, levelTutorialMetaDataByKey);
			case 10005:
				return new LevelTutorialSwapAttack(this, levelTutorialMetaDataByKey);
			case 10006:
				return new LevelTutorialSwapAndRestrain(this, levelTutorialMetaDataByKey);
			case 10007:
				return new LevelTutorialMonsterBlock(this, levelTutorialMetaDataByKey);
			case 10008:
				return new LevelTutorialMonsterTeleport(this, levelTutorialMetaDataByKey);
			case 10009:
				return new LevelTutorialMonsterShield(this, levelTutorialMetaDataByKey);
			case 10010:
				return new LevelTutorialMonsterRobotDodge(this, levelTutorialMetaDataByKey);
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public override void OnAdded()
		{
			foreach (BaseLevelTutorial tutorial in tutorialList)
			{
				tutorial.OnAdded();
			}
		}

		public override bool OnEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelTutorial tutorial in tutorialList)
			{
				if (tutorial.active)
				{
					flag |= tutorial.OnEvent(evt);
				}
			}
			return flag;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelTutorial tutorial in tutorialList)
			{
				if (tutorial.active)
				{
					flag |= tutorial.OnPostEvent(evt);
				}
			}
			return flag;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = false;
			foreach (BaseLevelTutorial tutorial in tutorialList)
			{
				if (tutorial.active)
				{
					flag |= tutorial.ListenEvent(evt);
				}
			}
			return flag;
		}

		public override void Core()
		{
			foreach (BaseLevelTutorial tutorial in tutorialList)
			{
				if (tutorial.active)
				{
					tutorial.Core();
				}
			}
		}
	}
}
