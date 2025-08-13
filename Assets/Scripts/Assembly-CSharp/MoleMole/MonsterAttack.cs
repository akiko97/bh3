using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class MonsterAttack : BehaviorDesigner.Runtime.Tasks.Action
	{
		protected enum State
		{
			WaitAttackStart = 0,
			Doing = 1,
			Success = 2,
			Fail = 3
		}

		private const float MAX_WAIT_TIME_AFTER_SET_TRIGGER = 5f;

		public string attackType;

		public float attackSuccessCD;

		public float attackFailCD;

		public float randomRange;

		public SharedFloat CDRatio;

		public SharedBool HitSuccess;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Attack needs to go through these IDs or it would be considered as fail")]
		public string[] skillIDS;

		protected int _skillIx;

		protected string _curSkillID;

		public SharedFloat AttackCD;

		public SharedInt AvatarBeAttackNum;

		private LevelAIPlugin _levelAIPlugin;

		private float _waitAttackTimer;

		protected State _state;

		private bool _triggerSetted;

		protected BaseMonoMonster _monster;

		protected IAIController _controller;

		private bool _isTargetLocalAvatar;

		public override void OnAwake()
		{
			_monster = GetComponent<BaseMonoMonster>();
			_controller = _monster.GetActiveAIController();
			_levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
		}

		public override void OnStart()
		{
			BaseMonoMonster monster = _monster;
			monster.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(monster.onCurrentSkillIDChanged, new Action<string, string>(MonsterSkillIDChanged));
			BaseMonoMonster monster2 = _monster;
			monster2.onBeHitCanceled = (Action<string>)Delegate.Combine(monster2.onBeHitCanceled, new Action<string>(MonsterBeHitCanceled));
			_waitAttackTimer = 0f;
			_skillIx = 0;
			_state = State.WaitAttackStart;
			BaseMonoAvatar baseMonoAvatar = _monster.AttackTarget as BaseMonoAvatar;
			if (baseMonoAvatar != null && Singleton<AvatarManager>.Instance.IsLocalAvatar(baseMonoAvatar.GetRuntimeID()))
			{
				if (AvatarBeAttackNum.IsShared)
				{
					_levelAIPlugin.AddAttackingMonster(GetComponent<BaseMonoMonster>());
				}
				_isTargetLocalAvatar = true;
			}
			DoCalcSteer();
			DoAttack();
			if (HitSuccess.IsShared)
			{
				HitSuccess.SetValue(false);
			}
		}

		public override void OnEnd()
		{
			if (_isTargetLocalAvatar && AvatarBeAttackNum.IsShared)
			{
				_levelAIPlugin.RemoveAttackingMonster(GetComponent<BaseMonoMonster>());
			}
			BaseMonoMonster monster = _monster;
			monster.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(monster.onCurrentSkillIDChanged, new Action<string, string>(MonsterSkillIDChanged));
			BaseMonoMonster monster2 = _monster;
			monster2.onBeHitCanceled = (Action<string>)Delegate.Remove(monster2.onBeHitCanceled, new Action<string>(MonsterBeHitCanceled));
			_triggerSetted = false;
		}

		protected virtual void OnTransit(State from, State to)
		{
		}

		protected void MonsterBeHitCanceled(string skillID)
		{
			if (_state == State.Doing && _state == State.WaitAttackStart)
			{
				OnTransit(_state, State.Fail);
				_state = State.Fail;
			}
		}

		protected void MonsterSkillIDChanged(string from, string to)
		{
			if (_state == State.WaitAttackStart)
			{
				if (to == skillIDS[0])
				{
					OnTransit(_state, State.Doing);
					_state = State.Doing;
					_curSkillID = to;
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.TriggerHintArrowEffect(_monster.GetRuntimeID(), MonoHintArrow.EffectType.Twinkle);
				}
			}
			else
			{
				if (_state != State.Doing)
				{
					return;
				}
				int num = Array.IndexOf(skillIDS, to, _skillIx);
				if (num >= _skillIx)
				{
					_skillIx = num;
					_curSkillID = to;
				}
				else if (_skillIx == skillIDS.Length - 1 && from == skillIDS[_skillIx])
				{
					if (_state != State.Fail)
					{
						OnTransit(_state, State.Success);
						_state = State.Success;
					}
				}
				else
				{
					OnTransit(_state, State.Fail);
					_state = State.Fail;
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (_state == State.WaitAttackStart)
			{
				_waitAttackTimer += Time.deltaTime * _monster.TimeScale;
				if (!_triggerSetted)
				{
					_triggerSetted = DoAttack();
				}
				if (_waitAttackTimer > 5f)
				{
					OnTransit(_state, State.Fail);
					_state = State.Fail;
				}
			}
			else
			{
				if (_state == State.Success)
				{
					SetAttackCDWithHitSuccess(true);
					return TaskStatus.Success;
				}
				if (_state == State.Fail)
				{
					SetAttackCDWithHitSuccess(false);
					return TaskStatus.Failure;
				}
			}
			return TaskStatus.Running;
		}

		protected virtual void DoCalcSteer()
		{
		}

		protected virtual bool DoAttack()
		{
			return _controller.TryUseSkill(attackType);
		}

		private void SetAttackCDWithHitSuccess(bool success)
		{
			if (HitSuccess.IsShared)
			{
				SetAttackCD(HitSuccess.Value);
			}
			else
			{
				SetAttackCD(success);
			}
		}

		private void SetAttackCD(bool success)
		{
			float num = 0f;
			num = ((!success) ? attackFailCD : attackSuccessCD);
			if (randomRange != 0f)
			{
				num += randomRange * UnityEngine.Random.Range(-1f, 1f);
			}
			if (CDRatio.Value > 0f)
			{
				num *= CDRatio.Value;
			}
			AttackCD.SetValue(num * _monster.GetProperty("AI_AttackCDRatio"));
		}
	}
}
