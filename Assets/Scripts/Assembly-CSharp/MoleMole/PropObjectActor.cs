using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class PropObjectActor : BaseAbilityActor
	{
		public const float GOODS_DROP_MAX_DISTANCE = 1f;

		public ConfigPropObject config;

		private BaseMonoPropObject prop;

		private float _opacity;

		private ActorTriggerFieldPlugin _triggerFieldPlugin;

		public List<LDDropDataItem> dropDataItems;

		public bool needDropReward = true;

		public float Opacity
		{
			get
			{
				return _opacity;
			}
			set
			{
				SetPorpObjectOpacity(value);
				_opacity = value;
			}
		}

		public override void Init(BaseMonoEntity entity)
		{
			prop = (BaseMonoPropObject)entity;
			config = prop.config;
			commonConfig = config.CommonConfig;
			base.Init(entity);
			for (int i = 0; i < config.Abilities.Length; i++)
			{
				ConfigEntityAbilityEntry configEntityAbilityEntry = config.Abilities[i];
				appliedAbilities.Add(Tuple.Create(AbilityData.GetAbilityConfig(configEntityAbilityEntry.AbilityName, configEntityAbilityEntry.AbilityOverride), new Dictionary<string, object>()));
			}
			if (config.PropArguments.IsTriggerField)
			{
				_triggerFieldPlugin = new ActorTriggerFieldPlugin(this);
				AddPlugin(_triggerFieldPlugin);
			}
			for (int j = 0; j < 27; j++)
			{
				AbilityState abilityState = (AbilityState)(1 << j);
				if ((abilityState & (AbilityState.Endure | AbilityState.MoveSpeedUp | AbilityState.AttackSpeedUp | AbilityState.PowerUp | AbilityState.Shielded | AbilityState.CritUp | AbilityState.Immune | AbilityState.MaxMoveSpeed | AbilityState.Undamagable)) != AbilityState.None || (abilityState & (AbilityState.Bleed | AbilityState.Stun | AbilityState.Paralyze | AbilityState.Burn | AbilityState.Poisoned | AbilityState.Frozen | AbilityState.MoveSpeedDown | AbilityState.AttackSpeedDown | AbilityState.Weak | AbilityState.Fragile | AbilityState.TargetLocked | AbilityState.Tied)) != AbilityState.None)
				{
					SetAbilityStateImmune(abilityState, true);
				}
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(runtimeID);
			_opacity = 1f;
		}

		public void InitProp(float HP, float attack)
		{
			baseMaxHP = (maxHP = (base.HP = HP));
			if (!config.PropArguments.UseOwnerAttack)
			{
				base.attack = attack;
				return;
			}
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(ownerID);
			if (actor == null)
			{
				ForceKill(562036737u, KillEffect.KillImmediately);
			}
			else
			{
				base.attack = actor.attack;
			}
		}

		public override void PostInit()
		{
			base.PostInit();
			abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.DoNotRemoveUntilDestroyed;
		}

		public override bool OnEventWithPlugins(BaseEvent evt)
		{
			bool result = base.OnEventWithPlugins(evt);
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			if (evt is EvtFieldHit)
			{
				return OnFieldHit((EvtFieldHit)evt);
			}
			if (evt is EvtPropObjectForceKilled)
			{
				return OnForceKilled((EvtPropObjectForceKilled)evt);
			}
			return result;
		}

		public override bool OnEventResolves(BaseEvent evt)
		{
			bool result = base.OnEventResolves(evt);
			if (evt is EvtFieldEnter)
			{
				return OnFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtBeingHit)
			{
				return OnBeingHitResolve((EvtBeingHit)evt);
			}
			if (evt is EvtHittingOther)
			{
				return OnHittingOtherResolve((EvtHittingOther)evt);
			}
			return result;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool result = base.ListenEvent(evt);
			if (evt is EvtLevelBuffState)
			{
				return OnLevelBuffState((EvtLevelBuffState)evt);
			}
			return result;
		}

		private bool OnLevelBuffState(EvtLevelBuffState evt)
		{
			if (evt.levelBuff == LevelBuffType.WitchTime)
			{
				if (evt.state == LevelBuffState.Start)
				{
					return OnWitchTimeStart();
				}
				if (evt.state == LevelBuffState.Stop)
				{
					return OnWitchTimeStop();
				}
			}
			return false;
		}

		private bool OnWitchTimeStart()
		{
			return true;
		}

		private bool OnWitchTimeStop()
		{
			return true;
		}

		public void SetPorpObjectOpacity(float opacity)
		{
			List<MeshRenderer> list = new List<MeshRenderer>(gameObject.GetComponentsInChildren<MeshRenderer>());
			List<SkinnedMeshRenderer> list2 = new List<SkinnedMeshRenderer>(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
			if (list != null)
			{
				foreach (MeshRenderer item in list)
				{
					item.material.SetFloat("_Opaqueness", opacity);
				}
			}
			if (list2 == null)
			{
				return;
			}
			foreach (SkinnedMeshRenderer item2 in list2)
			{
				item2.material.SetFloat("_Opaqueness", opacity);
			}
		}

		public float GetPropObjectCurrentOpacity()
		{
			MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
			if (componentInChildren != null)
			{
				return componentInChildren.sharedMaterial.GetFloat("_Opaqueness");
			}
			return 1f;
		}

		private bool OnFieldEnter(EvtFieldEnter evt)
		{
			if (config.PropArguments.TriggerHitWhenFieldEnter)
			{
				BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(evt.otherID);
				if (baseMonoEntity != null)
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(runtimeID, baseMonoEntity.GetRuntimeID(), config.PropArguments.AnimEventIDForHit));
				}
			}
			if (config.PropArguments.DieWhenFieldEnter)
			{
				Kill(Singleton<LevelManager>.Instance.levelActor.runtimeID, string.Empty);
			}
			return true;
		}

		private bool OnFieldHit(EvtFieldHit evt)
		{
			for (int i = 0; i < _triggerFieldPlugin.insideIDs.Count; i++)
			{
				uint item = _triggerFieldPlugin.insideIDs[i];
				BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(item);
				if (!(baseMonoEntity == null))
				{
					if (!baseMonoEntity.IsActive())
					{
						_triggerFieldPlugin.insideIDs.Remove(item);
					}
					else
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(runtimeID, _triggerFieldPlugin.insideIDs[i], evt.animEventID));
					}
				}
			}
			return true;
		}

		private bool OnForceKilled(EvtPropObjectForceKilled evt)
		{
			ForceKill(Singleton<LevelManager>.Instance.levelActor.runtimeID, KillEffect.KillNow);
			return true;
		}

		private bool OnHittingOther(EvtHittingOther evt)
		{
			if (evt.attackData == null)
			{
				evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(this, evt.animEventID);
			}
			if (evt.hitCollision == null)
			{
				BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.toID);
				if (actor != null)
				{
					BaseMonoEntity victimEntity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
					evt.hitCollision = CalcHitCollision(config.PropArguments.RetreatType, victimEntity);
				}
			}
			evt.attackData.hitCollision = evt.hitCollision;
			return true;
		}

		private AttackResult.HitCollsion CalcHitCollision(ConfigPropObject.E_RetreatType retreatType, BaseMonoEntity victimEntity)
		{
			AttackResult.HitCollsion hitCollsion = new AttackResult.HitCollsion();
			if (retreatType == ConfigPropObject.E_RetreatType.Spike)
			{
				hitCollsion.hitPoint = victimEntity.GetAttachPoint("RootNode").position;
				Vector3 vector = victimEntity.transform.position + Vector3.up * 0.5f;
				Vector3 direction = prop.transform.position - vector;
				RaycastHit hitInfo;
				if (Physics.Raycast(vector, direction, out hitInfo, 10f, 1 << InLevelData.PROP_LAYER))
				{
					hitCollsion.hitDir = hitInfo.normal;
				}
			}
			else
			{
				hitCollsion.hitPoint = entity.GetAttachPoint("RootNode").position;
				hitCollsion.hitDir = entity.XZPosition - prop.XZPosition;
			}
			return hitCollsion;
		}

		private bool AlmostEqualOrBigger(float a, float b)
		{
			return Mathf.Abs(a) + 0.001f > Mathf.Abs(b);
		}

		private bool OnHittingOtherResolve(EvtHittingOther evt)
		{
			evt.Resolve();
			Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, runtimeID, evt.animEventID, evt.attackData));
			return true;
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
			return true;
		}

		private bool OnBeingHitResolve(EvtBeingHit evt)
		{
			evt.Resolve();
			AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
			if (attackResult.hitCollision == null)
			{
				attackResult.hitCollision = new AttackResult.HitCollsion
				{
					hitPoint = prop.RootNode.position,
					hitDir = -prop.transform.forward
				};
			}
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if ((bool)isAlive)
			{
				float totalDamage = attackResult.GetTotalDamage();
				float num = (float)HP - totalDamage;
				if (num <= 0f)
				{
					num = 0f;
				}
				DelegateUtils.UpdateField(ref HP, num, num - (float)HP, onHPChanged);
				if ((float)HP == 0f)
				{
					if (abilityState.ContainsState(AbilityState.Limbo))
					{
						BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
					}
					else
					{
						BeingHit(attackResult, BeHitEffect.KillingBeHit, evt.sourceID);
						Kill(evt.sourceID, evt.animEventID);
					}
				}
				else
				{
					BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
				}
			}
			if (attackResult.attackEffectPattern != null && (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal || attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack))
			{
				AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, prop, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
			}
			if (attackResult.beHitEffectPattern != null && (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal || attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyBeHit))
			{
				AttackPattern.ActAttackEffects(attackResult.beHitEffectPattern, prop, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
			}
			if (evt.attackData.isAnimEventAttack)
			{
				EvtAttackLanded evtAttackLanded = new EvtAttackLanded(evt.sourceID, runtimeID, evt.animEventID, attackResult);
				Singleton<EventManager>.Instance.FireEvent(evtAttackLanded);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackLanded, evtAttackLanded));
			}
			else
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtDamageLanded(evt.sourceID, runtimeID, attackResult));
			}
			return true;
		}

		public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect, uint sourceID)
		{
			prop.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity, sourceID);
		}

		protected virtual void Kill(uint killerID, string killerAnimEventID)
		{
			isAlive = false;
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID, killerID, killerAnimEventID));
			prop.SetDied(KillEffect.KillNow);
		}

		public override void ForceKill(uint killerID, KillEffect killEffect)
		{
			isAlive = false;
			Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID));
			prop.SetDied(KillEffect.KillNow);
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			if (Singleton<LevelManager>.Instance == null || Singleton<LevelManager>.Instance.levelActor.levelState != LevelActor.LevelState.LevelRunning || dropDataItems == null || !needDropReward)
			{
				return;
			}
			if (dropDataItems.Count == 1)
			{
				dropDataItems[0].CreateDropGoods(GetDropPosition(), Vector3.forward);
			}
			else
			{
				if (dropDataItems.Count <= 1)
				{
					return;
				}
				foreach (LDDropDataItem dropDataItem in dropDataItems)
				{
					dropDataItem.CreateDropGoods(GetDropPosition(), Vector3.forward);
				}
			}
		}

		private Vector3 GetDropPosition()
		{
			Vector3 result = new Vector3(prop.XZPosition.x, 1.5f, prop.XZPosition.z);
			return result;
		}
	}
}
