using System;
using FlatBuffers;

namespace MoleMole
{
	public class MPEventManager : EventManager
	{
		private enum MPDispatchBehavior
		{
			NormalDispatch = 0,
			RemoteModeDropped = 1,
			RemoteModeDirectToAuthority = 2
		}

		private FlatBufferBuilder _eventBuilder;

		protected MPEventManager()
		{
			_eventBuilder = new FlatBufferBuilder(512);
		}

		protected override void ProcessInitedActor(BaseActor actor)
		{
			if (actor is MonsterActor)
			{
				((MonsterActor)actor).AddPlugin(new MPMonsterActorPlugin(actor));
			}
			else if (actor is AvatarActor)
			{
				((AvatarActor)actor).AddPlugin(new MPAvatarActorPlugin(actor));
			}
		}

		protected override void DispatchEvent(BaseEvent evt)
		{
			switch (CheckForDispatchBehavior(evt))
			{
			case MPDispatchBehavior.NormalDispatch:
				base.DispatchEvent(evt);
				break;
			case MPDispatchBehavior.RemoteModeDropped:
				break;
			case MPDispatchBehavior.RemoteModeDirectToAuthority:
				RedirecEventToAuthority(evt);
				break;
			}
		}

		protected override void DispatchListenEvent(BaseEvent evt)
		{
			if (evt.remoteState != EventRemoteState.IsRedirected)
			{
				base.DispatchListenEvent(evt);
				if (evt.remoteState == EventRemoteState.NeedToReplicateToRemote && Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(evt.targetID))
				{
					ReplicateResolvedEventToOthers(evt);
				}
			}
		}

		public override void FireEvent(BaseEvent evt, MPEventDispatchMode mode)
		{
			evt.remoteState = ((mode == MPEventDispatchMode.CheckRemoteMode) ? EventRemoteState.NeedCheckForRemote : EventRemoteState.Idle);
			if (mode == MPEventDispatchMode.CheckRemoteMode)
			{
				evt.fromPeerID = Singleton<MPManager>.Instance.peerID;
			}
			base.FireEvent(evt, mode);
		}

		private void RedirecEventToAuthority(BaseEvent evt)
		{
			BaseMPIdentity baseMPIdentity = Singleton<MPManager>.Instance.TryGetIdentity(evt.targetID);
			evt.remoteState = EventRemoteState.IsRedirected;
			RedirectEvent((IEvtWithRemoteID)evt, baseMPIdentity.authorityPeerID);
		}

		private void ReplicateResolvedEventToOthers(BaseEvent evt)
		{
			RedirectEvent((IEvtWithRemoteID)evt, 7);
		}

		private void RedirectEvent(IEvtWithRemoteID evt, int peerID)
		{
			Type type = MPMappings.SerializeToProtocol(_eventBuilder, evt);
			MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket(type, _eventBuilder);
			pc.state = MPSendContainerState.Finished;
			Singleton<MPManager>.Instance.SendReliableToPeer(evt.GetChannelID(), peerID, pc);
		}

		public void InjectReplicatedEvent(BaseEvent evt)
		{
			InjectEvent(evt);
		}

		public bool IsIdentityAuthority(uint runtimeID)
		{
			BaseMPIdentity baseMPIdentity = Singleton<MPManager>.Instance.TryGetIdentity(runtimeID);
			return baseMPIdentity != null && baseMPIdentity.isAuthority;
		}

		private MPDispatchBehavior CheckForDispatchBehavior(BaseEvent evt)
		{
			Type type = evt.GetType();
			if (!MPData.ReplicatedEventTypes.Contains(type))
			{
				return MPDispatchBehavior.NormalDispatch;
			}
			if (evt.remoteState == EventRemoteState.IsAutorityReceiveRedirected)
			{
				return MPDispatchBehavior.NormalDispatch;
			}
			if (evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated)
			{
				return MPDispatchBehavior.NormalDispatch;
			}
			bool flag = false;
			if (evt.remoteState == EventRemoteState.Idle)
			{
				flag = true;
			}
			else if (evt.remoteState == EventRemoteState.NeedCheckForRemote)
			{
				IEvtWithRemoteID evtWithRemoteID = (IEvtWithRemoteID)evt;
				uint senderID = evtWithRemoteID.GetSenderID();
				uint remoteID = evtWithRemoteID.GetRemoteID();
				BaseMPIdentity baseMPIdentity = ResolveRemoteModeIdentity(senderID);
				BaseMPIdentity baseMPIdentity2 = ResolveRemoteModeIdentity(remoteID);
				if (baseMPIdentity2 == null || baseMPIdentity == null)
				{
					return MPDispatchBehavior.RemoteModeDropped;
				}
				bool flag2 = baseMPIdentity.isAuthority || baseMPIdentity.remoteMode == IdentityRemoteMode.SendAndNoReceive || baseMPIdentity.remoteMode == IdentityRemoteMode.SendAndReceive;
				bool flag3 = baseMPIdentity2.isAuthority || baseMPIdentity2.remoteMode == IdentityRemoteMode.ReceiveAndNoSend || baseMPIdentity2.remoteMode == IdentityRemoteMode.SendAndReceive;
				flag = flag2 && flag3;
			}
			if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(evt.targetID))
			{
				BaseMPIdentity baseMPIdentity3 = Singleton<MPManager>.Instance.TryGetIdentity(evt.targetID);
				if (flag && baseMPIdentity3 != null)
				{
					if (baseMPIdentity3.isAuthority)
					{
						return MPDispatchBehavior.NormalDispatch;
					}
					return MPDispatchBehavior.RemoteModeDirectToAuthority;
				}
				return MPDispatchBehavior.RemoteModeDropped;
			}
			return MPDispatchBehavior.NormalDispatch;
		}

		private BaseMPIdentity ResolveRemoteModeIdentity(uint runtimeID)
		{
			if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
			{
				return Singleton<MPManager>.Instance.TryGetIdentity(runtimeID);
			}
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID);
			if (num == 6)
			{
				BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(runtimeID);
				if (dynamicObjectByRuntimeID.owner != null)
				{
					return Singleton<MPManager>.Instance.TryGetIdentity(dynamicObjectByRuntimeID.owner.GetRuntimeID());
				}
			}
			return null;
		}

		public void MarkEventReplicate(BaseEvent evt)
		{
			evt.remoteState = EventRemoteState.NeedToReplicateToRemote;
		}
	}
}
