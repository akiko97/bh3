using System;
using proto;

namespace MoleMole
{
	public class TestModule : BaseModule
	{
		private TestModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
		}

		public void RequestGMTalk(string msg_param)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			NetworkManager instance = Singleton<NetworkManager>.Instance;
			GmTalkReq val = new GmTalkReq();
			val.msg = msg_param;
			instance.SendPacket<GmTalkReq>(val);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 23)
			{
				return OnGmTalkRsp(pkt.getData<GmTalkRsp>());
			}
			return false;
		}

		private bool OnGmTalkRsp(GmTalkRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.msgSpecified && rsp.msg.Equals("CLEAR ALL", StringComparison.OrdinalIgnoreCase))
			{
				MiHoYoGameData.DeleteAllData();
			}
			return false;
		}
	}
}
