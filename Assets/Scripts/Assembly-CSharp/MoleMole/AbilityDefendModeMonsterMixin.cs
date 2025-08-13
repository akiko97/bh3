using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityDefendModeMonsterMixin : BaseAbilityMixin
	{
		private const string AI_TYPE_VARIABLE = "_CommonAIType";

		private DefendModeMonsterMixin config;

		private MonsterActor _monsterActor;

		private BaseMonoMonster _monster;

		private EntityTimer _hatredDecreaseTimer;

		private Dictionary<uint, float> _monsterHatredDic;

		private List<float> _hatredAIAreaSections = new List<float>();

		private List<int> _hatredAIValues = new List<int>();

		private int _currentHatredAIValue;

		private int _defaultHatredAIValue;

		private EntityTimer _minAISwitchTimer;

		public AbilityDefendModeMonsterMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (DefendModeMonsterMixin)config;
			_monsterActor = actor as MonsterActor;
			_monster = entity as BaseMonoMonster;
			_monsterHatredDic = new Dictionary<uint, float>();
			_monsterHatredDic.Clear();
			_hatredDecreaseTimer = new EntityTimer(instancedAbility.Evaluate(this.config.HatredDecreaseInterval));
			_hatredDecreaseTimer.Reset(false);
			_minAISwitchTimer = new EntityTimer(instancedAbility.Evaluate(this.config.MinAISwitchDuration));
			_minAISwitchTimer.Reset(false);
			for (int i = 0; i < this.config.hatredAIAreaSections.Length; i++)
			{
				_hatredAIAreaSections.Add(this.config.hatredAIAreaSections[i]);
			}
			_hatredAIAreaSections.Add(1f);
			for (int j = 0; j < this.config.hatredAIValues.Length; j++)
			{
				_hatredAIValues.Add(this.config.hatredAIValues[j]);
			}
			_defaultHatredAIValue = this.config.DefaultAIValue;
			_currentHatredAIValue = _defaultHatredAIValue;
			BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)_monster.GetActiveAIController();
			if (bTreeMonsterAIController != null)
			{
				bTreeMonsterAIController.OnAIActive = (Action<bool>)Delegate.Combine(bTreeMonsterAIController.OnAIActive, new Action<bool>(OnMonsterAIActive));
			}
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtBuffAdd)
			{
				return OnBuffAdd((EvtBuffAdd)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				return OnMonsterCreated((EvtMonsterCreated)evt);
			}
			return false;
		}

		public override void OnAdded()
		{
		}

		public override void OnRemoved()
		{
		}

		public override void Core()
		{
			if (!HasHatred())
			{
				return;
			}
			_hatredDecreaseTimer.Core(1f);
			_minAISwitchTimer.Core(1f);
			if (_hatredDecreaseTimer.isTimeUp && _hatredDecreaseTimer.isActive)
			{
				List<uint> list = new List<uint>();
				List<uint> list2 = new List<uint>();
				foreach (KeyValuePair<uint, float> item in _monsterHatredDic)
				{
					if (item.Value > 0f)
					{
						if (_monsterHatredDic[item.Key] - instancedAbility.Evaluate(config.HatredDecreateRateByInterval) < 0f)
						{
							list.Add(item.Key);
						}
						else
						{
							list2.Add(item.Key);
						}
					}
				}
				foreach (uint item2 in list2)
				{
					Dictionary<uint, float> monsterHatredDic;
					Dictionary<uint, float> dictionary = (monsterHatredDic = _monsterHatredDic);
					uint key2;
					uint key = (key2 = item2);
					float num = monsterHatredDic[key2];
					dictionary[key] = num - instancedAbility.Evaluate(config.HatredDecreateRateByInterval);
				}
				foreach (uint item3 in list)
				{
					_monsterHatredDic.Remove(item3);
				}
				list.Clear();
				list2.Clear();
				if (!HasHatred())
				{
					_hatredDecreaseTimer.Reset(false);
				}
				else
				{
					_hatredDecreaseTimer.Reset(true);
				}
			}
			uint num2 = 0u;
			foreach (uint key3 in _monsterHatredDic.Keys)
			{
				float num3 = 0f;
				if (_monsterHatredDic[key3] > num3)
				{
					num3 = _monsterHatredDic[key3];
					num2 = key3;
				}
			}
			if (num2 != 0)
			{
				int hatredAIValue = GetHatredAIValue(_monsterHatredDic[num2]);
				if (_currentHatredAIValue != hatredAIValue)
				{
					SwitchAI(hatredAIValue);
				}
			}
			else if (_currentHatredAIValue != _defaultHatredAIValue)
			{
				SwitchAI(_defaultHatredAIValue);
			}
		}

		private bool OnMonsterCreated(EvtMonsterCreated evt)
		{
			return false;
		}

		private void OnMonsterAIActive(bool active)
		{
			if (active)
			{
				BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)_monster.GetActiveAIController();
				bTreeMonsterAIController.SetBehaviorVariable("_CommonAIType", _defaultHatredAIValue);
				if (bTreeMonsterAIController != null)
				{
					bTreeMonsterAIController.OnAIActive = (Action<bool>)Delegate.Remove(bTreeMonsterAIController.OnAIActive, new Action<bool>(OnMonsterAIActive));
				}
			}
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (_monsterActor != null)
			{
				if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) != 3)
				{
					return false;
				}
				float totalDamage = evt.attackData.GetTotalDamage();
				if (totalDamage < 0f)
				{
					return false;
				}
				float num = _monsterActor.maxHP;
				float num2 = totalDamage / num;
				if (num2 > instancedAbility.Evaluate(config.HatredAddThreholdRatioByDamage))
				{
					AddHatred(evt.sourceID, instancedAbility.Evaluate(config.HatredAddRateByDamage));
				}
			}
			return false;
		}

		private bool OnBuffAdd(EvtBuffAdd evt)
		{
			return false;
		}

		private void AddHatred(uint targetID, float hatred)
		{
			if (!_hatredDecreaseTimer.isActive)
			{
				_hatredDecreaseTimer.Reset(true);
			}
			if (_monsterHatredDic.ContainsKey(targetID))
			{
				_monsterHatredDic[targetID] = Mathf.Clamp(_monsterHatredDic[targetID] + hatred, 0f, 1f);
			}
			else
			{
				_monsterHatredDic.Add(targetID, hatred);
			}
		}

		private bool HasHatred()
		{
			return _monsterHatredDic.Count > 0;
		}

		private void ClearHatred()
		{
			_monsterHatredDic.Clear();
			_hatredDecreaseTimer.Reset(false);
		}

		private void SwitchAI(int aiTypeValue)
		{
			if (!_minAISwitchTimer.isActive)
			{
				_minAISwitchTimer.Reset(true);
			}
			if (!_minAISwitchTimer.isTimeUp || !_monster.IsAIControllerActive())
			{
				return;
			}
			BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)_monster.GetActiveAIController();
			if (bTreeMonsterAIController != null)
			{
				if (!bTreeMonsterAIController.IsBehaviorRunning())
				{
					bTreeMonsterAIController.EnableBehavior();
				}
				bTreeMonsterAIController.SetBehaviorVariable("_CommonAIType", aiTypeValue);
				_currentHatredAIValue = aiTypeValue;
			}
		}

		private bool IsMonsterAIRunning()
		{
			if (_monster.IsAIControllerActive())
			{
				BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)_monster.GetActiveAIController();
				return bTreeMonsterAIController != null && bTreeMonsterAIController.IsBehaviorRunning();
			}
			return false;
		}

		private int GetHatredAIValue(float hatredValue)
		{
			int result = _currentHatredAIValue;
			for (int i = 0; i < _hatredAIAreaSections.Count; i++)
			{
				if (hatredValue <= _hatredAIAreaSections[i])
				{
					result = _hatredAIValues[i];
					break;
				}
			}
			return result;
		}
	}
}
