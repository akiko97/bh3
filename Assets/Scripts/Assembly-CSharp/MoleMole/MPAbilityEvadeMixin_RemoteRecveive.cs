using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPAbilityEvadeMixin_RemoteRecveive : AbilityEvadeMixin
	{
		private enum PeerReceiveState
		{
			Started = 0,
			Successed = 1,
			Failed = 2
		}

		private PeerReceiveState[] _recvStates;

		private EvtEvadeSuccess _firstSuccess;

		private MixinArg_Evade _mixinArg = new MixinArg_Evade();

		public MPAbilityEvadeMixin_RemoteRecveive(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			_recvStates = new PeerReceiveState[Singleton<MPManager>.Instance.peer.totalPeerCount + 1];
		}

		public override void OnAdded()
		{
			base.OnAdded();
		}

		public override void Core()
		{
			RecvCore();
		}

		private void RecvStart()
		{
			if (_dummyActor != null && _dummyActor.IsEntityExists())
			{
				_dummyActor.Kill();
			}
			uint runtimeID = Singleton<DynamicObjectManager>.Instance.CreateEvadeDummy(actor.runtimeID, config.EvadeDummyName, actor.entity.XZPosition, actor.entity.transform.forward);
			_dummyActor = Singleton<EventManager>.Instance.GetActor<EvadeEntityDummy>(runtimeID);
			if (selfIdentity.isAuthority)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeStartActions, instancedAbility, instancedModifier, null, null);
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtEvadeStart(actor.runtimeID));
			if (_state == State.Idle)
			{
				actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
				entity.SetCountedIsGhost(true);
			}
			_evadeTimer.Reset(true);
			_extendedBlockAttackTimer.Reset(true);
			_state = State.Evading;
		}

		private void RecvCore()
		{
			if (_state != State.Evading)
			{
				return;
			}
			_evadeTimer.Core(1f);
			if (!_evadeTimer.isTimeUp)
			{
				return;
			}
			if (selfIdentity.isAuthority)
			{
				if (_recvStates[selfIdentity.authorityPeerID] == PeerReceiveState.Started)
				{
					_recvStates[selfIdentity.authorityPeerID] = PeerReceiveState.Failed;
					OnAuthorityPeerEvadeStateChanged();
				}
			}
			else
			{
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.FailEvade);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_Evade);
			}
			_evadeTimer.Reset(false);
			_dummyActor.Kill();
			actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
			entity.SetCountedIsGhost(false);
			_state = State.Idle;
		}

		public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
		{
			if (selfIdentity.isAuthority)
			{
				_mixinArg = invokeEntry.GetArgument(_mixinArg);
				if (_mixinArg.Action == EvadeAction.FailEvade && _recvStates[fromPeerID] == PeerReceiveState.Started)
				{
					_recvStates[fromPeerID] = PeerReceiveState.Failed;
					OnAuthorityPeerEvadeStateChanged();
				}
			}
			else
			{
				_mixinArg = invokeEntry.GetArgument(_mixinArg);
				if (_mixinArg.Action == EvadeAction.StartEvade)
				{
					RecvStart();
				}
			}
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (selfIdentity.isAuthority)
			{
				RecvStart();
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_Evade);
				for (int i = 1; i < _recvStates.Length; i++)
				{
					_recvStates[i] = PeerReceiveState.Started;
				}
				_firstSuccess = null;
			}
		}

		protected override bool OnEvadeSuccess(EvtEvadeSuccess evt)
		{
			if (selfIdentity.isAuthority)
			{
				_recvStates[evt.fromPeerID] = PeerReceiveState.Successed;
				if (_firstSuccess == null)
				{
					_firstSuccess = evt;
				}
				Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
				OnAuthorityPeerEvadeStateChanged();
			}
			return true;
		}

		private void OnAuthorityPeerEvadeStateChanged()
		{
			for (int i = 1; i < _recvStates.Length; i++)
			{
				if (_recvStates[i] == PeerReceiveState.Started)
				{
					return;
				}
			}
			if (_firstSuccess != null)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_firstSuccess.attackerID), _firstSuccess);
			}
			else
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.EvadeFailActions, instancedAbility, instancedModifier, null, null);
			}
		}
	}
}
