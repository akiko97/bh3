using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class LevelBuffWitchTime : BaseLevelBuff
	{
		public const float WITCH_TIMESCALE = 0.1f;

		private float _duration;

		private float _timer;

		private int _witchTimeStageEffectStackIx;

		private bool _enteringTimeSlow;

		protected bool _notStartEffect;

		public LevelBuffWitchTime(LevelActor levelActor)
		{
			base.levelActor = levelActor;
			levelBuffType = LevelBuffType.WitchTime;
		}

		public void Setup(bool enteringTimeSlow, float duration, LevelBuffSide side, bool notStartEffect)
		{
			_enteringTimeSlow = enteringTimeSlow;
			_duration = duration;
			levelBuffSide = side;
			_notStartEffect = notStartEffect;
		}

		public virtual void SwitchSide(bool enteringTimeSlow, float duration, LevelBuffSide side, uint newOwnerID, bool notStartEffect)
		{
			if (_enteringTimeSlow)
			{
				Setup(false, duration, side, notStartEffect);
				DoWitchTimeStart();
				_enteringTimeSlow = false;
				return;
			}
			_enteringTimeSlow = false;
			_duration = duration;
			_timer = _duration;
			RemoveWitchTimeSlowedBySide();
			levelBuffSide = side;
			ownerID = newOwnerID;
			ActStartParticleEffect();
			ApplyWitchTimeSlowedBySide();
			Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(_witchTimeStageEffectStackIx);
			PushRenderingDataBySide();
		}

		public virtual bool Refresh(float duration, LevelBuffSide side, uint ownerID, bool enteringTimeSlow, bool useMaxDuration, bool notStartEffect)
		{
			if (side == levelBuffSide)
			{
				ExtendDuration(duration, enteringTimeSlow, useMaxDuration);
				return false;
			}
			SwitchSide(enteringTimeSlow, duration, side, ownerID, notStartEffect);
			return true;
		}

		public void ExtendDuration(float duration, bool enteringTimeSlow, bool useMaxDuration)
		{
			if (enteringTimeSlow)
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.5f);
			}
			if (useMaxDuration)
			{
				_duration = Mathf.Max(duration, _timer);
			}
			else
			{
				_duration = duration;
			}
			_timer = _duration;
			ActStartParticleEffect();
		}

		public override void OnAdded()
		{
			WitchTimeStart();
		}

		public override void OnRemoved()
		{
			_timer = 0f;
			WitchTimeStop();
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtMonsterCreated)
			{
				return OnPostMonsterCreated((EvtMonsterCreated)evt);
			}
			if (evt is EvtPropObjectCreated)
			{
				return OnPostPropCreated((EvtPropObjectCreated)evt);
			}
			return false;
		}

		public override void Core()
		{
			if (!muteUpdateDuration && _timer > 0f)
			{
				_timer -= Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime;
				if (_timer <= 0f)
				{
					levelActor.StopLevelBuff(this);
				}
			}
		}

		private void WitchTimeStart()
		{
			if (_enteringTimeSlow)
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.5f, 0.05f, delegate
				{
					if (_enteringTimeSlow && isActive)
					{
						DoWitchTimeStart();
					}
				});
			}
			else
			{
				DoWitchTimeStart();
			}
			RainController rainController = Singleton<StageManager>.Instance.GetPerpStage().rainController;
			if (rainController != null)
			{
				rainController.EnterSlowMode(1f);
			}
			WitchTimeSFX();
		}

		private void DoWitchTimeStart()
		{
			ActStartParticleEffect();
			PushRenderingDataBySide();
			ApplyWitchTimeSlowedBySide();
			Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Push(1, 0.1f);
			_timer = _duration;
			_enteringTimeSlow = false;
		}

		private void WitchTimeSFX()
		{
			Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSkill_Boomoff");
		}

		protected void ApplyWitchTimeEffect(uint actorID)
		{
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(actorID);
			if ((actor.abilityState & AbilityState.WitchTimeSlowed) == 0 && !actor.IsImmuneAbilityState(AbilityState.WitchTimeSlowed))
			{
				actor.AddAbilityState(AbilityState.WitchTimeSlowed, false);
			}
		}

		protected void RemoveWitchTimeEffect(uint actorID)
		{
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(actorID);
			if (actor.abilityState.ContainsState(AbilityState.WitchTimeSlowed))
			{
				actor.RemoveAbilityState(AbilityState.WitchTimeSlowed);
			}
		}

		protected virtual void PushRenderingDataBySide()
		{
			if (levelBuffSide == LevelBuffSide.FromMonster || levelBuffSide == LevelBuffSide.FromLevel)
			{
				PushRedRenderingData();
			}
			else
			{
				PushBlueRenderingData();
			}
		}

		protected void PushBlueRenderingData()
		{
			_witchTimeStageEffectStackIx = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData("Effect_WitchTime", 0.2f);
		}

		protected void PushRedRenderingData()
		{
			_witchTimeStageEffectStackIx = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData("Effect_WitchTime_Monster", 0.2f);
		}

		protected virtual void ApplyWitchTimeSlowedBySide()
		{
			if (levelBuffSide == LevelBuffSide.FromMonster || levelBuffSide == LevelBuffSide.FromLevel)
			{
				List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
				for (int i = 0; i < allAvatars.Count; i++)
				{
					ApplyWitchTimeEffect(allAvatars[i].GetRuntimeID());
				}
			}
			else if (levelBuffSide == LevelBuffSide.FromAvatar || levelBuffSide == LevelBuffSide.FromLevel)
			{
				List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
				for (int j = 0; j < allMonsters.Count; j++)
				{
					ApplyWitchTimeEffect(allMonsters[j].GetRuntimeID());
				}
			}
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			for (int k = 0; k < allPropObjects.Count; k++)
			{
				ApplyWitchTimeEffect(allPropObjects[k].GetRuntimeID());
			}
		}

		public virtual void ApplyWitchTimeSlowedBySideWithRuntimeID(uint runtimeID)
		{
			if (levelBuffSide == LevelBuffSide.FromMonster || levelBuffSide == LevelBuffSide.FromLevel)
			{
				ApplyWitchTimeEffect(runtimeID);
			}
			else if (levelBuffSide == LevelBuffSide.FromAvatar || levelBuffSide == LevelBuffSide.FromLevel)
			{
				ApplyWitchTimeEffect(runtimeID);
			}
		}

		protected virtual void RemoveWitchTimeSlowedBySide()
		{
			if (levelBuffSide == LevelBuffSide.FromMonster || levelBuffSide == LevelBuffSide.FromLevel)
			{
				List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
				for (int i = 0; i < allAvatars.Count; i++)
				{
					RemoveWitchTimeEffect(allAvatars[i].GetRuntimeID());
				}
			}
			else if (levelBuffSide == LevelBuffSide.FromAvatar || levelBuffSide == LevelBuffSide.FromLevel)
			{
				List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
				for (int j = 0; j < allMonsters.Count; j++)
				{
					RemoveWitchTimeEffect(allMonsters[j].GetRuntimeID());
				}
			}
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			for (int k = 0; k < allPropObjects.Count; k++)
			{
				RemoveWitchTimeEffect(allPropObjects[k].GetRuntimeID());
			}
		}

		private void WitchTimeStop()
		{
			ActStopParticleEffect();
			Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(_witchTimeStageEffectStackIx);
			RemoveWitchTimeSlowedBySide();
			RainController rainController = Singleton<StageManager>.Instance.GetPerpStage().rainController;
			if (rainController != null)
			{
				rainController.LeaveSlowMode();
			}
			Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.TryPop(1);
		}

		private bool OnPostMonsterCreated(EvtMonsterCreated evt)
		{
			if (levelBuffSide == LevelBuffSide.FromAvatar || levelBuffSide == LevelBuffSide.FromLevel)
			{
				ApplyWitchTimeEffect(evt.monsterID);
			}
			return true;
		}

		private bool OnPostPropCreated(EvtPropObjectCreated evt)
		{
			if (levelBuffSide == LevelBuffSide.FromMonster || levelBuffSide == LevelBuffSide.FromLevel)
			{
				ApplyWitchTimeEffect(evt.objectID);
			}
			return true;
		}

		protected virtual void ActStartParticleEffect()
		{
			if (!_notStartEffect)
			{
				if (levelBuffSide == LevelBuffSide.FromAvatar)
				{
					ActBlueOpenEffect();
				}
				else if (levelBuffSide == LevelBuffSide.FromMonster)
				{
					ActRedOpenEffect();
				}
			}
		}

		protected virtual void ActStopParticleEffect()
		{
			if (levelBuffSide == LevelBuffSide.FromAvatar)
			{
				ActBlueCloseEffect();
			}
			if (levelBuffSide == LevelBuffSide.FromMonster)
			{
				ActRedCloseEffect();
			}
		}

		protected void ActBlueOpenEffect()
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Witch_Open", Singleton<CameraManager>.Instance.GetMainCamera());
		}

		protected void ActRedOpenEffect()
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_Witch_Open", Singleton<CameraManager>.Instance.GetMainCamera());
		}

		protected void ActBlueCloseEffect()
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Witch_Close", Singleton<CameraManager>.Instance.GetMainCamera());
		}

		protected void ActRedCloseEffect()
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_Witch_Close", Singleton<CameraManager>.Instance.GetMainCamera());
		}
	}
}
