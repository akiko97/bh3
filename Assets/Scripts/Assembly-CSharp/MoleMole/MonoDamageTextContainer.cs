using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoDamageTextContainer : MonoBehaviour
	{
		private const string DAMAGE_TEXT_PATH = "UI/Menus/Widget/InLevel/DamageText";

		private List<MonoDamageText> _damageTextPool;

		private void Awake()
		{
			_damageTextPool = new List<MonoDamageText>();
		}

		public void ShowDamageText(AttackData attackData, BaseMonoEntity attackee)
		{
			if (GlobalVars.muteDamageText || attackData.hitLevel == AttackResult.ActorHitLevel.Mute)
			{
				return;
			}
			Vector3 hitPoint = attackData.hitCollision.hitPoint;
			if (attackData.damage > 0f)
			{
				if (attackData.natureDamageRatio < 1f)
				{
					ShowOneDamageText(DamageType.Restrain, attackData.damage, hitPoint, attackee);
				}
				else if (attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
				{
					ShowOneDamageText(DamageType.Critical, attackData.damage, hitPoint, attackee);
				}
				else
				{
					ShowOneDamageText(DamageType.Normal, attackData.damage, hitPoint, attackee);
				}
			}
			hitPoint = attackData.hitCollision.hitPoint + GetRandomDeltaDistance();
			if (attackData.plainDamage > 0f)
			{
				ShowOneDamageText(DamageType.ElementalNormal, attackData.plainDamage, hitPoint, attackee);
			}
			if (attackData.fireDamage > 0f)
			{
				ShowOneDamageText(DamageType.Fire, attackData.fireDamage, hitPoint, attackee);
			}
			if (attackData.thunderDamage > 0f)
			{
				ShowOneDamageText(DamageType.Thunder, attackData.thunderDamage, hitPoint, attackee);
			}
			if (attackData.iceDamage > 0f)
			{
				ShowOneDamageText(DamageType.Ice, attackData.iceDamage, hitPoint, attackee);
			}
			if (attackData.alienDamage > 0f)
			{
				ShowOneDamageText(DamageType.Allien, attackData.alienDamage, hitPoint, attackee);
			}
		}

		private void ShowOneDamageText(DamageType type, float damage, Vector3 worldPos, BaseMonoEntity attackee)
		{
			MonoDamageText monoDamageText = null;
			foreach (MonoDamageText item in _damageTextPool)
			{
				if (!item.gameObject.activeSelf)
				{
					monoDamageText = item;
					break;
				}
			}
			if (monoDamageText == null)
			{
				monoDamageText = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/DamageText")).GetComponent<MonoDamageText>();
				monoDamageText.transform.SetParent(base.transform, false);
				_damageTextPool.Add(monoDamageText);
			}
			else
			{
				monoDamageText.gameObject.SetActive(true);
			}
			monoDamageText.SetupView(type, damage, worldPos, attackee);
		}

		private Vector3 GetRandomDeltaDistance()
		{
			float x = (float)((Random.value < 0.5f) ? 1 : (-1)) * Random.Range(0.5f, 0.8f);
			float y = (float)((Random.value < 0.5f) ? 1 : (-1)) * Random.Range(0.1f, 0.2f);
			return new Vector3(x, y, 0f);
		}
	}
}
