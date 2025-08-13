using System;
using System.Collections.Generic;

namespace proto
{
	public class CommandMap
	{
		private Dictionary<ushort, Type> _cmdIDMap;

		private Dictionary<Type, ushort> _typeMap;

		private CommandMap()
		{
			_cmdIDMap = MakeCmdIDMap();
			_typeMap = GetReverseMap(_cmdIDMap);
		}

		private Dictionary<Type, ushort> GetReverseMap(Dictionary<ushort, Type> orgMap)
		{
			Dictionary<Type, ushort> dictionary = new Dictionary<Type, ushort>();
			foreach (KeyValuePair<ushort, Type> item in orgMap)
			{
				dictionary.Add(item.Value, item.Key);
			}
			return dictionary;
		}

		private Dictionary<ushort, Type> MakeCmdIDMap()
		{
			Dictionary<ushort, Type> dictionary = new Dictionary<ushort, Type>();
			dictionary.Add(1, typeof(KeepAliveNotify));
			dictionary.Add(2, typeof(GetGameserverReq));
			dictionary.Add(3, typeof(GetGameserverRsp));
			dictionary.Add(4, typeof(GetPlayerTokenReq));
			dictionary.Add(5, typeof(GetPlayerTokenRsp));
			dictionary.Add(6, typeof(PlayerLoginReq));
			dictionary.Add(7, typeof(PlayerLoginRsp));
			dictionary.Add(8, typeof(PlayerLogoutReq));
			dictionary.Add(10, typeof(GetMainDataReq));
			dictionary.Add(11, typeof(GetMainDataRsp));
			dictionary.Add(12, typeof(GetScoinExchangeInfoReq));
			dictionary.Add(13, typeof(GetScoinExchangeInfoRsp));
			dictionary.Add(14, typeof(ScoinExchangeReq));
			dictionary.Add(15, typeof(ScoinExchangeRsp));
			dictionary.Add(16, typeof(GetStaminaExchangeInfoReq));
			dictionary.Add(17, typeof(GetStaminaExchangeInfoRsp));
			dictionary.Add(18, typeof(StaminaExchangeReq));
			dictionary.Add(19, typeof(StaminaExchangeRsp));
			dictionary.Add(20, typeof(NicknameModifyReq));
			dictionary.Add(21, typeof(NicknameModifyRsp));
			dictionary.Add(22, typeof(GmTalkReq));
			dictionary.Add(23, typeof(GmTalkRsp));
			dictionary.Add(24, typeof(GetAvatarDataReq));
			dictionary.Add(25, typeof(GetAvatarDataRsp));
			dictionary.Add(26, typeof(GetEquipmentDataReq));
			dictionary.Add(27, typeof(GetEquipmentDataRsp));
			dictionary.Add(28, typeof(DelEquipmentNotify));
			dictionary.Add(29, typeof(AvatarStarUpReq));
			dictionary.Add(30, typeof(AvatarStarUpRsp));
			dictionary.Add(31, typeof(EquipmentPowerUpReq));
			dictionary.Add(32, typeof(EquipmentPowerUpRsp));
			dictionary.Add(33, typeof(EquipmentSellReq));
			dictionary.Add(34, typeof(EquipmentSellRsp));
			dictionary.Add(35, typeof(AddAvatarExpByMaterialReq));
			dictionary.Add(36, typeof(AddAvatarExpByMaterialRsp));
			dictionary.Add(37, typeof(EquipmentEvoReq));
			dictionary.Add(38, typeof(EquipmentEvoRsp));
			dictionary.Add(39, typeof(DressEquipmentReq));
			dictionary.Add(40, typeof(DressEquipmentRsp));
			dictionary.Add(41, typeof(GetStageDataReq));
			dictionary.Add(42, typeof(GetStageDataRsp));
			dictionary.Add(43, typeof(StageBeginReq));
			dictionary.Add(44, typeof(StageBeginRsp));
			dictionary.Add(45, typeof(StageEndReq));
			dictionary.Add(46, typeof(StageEndRsp));
			dictionary.Add(47, typeof(GetAvatarTeamDataReq));
			dictionary.Add(48, typeof(GetAvatarTeamDataRsp));
			dictionary.Add(49, typeof(UpdateAvatarTeamNotify));
			dictionary.Add(50, typeof(AvatarSubSkillLevelUpReq));
			dictionary.Add(51, typeof(AvatarSubSkillLevelUpRsp));
			dictionary.Add(52, typeof(GetSkillPointExchangeInfoReq));
			dictionary.Add(53, typeof(GetSkillPointExchangeInfoRsp));
			dictionary.Add(54, typeof(SkillPointExchangeReq));
			dictionary.Add(55, typeof(SkillPointExchangeRsp));
			dictionary.Add(56, typeof(MaterialEvoReq));
			dictionary.Add(57, typeof(MaterialEvoRsp));
			dictionary.Add(58, typeof(GachaReq));
			dictionary.Add(59, typeof(GachaRsp));
			dictionary.Add(60, typeof(GetStageDropDisplayReq));
			dictionary.Add(61, typeof(GetStageDropDisplayRsp));
			dictionary.Add(62, typeof(GetGachaDisplayReq));
			dictionary.Add(63, typeof(GetGachaDisplayRsp));
			dictionary.Add(64, typeof(GetFriendListReq));
			dictionary.Add(65, typeof(GetFriendListRsp));
			dictionary.Add(66, typeof(AddFriendReq));
			dictionary.Add(67, typeof(AddFriendRsp));
			dictionary.Add(68, typeof(DelFriendReq));
			dictionary.Add(69, typeof(DelFriendRsp));
			dictionary.Add(70, typeof(GetAskAddFriendListReq));
			dictionary.Add(71, typeof(GetAskAddFriendListRsp));
			dictionary.Add(72, typeof(GetPlayerDetailDataReq));
			dictionary.Add(73, typeof(GetPlayerDetailDataRsp));
			dictionary.Add(74, typeof(UpdateEquipmentProtectedStatusReq));
			dictionary.Add(75, typeof(UpdateEquipmentProtectedStatusRsp));
			dictionary.Add(76, typeof(GetRecommendFriendListReq));
			dictionary.Add(77, typeof(GetRecommendFriendListRsp));
			dictionary.Add(78, typeof(SetSelfDescReq));
			dictionary.Add(79, typeof(SetSelfDescRsp));
			dictionary.Add(80, typeof(DelFriendNotify));
			dictionary.Add(81, typeof(GetOfflineFriendsPointNotify));
			dictionary.Add(82, typeof(VerifyItunesOrderNotify));
			dictionary.Add(83, typeof(RechargeFinishNotify));
			dictionary.Add(84, typeof(GetMailDataReq));
			dictionary.Add(85, typeof(GetMailDataRsp));
			dictionary.Add(86, typeof(GetMailAttachmentReq));
			dictionary.Add(87, typeof(GetMailAttachmentRsp));
			dictionary.Add(88, typeof(EnterWorldChatroomReq));
			dictionary.Add(89, typeof(EnterWorldChatroomRsp));
			dictionary.Add(90, typeof(SendWorldChatMsgNotify));
			dictionary.Add(91, typeof(RecvWorldChatMsgNotify));
			dictionary.Add(92, typeof(SendFriendChatMsgNotify));
			dictionary.Add(93, typeof(RecvFriendChatMsgNotify));
			dictionary.Add(94, typeof(RecvFriendOfflineChatMsgNotify));
			dictionary.Add(95, typeof(LeaveChatroomNotify));
			dictionary.Add(96, typeof(SendSystemChatMsgNotify));
			dictionary.Add(97, typeof(RecvSystemChatMsgNotify));
			dictionary.Add(98, typeof(GetProductListReq));
			dictionary.Add(99, typeof(GetProductListRsp));
			dictionary.Add(100, typeof(GetAssistantFrozenListReq));
			dictionary.Add(101, typeof(GetAssistantFrozenListRsp));
			dictionary.Add(102, typeof(SellAvatarFragmentReq));
			dictionary.Add(103, typeof(SellAvatarFragmentRsp));
			dictionary.Add(104, typeof(GetHasGotItemIdListReq));
			dictionary.Add(105, typeof(GetHasGotItemIdListRsp));
			dictionary.Add(106, typeof(AvatarReviveReq));
			dictionary.Add(107, typeof(AvatarReviveRsp));
			dictionary.Add(108, typeof(ResetStageEnterTimesReq));
			dictionary.Add(109, typeof(ResetStageEnterTimesRsp));
			dictionary.Add(110, typeof(GetConfigReq));
			dictionary.Add(111, typeof(GetConfigRsp));
			dictionary.Add(112, typeof(GetMissionDataReq));
			dictionary.Add(113, typeof(GetMissionDataRsp));
			dictionary.Add(114, typeof(GetMissionRewardReq));
			dictionary.Add(115, typeof(GetMissionRewardRsp));
			dictionary.Add(116, typeof(DelMissionNotify));
			dictionary.Add(117, typeof(UpdateMissionProgressReq));
			dictionary.Add(118, typeof(UpdateMissionProgressRsp));
			dictionary.Add(119, typeof(BindAccountReq));
			dictionary.Add(120, typeof(BindAccountRsp));
			dictionary.Add(121, typeof(GetSignInRewardStatusReq));
			dictionary.Add(122, typeof(GetSignInRewardStatusRsp));
			dictionary.Add(123, typeof(GetSignInRewardReq));
			dictionary.Add(124, typeof(GetSignInRewardRsp));
			dictionary.Add(125, typeof(GetWeekDayActivityDataReq));
			dictionary.Add(126, typeof(GetWeekDayActivityDataRsp));
			dictionary.Add(127, typeof(GetFinishGuideDataReq));
			dictionary.Add(128, typeof(GetFinishGuideDataRsp));
			dictionary.Add(129, typeof(FinishGuideReportReq));
			dictionary.Add(130, typeof(FinishGuideReportRsp));
			dictionary.Add(131, typeof(StageInnerDataReportReq));
			dictionary.Add(132, typeof(StageInnerDataReportRsp));
			dictionary.Add(133, typeof(GetDispatchReq));
			dictionary.Add(134, typeof(GetDispatchRsp));
			dictionary.Add(135, typeof(ExchangeAvatarWeaponReq));
			dictionary.Add(136, typeof(ExchangeAvatarWeaponRsp));
			dictionary.Add(137, typeof(GetBulletinReq));
			dictionary.Add(138, typeof(GetBulletinRsp));
			dictionary.Add(139, typeof(GetEndlessDataReq));
			dictionary.Add(140, typeof(GetEndlessDataRsp));
			dictionary.Add(141, typeof(EndlessStageBeginReq));
			dictionary.Add(142, typeof(EndlessStageBeginRsp));
			dictionary.Add(143, typeof(EndlessStageEndReq));
			dictionary.Add(144, typeof(EndlessStageEndRsp));
			dictionary.Add(145, typeof(GetLastEndlessRewardDataReq));
			dictionary.Add(146, typeof(GetLastEndlessRewardDataRsp));
			dictionary.Add(147, typeof(UseEndlessItemReq));
			dictionary.Add(148, typeof(UseEndlessItemRsp));
			dictionary.Add(149, typeof(GetEndlessAvatarHpReq));
			dictionary.Add(150, typeof(GetEndlessAvatarHpRsp));
			dictionary.Add(151, typeof(EndlessPlayerDataUpdateNotify));
			dictionary.Add(152, typeof(EndlessItemDataUpdateNotify));
			dictionary.Add(153, typeof(EndlessWarInfoNotify));
			dictionary.Add(154, typeof(AddGoodfeelReq));
			dictionary.Add(155, typeof(AddGoodfeelRsp));
			dictionary.Add(156, typeof(GetIslandReq));
			dictionary.Add(157, typeof(GetIslandRsp));
			dictionary.Add(158, typeof(LevelUpCabinReq));
			dictionary.Add(159, typeof(LevelUpCabinRsp));
			dictionary.Add(160, typeof(ExtendCabinReq));
			dictionary.Add(161, typeof(ExtendCabinRsp));
			dictionary.Add(162, typeof(FinishCabinLevelUpReq));
			dictionary.Add(163, typeof(FinishCabinLevelUpRsp));
			dictionary.Add(164, typeof(AddCabinTechReq));
			dictionary.Add(165, typeof(AddCabinTechRsp));
			dictionary.Add(166, typeof(ResetCabinTechReq));
			dictionary.Add(167, typeof(ResetCabinTechRsp));
			dictionary.Add(168, typeof(GetIslandVentureReq));
			dictionary.Add(169, typeof(GetIslandVentureRsp));
			dictionary.Add(170, typeof(DelIslandVentureNotify));
			dictionary.Add(171, typeof(RefreshIslandVentureReq));
			dictionary.Add(172, typeof(RefreshIslandVentureRsp));
			dictionary.Add(173, typeof(DispatchIslandVentureReq));
			dictionary.Add(174, typeof(DispatchIslandVentureRsp));
			dictionary.Add(175, typeof(GetIslandVentureRewardReq));
			dictionary.Add(176, typeof(GetIslandVentureRewardRsp));
			dictionary.Add(177, typeof(CancelDispatchIslandVentureReq));
			dictionary.Add(178, typeof(CancelDispatchIslandVentureRsp));
			dictionary.Add(179, typeof(IslandDisjoinEquipmentReq));
			dictionary.Add(180, typeof(IslandDisjoinEquipmentRsp));
			dictionary.Add(181, typeof(IslandCollectReq));
			dictionary.Add(182, typeof(IslandCollectRsp));
			dictionary.Add(183, typeof(GetCollectCabinReq));
			dictionary.Add(184, typeof(GetCollectCabinRsp));
			dictionary.Add(185, typeof(GetGuideRewardReq));
			dictionary.Add(186, typeof(GetGuideRewardRsp));
			dictionary.Add(187, typeof(UrgencyMsgNotify));
			dictionary.Add(191, typeof(IdentifyStigmataAffixReq));
			dictionary.Add(192, typeof(IdentifyStigmataAffixRsp));
			dictionary.Add(193, typeof(FeedStigmataAffixReq));
			dictionary.Add(194, typeof(FeedStigmataAffixRsp));
			dictionary.Add(195, typeof(SelectNewStigmataAffixReq));
			dictionary.Add(196, typeof(SelectNewStigmataAffixRsp));
			dictionary.Add(197, typeof(GetVipRewardDataReq));
			dictionary.Add(198, typeof(GetVipRewardDataRsp));
			dictionary.Add(199, typeof(GetVipRewardReq));
			dictionary.Add(200, typeof(GetVipRewardRsp));
			dictionary.Add(201, typeof(GetShopListReq));
			dictionary.Add(202, typeof(GetShopListRsp));
			dictionary.Add(203, typeof(BuyGoodsReq));
			dictionary.Add(204, typeof(BuyGoodsRsp));
			dictionary.Add(205, typeof(ManualRefreshShopReq));
			dictionary.Add(206, typeof(ManualRefreshShopRsp));
			dictionary.Add(207, typeof(CreateWeiXinOrderReq));
			dictionary.Add(208, typeof(CreateWeiXinOrderRsp));
			dictionary.Add(209, typeof(SpeedUpIslandVentureReq));
			dictionary.Add(210, typeof(SpeedUpIslandVentureRsp));
			dictionary.Add(211, typeof(GetRedeemCodeInfoReq));
			dictionary.Add(212, typeof(GetRedeemCodeInfoRsp));
			dictionary.Add(213, typeof(ExchangeRedeemCodeReq));
			dictionary.Add(214, typeof(ExchangeRedeemCodeRsp));
			dictionary.Add(215, typeof(BuyGachaTicketReq));
			dictionary.Add(216, typeof(BuyGachaTicketRsp));
			dictionary.Add(217, typeof(AntiCheatSDKReportReq));
			dictionary.Add(218, typeof(AntiCheatSDKReportRsp));
			dictionary.Add(219, typeof(GetEndlessTopGroupReq));
			dictionary.Add(220, typeof(GetEndlessTopGroupRsp));
			dictionary.Add(221, typeof(GetRefreshIslandVentureInfoReq));
			dictionary.Add(222, typeof(GetRefreshIslandVentureInfoRsp));
			dictionary.Add(301, typeof(EnterLobbyReq));
			dictionary.Add(302, typeof(EnterLobbyRsp));
			dictionary.Add(303, typeof(LeaveLobbyReq));
			dictionary.Add(304, typeof(LeaveLobbyRsp));
			dictionary.Add(305, typeof(ExchangeLobbyAvatarReq));
			dictionary.Add(306, typeof(ExchangeLobbyAvatarRsp));
			dictionary.Add(307, typeof(SwitchMemberStatusReq));
			dictionary.Add(308, typeof(SwitchMemberStatusRsp));
			dictionary.Add(309, typeof(SwitchLeaderReq));
			dictionary.Add(310, typeof(SwitchLeaderRsp));
			dictionary.Add(311, typeof(LobbyFightStartReq));
			dictionary.Add(312, typeof(LobbyFightStartRsp));
			dictionary.Add(313, typeof(LobbyFightEndReq));
			dictionary.Add(314, typeof(LobbyFightEndRsp));
			dictionary.Add(315, typeof(GetLobbyDataReq));
			dictionary.Add(316, typeof(GetLobbyDataRsp));
			dictionary.Add(223, typeof(GetInviteFriendReq));
			dictionary.Add(224, typeof(GetInviteFriendRsp));
			dictionary.Add(225, typeof(GetInviteeFriendReq));
			dictionary.Add(226, typeof(GetInviteeFriendRsp));
			dictionary.Add(227, typeof(AcceptFriendInviteReq));
			dictionary.Add(228, typeof(AcceptFriendInviteRsp));
			dictionary.Add(229, typeof(CommentReportReq));
			dictionary.Add(230, typeof(CommentReportRsp));
			dictionary.Add(231, typeof(GetExtraStoryDataReq));
			dictionary.Add(232, typeof(GetExtraStoryDataRsp));
			dictionary.Add(233, typeof(GetExtraStoryActivityActReq));
			dictionary.Add(234, typeof(GetExtraStoryActivityActRsp));
			return dictionary;
		}

		public ushort GetCmdIDByType(Type type)
		{
			ushort value;
			if (!_typeMap.TryGetValue(type, out value))
			{
			}
			return value;
		}

		public Type GetTypeByCmdID(ushort cmdID)
		{
			Type value;
			if (!_cmdIDMap.TryGetValue(cmdID, out value))
			{
			}
			return value;
		}
	}
}
