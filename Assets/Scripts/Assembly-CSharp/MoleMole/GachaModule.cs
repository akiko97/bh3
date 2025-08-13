using System;
using proto;

namespace MoleMole
{
	public class GachaModule : BaseModule
	{
		private DateTime _lastCheckGachaInfoTime;

		private GachaDisplayInfo _gachaDisplayInfo;

		public GachaDisplayInfo GachaDisplay
		{
			get
			{
				if (IsGachaInfoValid())
				{
					return _gachaDisplayInfo;
				}
				return null;
			}
		}

		private GachaModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_gachaDisplayInfo = null;
			_lastCheckGachaInfoTime = DateTime.MinValue;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 63:
				return OnGetGachaDisplayRsp(pkt.getData<GetGachaDisplayRsp>());
			case 59:
				return OnGachaRsp(pkt.getData<GachaRsp>());
			default:
				return false;
			}
		}

		private bool OnGetGachaDisplayRsp(GetGachaDisplayRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (_gachaDisplayInfo == null)
				{
					_gachaDisplayInfo = new GachaDisplayInfo();
				}
				_gachaDisplayInfo.hcoinGachaData = rsp.hcoin_gacha_data;
				_gachaDisplayInfo.specialGachaData = ((rsp.special_hcoin_gacha_data_list.Count <= 0) ? null : rsp.special_hcoin_gacha_data_list[0]);
				_gachaDisplayInfo.friendPointGachaData = rsp.friends_point_gacha_data;
				_lastCheckGachaInfoTime = TimeUtil.Now;
			}
			return false;
		}

		public bool OnGachaRsp(GachaRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && !IsGachaInfoValid())
			{
				Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
			}
			return false;
		}

		public bool IsGachaInfoValid()
		{
			return TimeUtil.Now < _lastCheckGachaInfoTime.AddSeconds(MiscData.Config.BasicConfig.CheckGachaInfoIntervalSecond);
		}
	}
}
