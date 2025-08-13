using System.Collections;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoLevelDropIconButton : MonoBehaviour
	{
		private const string _bg_path_jixie = "SpriteOutput/AvatarIcon/AttrJiXie1";

		private const string _bg_path_shengwu = "SpriteOutput/AvatarIcon/AttrShengWu1";

		private const string _bg_path_yineng = "SpriteOutput/AvatarIcon/AttrYiNeng1";

		private const string _itemIconCommonBGSpritePath = "SpriteOutput/SpecialIcons/ItemCommonBG";

		public bool showNewIcon;

		public float width;

		public float height;

		public string effectAudioPattern;

		private StorageDataItemBase _dropItem;

		private DropItemButtonClickCallBack _callBack;

		private bool _hideBg;

		private bool _isGrey;

		private bool _originBGDescEnabled;

		private bool _hasSetBGDesc;

		public void SetupView(StorageDataItemBase itemData, DropItemButtonClickCallBack callBack = null, bool showDesc = false, bool showNewIcon = false, bool isGrey = false, bool hideBg = false)
		{
			if (!_hasSetBGDesc)
			{
				_originBGDescEnabled = base.transform.Find("BG/Desc").gameObject.activeSelf;
				_hasSetBGDesc = true;
			}
			Clear();
			_dropItem = itemData;
			_callBack = callBack;
			this.showNewIcon = showNewIcon;
			_hideBg = hideBg;
			_isGrey = isGrey;
			SetupFrame();
			base.transform.Find("FragmentIcon").gameObject.SetActive(itemData is AvatarFragmentDataItem);
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(_dropItem.GetIconPath());
			base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[itemData.rarity]);
			base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = Color.white;
			base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
			SetupRarityView(showDesc);
			SetupStigmataTypeIcon();
			base.transform.Find("BG").gameObject.SetActive(showDesc);
			base.transform.Find("BG/Image").gameObject.SetActive(showDesc);
			string text = "×" + _dropItem.number;
			if (_dropItem is WeaponDataItem || _dropItem is StigmataDataItem)
			{
				text = "Lv." + _dropItem.level;
				base.transform.Find("BG/Desc").GetComponent<Text>().text = text;
			}
			else if (_dropItem is AvatarCardDataItem)
			{
				base.transform.Find("BG/Desc").GetComponent<Text>().text = string.Empty;
			}
			else
			{
				base.transform.Find("BG/Desc").GetComponent<Text>().text = text;
			}
			Transform transform = base.transform.Find("BG/ToFragment");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			bool flag = Singleton<StorageModule>.Instance.IsItemNew(_dropItem.ID);
			if (_dropItem is AvatarCardDataItem)
			{
				int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(_dropItem.ID).avatarID;
				flag = !Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID).UnLocked;
			}
			base.transform.Find("NewMark").gameObject.SetActive(showNewIcon && flag);
			if (showNewIcon && flag)
			{
				Singleton<StorageModule>.Instance.RecordNewItem(_dropItem.ID);
			}
			if (_dropItem is StigmataDataItem)
			{
				SetupStigmataAffixView((_dropItem as StigmataDataItem).IsAffixIdentify);
			}
			if (hideBg)
			{
				base.transform.Find("BG").gameObject.SetActive(false);
			}
			if (_isGrey)
			{
				SetItemGrey();
			}
		}

		public void Clear()
		{
			_dropItem = null;
			_callBack = null;
			Transform transform = base.transform.Find("BG/UnidentifyText");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			if (_hasSetBGDesc)
			{
				base.transform.Find("BG/Desc").gameObject.SetActive(_originBGDescEnabled);
			}
			SetItemDefaultMaterialAndColor();
		}

		public void OnClick()
		{
			if (_callBack != null)
			{
				_callBack(_dropItem);
			}
		}

		public void PlayVFX(float delay, bool isRareDrop)
		{
			StartCoroutine(Coroutine_AvatarEffect(delay, isRareDrop));
		}

		public bool CanSplit()
		{
			AvatarCardDataItem avatarCardDataItem = _dropItem as AvatarCardDataItem;
			return avatarCardDataItem != null && avatarCardDataItem.IsSplite();
		}

		public bool IsAvatarCard()
		{
			AvatarCardDataItem avatarCardDataItem = _dropItem as AvatarCardDataItem;
			return avatarCardDataItem != null;
		}

		private IEnumerator Coroutine_AvatarEffect(float delay, bool isRareDrop)
		{
			yield return new WaitForSeconds(delay);
			if (CanSplit())
			{
				base.transform.Find("BecomeFragmentEffect").gameObject.GetComponent<ParticleSystem>().Play();
				if (!string.IsNullOrEmpty(effectAudioPattern))
				{
					Singleton<WwiseAudioManager>.Instance.Post(effectAudioPattern);
				}
				yield return new WaitForSeconds(0.2f);
				AvatarCardDataItem avatarCard = _dropItem as AvatarCardDataItem;
				int fragmentId = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(_dropItem.ID).avatarFragmentID;
				AvatarFragmentMetaData metaData = AvatarFragmentMetaDataReader.GetAvatarFragmentMetaDataByKey(fragmentId);
				SetupView(new AvatarFragmentDataItem(metaData)
				{
					number = avatarCard.GetSpliteFragmentNum()
				}, _callBack, true, true);
				base.transform.Find("AvatarStar").gameObject.SetActive(false);
				base.transform.Find("BG/ToFragment").gameObject.SetActive(true);
			}
			if (isRareDrop)
			{
				base.transform.Find("Particle1").gameObject.SetActive(true);
				base.transform.Find("Particle2").gameObject.SetActive(true);
				ParticleSystem[] componentsInChildren = base.transform.Find("Particle1").GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps in componentsInChildren)
				{
					ps.Play();
				}
				ParticleSystem[] componentsInChildren2 = base.transform.Find("Particle2").GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps2 in componentsInChildren2)
				{
					ps2.Play();
				}
				if (!string.IsNullOrEmpty(effectAudioPattern))
				{
					Singleton<WwiseAudioManager>.Instance.Post(effectAudioPattern);
				}
			}
			yield return new WaitForSeconds(1.5f);
			base.transform.Find("Particle2").gameObject.SetActive(false);
		}

		public void StopRareEffect()
		{
			base.transform.Find("Particle1").gameObject.SetActive(false);
			base.transform.Find("Particle2").gameObject.SetActive(false);
			ParticleSystem[] componentsInChildren = base.transform.Find("Particle1").GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
			ParticleSystem[] componentsInChildren2 = base.transform.Find("Particle2").GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem2 in componentsInChildren2)
			{
				particleSystem2.Stop();
			}
		}

		private void SetupFrame()
		{
			if (_dropItem is AvatarCardDataItem)
			{
				Image component = base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>();
				component.sprite = GetBGSprite();
				component.color = Color.white;
			}
			else
			{
				Image component2 = base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>();
				component2.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/SpecialIcons/ItemCommonBG");
			}
		}

		private Sprite GetBGSprite()
		{
			switch (GetAvatarAttribte())
			{
			case EntityNature.Mechanic:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrJiXie1");
			case EntityNature.Biology:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrShengWu1");
			case EntityNature.Psycho:
				return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrYiNeng1");
			default:
				return null;
			}
		}

		private EntityNature GetAvatarAttribte()
		{
			return (EntityNature)Singleton<AvatarModule>.Instance.GetAvatarByID(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(_dropItem.ID).avatarID).Attribute;
		}

		private void SetupColor()
		{
			Color color = Color.white;
			string htmlString = MiscData.Config.ItemRarityColorList[_dropItem.rarity];
			if (ColorUtility.TryParseHtmlString(htmlString, out color))
			{
				base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = color;
			}
		}

		private void SetupRarityView(bool showDesc)
		{
			base.transform.Find("AvatarStar").gameObject.SetActive(false);
			base.transform.Find("Star").gameObject.SetActive(false);
			if (_dropItem is AvatarCardDataItem)
			{
				AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(_dropItem.ID).avatarID);
				SetupAvatarStar(dummyAvatarDataItem.star);
				base.transform.Find("AvatarStar").gameObject.SetActive(showDesc && _dropItem is AvatarCardDataItem);
			}
			else if (!(_dropItem is AvatarFragmentDataItem))
			{
				base.transform.Find("Star").gameObject.SetActive(true);
				int maxStar = _dropItem.rarity;
				if (_dropItem is WeaponDataItem)
				{
					maxStar = (_dropItem as WeaponDataItem).GetMaxRarity();
				}
				else if (_dropItem is StigmataDataItem)
				{
					maxStar = (_dropItem as StigmataDataItem).GetMaxRarity();
				}
				base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(_dropItem.rarity, maxStar);
			}
		}

		private void SetupStigmataTypeIcon()
		{
			base.transform.Find("StigmataType").gameObject.SetActive(_dropItem is StigmataDataItem);
			if (_dropItem is StigmataDataItem)
			{
				base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[_dropItem.GetBaseType()]);
			}
		}

		private void SetupStigmataAffixView(bool isIdentify)
		{
			if (!_hideBg)
			{
				if (base.transform.Find("BG/UnidentifyText") != null)
				{
					base.transform.Find("BG/UnidentifyText").gameObject.SetActive(!isIdentify);
				}
				base.transform.Find("BG/Desc").gameObject.SetActive(isIdentify);
			}
			Image component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
			if (isIdentify)
			{
				component.material = null;
				component.color = Color.white;
			}
			else
			{
				Material material = Miscs.LoadResource<Material>("Material/ImageMonoColor");
				component.material = material;
				component.color = MiscData.GetColor("DarkBlue");
			}
			if (base.transform.Find("QuestionMark") != null)
			{
				base.transform.Find("QuestionMark").gameObject.SetActive(!isIdentify);
			}
		}

		private void SetupAvatarStar(int starNum)
		{
			for (int i = 1; i < 6; i++)
			{
				string text = string.Format("AvatarStar/{0}", i);
				base.transform.Find(text).gameObject.SetActive(i == starNum);
			}
		}

		public void SetupFinishStageNomalDropView()
		{
			base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.blue;
		}

		public void SetupFinishStageFastDropView()
		{
			base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.yellow;
		}

		public void SetupFinishStageVeryFastDropView()
		{
			base.transform.Find("BG/Unselected").GetComponent<Image>().color = Color.red;
		}

		private void SetItemGrey()
		{
			base.transform.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			base.transform.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
			Image component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
			if (component.material != component.defaultMaterial)
			{
				component.color = MiscData.GetColor("DropItemIconFullGrey");
			}
			else
			{
				component.color = MiscData.GetColor("DropItemIconGrey");
			}
			for (int i = 1; i < 6; i++)
			{
				Transform transform = base.transform.Find("Star/" + i);
				if (!(transform == null))
				{
					transform.GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
				}
			}
			for (int j = 1; j < 6; j++)
			{
				Transform transform2 = base.transform.Find("AvatarStar/" + j);
				if (!(transform2 == null))
				{
					transform2.GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
				}
			}
			base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
			Transform transform3 = base.transform.Find("BG/UnidentifyText");
			if (transform3 != null)
			{
				transform3.GetComponent<Text>().color = MiscData.GetColor("DropItemUnidentifyBlack");
			}
		}

		private void SetItemDefaultMaterialAndColor()
		{
			Image component = base.transform.Find("BG/Unselected").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemImageDefaultColor");
			component = base.transform.Find("BG/Image").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemImageDefaultColor");
			component = base.transform.Find("ItemIcon/ItemIcon").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemIconDefaultColor");
			component = base.transform.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemImageDefaultColor");
			for (int i = 1; i < 6; i++)
			{
				Transform transform = base.transform.Find("Star/" + i);
				if (!(transform == null))
				{
					component = transform.GetComponent<Image>();
					component.material = null;
					component.color = MiscData.GetColor("DropItemImageDefaultColor");
				}
			}
			for (int j = 1; j <= 6; j++)
			{
				Transform transform2 = base.transform.Find("AvatarStar/" + j);
				if (!(transform2 == null))
				{
					component = transform2.GetComponent<Image>();
					component.material = null;
					component.color = MiscData.GetColor("DropItemImageDefaultColor");
				}
			}
			component = base.transform.Find("StigmataType/Image").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemImageDefaultColor");
			component = base.transform.Find("FragmentIcon").GetComponent<Image>();
			component.material = null;
			component.color = MiscData.GetColor("DropItemImageDefaultColor");
			Transform transform3 = base.transform.Find("BG/UnidentifyText");
			if (transform3 != null)
			{
				transform3.GetComponent<Text>().color = MiscData.GetColor("DropItemUnidentifyDefaultColor");
			}
		}

		private void OnDestroy()
		{
		}

		public int GetDropItemID()
		{
			return _dropItem.ID;
		}
	}
}
