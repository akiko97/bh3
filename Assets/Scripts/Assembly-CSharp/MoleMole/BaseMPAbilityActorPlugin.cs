namespace MoleMole
{
	public abstract class BaseMPAbilityActorPlugin : BaseActorPlugin
	{
		private BaseAbilityActor _actor;

		private BaseAbilityEntityIdentiy _identity;

		protected void Setup(BaseAbilityActor actor, BaseAbilityEntityIdentiy identity)
		{
			_actor = actor;
			_identity = identity;
			if (_identity.isAuthority)
			{
				OnAuthorityStart();
			}
			else
			{
				OnRemoteStart();
			}
		}

		protected virtual void OnAuthorityStart()
		{
		}

		protected virtual void OnRemoteStart()
		{
			_actor.rejectBaseEventHandlingPredicate = RejectRemoteReplicatedResults;
		}

		private bool RejectRemoteReplicatedResults(BaseEvent evt)
		{
			return evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated;
		}

		public sealed override bool OnEvent(BaseEvent evt)
		{
			if (!_identity.isAuthority && evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated)
			{
				return OnRemoteReplicatedEvent(evt);
			}
			return false;
		}

		protected abstract bool OnRemoteReplicatedEvent(BaseEvent evt);

		public override void OnRemoved()
		{
			if (_identity.isAuthority)
			{
				Singleton<MPManager>.Instance.DestroyMPIdentity(_identity.runtimeID);
			}
		}
	}
}
