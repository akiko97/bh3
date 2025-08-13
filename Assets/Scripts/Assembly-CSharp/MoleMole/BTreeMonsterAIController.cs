using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class BTreeMonsterAIController : BaseMonsterAIController
	{
		private enum State
		{
			WaitForStandby = 0,
			Running = 1
		}

		private BehaviorTree _btree;

		private string _AIName;

		private bool _disableBehaviorWhenInit;

		public Action<bool> OnAIActive;

		private State _state;

		private List<Tuple<EntityTimer, ConfigDynamicArguments>> _delayedParameters;

		public BehaviorTree btree
		{
			get
			{
				return _btree;
			}
		}

		public BTreeMonsterAIController(BaseMonoMonster monster, string AIName, bool disableBehaviorWhenInit)
			: base(monster)
		{
			_monster = monster;
			_AIName = AIName;
			_disableBehaviorWhenInit = disableBehaviorWhenInit;
			InitBTree(AIName);
			base.SetActive(true);
			_state = State.WaitForStandby;
			monster.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(monster.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(WaitFirstStandbyCallback));
		}

		public void EnableBehavior()
		{
			if (!BehaviorManager.instance.IsBehaviorEnabled(_btree))
			{
				_btree.EnableBehavior();
			}
			if (OnAIActive != null)
			{
				OnAIActive(true);
			}
		}

		public void DisableBehavior()
		{
			_btree.DisableBehavior();
		}

		public void RefreshBehavior()
		{
			DisableBehavior();
			UnityEngine.Object.DestroyImmediate(_btree);
			_btree = null;
			InitBTree(_AIName);
		}

		private void InitBTree(string AIName)
		{
			_btree = _monster.gameObject.AddComponent<BehaviorTree>();
			_btree.RestartWhenComplete = true;
			_btree.StartWhenEnabled = false;
			if (!_disableBehaviorWhenInit)
			{
				_btree.DisableBehaviorWhenMonoDisabled = false;
				_btree.TryEnableBehaviorWhenMonoEnabled = false;
			}
			string path = "AI/Monster/" + AIName;
			ExternalBehaviorTree externalBehavior = Miscs.LoadResource<ExternalBehaviorTree>(path);
			_btree.ExternalBehavior = externalBehavior;
			if (_disableBehaviorWhenInit)
			{
				_btree.CheckForSerialization();
				_btree.DisableBehavior();
			}
			else
			{
				_btree.UpdateInterval = UpdateIntervalType.Manual;
			}
		}

		private void WaitFirstStandbyCallback(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (toState.tagHash == MonsterData.MONSTER_IDLESUB_TAG)
			{
				_state = State.Running;
				if (base.active)
				{
					EnableBehavior();
					_btree.UpdateInterval = UpdateIntervalType.EveryFrame;
				}
				BaseMonoMonster monster = _monster;
				monster.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(monster.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(WaitFirstStandbyCallback));
			}
		}

		public override void Core()
		{
			if (_delayedParameters == null)
			{
				return;
			}
			for (int i = 0; i < _delayedParameters.Count; i++)
			{
				if (_delayedParameters[i] != null)
				{
					_delayedParameters[i].Item1.Core(1f);
					if (_delayedParameters[i].Item1.isTimeUp)
					{
						AIData.SetSharedVariableCompat(_btree, _delayedParameters[i].Item2);
						_delayedParameters[i] = null;
					}
				}
			}
		}

		public override void SetActive(bool isActive)
		{
			if (base.active == isActive)
			{
				return;
			}
			base.SetActive(isActive);
			if (_state == State.Running)
			{
				if (!isActive)
				{
					_btree.UpdateInterval = UpdateIntervalType.Manual;
					return;
				}
				EnableBehavior();
				_btree.UpdateInterval = UpdateIntervalType.EveryFrame;
			}
		}

		public bool IsBehaviorRunning()
		{
			return _state == State.Running;
		}

		public void ChangeBehavior(string AIName)
		{
			string path = "AI/Monster/" + AIName;
			ExternalBehaviorTree externalBehaviorTree = Miscs.LoadResource<ExternalBehaviorTree>(path);
			if (externalBehaviorTree != null)
			{
				_btree.ExternalBehavior = externalBehaviorTree;
				EnableBehavior();
			}
		}

		public void SetBehaviorVariable<T>(string variableName, T variableValue)
		{
			List<SharedVariable> allVariables = _btree.GetAllVariables();
			for (int i = 0; i < allVariables.Count; i++)
			{
				if (allVariables[i].Name == variableName)
				{
					((SharedVariable<T>)allVariables[i]).SetValue(variableValue);
					break;
				}
			}
		}

		public void AddBehaviorVariableFloat(string variableName, float variableValue)
		{
			List<SharedVariable> allVariables = _btree.GetAllVariables();
			for (int i = 0; i < allVariables.Count; i++)
			{
				if (allVariables[i].Name == variableName)
				{
					float num = (float)allVariables[i].GetValue() + variableValue;
					allVariables[i].SetValue(num);
					break;
				}
			}
		}

		public void DelayedSetParameter(float delay, ConfigDynamicArguments parameters)
		{
			if (_delayedParameters == null)
			{
				_delayedParameters = new List<Tuple<EntityTimer, ConfigDynamicArguments>>();
			}
			int index = _delayedParameters.SeekAddPosition();
			_delayedParameters[index] = Tuple.Create(new EntityTimer(delay, _monster, true), parameters);
		}
	}
}
