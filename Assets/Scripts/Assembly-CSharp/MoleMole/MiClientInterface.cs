using System;

namespace MoleMole
{
	public interface MiClientInterface
	{
		bool connect(string host, ushort port, int timeout_ms = 2000);

		void disconnect();

		bool isConnected();

		bool send(NetPacketV1 packet);

		NetPacketV1 recv(int timeout_ms = 0);

		void setDisconnectCallback(Action callback);

		bool setKeepalive(int time_ms, NetPacketV1 packet);
	}
}
