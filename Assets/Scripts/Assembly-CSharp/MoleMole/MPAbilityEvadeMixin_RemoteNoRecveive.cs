using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPAbilityEvadeMixin_RemoteNoRecveive : AbilityEvadeMixin
	{
		private MixinArg_Evade _mixinArg = new MixinArg_Evade();

		public MPAbilityEvadeMixin_RemoteNoRecveive(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (selfIdentity.isAuthority)
			{
				base.OnAbilityTriggered(evt);
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_Evade);
			}
		}

		public override void Core()
		{
			if (selfIdentity.isAuthority)
			{
				base.Core();
			}
			else if (_state == State.EvadeSuccessed)
			{
				base.Core();
			}
		}

		protected override bool OnEvadeSuccess(EvtEvadeSuccess evt)
		{
			if (selfIdentity.isAuthority)
			{
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.SuccessEvade);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_Evade);
				Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
				return base.OnEvadeSuccess(evt);
			}
			return false;
		}

		protected override void EvadeFail()
		{
			if (selfIdentity.isAuthority)
			{
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.FailEvade);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_Evade);
				base.EvadeFail();
			}
		}

		public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
		{
			_mixinArg = invokeEntry.GetArgument(_mixinArg);
			if (_mixinArg.Action == EvadeAction.StartEvade)
			{
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
			else if (_mixinArg.Action == EvadeAction.SuccessEvade)
			{
				_state = State.EvadeSuccessed;
				_extendedBlockAttackTimer.Reset(true);
			}
			else if (_mixinArg.Action == EvadeAction.FailEvade)
			{
				_evadeTimer.Reset(false);
				actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
				entity.SetCountedIsGhost(false);
				_state = State.Idle;
			}
		}
	}
}
