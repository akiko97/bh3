using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMirrorMixin : BaseAbilityMixin
	{
		private class MirrorData
		{
			public uint mirrorRuntimeID;

			public int mirrorSkillCount;

			public float mirrorLifetime;

			public bool destroyAheadActionTriggered;

			public void Reset()
			{
				mirrorRuntimeID = 0u;
				mirrorSkillCount = 0;
				mirrorLifetime = 0f;
				destroyAheadActionTriggered = false;
			}
		}

		private enum State
		{
			Idle = 0,
			SpawningMirrors = 1,
			MirrorActive = 2
		}

		private MirrorMixin config;

		private int _mirrorAmount;

		private float _mirrorLifespan;

		private EntityTimer _delayTimer;

		private MirrorData[] _mirrorDatas;

		private int _curMirrorIx;

		private State _state;

		public AbilityMirrorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MirrorMixin)config;
			_mirrorLifespan = instancedAbility.Evaluate(this.config.MirrorLastingTime);
			_mirrorAmount = instancedAbility.Evaluate(this.config.MirrorAmount);
			_delayTimer = new EntityTimer(this.config.DelayTime);
			_mirrorDatas = new MirrorData[_mirrorAmount];
			for (int i = 0; i < _mirrorAmount; i++)
			{
				_mirrorDatas[i] = new MirrorData();
			}
			_state = State.Idle;
		}

		public override void OnAdded()
		{
			_delayTimer.SetActive(false);
			_state = State.Idle;
			for (int i = 0; i < _mirrorDatas.Length; i++)
			{
				_mirrorDatas[i].Reset();
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(actor.runtimeID);
			_curMirrorIx = 0;
		}

		public override void OnRemoved()
		{
			for (int i = 0; i < _mirrorDatas.Length; i++)
			{
				if (_mirrorDatas[i].mirrorRuntimeID != 0)
				{
					KillSingleMirror(i);
				}
			}
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return ListenKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			for (int i = 0; i < _mirrorDatas.Length; i++)
			{
				MirrorData mirrorData = _mirrorDatas[i];
				if (mirrorData.mirrorRuntimeID == evt.targetID)
				{
					KillSingleMirror(i);
					return true;
				}
			}
			return false;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (_state == State.SpawningMirrors || _state == State.MirrorActive)
			{
				for (int i = 0; i < _mirrorDatas.Length; i++)
				{
					if (_mirrorDatas[i].mirrorRuntimeID != 0)
					{
						KillSingleMirror(i);
					}
				}
			}
			_state = State.SpawningMirrors;
			_curMirrorIx = 0;
			_delayTimer.SetActive(true);
			_mirrorLifespan = instancedAbility.Evaluate(config.MirrorLastingTime);
			if (config.ApplyAttackerWitchTimeRatio && evt.TriggerEvent != null)
			{
				EvtEvadeSuccess evtEvadeSuccess = evt.TriggerEvent as EvtEvadeSuccess;
				if (evtEvadeSuccess != null)
				{
					MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evtEvadeSuccess.attackerID);
					if (monsterActor != null)
					{
						ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(monsterActor.config, evtEvadeSuccess.skillID);
						if (configMonsterAnimEvent != null)
						{
							_mirrorLifespan *= configMonsterAnimEvent.AttackProperty.WitchTimeRatio;
						}
					}
				}
			}
			for (int j = 0; j < config.SelfModifiers.Length; j++)
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, config.SelfModifiers[j]);
			}
		}

		private void SpawnSingleMirror(int ix)
		{
			if (actor is AvatarActor)
			{
				uint mirrorRuntimeID = Singleton<AvatarManager>.Instance.CreateAvatarMirror((BaseMonoAvatar)entity, entity.XZPosition, entity.transform.forward, config.MirrorAIName, instancedAbility.Evaluate(config.HPRatioOfParent));
				_mirrorDatas[ix].mirrorRuntimeID = mirrorRuntimeID;
			}
			else if (actor is MonsterActor)
			{
				uint mirrorRuntimeID2 = Singleton<MonsterManager>.Instance.CreateMonsterMirror((BaseMonoMonster)entity, entity.XZPosition, entity.transform.forward, config.MirrorAIName, instancedAbility.Evaluate(config.HPRatioOfParent));
				_mirrorDatas[ix].mirrorRuntimeID = mirrorRuntimeID2;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_mirrorDatas[ix].mirrorRuntimeID);
			if (config.MirrorAbilities != null)
			{
				string[] mirrorAbilities = config.MirrorAbilities;
				foreach (string abilityName in mirrorAbilities)
				{
					if (string.IsNullOrEmpty(config.MirrorAbilitiesOverrideName))
					{
						baseAbilityActor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(abilityName));
					}
					else
					{
						baseAbilityActor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(abilityName, config.MirrorAbilitiesOverrideName));
					}
				}
			}
			BehaviorTree component = baseAbilityActor.entity.GetComponent<BehaviorTree>();
			if (component != null)
			{
				component.SetVariableValue("MirrorCreationIndex", ix);
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.MirrorCreateActions, instancedAbility, instancedModifier, baseAbilityActor, null);
		}

		private void KillSingleMirror(int ix)
		{
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_mirrorDatas[ix].mirrorRuntimeID);
			if (!_mirrorDatas[ix].destroyAheadActionTriggered)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.MirrorAheadDestroyActions, instancedAbility, instancedModifier, baseAbilityActor, null);
			}
			actor.abilityPlugin.HandleActionTargetDispatch(config.MirrorDestroyActions, instancedAbility, instancedModifier, baseAbilityActor, null);
			baseAbilityActor.isAlive = false;
			baseAbilityActor.entity.SetDied(KillEffect.KillImmediately);
			_mirrorDatas[ix].Reset();
		}

		public override void Core()
		{
			if (_state == State.SpawningMirrors)
			{
				_delayTimer.Core(1f);
				if (_delayTimer.isTimeUp)
				{
					SpawnSingleMirror(_curMirrorIx);
					_curMirrorIx++;
					if (_curMirrorIx >= _mirrorAmount)
					{
						_state = State.MirrorActive;
					}
					else
					{
						_delayTimer.timespan = config.PerMirrorDelayTime;
						_delayTimer.Reset(true);
					}
				}
			}
			if (_state != State.SpawningMirrors && _state != State.MirrorActive)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < _mirrorAmount; i++)
			{
				if (_mirrorDatas[i].mirrorRuntimeID != 0)
				{
					num++;
					_mirrorDatas[i].mirrorLifetime += Time.deltaTime * entity.TimeScale;
					if (_mirrorDatas[i].mirrorLifetime > _mirrorLifespan)
					{
						KillSingleMirror(i);
					}
					else if (_mirrorDatas[i].mirrorLifetime > _mirrorLifespan - config.AheadTime && !_mirrorDatas[i].destroyAheadActionTriggered)
					{
						BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_mirrorDatas[i].mirrorRuntimeID);
						actor.abilityPlugin.HandleActionTargetDispatch(config.MirrorAheadDestroyActions, instancedAbility, instancedModifier, other, null);
						_mirrorDatas[i].destroyAheadActionTriggered = true;
					}
				}
			}
			if (_state == State.MirrorActive && num == 0)
			{
				for (int j = 0; j < config.SelfModifiers.Length; j++)
				{
					actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.SelfModifiers[j]);
				}
				_state = State.Idle;
			}
		}
	}
}
