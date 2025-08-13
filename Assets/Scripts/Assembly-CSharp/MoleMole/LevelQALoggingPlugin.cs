using System;

namespace MoleMole
{
	public class LevelQALoggingPlugin : BaseActorPlugin
	{
		public LevelActor _levelActor;

		public LevelQALoggingPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtDamageLanded>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackLanded>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtDamageLanded>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_levelActor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAvatarCreated)
			{
				return ListenAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			if (evt is EvtAttackLanded)
			{
				return ListenAttackLanded((EvtAttackLanded)evt);
			}
			if (evt is EvtDamageLanded)
			{
				return ListenDamageLanded((EvtDamageLanded)evt);
			}
			return false;
		}

		private bool ListenAvatarCreated(EvtAvatarCreated evt)
		{
			AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.avatarID);
			AvatarActor avatarActor2 = avatarActor;
			avatarActor2.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Combine(avatarActor2.onAbilityStateAdd, (Action<AbilityState, bool>)delegate(AbilityState state, bool muteDisplay)
			{
				DebugLog("{0} 开始Buff/Debuff: {1}", Miscs.GetDebugActorName(avatarActor), state);
			});
			AvatarActor avatarActor3 = avatarActor;
			avatarActor3.onAbilityStateRemove = (Action<AbilityState>)Delegate.Combine(avatarActor3.onAbilityStateRemove, (Action<AbilityState>)delegate(AbilityState state)
			{
				DebugLog("{0} 停止Buff/Debuff: {1}", Miscs.GetDebugActorName(avatarActor), state);
			});
			AvatarActor avatarActor4 = avatarActor;
			avatarActor4.onHPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor4.onHPChanged, (Action<float, float, float>)delegate(float orig, float newValue, float amount)
			{
				if (amount > 0f)
				{
					DebugLog("{0} 回复HP {1}, 新 HP 值为 {2}", Miscs.GetDebugActorName(avatarActor), amount, newValue);
				}
			});
			AvatarActor avatarActor5 = avatarActor;
			avatarActor5.onSPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor5.onSPChanged, (Action<float, float, float>)delegate(float orig, float newValue, float amount)
			{
				if (amount > 0f)
				{
					DebugLog("{0} 回复SP {1}，新 SP 值为 {2}", Miscs.GetDebugActorName(avatarActor), amount, newValue);
				}
			});
			BaseMonoAbilityEntity entity = avatarActor.entity;
			entity.onActiveChanged = (Action<bool>)Delegate.Combine(entity.onActiveChanged, (Action<bool>)delegate(bool active)
			{
				DebugLog("{0} {1}", Miscs.GetDebugActorName(avatarActor), (!active) ? "下场" : "上场");
			});
			BaseMonoAbilityEntity entity2 = avatarActor.entity;
			entity2.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(entity2.onCurrentSkillIDChanged, (Action<string, string>)delegate(string from, string to)
			{
				DebugLog("{0} SkillID 变动为 {1}", Miscs.GetDebugActorName(avatarActor), (to != null) ? to : "<null>");
			});
			return true;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID);
			MonsterActor monsterActor2 = monsterActor;
			monsterActor2.onAbilityStateAdd = (Action<AbilityState, bool>)Delegate.Combine(monsterActor2.onAbilityStateAdd, (Action<AbilityState, bool>)delegate(AbilityState state, bool muteDisplay)
			{
				DebugLog("{0} 开始Buff/Debuff: {1}", Miscs.GetDebugActorName(monsterActor), state);
			});
			MonsterActor monsterActor3 = monsterActor;
			monsterActor3.onAbilityStateRemove = (Action<AbilityState>)Delegate.Combine(monsterActor3.onAbilityStateRemove, (Action<AbilityState>)delegate(AbilityState state)
			{
				DebugLog("{0} 停止Buff/Debuff: {1}", Miscs.GetDebugActorName(monsterActor), state);
			});
			MonsterActor monsterActor4 = monsterActor;
			monsterActor4.onHPChanged = (Action<float, float, float>)Delegate.Combine(monsterActor4.onHPChanged, (Action<float, float, float>)delegate(float orig, float newValue, float amount)
			{
				if (amount > 0f)
				{
					DebugLog("{0} 回复HP {1}, 新 HP 值为 {2}", Miscs.GetDebugActorName(monsterActor), amount, newValue);
				}
			});
			BaseMonoAbilityEntity entity = monsterActor.entity;
			entity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(entity.onCurrentSkillIDChanged, (Action<string, string>)delegate(string from, string to)
			{
				DebugLog("{0} SkillID 变动为 {1}", Miscs.GetDebugActorName(monsterActor), (to != null) ? to : "<null>");
			});
			return true;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3 || Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4)
			{
				DebugLog("{0} 被 {1} 杀死", Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.killerID)));
				return true;
			}
			return false;
		}

		private bool ListenAttackLanded(EvtAttackLanded evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID);
			if ((num == 3 || num == 4) && (num2 == 3 || num2 == 4))
			{
				DebugLog("{0} 外在攻击到 {1} 成功, 判定 ID {2}, 攻击结果 {3}", Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID)), (evt.animEventID != null) ? evt.animEventID : "<!null>", evt.attackResult.GetDebugOutput());
			}
			return true;
		}

		private bool ListenDamageLanded(EvtDamageLanded evt)
		{
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID);
			if ((num == 3 || num == 4) && (num2 == 3 || num2 == 4))
			{
				DebugLog("{0} 内在攻击到 {1} 成功，攻击结果 {2}", Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID)), evt.attackResult.GetDebugOutput());
			}
			return true;
		}

		private void DebugLog(string format, params object[] args)
		{
		}
	}
}
