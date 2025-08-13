using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoSubMonsterHPBar : MonoBehaviour
	{
		private const string HEAD_TRAN_STR = "Head";

		public MonsterActor attackee;

		private HashSet<uint> attackerSet;

		private float _offset;

		public bool enable;

		private BaseMonoMonster _monster;

		private Action<MonoSubMonsterHPBar> _hideHPBarCallBack;

		private void Awake()
		{
			attackerSet = new HashSet<uint>();
			attackee = null;
			enable = false;
		}

		private void Update()
		{
			if (!enable)
			{
				return;
			}
			if (!attackee.isAlive || !Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(attackee.runtimeID)))
			{
				SetDisable();
				return;
			}
			base.transform.Find("HPBar").GetComponent<MonoSliderGroupWithPhase>().UpdateValue(attackee.HP, attackee.maxHP, 0f);
			Vector3 xZPosition = _monster.XZPosition;
			if (_monster.transform.GetComponent<CapsuleCollider>() == null)
			{
				SetDisable();
			}
			xZPosition.y = _monster.transform.GetComponent<CapsuleCollider>().height + _offset;
			base.transform.position = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(xZPosition);
		}

		public void SetupView(AvatarActor attacker, MonsterActor attackee, float offset, Action<MonoSubMonsterHPBar> hideHPBarCallBack = null)
		{
			base.gameObject.SetActive(true);
			if (!attackerSet.Contains(attacker.runtimeID))
			{
				attackerSet.Add(attacker.runtimeID);
			}
			this.attackee = attackee;
			_offset = offset;
			enable = true;
			_monster = attackee.entity as BaseMonoMonster;
			_hideHPBarCallBack = hideHPBarCallBack;
		}

		public void SetDisable()
		{
			enable = false;
			base.gameObject.SetActive(false);
			if (_hideHPBarCallBack != null)
			{
				_hideHPBarCallBack(this);
			}
		}

		public void RemoveAttacker(AvatarActor attackerActor)
		{
			attackerSet.Remove(attackerActor.runtimeID);
			if (attackerSet.Count <= 0)
			{
				SetDisable();
			}
		}
	}
}
