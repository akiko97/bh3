using System;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMonsterStatus : MonoBehaviour
	{
		private const string ELITE_ICON_PREFAB = "UI/Menus/Widget/InLevel/EliteIcon";

		private MonoSliderGroupWithPhase _hpSlider;

		private MonoMaskSlider _shieldSlider;

		private MonsterActor _currentMonster;

		private int WHITE_NAME_DIFF_LEVEL;

		private int YELLOW_NAME_DIFF_LEVEL;

		private int RED_NAME_DIFF_LEVEL;

		private void Awake()
		{
			_hpSlider = base.transform.Find("HPBar").GetComponent<MonoSliderGroupWithPhase>();
			_shieldSlider = base.transform.Find("ShieldBar").GetComponent<MonoMaskSlider>();
			WHITE_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepOne;
			YELLOW_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepTwo;
			RED_NAME_DIFF_LEVEL = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepThree;
		}

		public void SetupView(MonsterActor monsterBefore, MonsterActor monsterAfter)
		{
			if (monsterBefore != null)
			{
				monsterBefore.onHPChanged = (Action<float, float, float>)Delegate.Remove(monsterBefore.onHPChanged, new Action<float, float, float>(UpdateMonsterHPBar));
				monsterBefore.onMaxHPChanged = (Action<float, float>)Delegate.Remove(monsterBefore.onMaxHPChanged, new Action<float, float>(UpdateMonsterMaxHP));
				if (monsterBefore.abilityPlugin.HasDisplayFloat("Shield"))
				{
					monsterBefore.abilityPlugin.SubDetachDisplayFloat("Shield", UpdateMonsterShieldBar);
				}
			}
			if (monsterAfter != null)
			{
				monsterAfter.onHPChanged = (Action<float, float, float>)Delegate.Combine(monsterAfter.onHPChanged, new Action<float, float, float>(UpdateMonsterHPBar));
				monsterAfter.onMaxHPChanged = (Action<float, float>)Delegate.Combine(monsterAfter.onMaxHPChanged, new Action<float, float>(UpdateMonsterMaxHP));
				base.gameObject.SetActive(true);
				SetupHPBar(monsterAfter);
				SetupShieldBar(monsterAfter);
				if (!SetupEliteIcon(monsterAfter))
				{
					SetupNameText(monsterAfter);
				}
				else
				{
					base.transform.Find("NameText").gameObject.SetActive(false);
				}
			}
			_currentMonster = monsterAfter;
			SetupNatureBonus();
			SetupMonsterNameByLevelPunish();
		}

		private void SetupHPBar(MonsterActor monsterActor)
		{
			int newMaxPhase = 1;
			if (monsterActor.uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monsterActor.uniqueMonsterID);
				newMaxPhase = ((uniqueMonsterMetaData == null) ? 1 : uniqueMonsterMetaData.hpPhaseNum);
			}
			_hpSlider.UpdateMaxPhase(newMaxPhase);
			_hpSlider.UpdateValue(monsterActor.HP, monsterActor.maxHP, 0f);
		}

		private void SetupShieldBar(MonsterActor monsterActor)
		{
			bool flag = monsterActor.abilityPlugin.HasDisplayFloat("Shield");
			base.transform.Find("ShieldBar").gameObject.SetActive(flag);
			if (flag)
			{
				float curValue = 0f;
				float ceiling = 0f;
				float floor = 0f;
				monsterActor.abilityPlugin.SubAttachDisplayFloat("Shield", UpdateMonsterShieldBar, ref curValue, ref floor, ref ceiling);
				_shieldSlider.UpdateValue(curValue, ceiling, floor);
			}
		}

		private void SetupNameText(MonsterActor monsterActor)
		{
			base.transform.Find("NameText").gameObject.SetActive(true);
			string empty = string.Empty;
			if (monsterActor.uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(monsterActor.uniqueMonsterID);
				empty = ((uniqueMonsterMetaData != null) ? LocalizationGeneralLogic.GetText(uniqueMonsterMetaData.name) : monsterActor.uniqueMonsterID.ToString());
			}
			else
			{
				MonsterUIMetaData monsterUIMetaDataByName = MonsterUIMetaDataReaderExtend.GetMonsterUIMetaDataByName(monsterActor.metaConfig.subTypeName);
				empty = ((monsterUIMetaDataByName != null) ? LocalizationGeneralLogic.GetText(monsterUIMetaDataByName.displayTitle) : monsterActor.metaConfig.subTypeName);
			}
			base.transform.Find("NameText").GetComponent<Text>().text = empty;
		}

		private bool SetupEliteIcon(MonsterActor monsterActor)
		{
			Transform transform = base.transform.Find("EliteIcons");
			transform.DestroyChildren();
			bool flag = false;
			if (monsterActor.uniqueMonsterID != 0)
			{
				return false;
			}
			foreach (ActorAbility appliedAbility in monsterActor.abilityPlugin.GetAppliedAbilities())
			{
				if (appliedAbility == null || appliedAbility.config == null)
				{
					continue;
				}
				string abilityName = appliedAbility.config.AbilityName;
				if (MiscData.Config.EliteAbilityIcon.ContainsKey(abilityName))
				{
					string text = MiscData.Config.EliteAbilityIcon[abilityName].ToString();
					string textID = MiscData.Config.EliteAbilityText[abilityName].ToString();
					if (!string.IsNullOrEmpty(text))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/EliteIcon"));
						gameObject.transform.SetParent(transform, false);
						gameObject.transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(text);
						gameObject.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
						flag = true;
					}
				}
			}
			transform.gameObject.SetActive(flag);
			return flag;
		}

		public void SetupMonsterNameByLevelPunish()
		{
			Text component = base.transform.Find("NameText").GetComponent<Text>();
			Gradient component2 = base.transform.Find("NameText").GetComponent<Gradient>();
			Outline component3 = base.transform.Find("NameText").GetComponent<Outline>();
			if (_currentMonster == null || !(component2 != null) || !(component3 != null) || !(component != null))
			{
				return;
			}
			if (Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
			{
				int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
				int num = Mathf.Clamp((int)_currentMonster.level - teamLevel, 0, 10);
				if (num >= WHITE_NAME_DIFF_LEVEL && num < YELLOW_NAME_DIFF_LEVEL)
				{
					component.color = MiscData.GetColor("PunishWhiteTopColor");
					component3.effectColor = MiscData.GetColor("PunishWhiteOutlineColor");
				}
				else if (num >= YELLOW_NAME_DIFF_LEVEL && num < RED_NAME_DIFF_LEVEL)
				{
					component.color = MiscData.GetColor("PunishYellowTopColor");
					component3.effectColor = MiscData.GetColor("PunishYellowOutlineColor");
				}
				else
				{
					component.color = MiscData.GetColor("PunishRedTopColor");
					component3.effectColor = MiscData.GetColor("PunishRedOutlineColor");
				}
			}
			else
			{
				component2.topColor = Color.white;
				component2.bottomColor = Color.white;
				component3.effectColor = MiscData.GetColor("PunishWhiteOutlineColor");
			}
		}

		public void SetupNatureBonus()
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
			EntityNature attribute = (EntityNature)actor.avatarDataItem.Attribute;
			EntityNature nature = (EntityNature)_currentMonster.metaConfig.nature;
			int natureBonusType = DamageModelLogic.GetNatureBonusType(attribute, nature);
			base.transform.Find("DamageMark/Up").gameObject.SetActive(natureBonusType == 1);
			base.transform.Find("DamageMark/Down").gameObject.SetActive(natureBonusType == -1);
		}

		private void UpdateMonsterHPBar(float from, float to, float delta)
		{
			_hpSlider.UpdateValue(to, _hpSlider.maxValue, 0f);
			if (to <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void UpdateMonsterMaxHP(float from, float to)
		{
			_hpSlider.UpdateValue(_hpSlider.value, to, 0f);
		}

		private void UpdateMonsterShieldBar(float from, float to)
		{
			_shieldSlider.UpdateValue(to, _shieldSlider.maxValue, 0f);
		}
	}
}
