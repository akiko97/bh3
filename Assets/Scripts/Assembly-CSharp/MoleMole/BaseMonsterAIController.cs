using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleJSON;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonsterAIController : BaseController, IAIController
	{
		protected BaseMonoMonster _monster;

		public bool active { get; protected set; }

		public BaseMonsterAIController(BaseMonoMonster monster)
			: base(257u, monster)
		{
			_monster = monster;
		}

		public virtual void SetActive(bool isActive)
		{
			active = isActive;
		}

		public virtual void LoadMetaDataAndInit(BaseMonoMonster aMonster, Hashtable dynamicParamTable, Dictionary<string, MethodInfo> eventHandlerDict, JSONNode aJson)
		{
			throw new NotImplementedException();
		}

		public override void Core()
		{
		}

		public void TrySteer(Vector3 dir)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || _monster.IsMuteControl())
			{
				_monster.OrderMove = false;
			}
			else
			{
				_monster.SteerFaceDirectionTo(Vector3.Lerp(_monster.FaceDirection, dir, _monster.config.StateMachinePattern.ChangeDirLerpRatioForMove * _monster.TimeScale * Time.deltaTime));
			}
		}

		public void TrySteer(Vector3 dir, float lerpRatio)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || _monster.IsMuteControl())
			{
				_monster.OrderMove = false;
				return;
			}
			dir.Normalize();
			_monster.SteerFaceDirectionTo(Vector3.Slerp(_monster.FaceDirection, dir, lerpRatio * _monster.TimeScale * Time.deltaTime));
		}

		public void TrySteerInstant(Vector3 dir)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || _monster.IsMuteControl())
			{
				_monster.OrderMove = false;
			}
			else
			{
				_monster.SteerFaceDirectionTo(dir);
			}
		}

		public bool TryUseSkill(string skillName)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) || _monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit) || _monster.IsMuteControl())
			{
				return false;
			}
			_monster.SetTrigger(skillName + "Trigger");
			return true;
		}

		public void TryMove(float speed)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || _monster.IsMuteControl())
			{
				_monster.OrderMove = false;
				_monster.MoveHorizontal = false;
			}
			else
			{
				_monster.MoveSpeedRatio = speed;
				_monster.MoveHorizontal = false;
				_monster.OrderMove = true;
			}
		}

		public void TryMoveHorizontal(float speed)
		{
			if (_monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.FreezeDirection) || _monster.IsMuteControl())
			{
				_monster.OrderMove = false;
				_monster.MoveHorizontal = false;
			}
			else
			{
				_monster.MoveSpeedRatio = speed;
				_monster.MoveHorizontal = true;
				_monster.OrderMove = true;
			}
		}

		public void TryStop()
		{
			_monster.OrderMove = false;
		}

		public void TrySetAttackTarget(BaseMonoEntity attackTarget)
		{
			_monster.SetAttackTarget(attackTarget);
		}

		public void TryClearAttackTarget()
		{
			_monster.SetAttackTarget(null);
		}
	}
}
