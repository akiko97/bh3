using System;
using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class LevelAntiCheatPlugin : BaseActorPlugin
	{
		private LevelDamageStasticsPlugin _damagePlugin;

		private SafeFloat _levelStartTime = 0f;

		private SafeFloat _unscaledLevelTime = 0f;

		private SafeInt32 _frameCount = 0;

		public List<StageCheatData> cheatDataList { get; private set; }

		public LevelAntiCheatPlugin(LevelDamageStasticsPlugin damagePlugin)
		{
			_damagePlugin = damagePlugin;
			_levelStartTime = Miscs.GetTimeStampFromDateTime(DateTime.Now);
		}

		public override void Core()
		{
			base.Core();
			_unscaledLevelTime = (float)_unscaledLevelTime + Time.unscaledDeltaTime;
			_frameCount = (int)_frameCount + 1;
		}

		public void CollectAntiCheatData()
		{
			/*cheatDataList = new List<StageCheatData>();
			AddData((Type)1001, _levelStartTime);
			AddData((Type)1002, _unscaledLevelTime);
			AddData((Type)1003, (float)_unscaledLevelTime / (float)(int)_frameCount);
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(baseMonoAvatar.GetRuntimeID());
				AvatarStastics avatarStastics = _damagePlugin.GetAvatarStastics(baseMonoAvatar.GetRuntimeID());
				int num = i * 100;
				AddData((Type)(2001 + num), baseMonoAvatar.AvatarTypeID);
				AddData((Type)(2002 + num), (int)actor.level);
				AddData((Type)(2003 + num), actor.avatarDataItem.CombatNum);
				AddData((Type)(2004 + num), actor.attack);
				AddData((Type)(2022 + num), avatarStastics.onStageTime);
				AddData((Type)(2010 + num), avatarStastics.hpMax);
				AddData((Type)(2011 + num), avatarStastics.hpBegin);
				AddData((Type)(2012 + num), actor.HP);
				AddData((Type)(2013 + num), avatarStastics.hpGain);
				AddData((Type)(2005 + num), avatarStastics.spMax);
				AddData((Type)(2006 + num), avatarStastics.spBegin);
				AddData((Type)(2007 + num), actor.SP);
				AddData((Type)(2008 + num), avatarStastics.SpRecover);
				AddData((Type)(2009 + num), avatarStastics.spUse);
				AddData((Type)(2014 + num), (int)avatarStastics.avatarHitTimes);
				AddData((Type)(2015 + num), avatarStastics.avatarDamage);
				AddData((Type)(2016 + num), avatarStastics.hitNormalDamageMax);
				AddData((Type)(2017 + num), avatarStastics.hitCriticalDamageMax);
				AddData((Type)(2018 + num), (int)avatarStastics.avatarBeingHitTimes);
				AddData((Type)(2019 + num), avatarStastics.behitNormalDamageMax);
				AddData((Type)(2020 + num), avatarStastics.behitCriticalDamageMax);
				AddData((Type)(2021 + num), avatarStastics.comboMax);
			}*/
		}

		private void AddData(Type type, float value)
		{
			/*//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			List<StageCheatData> list = cheatDataList;
			StageCheatData val = new StageCheatData();
			val.type = type;
			val.value = value;
			list.Add(val);*/
		}
	}
}
