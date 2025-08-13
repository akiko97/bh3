using BehaviorDesigner.Runtime;

namespace MoleMole
{
	public class AvatarAIPlugin : BaseActorPlugin
	{
		protected BehaviorTree _aiTree;

		protected AvatarActor _owner;

		private LevelActor _levelActor;

		public AvatarAIPlugin(AvatarActor owner)
		{
			_owner = owner;
			_aiTree = _owner.avatar.GetComponent<BehaviorTree>();
			_levelActor = Singleton<LevelManager>.Instance.levelActor;
		}

		public override void OnAdded()
		{
			if (_levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Preparation();
			}
			else
			{
				Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_owner.runtimeID);
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(_owner.runtimeID);
		}

		private void Preparation()
		{
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				return ListenStageReady((EvtStageReady)evt);
			}
			if (!_owner.avatar.IsAIActive())
			{
				return false;
			}
			if (evt is EvtAttackStart)
			{
				return ListenAttackStart((EvtAttackStart)evt);
			}
			return false;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			if (evt.isBorn)
			{
				Preparation();
				Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_owner.runtimeID);
			}
			return false;
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			if (_owner.avatar.AttackTarget != null && _owner.avatar.AttackTarget.GetRuntimeID() == evt.targetID)
			{
				_aiTree.SendEvent("AITargetAttackStart_0");
			}
			return false;
		}
	}
}
