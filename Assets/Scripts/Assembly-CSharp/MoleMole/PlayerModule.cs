using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class PlayerModule : BaseModule
	{
		public PlayerDataItem playerData { get; private set; }

		private PlayerModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			playerData = new PlayerDataItem();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 5:
				return OnGetPlayerTokenRsp(pkt.getData<GetPlayerTokenRsp>());
			case 7:
				return OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());
			case 11:
				return OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());
			case 111:
				return OnGetConfigDataRsp(pkt.getData<GetConfigRsp>());
			case 13:
				return OnGetScoinExchangeInfoRsp(pkt.getData<GetScoinExchangeInfoRsp>());
			case 17:
				return OnGetStaminaExchangeInfoRsp(pkt.getData<GetStaminaExchangeInfoRsp>());
			case 48:
				return OnGetAvatarTeamDataRsp(pkt.getData<GetAvatarTeamDataRsp>());
			case 53:
				return OnGetSkillPointExchangeInfoRsp(pkt.getData<GetSkillPointExchangeInfoRsp>());
			case 81:
				return OnGetOfflineFriendsPointNotify(pkt.getData<GetOfflineFriendsPointNotify>());
			case 120:
				return OnBindAccountRsp(pkt.getData<BindAccountRsp>());
			case 122:
				return OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());
			case 146:
				return OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());
			case 218:
				return OnAntiCheatSDKReportRsp(pkt.getData<AntiCheatSDKReportRsp>());
			default:
				return false;
			}
		}

		private bool OnGetPlayerTokenRsp(GetPlayerTokenRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				playerData.userId = (int)rsp.uid;
				SaveLastLoginAccountInfo();
				Singleton<NetworkManager>.Instance.RequestPlayerLogin();
			}
			return false;
		}

		private bool OnBindAccountRsp(BindAccountRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				SaveLastLoginAccountInfo();
			}
			return false;
		}

		private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
		{
			playerData.signInStatus = rsp;
			return false;
		}

		private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.reward_list.Count > 0)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = rsp;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private void SaveLastLoginAccountInfo()
		{
			Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId = playerData.userId;
			Singleton<AccountManager>.Instance.manager.SaveAccountToLocal();
		}

		private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Invalid comparison between Unknown and I4
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				uint serverProcessedPacketId = (rsp.last_client_packet_idSpecified ? rsp.last_client_packet_id : 0u);
				Singleton<NetworkManager>.Instance.SendPacketsOnLoginSuccess(false, serverProcessedPacketId);
				Singleton<NetworkManager>.Instance.alreadyLogin = true;
			}
			else if ((int)rsp.retcode == 4)
			{
				Singleton<NetworkManager>.Instance.ProcessWaitStopAnotherLogin();
			}
			else
			{
				Singleton<NetworkManager>.Instance.DisConnect();
				if (MiscData.Config.BasicConfig.IsBlockUserWhenRepeatLogin && (int)rsp.retcode == 2 && Singleton<NetworkManager>.Instance.alreadyLogin)
				{
					Singleton<NetworkManager>.Instance.SetRepeatLogin();
				}
			}
			return false;
		}

		private bool OnGetMainDataRsp(GetMainDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				playerData.initByGetMainDataRsp = true;
				UpdateField(rsp.nicknameSpecified, ref playerData.nickname, rsp.nickname);
				UpdateField(rsp.levelSpecified, ref playerData.teamLevel, (int)rsp.level, playerData.OnLevelChange);
				UpdateField(rsp.expSpecified, ref playerData.teamExp, (int)rsp.exp);
				UpdateField(rsp.hcoinSpecified, ref playerData.hcoin, (int)rsp.hcoin, playerData.OnCoinChange);
				UpdateField(rsp.scoinSpecified, ref playerData.scoin, (int)rsp.scoin, playerData.OnCoinChange);
				UpdateField(rsp.staminaSpecified, ref playerData.stamina, (int)rsp.stamina);
				UpdateField(rsp.stamina_recover_left_timeSpecified, ref playerData.staminaRecoverLeftTime, (int)rsp.stamina_recover_left_time, playerData.OnStaminaRecoverTimeChange);
				UpdateField(rsp.stamina_recover_config_timeSpecified, ref playerData.staminaRecoverConfigTime, (int)rsp.stamina_recover_config_time, playerData.OnStaminaRecoverTimeChange);
				UpdateField(rsp.skill_pointSpecified, ref playerData.skillPoint, (int)rsp.skill_point);
				UpdateField(rsp.skill_point_recover_left_timeSpecified, ref playerData.skillPointRecoverLeftTime, (int)rsp.skill_point_recover_left_time, playerData.OnSkillPointRecoverTimeChange);
				UpdateField(rsp.skill_point_recover_config_timeSpecified, ref playerData.skillPointRecoverConfigTime, (int)rsp.skill_point_recover_config_time, playerData.OnSkillPointRecoverTimeChange);
				UpdateField(rsp.skill_point_limitSpecified, ref playerData.skillPointLimit, (int)rsp.skill_point_limit);
				UpdateField(rsp.equipment_size_limitSpecified, ref playerData.equipmentSizeLimit, (int)rsp.equipment_size_limit);
				UpdateField(rsp.friends_pointSpecified, ref playerData.friendsPoint, (int)rsp.friends_point);
				UpdateField(rsp.self_descSpecified, ref playerData.selfDesc, rsp.self_desc);
			}
			return false;
		}

		private bool OnGetConfigDataRsp(GetConfigRsp rsp)
		{
			UpdateField(ref playerData.staminaRecoverConfigTime, (int)rsp.stamina_recover_config_time);
			UpdateField(ref playerData.skillPointRecoverConfigTime, (int)rsp.skill_point_recover_config_time);
			UpdateField(ref playerData.reviveHcoinCost, (int)rsp.avatar_revive_hcoin_cost);
			UpdateField(ref playerData.sameTypePowerUpRataInt, (int)rsp.same_type_power_up_rate);
			UpdateField(ref playerData.powerUpScoinCostRate, (int)rsp.power_up_scoin_cost_rate);
			UpdateField(ref playerData.maxFriend, (int)rsp.max_friend_num);
			UpdateField(ref playerData.endlessMinPlayerLevel, (int)rsp.endless_min_player_level);
			UpdateField(ref playerData.endlessMaxProgress, (int)rsp.endless_max_progress);
			UpdateField(rsp.endless_use_item_cd_timeSpecified, ref playerData.endlessUseItemCDTime, (int)rsp.endless_use_item_cd_time);
			UpdateField(ref playerData.disjoin_equipment_back_exp_percent, (int)rsp.disjoin_equipment_back_exp_percent);
			UpdateField(ref playerData.avatarCombatBaseWeight, (int)rsp.avatar_combat_base_weight);
			UpdateField(ref playerData.avatarCombatBaseStarRate, (int)rsp.avatar_combat_base_star_rate);
			UpdateField(ref playerData.avatarCombatBaseLevelRate, (int)rsp.avatar_combat_base_level_rate);
			UpdateField(ref playerData.avatarCombatBaseUnlockStarRate, (int)rsp.avatar_combat_base_unlock_star_rate);
			UpdateField(ref playerData.avatarCombatSkillWeight, (int)rsp.avatar_combat_skill_weight);
			UpdateField(ref playerData.avatarCombatIslandWeight, (int)rsp.avatar_combat_island_weight);
			UpdateField(ref playerData.avatarCombatWeaponWeight, (int)rsp.avatar_combat_weapon_weight);
			UpdateField(ref playerData.avatarCombatWeaponRarityRate, (int)rsp.avatar_combat_weapon_rarity_rate);
			UpdateField(ref playerData.avatarCombatWeaponSubRarityRate, (int)rsp.avatar_combat_weapon_sub_rarity_rate);
			UpdateField(ref playerData.avatarCombatWeaponLevelRate, (int)rsp.avatar_combat_weapon_level_rate);
			UpdateField(ref playerData.avatarCombatStigmataWeight, (int)rsp.avatar_combat_stigmata_weight);
			UpdateField(ref playerData.avatarCombatStigmataRarityRate, (int)rsp.avatar_combat_stigmata_rarity_rate);
			UpdateField(ref playerData.avatarCombatStigmataSubRarityRate, (int)rsp.avatar_combat_stigmata_sub_rarity_rate);
			UpdateField(ref playerData.avatarCombatStigmataLevelRate, (int)rsp.avatar_combat_stigmata_level_rate);
			UpdateField(ref playerData.avatarCombatStigmataSuitNumRate, (int)rsp.avatar_combat_stigmata_suit_num_rate);
			UpdateField(ref playerData.minLevelToGenerateInviteCode, (int)rsp.min_invite_level);
			UpdateField(ref playerData.maxLevelToAcceptInvite, (int)rsp.max_accept_invitee_level);
			foreach (AvatarCostPlusConfig item in rsp.avatar_cost_plus_config_list)
			{
				playerData.costAddByAvatarStar[(int)item.star] = (int)item.cost_plus;
			}
			foreach (proto.GetConfigRsp.GachaTicket item2 in rsp.gacha_ticket_list)
			{
				playerData.gachaTicketPriceDict[(int)item2.material_id] = (int)item2.hcoin_cost;
				playerData.gachaTicketPriceDict[(int)(item2.material_id * 10)] = (int)(item2.hcoin_cost * 10);
			}
			TimeUtil.SetServerCurTime(rsp.server_cur_time);
			TimeUtil.SetDayTimeOffset((int)rsp.day_time_offset);
			if (rsp.region_nameSpecified && Singleton<AccountManager>.Instance.manager != null)
			{
				Singleton<AccountManager>.Instance.manager.ChannelRegion = rsp.region_name;
			}
			return false;
		}

		private bool OnGetScoinExchangeInfoRsp(GetScoinExchangeInfoRsp rsp)
		{
			bool flag = playerData.scoinExchangeCache.Value == null;
			playerData.scoinExchangeCache.Value = new PlayerScoinExchangeInfo
			{
				usableTimes = (int)rsp.usable_times,
				totalTimes = (int)rsp.total_times,
				hcoinCost = (int)rsp.hcoin_cost,
				scoinGet = (int)rsp.scoin_get
			};
			if (flag)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowScoinExchangeInfo, rsp));
			}
			return false;
		}

		private bool OnGetStaminaExchangeInfoRsp(GetStaminaExchangeInfoRsp rsp)
		{
			playerData.staminaExchangeCache.Value = new PlayerStaminaExchangeInfo
			{
				usableTimes = (int)rsp.usable_times,
				totalTimes = (int)rsp.total_times,
				hcoinCost = (int)rsp.hcoin_cost,
				staminaGet = (int)rsp.stamina_get
			};
			playerData._cacheDataUtil.OnGetRsp(17);
			return false;
		}

		private bool OnGetSkillPointExchangeInfoRsp(GetSkillPointExchangeInfoRsp rsp)
		{
			playerData.skillPointExchangeCache.Value = new PlayerSkillPointExchangeInfo
			{
				usableTimes = (int)rsp.usable_times,
				totalTimes = (int)rsp.total_times,
				hcoinCost = (int)rsp.hcoin_cost,
				skillPointGet = (int)rsp.skill_point_get
			};
			return false;
		}

		private bool OnGetAvatarTeamDataRsp(GetAvatarTeamDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (AvatarTeam item in rsp.avatar_team_list)
				{
					StageType key = (StageType)item.stage_type;
					if (!playerData.teamDict.ContainsKey(key))
					{
						playerData.teamDict.Add(key, new List<int>());
					}
					playerData.teamDict[key] = ConvertList(item.avatar_id_list);
				}
			}
			return false;
		}

		private bool OnGetOfflineFriendsPointNotify(GetOfflineFriendsPointNotify rsp)
		{
			playerData.offlineFriendsPoint = (int)rsp.friends_point;
			return false;
		}

		public void SetBehaviourDone(string key)
		{
			if (!IsBehaviourDone(key))
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.DoneBehaviourList.Add(key);
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public bool IsBehaviourDone(string key)
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.DoneBehaviourList.Contains(key);
		}

		private bool OnAntiCheatSDKReportRsp(AntiCheatSDKReportRsp rsp)
		{
			return false;
		}
	}
}
