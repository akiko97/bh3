using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class Attack : BehaviorDesigner.Runtime.Tasks.Action
	{
		private enum AttackState
		{
			NONE = 0,
			TO_START = 1,
			DOING = 2,
			SUCCESS = 3,
			FAIL = 4
		}

		private const float MAX_WAIT_TIME_AFTER_SET_TRIGGER = 5f;

		public string attackType;

		public float attackSuccessCD;

		public float attackFailCD;

		public float randomRange;

		public SharedFloat CDRatio;

		public SharedBool HitSuccess;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("Attack needs to go through these attacks or it would be considered as fail")]
		public string[] SkillIDs;

		private int _skillIDIx;

		public SharedFloat AttackCD;

		public SharedInt AvatarBeAttackNum;

		private LevelAIPlugin _levelAIPlugin;

		public bool SteerToTargetOnStart;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("How many seconds it takes to steer to avatar. 0 means instant.")]
		public float SteerToTargetDuration;

		public bool SteerInAllSkillIDs;

		private float _timer;

		private AttackState _attackState;

		private IAIEntity _aiEntity;

		private IAIController _aiController;

		private BaseMonoAbilityEntity _abilityEntity;

		private bool _isTargetLocalAvatar;

		private string _attackSkillID;

		private bool _hasSteeredToFacing;

		private Coroutine _steerToFacingIter;

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoAvatar)
			{
				_aiEntity = (BaseMonoAvatar)component;
			}
			else if (component is BaseMonoMonster)
			{
				_aiEntity = (BaseMonoMonster)component;
			}
			_aiController = _aiEntity.GetActiveAIController();
			_abilityEntity = component;
			_levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
		}

		public override void OnStart()
		{
			_attackState = AttackState.NONE;
			BaseMonoAvatar baseMonoAvatar = _aiEntity.AttackTarget as BaseMonoAvatar;
			if (baseMonoAvatar == null)
			{
				_isTargetLocalAvatar = false;
			}
			else
			{
				_isTargetLocalAvatar = Singleton<AvatarManager>.Instance.IsLocalAvatar(baseMonoAvatar.GetRuntimeID());
			}
			if (_isTargetLocalAvatar && AvatarBeAttackNum.IsShared)
			{
				_levelAIPlugin.AddAttackingMonster(GetComponent<BaseMonoMonster>());
			}
			if (SteerToTargetOnStart || (SkillIDs != null && SkillIDs.Length > 0))
			{
				BaseMonoAbilityEntity abilityEntity = _abilityEntity;
				abilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(abilityEntity.onCurrentSkillIDChanged, new Action<string, string>(MonsterSkillIDChanged));
				_skillIDIx = -1;
				_hasSteeredToFacing = SteerToTargetOnStart;
			}
			BaseMonoAbilityEntity abilityEntity2 = _abilityEntity;
			abilityEntity2.onBeHitCanceled = (Action<string>)Delegate.Combine(abilityEntity2.onBeHitCanceled, new Action<string>(MonsterBeHitCanceld));
			if (HitSuccess.IsShared)
			{
				HitSuccess.SetValue(false);
			}
		}

		private IEnumerator SteerToTargetFacingIter(Vector3 fromForward, Vector3 targetForward, float duration)
		{
			float t = 0f;
			fromForward.y = 0f;
			fromForward.Normalize();
			targetForward.y = 0f;
			targetForward.Normalize();
			for (; t < duration; t += _abilityEntity.TimeScale * Time.deltaTime)
			{
				_abilityEntity.SteerFaceDirectionTo(Vector3.Slerp(fromForward, targetForward, t / duration));
				yield return null;
			}
			_steerToFacingIter = null;
		}

		private void MonsterBeHitCanceld(string skillID)
		{
			if (_attackState != AttackState.TO_START && _attackState == AttackState.DOING)
			{
				_attackState = AttackState.FAIL;
			}
		}

		private void MonsterSkillIDChanged(string from, string to)
		{
			if (_attackSkillID == null && to != null)
			{
				_attackSkillID = to;
			}
			bool flag = false;
			if (SteerInAllSkillIDs)
			{
				for (int i = 0; i < SkillIDs.Length; i++)
				{
					if (to == SkillIDs[i])
					{
						flag = true;
						_hasSteeredToFacing = true;
					}
				}
			}
			else
			{
				string text = ((SkillIDs.Length == 0) ? attackType : SkillIDs[0]);
				if (to == text)
				{
					flag = true;
				}
			}
			if (_hasSteeredToFacing && flag && _abilityEntity.GetAttackTarget() != null && _abilityEntity.GetAttackTarget().IsActive())
			{
				Vector3 vector = _abilityEntity.GetAttackTarget().XZPosition - _abilityEntity.XZPosition;
				vector.Normalize();
				if (SteerToTargetDuration == 0f)
				{
					_abilityEntity.SteerFaceDirectionTo(vector);
				}
				else
				{
					_steerToFacingIter = _abilityEntity.StartCoroutine(SteerToTargetFacingIter(_abilityEntity.transform.forward, vector, SteerToTargetDuration));
				}
				_hasSteeredToFacing = false;
			}
			if (SkillIDs == null || SkillIDs.Length <= 0)
			{
				return;
			}
			if (_attackState == AttackState.TO_START)
			{
				if (to == SkillIDs[0])
				{
					_attackState = AttackState.DOING;
					_skillIDIx = 0;
					Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.TriggerHintArrowEffect(_abilityEntity.GetRuntimeID(), MonoHintArrow.EffectType.Twinkle);
				}
			}
			else
			{
				if (_attackState != AttackState.DOING)
				{
					return;
				}
				int num = Array.IndexOf(SkillIDs, to);
				if (num >= _skillIDIx)
				{
					_attackState = AttackState.DOING;
					_skillIDIx = num;
				}
				else if (_skillIDIx == SkillIDs.Length - 1 && from == SkillIDs[_skillIDIx])
				{
					if (_attackState != AttackState.FAIL)
					{
						_attackState = AttackState.SUCCESS;
					}
				}
				else
				{
					_attackState = AttackState.FAIL;
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (_attackState == AttackState.NONE)
			{
				if (DoAttack())
				{
					_attackState = AttackState.TO_START;
					_timer = 5f;
				}
				else
				{
					_attackState = AttackState.FAIL;
				}
			}
			if (_attackState == AttackState.FAIL)
			{
				SetAttackCDWithHitSuccess(false);
				return TaskStatus.Failure;
			}
			if (_attackState == AttackState.SUCCESS)
			{
				SetAttackCDWithHitSuccess(true);
				return TaskStatus.Success;
			}
			if (_attackState == AttackState.TO_START)
			{
				_timer -= Time.deltaTime * _aiEntity.TimeScale;
				if (_timer < 0f)
				{
					SetAttackCDWithHitSuccess(false);
					return TaskStatus.Failure;
				}
			}
			return TaskStatus.Running;
		}

		public override void OnEnd()
		{
			if (_isTargetLocalAvatar && AvatarBeAttackNum.IsShared)
			{
				_levelAIPlugin.RemoveAttackingMonster(GetComponent<BaseMonoMonster>());
			}
			if (SteerToTargetOnStart || (SkillIDs != null && SkillIDs.Length > 0))
			{
				BaseMonoAbilityEntity abilityEntity = _abilityEntity;
				abilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(abilityEntity.onCurrentSkillIDChanged, new Action<string, string>(MonsterSkillIDChanged));
			}
			BaseMonoAbilityEntity abilityEntity2 = _abilityEntity;
			abilityEntity2.onBeHitCanceled = (Action<string>)Delegate.Remove(abilityEntity2.onBeHitCanceled, new Action<string>(MonsterBeHitCanceld));
			if (_steerToFacingIter != null && _abilityEntity != null && _abilityEntity.gameObject.activeSelf)
			{
				_abilityEntity.StopCoroutine(_steerToFacingIter);
				_steerToFacingIter = null;
			}
		}

		private bool DoAttack()
		{
			return _aiController.TryUseSkill(attackType);
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
			AttackCD.SetValue(num * _aiEntity.GetProperty("AI_AttackCDRatio"));
		}
	}
}
