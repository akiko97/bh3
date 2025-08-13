using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPAbilityHitExplodeBulletMixin_Old : AbilityHitExplodeBulletMixin
	{
		private MixinArg_HitExplodeMixin _mixinArg = new MixinArg_HitExplodeMixin();

		public MPAbilityHitExplodeBulletMixin_Old(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (selfIdentity.isAuthority)
			{
				HitExplodeTracingBulletMixinArgument hitExplodeTracingBulletMixinArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
				uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
				RecordInvokeEntryContext context;
				StartRecordMixinInvokeEntry(out context, 0u);
				Offset<MixinArg_HitExplodeMixin> offset = MixinArg_HitExplodeMixin.CreateMixinArg_HitExplodeMixin(context.builder, HitExplodeBulletAction.Trigger, (ushort)((hitExplodeTracingBulletMixinArgument != null) ? ((ushort)IndexedConfig<HitExplodeTracingBulletMixinArgument>.Mapping.Get(hitExplodeTracingBulletMixinArgument)) : (ushort)0), nextRuntimeID, evt.otherID);
				context.Finish(offset, AbilityInvokeArgument.MixinArg_HitExplodeMixin);
				CreateBullet(hitExplodeTracingBulletMixinArgument, nextRuntimeID, evt.otherID);
			}
		}

		public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
		{
			if (!selfIdentity.isAuthority)
			{
				RemoteHandleInvokeEntry(invokeEntry, fromPeerID);
			}
		}

		protected override bool ListenBulletHit(EvtBulletHit evt)
		{
			if (selfIdentity.isAuthority)
			{
				Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
				return base.ListenBulletHit(evt);
			}
			return false;
		}

		private void RemoteHandleInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
		{
			_mixinArg = invokeEntry.GetArgument(_mixinArg);
			if (_mixinArg.Action == HitExplodeBulletAction.Trigger)
			{
				HitExplodeTracingBulletMixinArgument arg = null;
				if (_mixinArg.ArgMappingID != 0)
				{
					arg = IndexedConfig<HitExplodeTracingBulletMixinArgument>.Mapping.Get(_mixinArg.ArgMappingID);
				}
				CreateBullet(arg, _mixinArg.BulletID, _mixinArg.OtherID);
			}
		}
	}
}
