using FullInspector;

namespace MoleMole
{
	public class LevelTutorialMonsterTeleport : BaseLevelTutorial
	{
		[ShowInInspector]
		private bool _finished;

		private string _monsterSubTypeName;

		private string _monsterConfigTypeName;

		public LevelTutorialMonsterTeleport(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
			: base(helper, metaData)
		{
			_finished = false;
			_monsterSubTypeName = "DG_031";
			_monsterConfigTypeName = "CS_Default";
		}

		public override bool IsFinished()
		{
			return _finished;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtTeleport>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
		}

		public override void OnDecided()
		{
			base.OnDecided();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtTeleport>(_helper.levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_helper.levelActor.runtimeID);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtTeleport)
			{
				return ListenTeleport((EvtTeleport)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			return false;
		}

		private bool IsMonsterDeadKillerWithTeleportSkill(MonsterActor monsterActor)
		{
			bool result = false;
			if (monsterActor != null && monsterActor.metaConfig.subTypeName == _monsterSubTypeName && monsterActor.metaConfig.configType == _monsterConfigTypeName)
			{
				result = true;
			}
			return result;
		}

		private bool ListenTeleport(EvtTeleport evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
			if (actor != null && IsMonsterDeadKillerWithTeleportSkill(actor) && base.step == 1 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep2);
			}
			return false;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID);
			if (actor != null && IsMonsterDeadKillerWithTeleportSkill(actor) && base.step == 0 && GetCurrentStepState() == StepState.Sleep && !IsAllStepDone())
			{
				ActiveCurrentStep();
				WaitShowTutorialStep(GetDelayTime(base.step), ShowTutorialStep1);
			}
			return false;
		}

		public override void Core()
		{
			if (active && IsAllStepDone())
			{
				Finish();
			}
		}

		private void ShowTutorialStep1()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
			ActiveCurrentStep();
			PauseGame();
		}

		public void OnTutorialStep1Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void ShowTutorialStep2()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
			ActiveCurrentStep();
			PauseGame();
		}

		public void OnTutorialStep2Done()
		{
			MoveToNextStep();
			ResumeGame();
		}

		private void Finish()
		{
			_finished = true;
			Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(tutorialId);
			OnDecided();
		}

		private void Fail()
		{
			_finished = false;
			OnDecided();
		}
	}
}
