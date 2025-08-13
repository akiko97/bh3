using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityMonsterSummonAttack : BaseAbilityMixin
	{
		private class SummonItem
		{
			public bool effectFired;

			public float effectTimer;

			public float summonTimer;

			public Vector3 summonPosition;

			public MixinSummonItem summon;
		}

		private MonsterSummonMixin config;

		private List<SummonItem> summonList = new List<SummonItem>();

		private List<SummonItem> summonListDelete = new List<SummonItem>();

		public AbilityMonsterSummonAttack(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (MonsterSummonMixin)config;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			OnSummon(evt);
		}

		private void OnSummon(EvtAbilityStart evt)
		{
			MonsterSummonMixinArgument monsterSummonMixinArgument = evt.abilityArgument as MonsterSummonMixinArgument;
			BaseMonoAnimatorEntity baseMonoAnimatorEntity = entity as BaseMonoAnimatorEntity;
			for (int i = 2; i < 6; i++)
			{
				if (baseMonoAnimatorEntity != null)
				{
					baseMonoAnimatorEntity.StartFadeAnimatorLayerWeight(i, 0f, 0.01f);
				}
			}
			if (monsterSummonMixinArgument != null)
			{
				MixinSummonItem mixinSummonItem = config.SummonMonsters[monsterSummonMixinArgument.SummonMonsterIndex];
				OrderSummonMonster(mixinSummonItem.EffectDelay, mixinSummonItem.SummonDelay, mixinSummonItem, evt);
				SetAnimatorLayer(mixinSummonItem, baseMonoAnimatorEntity);
				return;
			}
			int j = 0;
			for (int num = config.SummonMonsters.Length; j < num; j++)
			{
				MixinSummonItem mixinSummonItem2 = config.SummonMonsters[j];
				OrderSummonMonster(mixinSummonItem2.EffectDelay, mixinSummonItem2.SummonDelay, mixinSummonItem2, evt);
				SetAnimatorLayer(mixinSummonItem2, baseMonoAnimatorEntity);
			}
		}

		private void SetAnimatorLayer(MixinSummonItem item, BaseMonoAnimatorEntity animEntity)
		{
			if (item.UseCoffinAnim && animEntity != null)
			{
				animEntity.StartFadeAnimatorLayerWeight(item.CoffinIndex + 1, 1f, 0.01f);
			}
		}

		private void OrderSummonMonster(float effectDelay, float summonDelay, MixinSummonItem item, EvtAbilityStart evt)
		{
			SummonItem summonItem = new SummonItem();
			summonItem.effectTimer = effectDelay;
			summonItem.summonTimer = effectDelay + summonDelay;
			summonItem.summon = item;
			if (evt.hitCollision != null)
			{
				summonItem.summonPosition = evt.hitCollision.hitPoint;
				summonItem.summonPosition.y = 0f;
				summonItem.summonPosition = CalculateSummonPosition(summonItem.summonPosition);
			}
			else
			{
				summonItem.summonPosition = CalculateSummonPosition(item.BaseOnTarget, item.Distance, item.Angle);
				summonItem.summonPosition = CalculateSummonPosition(summonItem.summonPosition);
			}
			summonList.Add(summonItem);
		}

		private void SummonMonster(MixinSummonItem item, Vector3 position)
		{
			uint runtimeID = Singleton<MonsterManager>.Instance.CreateMonster(instancedAbility.Evaluate(item.MonsterName), instancedAbility.Evaluate(item.TypeName), Singleton<LevelScoreManager>.Instance.NPCHardLevel, true, position, 0u, false, 0u);
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(runtimeID);
			baseAbilityActor.ownerID = actor.runtimeID;
			if (item.Abilities == null)
			{
				return;
			}
			foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability in item.Abilities)
			{
				baseAbilityActor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(ability.Value.AbilityName, ability.Value.AbilityOverride));
			}
		}

		private Vector3 CalculateSummonPosition(bool baseType, float distance, float angle)
		{
			Vector3 result = Vector3.zero;
			BaseMonoEntity baseMonoEntity = null;
			baseMonoEntity = ((!baseType) ? entity : entity.GetAttackTarget());
			if (baseMonoEntity != null)
			{
				result = AdjustLevelCollision(baseMonoEntity.XZPosition, Quaternion.Euler(0f, angle, 0f) * baseMonoEntity.transform.forward * distance);
			}
			return result;
		}

		private Vector3 CalculateSummonPosition(Vector3 summonPosition)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar == null)
			{
				return summonPosition;
			}
			bool flag = false;
			Vector3 vector = new Vector3(localAvatar.transform.position.x, 0.1f, localAvatar.transform.position.z);
			Vector3 vector2 = new Vector3(summonPosition.x, 0.1f, summonPosition.z);
			RaycastHit hitInfo;
			if (Physics.Linecast(vector, vector2, out hitInfo, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
			{
				flag = true;
			}
			if (flag)
			{
				Vector3 point = hitInfo.point;
				Vector3 normalized = (vector - vector2).normalized;
				float num = 0.1f;
				Vector3 vector3 = point + normalized * num;
				return new Vector3(vector3.x, summonPosition.y, vector3.z);
			}
			return summonPosition;
		}

		private Vector3 AdjustLevelCollision(Vector3 origin, Vector3 offset)
		{
			float num = 0.2f;
			Vector3 vector = offset;
			int num2 = 4;
			for (int i = 0; i < num2; i++)
			{
				Ray ray = new Ray(origin + Vector3.up * num, vector.normalized);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, vector.magnitude, (1 << InLevelData.OBSTACLE_COLLIDER_LAYER) | (1 << InLevelData.STAGE_COLLIDER_LAYER)))
				{
					vector = Quaternion.AngleAxis(360f / (float)num2, Vector3.up) * vector;
					continue;
				}
				break;
			}
			return origin + vector;
		}

		private void SummonEffect(Vector3 position, MixinEffect effect)
		{
			if (effect != null)
			{
				if (effect.EffectPattern != null)
				{
					entity.FireEffect(effect.EffectPattern, position, Vector3.forward);
				}
				if (effect.AudioPattern != null)
				{
					entity.PlayAudio(effect.AudioPattern);
				}
			}
		}

		public override void Core()
		{
			float num = Time.deltaTime * entity.TimeScale;
			int i = 0;
			for (int count = summonList.Count; i < count; i++)
			{
				SummonItem summonItem = summonList[i];
				summonItem.effectTimer -= num;
				summonItem.summonTimer -= num;
				if (!summonItem.effectFired && summonItem.effectTimer <= 0f)
				{
					SummonEffect(summonItem.summonPosition, config.SummonEffect);
					summonItem.effectFired = true;
				}
				if (summonItem.summonTimer <= 0f)
				{
					SummonMonster(summonItem.summon, summonItem.summonPosition);
				}
				if (summonItem.effectTimer <= 0f && summonItem.summonTimer <= 0f)
				{
					summonListDelete.Add(summonItem);
				}
			}
			if (summonListDelete.Count > 0)
			{
				int j = 0;
				for (int count2 = summonListDelete.Count; j < count2; j++)
				{
					summonList.Remove(summonListDelete[j]);
				}
				summonListDelete.Clear();
			}
		}
	}
}
