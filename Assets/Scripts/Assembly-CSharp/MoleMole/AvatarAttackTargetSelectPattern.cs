using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public static class AvatarAttackTargetSelectPattern
	{
		public static void SelectNearestEnemyV1(BaseMonoAvatar aAvatar)
		{
			List<BaseMonoMonster> list = new List<BaseMonoMonster>();
			foreach (BaseMonoMonster allMonster in Singleton<MonsterManager>.Instance.GetAllMonsters())
			{
				if (allMonster.IsActive() && !allMonster.denySelect)
				{
					list.Add(allMonster);
				}
			}
			if (list.Count != 0)
			{
				BaseMonoEntity attackTarget = null;
				if (list.Count > 0)
				{
					FilterNearestMonsterTarget(aAvatar, list, ref attackTarget);
				}
				else
				{
					attackTarget = null;
				}
				aAvatar.SetAttackTarget(attackTarget);
			}
		}

		public static void SelectEnemyByEllipse(BaseMonoAvatar aAvatar)
		{
			List<BaseMonoMonster> list = new List<BaseMonoMonster>();
			foreach (BaseMonoMonster allMonster in Singleton<MonsterManager>.Instance.GetAllMonsters())
			{
				if (allMonster.IsActive() && !allMonster.denySelect)
				{
					list.Add(allMonster);
				}
			}
			if (list.Count != 0)
			{
				Vector3 mainDirection = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
				if (aAvatar.GetActiveControlData().hasSteer)
				{
					mainDirection = aAvatar.GetActiveControlData().steerDirection;
				}
				mainDirection.y = 0f;
				BaseMonoEntity monsterTarget;
				if (list.Count > 0)
				{
					monsterTarget = list[0];
					float eccentricity = 0.9f;
					float monsterScore = GetScoreByEllipse(aAvatar, monsterTarget, mainDirection, eccentricity);
					FilterMonsterTargetByEllipse(aAvatar, list, mainDirection, eccentricity, ref monsterTarget, ref monsterScore);
				}
				else
				{
					monsterTarget = null;
				}
				aAvatar.SetAttackTarget(monsterTarget);
			}
		}

		private static float GetScoreByEllipse(BaseMonoAvatar aAvatar, BaseMonoEntity target, Vector3 mainDirection, float eccentricity)
		{
			Vector3 vector = target.XZPosition - aAvatar.XZPosition;
			Vector3 vector2 = Vector3.Project(vector, mainDirection);
			float num = (vector2.x + vector2.z) / (mainDirection.x + mainDirection.z);
			return (vector.magnitude / eccentricity - num) / (1f / eccentricity - eccentricity);
		}

		public static void SelectMonsterAndPropByEllipse(BaseMonoAvatar aAvatar)
		{
			List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			if (allMonsters.Count == 0 && allPropObjects.Count == 0)
			{
				return;
			}
			Vector3 mainDirection = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
			if (aAvatar.GetActiveControlData().hasSteer)
			{
				mainDirection = aAvatar.GetActiveControlData().steerDirection;
			}
			mainDirection.y = 0f;
			float eccentricity = 0.9f;
			BaseMonoEntity monsterTarget = null;
			float monsterScore = float.MaxValue;
			FilterMonsterTargetByEllipse(aAvatar, allMonsters, mainDirection, eccentricity, ref monsterTarget, ref monsterScore);
			BaseMonoEntity baseMonoEntity = null;
			float num = float.MaxValue;
			for (int i = 0; i < allPropObjects.Count; i++)
			{
				BaseMonoPropObject baseMonoPropObject = allPropObjects[i];
				if (baseMonoPropObject.IsActive() && !baseMonoPropObject.denySelect && Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(baseMonoPropObject))
				{
					float num2 = 1.5f * GetScoreByEllipse(aAvatar, baseMonoPropObject, mainDirection, eccentricity);
					if (num2 < num)
					{
						baseMonoEntity = baseMonoPropObject;
						num = num2;
					}
				}
			}
			if (monsterTarget == null || baseMonoEntity == null)
			{
				if (monsterTarget != null)
				{
					aAvatar.SetAttackTarget(monsterTarget);
				}
				else if (baseMonoEntity != null)
				{
					aAvatar.SetAttackTarget(baseMonoEntity);
				}
				else
				{
					aAvatar.SetAttackTarget(null);
				}
			}
			else
			{
				BaseMonoEntity attackTarget = ((!(monsterScore < num)) ? baseMonoEntity : monsterTarget);
				aAvatar.SetAttackTarget(attackTarget);
			}
		}

		private static void FilterNearestMonsterTarget(BaseMonoAvatar aAvatar, List<BaseMonoMonster> monsters, ref BaseMonoEntity attackTarget)
		{
			float num = float.MaxValue;
			foreach (BaseMonoMonster monster in monsters)
			{
				List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = monster.GetAllHitboxEnabledBodyParts();
				if (allHitboxEnabledBodyParts.Count > 0)
				{
					foreach (BaseMonoAbilityEntity item in allHitboxEnabledBodyParts)
					{
						float num2 = Vector3.Distance(item.XZPosition, aAvatar.XZPosition);
						if (num2 < num)
						{
							attackTarget = item;
							num = num2;
						}
					}
				}
				else
				{
					float num3 = Vector3.Distance(monster.XZPosition, aAvatar.XZPosition);
					if (num3 < num)
					{
						attackTarget = monster;
						num = num3;
					}
				}
			}
		}

		private static void FilterMonsterTargetByEllipse(BaseMonoAvatar aAvatar, List<BaseMonoMonster> monsters, Vector3 mainDirection, float eccentricity, ref BaseMonoEntity monsterTarget, ref float monsterScore)
		{
			for (int i = 0; i < monsters.Count; i++)
			{
				BaseMonoMonster baseMonoMonster = monsters[i];
				if (baseMonoMonster.denySelect || !baseMonoMonster.IsActive())
				{
					continue;
				}
				List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = baseMonoMonster.GetAllHitboxEnabledBodyParts();
				if (allHitboxEnabledBodyParts.Count > 0)
				{
					foreach (BaseMonoAbilityEntity item in allHitboxEnabledBodyParts)
					{
						float scoreByEllipse = GetScoreByEllipse(aAvatar, item, mainDirection, eccentricity);
						if (scoreByEllipse < monsterScore)
						{
							monsterTarget = item;
							monsterScore = scoreByEllipse;
						}
					}
				}
				else
				{
					float scoreByEllipse2 = GetScoreByEllipse(aAvatar, baseMonoMonster, mainDirection, eccentricity);
					if (scoreByEllipse2 < monsterScore)
					{
						monsterTarget = baseMonoMonster;
						monsterScore = scoreByEllipse2;
					}
				}
			}
		}

		public static void PvPSelectRemoteAvatar(BaseMonoAvatar aAvatar)
		{
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
			if (allAvatars.Count == 0 && allPropObjects.Count == 0)
			{
				return;
			}
			Vector3 mainDirection = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
			if (aAvatar.GetActiveControlData().hasSteer)
			{
				mainDirection = aAvatar.GetActiveControlData().steerDirection;
			}
			mainDirection.y = 0f;
			float eccentricity = 0.9f;
			BaseMonoEntity monsterTarget = null;
			float monsterScore = float.MaxValue;
			FilterAvatarTargetByEllipse(aAvatar, allAvatars, mainDirection, eccentricity, ref monsterTarget, ref monsterScore);
			BaseMonoEntity baseMonoEntity = null;
			float num = float.MaxValue;
			for (int i = 0; i < allPropObjects.Count; i++)
			{
				BaseMonoPropObject baseMonoPropObject = allPropObjects[i];
				if (baseMonoPropObject.IsActive() && !baseMonoPropObject.denySelect && Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(baseMonoPropObject))
				{
					float num2 = 1.5f * GetScoreByEllipse(aAvatar, baseMonoPropObject, mainDirection, eccentricity);
					if (num2 < num)
					{
						baseMonoEntity = baseMonoPropObject;
						num = num2;
					}
				}
			}
			if (monsterTarget == null || baseMonoEntity == null)
			{
				if (monsterTarget != null)
				{
					aAvatar.SetAttackTarget(monsterTarget);
				}
				else if (baseMonoEntity != null)
				{
					aAvatar.SetAttackTarget(baseMonoEntity);
				}
				else
				{
					aAvatar.SetAttackTarget(null);
				}
			}
			else
			{
				BaseMonoEntity attackTarget = ((!(monsterScore < num)) ? baseMonoEntity : monsterTarget);
				aAvatar.SetAttackTarget(attackTarget);
			}
		}

		private static void FilterAvatarTargetByEllipse(BaseMonoAvatar aAvatar, List<BaseMonoAvatar> avatars, Vector3 mainDirection, float eccentricity, ref BaseMonoEntity monsterTarget, ref float monsterScore)
		{
			for (int i = 0; i < avatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = avatars[i];
				if (!(baseMonoAvatar == aAvatar) && !baseMonoAvatar.denySelect)
				{
					float scoreByEllipse = GetScoreByEllipse(aAvatar, baseMonoAvatar, mainDirection, eccentricity);
					if (scoreByEllipse < monsterScore)
					{
						monsterTarget = baseMonoAvatar;
						monsterScore = scoreByEllipse;
					}
				}
			}
		}
	}
}
