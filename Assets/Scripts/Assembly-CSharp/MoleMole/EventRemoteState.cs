namespace MoleMole
{
	public enum EventRemoteState
	{
		Idle = 0,
		NeedCheckForRemote = 1,
		IsRedirected = 2,
		NeedToReplicateToRemote = 3,
		IsAutorityReceiveRedirected = 4,
		IsRemoteReceiveHandledReplcated = 5
	}
}
