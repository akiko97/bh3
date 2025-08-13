using FlatBuffers;

namespace MoleMole
{
	public struct MPSendPacketContainer
	{
		public uint runtimeID;

		public int packetTypeID;

		public FlatBufferBuilder builder;

		public MPSendContainerState state;

		public void Finish<T>(Offset<T> offset) where T : Table
		{
			builder.Finish(offset.Value);
			state = MPSendContainerState.Finished;
		}

		public T ReadAs<T>() where T : Table, new()
		{
			T result = new T();
			result.ResetAndInitTo(builder.DataBuffer);
			return result;
		}
	}
}
