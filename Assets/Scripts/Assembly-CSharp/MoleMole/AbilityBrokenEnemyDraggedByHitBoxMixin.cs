using System;
using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class AbilityBrokenEnemyDraggedByHitBoxMixin : BaseAbilityMixin
	{
		private BrokenEnemyDraggedByHitBoxMixin config;

		private Dictionary<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>, int> _addedVelocityActorsAndIndexDic = new Dictionary<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>, int>();

		private List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>> _draggedEnemyList = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>>();

		private List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>> _touchedEnemyList = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>>();

		private float _pullVelocity;

		public AbilityBrokenEnemyDraggedByHitBoxMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BrokenEnemyDraggedByHitBoxMixin)config;
			_pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
		}

		public override void OnAdded()
		{
			(entity as IAttacker).onAnimatedHitBoxCreatedCallBack += onAnimatedHitBoxCreated;
		}

		public override void OnRemoved()
		{
			(entity as IAttacker).onAnimatedHitBoxCreatedCallBack -= onAnimatedHitBoxCreated;
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> draggedEnemy in _draggedEnemyList)
			{
				foreach (BaseAbilityActor item in draggedEnemy.Item2)
				{
					RemoveAdditiveVelocity(item, draggedEnemy.Item1);
				}
			}
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtAttackLanded)
			{
				OnAttackLandedOther((EvtAttackLanded)evt);
			}
			return false;
		}

		private bool OnAttackLandedOther(EvtAttackLanded evt)
		{
			MonoAnimatedHitboxDetect monoAnimatedHitboxDetect = null;
			bool flag = false;
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> touchedEnemy in _touchedEnemyList)
			{
				if (touchedEnemy.Item1 == null || !touchedEnemy.Item2.Contains(evt.attackeeID))
				{
					continue;
				}
				monoAnimatedHitboxDetect = touchedEnemy.Item1;
				flag = true;
				break;
			}
			if (!flag)
			{
				return false;
			}
			if (evt.attackResult.hitEffect <= AttackResult.AnimatorHitEffect.Light)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
			if (baseAbilityActor == null)
			{
				return false;
			}
			HashSet<BaseAbilityActor> hashSet = null;
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> draggedEnemy in _draggedEnemyList)
			{
				if (draggedEnemy.Item1 == monoAnimatedHitboxDetect)
				{
					hashSet = draggedEnemy.Item2;
					if (draggedEnemy.Item2.Contains(baseAbilityActor))
					{
						return false;
					}
				}
			}
			if (hashSet != null)
			{
				hashSet.Add(baseAbilityActor);
				SetAdditiveVelocity(baseAbilityActor, monoAnimatedHitboxDetect);
			}
			return true;
		}

		private void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
		{
			if (!(attackPattern is AnimatedColliderDetect) && !(attackPattern is TargetLockedAnimatedColliderDetect))
			{
				return;
			}
			if (attackPattern is AnimatedColliderDetect)
			{
				AnimatedColliderDetect animatedColliderDetect = attackPattern as AnimatedColliderDetect;
				if (!animatedColliderDetect.brokenEnemyDragged)
				{
					return;
				}
			}
			else if (attackPattern is TargetLockedAnimatedColliderDetect)
			{
				TargetLockedAnimatedColliderDetect targetLockedAnimatedColliderDetect = attackPattern as TargetLockedAnimatedColliderDetect;
				if (!targetLockedAnimatedColliderDetect.brokenEnemyDragged)
				{
					return;
				}
			}
			if (!(hitBox.entryName != config.ColliderEntryName))
			{
				hitBox.enemyEnterCallback = (Action<MonoAnimatedHitboxDetect, Collider>)Delegate.Combine(hitBox.enemyEnterCallback, new Action<MonoAnimatedHitboxDetect, Collider>(HitBoxTriggerEnterCallback));
				hitBox.destroyCallback = (Action<MonoAnimatedHitboxDetect>)Delegate.Combine(hitBox.destroyCallback, new Action<MonoAnimatedHitboxDetect>(onAnimatedHitBoxDestroy));
				_draggedEnemyList.Add(new Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>(hitBox, new HashSet<BaseAbilityActor>()));
				_touchedEnemyList.Add(new Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>(hitBox, new HashSet<uint>()));
			}
		}

		private void onAnimatedHitBoxDestroy(MonoAnimatedHitboxDetect hitbox)
		{
			List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>> list = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>>();
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> draggedEnemy in _draggedEnemyList)
			{
				if (!(draggedEnemy.Item1 == hitbox))
				{
					continue;
				}
				list.Add(draggedEnemy);
				foreach (BaseAbilityActor item in draggedEnemy.Item2)
				{
					RemoveAdditiveVelocity(item, hitbox);
				}
			}
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> item2 in list)
			{
				_draggedEnemyList.Remove(item2);
			}
			List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>> list2 = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>>();
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> touchedEnemy in _touchedEnemyList)
			{
				if (touchedEnemy.Item1 == hitbox)
				{
					list2.Add(touchedEnemy);
				}
			}
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> item3 in list2)
			{
				_touchedEnemyList.Remove(item3);
			}
			List<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>> list3 = new List<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>>();
			foreach (Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> key in _addedVelocityActorsAndIndexDic.Keys)
			{
				if (key.Item1 == hitbox)
				{
					list3.Add(key);
				}
			}
		}

		private void HitBoxTriggerEnterCallback(MonoAnimatedHitboxDetect hitbox, Collider other)
		{
			if (hitbox.entryName != config.ColliderEntryName)
			{
				return;
			}
			BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(componentInParent.GetRuntimeID());
			if (baseAbilityActor == null)
			{
				return;
			}
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(componentInParent.GetRuntimeID());
			if (num != 3 && num != 4)
			{
				return;
			}
			HashSet<BaseAbilityActor> hashSet = new HashSet<BaseAbilityActor>(Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(baseAbilityActor));
			if (!hashSet.Contains(actor))
			{
				return;
			}
			bool flag = false;
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> draggedEnemy in _draggedEnemyList)
			{
				if (draggedEnemy.Item1 == hitbox)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			HashSet<uint> hashSet2 = null;
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> touchedEnemy in _touchedEnemyList)
			{
				if (touchedEnemy.Item1 == hitbox)
				{
					hashSet2 = touchedEnemy.Item2;
					if (touchedEnemy.Item2.Contains(baseAbilityActor.runtimeID))
					{
						return;
					}
				}
			}
			if (hashSet2 != null)
			{
				hashSet2.Add(baseAbilityActor.runtimeID);
			}
		}

		public override void Core()
		{
			foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> draggedEnemy in _draggedEnemyList)
			{
				if (draggedEnemy.Item1 == null)
				{
					foreach (BaseAbilityActor item in draggedEnemy.Item2)
					{
						RemoveAdditiveVelocity(item, draggedEnemy.Item1);
					}
					continue;
				}
				foreach (BaseAbilityActor item2 in draggedEnemy.Item2)
				{
					SetAdditiveVelocity(item2, draggedEnemy.Item1);
				}
			}
		}

		private void SetAdditiveVelocity(BaseAbilityActor enemyActor, MonoAnimatedHitboxDetect hitbox)
		{
			if (enemyActor != null && (bool)enemyActor.isAlive && !(enemyActor.entity == null))
			{
				Vector3 additiveVelocity = hitbox.collideCenterTransform.position - enemyActor.entity.XZPosition;
				additiveVelocity.y = 0f;
				additiveVelocity.Normalize();
				DoSetAdditiveVelocity(enemyActor, hitbox, additiveVelocity);
			}
		}

		private void DoSetAdditiveVelocity(BaseAbilityActor targetActor, MonoAnimatedHitboxDetect hitbox, Vector3 additiveVelocity)
		{
			if (targetActor != null)
			{
				Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> key = new Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>(hitbox, targetActor);
				if (!_addedVelocityActorsAndIndexDic.ContainsKey(key))
				{
					targetActor.entity.SetHasAdditiveVelocity(true);
					int value = targetActor.entity.AddAdditiveVelocity(additiveVelocity * _pullVelocity);
					_addedVelocityActorsAndIndexDic.Add(key, value);
				}
				else
				{
					targetActor.entity.SetHasAdditiveVelocity(true);
					int index = _addedVelocityActorsAndIndexDic[key];
					targetActor.entity.SetAdditiveVelocityOfIndex(additiveVelocity * _pullVelocity, index);
				}
			}
		}

		private void RemoveAdditiveVelocity(BaseAbilityActor targetActor, MonoAnimatedHitboxDetect hitbox)
		{
			if (targetActor != null && (bool)targetActor.isAlive && !(targetActor.entity == null))
			{
				Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> key = new Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>(hitbox, targetActor);
				if (_addedVelocityActorsAndIndexDic.ContainsKey(key))
				{
					int index = _addedVelocityActorsAndIndexDic[key];
					targetActor.entity.SetAdditiveVelocityOfIndex(Vector3.zero, index);
					targetActor.entity.SetHasAdditiveVelocity(false);
					_addedVelocityActorsAndIndexDic.Remove(key);
				}
			}
		}
	}
}
