using System;
using MoleMole.Config;

namespace MoleMole
{
	public class MPManager : BaseMPManager
	{
		public Action OnFrameStart;

		public Action OnFrameEnd;

		public MPPeer peer
		{
			get
			{
				return _peer;
			}
		}

		public MPManager()
		{
			IndexedConfig<ConfigEntityCameraShake>.InitializeMapping();
			IndexedConfig<ConfigEntityAttackEffect>.InitializeMapping();
			IndexedConfig<HitExplodeTracingBulletMixinArgument>.InitializeMapping();
		}

		public void Setup(MPPeer peer)
		{
			SetupPeer(peer, peer.peerID == 1);
			Singleton<RuntimeIDManager>.Instance.SetupPeerID(base.peerID);
			Singleton<MPManager>.Instance.RegisterIdentity(562036737u, 0, new LevelIdentity());
		}

		public void RedirectventDispatch(MPRecvPacketContainer pc)
		{
			BaseEvent baseEvent = (BaseEvent)MPMappings.DeserializeToObject(pc.packet, null);
			if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(pc.runtimeID))
			{
				baseEvent.remoteState = EventRemoteState.IsAutorityReceiveRedirected;
			}
			else
			{
				baseEvent.remoteState = EventRemoteState.IsRemoteReceiveHandledReplcated;
			}
			baseEvent.fromPeerID = pc.fromPeerID;
			Singleton<MPEventManager>.Instance.InjectReplicatedEvent(baseEvent);
		}

		protected override void DispatchPacket(MPRecvPacketContainer pc)
		{
			if (MPData.ReplicatedEventWireTypes.Contains(pc.packet.GetType()))
			{
				RedirectventDispatch(pc);
			}
			else
			{
				base.DispatchPacket(pc);
			}
		}

		public override void Core()
		{
			if (OnFrameStart != null)
			{
				OnFrameStart();
			}
			base.Core();
		}

		public override void PostCore()
		{
			base.PostCore();
			if (OnFrameEnd != null)
			{
				OnFrameEnd();
			}
		}
	}
}
