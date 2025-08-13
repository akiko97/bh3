using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class EquipPowerUpEffectDialogContext : BaseDialogContext
	{
		public enum DialogType
		{
			PowerUp = 0,
			Evo = 1,
			NewAffix = 2
		}

		public enum EffectType
		{
			Small = 0,
			Large = 1
		}

		private const string POWERUP_RESULT_PREFAB_PRE = "SpriteOutput/EquipPowerUpResult/Success";

		private const float EFFECT_POSITION_RANGE = 50f;

		private const string SMALL_SPOT_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/SmallSpot";

		private const string SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradeSuccess";

		private const string BIG_SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";

		private const string LARGE_SUCCESS_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";

		private const string EVOLUTION_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/Evolution";

		private const string STAR_ICON_PATH = "SpriteOutput/StarBig";

		private const string STAR_GREY_ICON_PATH = "SpriteOutput/StarBigGray";

		private const string SUB_STAR_ICON_PATH = "SpriteOutput/SubStarActive";

		private const string SUB_STAR_GREY_ICON_PATH = "SpriteOutput/SubStarActiveGray";

		public readonly StorageDataItemBase itemDataAfter;

		public readonly StorageDataItemBase itemDataBefore;

		private List<StorageDataItemBase> _materialList;

		public readonly int boostRate;

		public readonly DialogType dialogType;

		private SequenceAnimationManager _animationManager;

		private CanvasTimer _effectDelayTimer;

		private int _powerUpResultIndex;

		private List<Vector2> _effectPosition = new List<Vector2>
		{
			new Vector2(50f, 0f),
			new Vector2(-50f, 0f),
			new Vector2(0f, 50f),
			new Vector2(0f, -50f),
			new Vector2(50f, 50f),
			new Vector2(0f, 0f)
		};

		private int _effectIndex;

		private CanvasTimer _flashStartTimer;

		private CanvasTimer _flashResetTimer;

		public EquipPowerUpEffectDialogContext(StorageDataItemBase itemDataBefore, StorageDataItemBase itemDataAfter, List<StorageDataItemBase> materialList, DialogType type = DialogType.PowerUp, int boostRate = 100)
		{
			config = new ContextPattern
			{
				contextName = "EquipPowerUpEffectDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/EquipPowerUpEffectDialog",
				cacheType = ViewCacheType.DontCache
			};
			this.itemDataAfter = itemDataAfter;
			this.itemDataBefore = itemDataBefore;
			_materialList = new List<StorageDataItemBase>();
			foreach (StorageDataItemBase material in materialList)
			{
				_materialList.Add(material.Clone());
			}
			this.boostRate = boostRate;
			dialogType = type;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent)
		{
			base.StartUp(canvasTrans, viewParent);
			if (dialogType == DialogType.Evo)
			{
				string text = null;
				if (itemDataAfter is WeaponDataItem)
				{
					text = "VO_M_Con_05_Weapon_Upgread";
				}
				else if (itemDataAfter is StigmataDataItem)
				{
					text = "VO_M_Con_06_Stigmata_Upgread";
				}
				if (text != null)
				{
					Singleton<WwiseAudioManager>.Instance.Post(text);
				}
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.PowerUpAndEvoEffect)
			{
				return PlayEffect(ntf);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 196)
			{
				return OnSelectNewStigmataAffixRsp(pkt.getData<SelectNewStigmataAffixRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), OnBGClick);
			if (dialogType == DialogType.NewAffix)
			{
				BindViewCallback(base.view.transform.Find("SkillPopup/SkillBtn").GetComponent<Button>(), OnStigmataSkillBtnClick);
				BindViewCallback(base.view.transform.Find("StigmataAffixInfo/OldAffix/OkBtn").GetComponent<Button>(), OnOldAffixBtnClick);
				BindViewCallback(base.view.transform.Find("StigmataAffixInfo/NewAffix/OkBtn").GetComponent<Button>(), OnNewAffixBtnClick);
			}
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			ParticleSystem[] componentsInChildren = base.view.transform.Find("ItemPanel/Effects").GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
			ParticleSystem[] componentsInChildren2 = base.view.transform.Find("Result").GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem2 in componentsInChildren2)
			{
				particleSystem2.Stop();
			}
			base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
			base.view.transform.Find("SkillPopup").gameObject.SetActive(false);
			if (itemDataBefore is WeaponDataItem)
			{
				base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(itemDataBefore as WeaponDataItem);
			}
			else if (itemDataBefore is StigmataDataItem)
			{
				base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(itemDataBefore as StigmataDataItem);
			}
			else
			{
				base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemDataBefore.GetImagePath());
			}
			SetupMaterialList();
			base.view.transform.Find("Button").gameObject.SetActive(false);
			base.view.transform.Find("StarBG").GetComponent<CanvasGroup>().alpha = 0f;
			_effectIndex = 0;
			base.view.transform.Find("StigmataAffixInfo").gameObject.SetActive(false);
			return false;
		}

		public bool PlayEffect(Notify ntf)
		{
			string text = ntf.body.ToString();
			ParticleSystem smallEffect = GetSmallEffect();
			if (text == "EatAll")
			{
				PlayEffectWithDelay(smallEffect.duration / 8f);
			}
			return false;
		}

		public bool OnSelectNewStigmataAffixRsp(SelectNewStigmataAffixRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				OnBGClick();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		public void OnBGClick()
		{
			Destroy();
			Singleton<MainUIManager>.Instance.BackPage();
			if (dialogType == DialogType.Evo || dialogType == DialogType.PowerUp)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EquipPowerupOrEvo, itemDataAfter));
			}
			else if (dialogType == DialogType.NewAffix)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StigmataNewAffix));
			}
		}

		public void OnOldAffixBtnClick()
		{
			Destroy();
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnNewAffixBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestSelectNewStigmataAffix();
		}

		public void OnStigmataSkillBtnClick()
		{
			GameObject gameObject = base.view.transform.Find("SkillPopup/StigmataSkills").gameObject;
			gameObject.SetActive(!gameObject.activeSelf);
		}

		public override void Destroy()
		{
			if (_flashStartTimer != null)
			{
				_flashStartTimer.Destroy();
			}
			if (_flashResetTimer != null)
			{
				_flashResetTimer.Destroy();
			}
			base.Destroy();
		}

		private void SetupMaterialList()
		{
			if (_materialList != null && _materialList.Count > 0)
			{
				base.view.transform.Find("MaterialListPanel").GetComponent<MonoMaterialPanel>().SetupView(_materialList);
			}
		}

		private void SetStarColor(Transform starTrans)
		{
			if (starTrans != null && starTrans.GetComponent<Image>() != null)
			{
				starTrans.GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
			}
		}

		private void EnableBGClick()
		{
			base.view.transform.Find("Button").gameObject.SetActive(true);
			Transform transform = base.view.transform.Find("Result");
			switch (dialogType)
			{
			case DialogType.PowerUp:
				transform.Find("Particle1").gameObject.SetActive(true);
				transform.Find("Particle1/Spot").gameObject.SetActive(false);
				switch (_powerUpResultIndex)
				{
				case 1:
					transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess1");
					break;
				case 2:
					transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess2");
					break;
				case 3:
					transform.Find("Particle1").GetComponent<ParticleSystem>().startColor = MiscData.GetColor("PowerUpSuccess3");
					transform.Find("Particle1/Spot").gameObject.SetActive(true);
					break;
				}
				break;
			case DialogType.Evo:
				transform.Find("Particle1").gameObject.SetActive(false);
				transform.Find("Particle2").gameObject.SetActive(true);
				break;
			}
		}

		private void SetupLvInfoPanel(float starDelay = 0f)
		{
			_animationManager = new SequenceAnimationManager(EnableBGClick);
			base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
			if (itemDataBefore is WeaponDataItem)
			{
				base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(itemDataAfter as WeaponDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>());
			}
			else if (itemDataBefore is StigmataDataItem)
			{
				base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(itemDataBefore as StigmataDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			else
			{
				base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemDataAfter.GetImagePath());
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			int equipPowerUpResultIndex = MiscData.GetEquipPowerUpResultIndex(boostRate);
			base.view.transform.Find("Result").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/EquipPowerUpResult/Success" + equipPowerUpResultIndex);
			_animationManager.AddAnimation(base.view.transform.Find("Result").GetComponent<MonoAnimationinSequence>());
			base.view.transform.Find("InfoRowLv").GetComponent<MonoEquipExpGrow>().SetData(itemDataBefore.level, itemDataBefore.GetMaxExp(), itemDataBefore.exp, itemDataAfter.exp, UIUtil.GetEquipmentMaxExpList(itemDataBefore, itemDataBefore.level, itemDataAfter.level));
			_animationManager.AddAnimation(base.view.transform.Find("InfoRowLv").GetComponent<MonoAnimationinSequence>());
			_animationManager.StartPlay(starDelay);
		}

		private void SetupTitleAndStar(float delay = 0f, bool showAfterItem = false)
		{
			base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
			_animationManager = new SequenceAnimationManager(EnableBGClick);
			if (itemDataBefore is WeaponDataItem)
			{
				base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(itemDataAfter as WeaponDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>());
			}
			else if (itemDataBefore is StigmataDataItem)
			{
				base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(itemDataAfter as StigmataDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			else
			{
				base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemDataAfter.GetImagePath());
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			base.view.transform.Find("Title").gameObject.SetActive(false);
			base.view.transform.Find("Result").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/EquipPowerUpResult/Success4");
			_animationManager.AddAnimation(base.view.transform.Find("Result").GetComponent<MonoAnimationinSequence>());
			_animationManager.AddAnimation(base.view.transform.Find("StarBG").GetComponent<MonoAnimationinSequence>());
			Transform transform = base.view.transform.Find("Stars");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(i < itemDataAfter.GetMaxRarity());
				if (i < itemDataAfter.GetMaxRarity())
				{
					child.GetComponent<Image>().color = MiscData.GetColor("RarityStarGrey");
					child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StarBigGray");
					child.GetComponent<CanvasGroup>().alpha = 1f;
					if (i < itemDataAfter.rarity)
					{
						_animationManager.AddAnimation(child.GetComponent<MonoAnimationinSequence>(), SetStarColor);
						child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StarBig");
					}
				}
			}
			Transform transform2 = base.view.transform.Find("StarsMini");
			if (itemDataAfter.GetMaxSubRarity() > 0)
			{
				_animationManager.AddAnimation(transform2.GetComponent<MonoAnimationinSequence>());
			}
			for (int j = 0; j < transform2.childCount; j++)
			{
				Transform child2 = transform2.GetChild(j);
				child2.gameObject.SetActive(j < itemDataAfter.GetMaxSubRarity() - 1);
				if (j < itemDataAfter.GetMaxSubRarity() - 1)
				{
					child2.GetComponent<Image>().color = MiscData.GetColor("RarityStarGrey");
					child2.GetComponent<CanvasGroup>().alpha = 1f;
					child2.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SubStarActiveGray");
					if (j < itemDataAfter.GetSubRarity())
					{
						_animationManager.AddAnimation(child2.GetComponent<MonoAnimationinSequence>(), SetStarColor);
						child2.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SubStarActive");
					}
				}
			}
			_animationManager.StartPlay(delay);
		}

		private void SetupNewAffixInfo(float delay = 0f)
		{
			StigmataDataItem stigmataDataItem = itemDataBefore as StigmataDataItem;
			StigmataDataItem stigmataDataItem2 = itemDataAfter as StigmataDataItem;
			base.view.transform.Find("StigmataAffixInfo").gameObject.SetActive(true);
			if (stigmataDataItem.GetAffixSkillList().Count > 0)
			{
				base.view.transform.Find("StigmataAffixInfo/OldAffix").gameObject.SetActive(true);
				base.view.transform.Find("StigmataAffixInfo/OldAffix/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(stigmataDataItem, stigmataDataItem.GetAffixSkillList());
			}
			else
			{
				base.view.transform.Find("StigmataAffixInfo/OldAffix").gameObject.SetActive(false);
			}
			base.view.transform.Find("StigmataAffixInfo/NewAffix/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(stigmataDataItem2, stigmataDataItem2.GetAffixSkillList());
			SetupStigmataSkillInfo();
			base.view.transform.Find("SkillPopup").gameObject.SetActive(true);
		}

		private void SetupStigmataSkillInfo()
		{
			int num = 3;
			StigmataDataItem stigmataDataItem = itemDataBefore as StigmataDataItem;
			List<EquipSkillDataItem> skills = stigmataDataItem.skills;
			Transform transform = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/NaturalSkills");
			transform.gameObject.SetActive(skills.Count > 0);
			string text = LocalizationGeneralLogic.GetText("Menu_Title_StigmataSkill");
			transform.Find("Name/Label").GetComponent<Text>().text = text;
			for (int i = 1; i <= num; i++)
			{
				Transform transform2 = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/NaturalSkills/Desc/Skill_" + i);
				transform2.gameObject.SetActive(true);
				if (i > skills.Count)
				{
					transform2.gameObject.SetActive(false);
					continue;
				}
				EquipSkillDataItem skillData = skills[i - 1];
				UpdateSkillContent(transform2, skillData);
			}
			Transform transform3 = base.view.transform.Find("SkillPopup/StigmataSkills/ScrollerView/Content/SetSkills");
			SortedDictionary<int, EquipSkillDataItem> allSetSkills = stigmataDataItem.GetAllSetSkills();
			if (allSetSkills.Count == 0)
			{
				transform3.gameObject.SetActive(false);
			}
			else
			{
				transform3.gameObject.SetActive(true);
				transform3.Find("Name/Text").GetComponent<Text>().text = stigmataDataItem.GetEquipSetName();
				Transform transform4 = transform3.Find("Desc");
				for (int j = 0; j < transform3.Find("Desc").childCount; j++)
				{
					int key = j + 2;
					Transform child = transform4.GetChild(j);
					if (!(child == null))
					{
						EquipSkillDataItem value;
						allSetSkills.TryGetValue(key, out value);
						if (value == null)
						{
							child.gameObject.SetActive(false);
						}
						else
						{
							child.Find("Desc").GetComponent<Text>().text = value.GetSkillDisplay();
						}
					}
				}
			}
			base.view.transform.Find("SkillPopup/StigmataSkills").gameObject.SetActive(false);
		}

		private void UpdateSkillContent(Transform trans, EquipSkillDataItem skillData)
		{
			trans.Find("Label").GetComponent<Text>().text = skillData.skillName;
			trans.Find("Desc").GetComponent<Text>().text = skillData.GetSkillDisplay(itemDataBefore.level);
		}

		private void PlayEffectWithDelay(float delay)
		{
			_effectDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(delay, 0f);
			_effectDelayTimer.timeUpCallback = PlayPowerUpEffect;
		}

		private void PlayPowerUpEffect()
		{
			Transform transform;
			if (dialogType == DialogType.PowerUp)
			{
				_powerUpResultIndex = MiscData.GetEquipPowerUpResultIndex(boostRate);
				switch (_powerUpResultIndex)
				{
				case 2:
					Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small");
					transform = base.view.transform.Find("ItemPanel/Effects/UpgradingBigSuccess");
					transform.gameObject.SetActive(true);
					break;
				case 3:
					Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large");
					transform = base.view.transform.Find("ItemPanel/Effects/UpgradingLargeSuccess");
					transform.gameObject.SetActive(true);
					break;
				default:
					Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small");
					transform = base.view.transform.Find("ItemPanel/Effects/UpgradeSuccess");
					transform.gameObject.SetActive(true);
					break;
				}
			}
			else
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large");
				transform = base.view.transform.Find("ItemPanel/Effects/Evolution");
				transform.gameObject.SetActive(true);
			}
			ParticleSystem[] componentsInChildren = transform.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
			if (dialogType == DialogType.PowerUp)
			{
				SetupLvInfoPanel();
			}
			else if (dialogType == DialogType.Evo)
			{
				SetupTitleAndStar();
			}
			else if (dialogType == DialogType.NewAffix)
			{
				SetupNewAffixInfo();
			}
		}

		private ParticleSystem GetSmallEffect()
		{
			Transform transform = base.view.transform.Find("ItemPanel/Effects/Small/" + _effectIndex);
			transform.gameObject.SetActive(true);
			if (_effectIndex == _materialList.Count - 1)
			{
				transform.localPosition = _effectPosition[_effectPosition.Count - 1];
			}
			else
			{
				transform.localPosition = _effectPosition[_effectIndex];
			}
			_effectIndex++;
			ParticleSystem[] componentsInChildren = transform.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
			if (_flashStartTimer != null)
			{
				_flashStartTimer.Destroy();
			}
			_flashStartTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(1f / 60f, 0f);
			_flashStartTimer.timeUpCallback = StartImageColorScaler;
			return transform.GetComponent<ParticleSystem>();
		}

		private void SetEquipImageFlash(float scaler, float timeSpan)
		{
			if (_flashResetTimer != null)
			{
				_flashResetTimer.Destroy();
			}
			_flashResetTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(timeSpan, 0f);
			_flashResetTimer.timeUpCallback = ResetImageColorScaler;
			SetImageColorScaler(scaler);
		}

		private void StartImageColorScaler()
		{
			SetEquipImageFlash(3f, 0.05f);
		}

		private void ResetImageColorScaler()
		{
			SetImageColorScaler(1f);
		}

		private void SetImageColorScaler(float scaler)
		{
			if (itemDataBefore is WeaponDataItem)
			{
				base.view.transform.Find("ItemPanel/3dModel").GetComponent<RawImage>().material.SetFloat("_ColorScaler", scaler);
			}
		}
	}
}
