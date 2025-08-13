using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityHitExplodeRandomPosBulletMixin : AbilityHitExplodeBulletMixin
	{
		private class BulletInfo
		{
			public uint bulletID;

			public float backTimer;

			public float holdTimer;

			public float lifeTimer;

			public Vector3 startPosRelative;
		}

		private HitExplodeRandomPosBulletMixin config;

		private BaseMonoEntity _attackTarget;

		private int randPosIx;

		private List<BulletInfo> _bulletInfoList;

		private string _bulletName;

		private int _bulletNum;

		private int _bulletNumCount;

		private float _internalTimer;

		private List<int> _posIndexOrderList;

		public AbilityHitExplodeRandomPosBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (HitExplodeRandomPosBulletMixin)config;
			randPosIx = 0;
			_bulletInfoList = new List<BulletInfo>();
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_bulletNum = ((config.Type == HitExplodeRandomPosBulletMixin.CreateType.CreateOne) ? 1 : config.RandomPosPool.Length);
			_bulletNumCount = 0;
			_bulletName = config.BulletTypeName;
			HitExplodeTracingBulletMixinArgument hitExplodeTracingBulletMixinArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
			if (hitExplodeTracingBulletMixinArgument != null)
			{
				if (hitExplodeTracingBulletMixinArgument.BulletName != null)
				{
					_bulletName = hitExplodeTracingBulletMixinArgument.BulletName;
				}
				if (hitExplodeTracingBulletMixinArgument.RandomBulletNames != null)
				{
					_bulletName = hitExplodeTracingBulletMixinArgument.RandomBulletNames[Random.Range(0, hitExplodeTracingBulletMixinArgument.RandomBulletNames.Length)];
				}
			}
			_internalTimer = 0f;
			GeneralPosIndexOrderList(config.NeedShuffle);
		}

		private BulletInfo CreateOneBullet()
		{
			AbilityTriggerBullet abilityTriggerBullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(_bulletName, actor, instancedAbility.Evaluate(config.BulletSpeed), config.Targetting, config.IgnoreTimeScale, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), -1f);
			Vector3 startPos = Vector3.zero;
			InitBulletPosAndForward(abilityTriggerBullet, out startPos);
			BulletInfo bulletInfo = new BulletInfo();
			bulletInfo.bulletID = abilityTriggerBullet.runtimeID;
			bulletInfo.backTimer = config.BackTime;
			bulletInfo.holdTimer = config.HoldTime;
			bulletInfo.lifeTimer = config.LifeTime;
			bulletInfo.startPosRelative = startPos;
			BulletInfo result = bulletInfo;
			_bulletAttackDatas.Add(abilityTriggerBullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(actor, config.HitAnimEventID));
			abilityTriggerBullet.triggerBullet.acceleration = config.Acceleration;
			abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
			return result;
		}

		private void SelectATarget()
		{
			if (entity is BaseMonoAvatar)
			{
				(entity as BaseMonoAvatar).SelectTarget();
				_attackTarget = entity.GetAttackTarget();
			}
			else if (entity is BaseMonoMonster)
			{
				_attackTarget = entity.GetAttackTarget();
			}
		}

		private void InitBulletPosAndForward(AbilityTriggerBullet bullet, out Vector3 startPos)
		{
			Vector3 position = Vector3.zero;
			if (config.RandomPosPool.Length > 0)
			{
				randPosIx = ((randPosIx + 1 != config.RandomPosPool.Length) ? (randPosIx + 1) : 0);
				float[] array = config.RandomPosPool[_posIndexOrderList[randPosIx]];
				position = new Vector3(array[0], array[1], array[2]);
			}
			startPos = bullet.triggerBullet.transform.TransformPoint(position) - bullet.triggerBullet.transform.localPosition;
			bullet.triggerBullet.transform.position += startPos;
			BaseMonoEntity attackTarget = _attackTarget;
			Vector3 forward;
			if (attackTarget == null || !config.FaceTarget)
			{
				forward = entity.transform.forward;
			}
			else
			{
				Vector3 position2 = attackTarget.GetAttachPoint("RootNode").position;
				forward = position2 - bullet.triggerBullet.transform.position;
			}
			bullet.triggerBullet.transform.forward = forward;
			if (config.BackDistance > 0f)
			{
				bullet.triggerBullet.transform.position += forward.normalized * config.BackDistance;
			}
		}

		public override void Core()
		{
			base.Core();
			if (_bulletNumCount < _bulletNum)
			{
				CreateBullet();
			}
			for (int i = 0; i < _bulletInfoList.Count; i++)
			{
				if (_bulletInfoList[i] == null)
				{
					continue;
				}
				BulletInfo bulletInfo = _bulletInfoList[i];
				AbilityTriggerBullet abilityTriggerBullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(bulletInfo.bulletID);
				if (abilityTriggerBullet == null)
				{
					continue;
				}
				_attackTarget = entity.GetAttackTarget();
				if (_attackTarget == null)
				{
					SelectATarget();
				}
				if (bulletInfo.holdTimer > 0f)
				{
					abilityTriggerBullet.triggerBullet.SetupAtReset();
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
					bulletInfo.holdTimer -= Time.deltaTime * entity.TimeScale;
					BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
					if (localAvatar != null)
					{
						abilityTriggerBullet.triggerBullet.transform.position = localAvatar.XZPosition + bulletInfo.startPosRelative;
					}
					if (_attackTarget != null)
					{
						Vector3 position = _attackTarget.GetAttachPoint("RootNode").position;
						abilityTriggerBullet.triggerBullet.transform.forward = position - abilityTriggerBullet.triggerBullet.transform.position;
					}
				}
				else if (bulletInfo.backTimer > 0f)
				{
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled(false);
					bulletInfo.backTimer -= Time.deltaTime * entity.TimeScale;
					abilityTriggerBullet.triggerBullet.transform.position -= abilityTriggerBullet.triggerBullet.transform.forward.normalized * Mathf.Lerp(0f, config.BackDistance, 1f - bulletInfo.backTimer / config.BackTime);
					if (bulletInfo.backTimer <= 0f)
					{
						FireBulletEffectSmoke(abilityTriggerBullet.triggerBullet);
					}
				}
				else if (bulletInfo.lifeTimer > 0f)
				{
					abilityTriggerBullet.triggerBullet.SetCollisionEnabled();
					if (_attackTarget != null)
					{
						abilityTriggerBullet.triggerBullet.SetupTracing(_attackTarget.GetAttachPoint("RootNode").position, 99f, 0f);
					}
					else
					{
						abilityTriggerBullet.triggerBullet.SetupLinear();
					}
					bulletInfo.lifeTimer -= Time.deltaTime * entity.TimeScale;
				}
				else
				{
					_bulletInfoList[i] = null;
				}
			}
		}

		private void CreateBullet()
		{
			_attackTarget = entity.GetAttackTarget();
			if (_attackTarget == null)
			{
				SelectATarget();
			}
			switch (config.Type)
			{
			case HitExplodeRandomPosBulletMixin.CreateType.CreateOne:
				DoCreateOne();
				break;
			case HitExplodeRandomPosBulletMixin.CreateType.CreateAllAtSameTime:
				DoCreateAllAtSameTime();
				break;
			case HitExplodeRandomPosBulletMixin.CreateType.CreateAllInterval:
				DoCreateAllInterval();
				break;
			}
		}

		private void DoCreateOne()
		{
			int index = _bulletInfoList.SeekAddPosition();
			_bulletInfoList[index] = CreateOneBullet();
			_bulletNumCount++;
		}

		private void DoCreateAllAtSameTime()
		{
			for (int i = 0; i < _bulletNum; i++)
			{
				int index = _bulletInfoList.SeekAddPosition();
				_bulletInfoList[index] = CreateOneBullet();
				_bulletNumCount++;
				_bulletInfoList[index].holdTimer += (float)(_bulletNumCount - 1) * config.ShootInternalTime;
			}
		}

		private void DoCreateAllInterval()
		{
			_internalTimer -= Time.deltaTime;
			if (_internalTimer <= 0f)
			{
				int index = _bulletInfoList.SeekAddPosition();
				_bulletInfoList[index] = CreateOneBullet();
				_bulletNumCount++;
				_bulletInfoList[index].holdTimer += (float)(_bulletNumCount - 1) * config.ShootInternalTime;
				_internalTimer = config.CreateInternalTime;
			}
		}

		private void GeneralPosIndexOrderList(bool needShuffle)
		{
			if (_posIndexOrderList == null)
			{
				_posIndexOrderList = new List<int>();
				for (int i = 0; i < config.RandomPosPool.Length; i++)
				{
					_posIndexOrderList.Add(i);
				}
			}
			if (needShuffle)
			{
				_posIndexOrderList.Shuffle();
			}
		}

		private void FireBulletEffectSmoke(MonoTriggerBullet triggerBullet)
		{
			if (config.BulletEffect != null && config.BulletEffect.EffectPattern != null)
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(config.BulletEffect.EffectPattern, triggerBullet, config.BulletEffectGround);
			}
		}
	}
}
