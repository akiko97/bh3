namespace MoleMole
{
	public class PeerIdentity : BaseMPIdentity
	{
		public override IdentityRemoteMode remoteMode
		{
			get
			{
				return IdentityRemoteMode.Mute;
			}
		}

		public override void OnStateUpdatePacket(MPRecvPacketContainer pc)
		{
		}
	}
}
