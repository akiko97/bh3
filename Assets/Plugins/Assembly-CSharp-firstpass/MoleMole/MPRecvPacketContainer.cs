using FlatBuffers;

namespace MoleMole
{
	public struct MPRecvPacketContainer
	{
		public uint runtimeID;

		public int channel;

		public int fromPeerID;

		public Table packet;

		public T As<T>() where T : Table
		{
			return (T)packet;
		}
	}
}
