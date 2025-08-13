using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMaterialPanel : MonoBehaviour
	{
		public enum AnimStep
		{
			StepNone = 0,
			StepOne = 1,
			StepTwo = 2,
			StepThree = 3,
			StepFour = 4
		}

		private const string MaterialRarityIconPrefabPathPre = "SpriteOutput/MaterialRarityIcons/Metial";

		private const string ANI_CLIP_PRE = "PowerUpMaterialMove_";

		private List<StorageDataItemBase> _materialList;

		private AnimStep _step;

		private void Update()
		{
			switch (_step)
			{
			case AnimStep.StepNone:
				break;
			case AnimStep.StepOne:
				_step = AnimStep.StepTwo;
				break;
			case AnimStep.StepTwo:
				base.transform.GetComponent<ContentSizeFitter>().enabled = false;
				_step = AnimStep.StepThree;
				break;
			case AnimStep.StepThree:
				base.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
				_step = AnimStep.StepFour;
				break;
			case AnimStep.StepFour:
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_Mat_Drop");
				Animation component = base.transform.GetComponent<Animation>();
				string animation = "PowerUpMaterialMove_" + _materialList.Count;
				component.Play(animation, PlayMode.StopAll);
				_step = AnimStep.StepNone;
				break;
			}
			}
		}

		public void SetupView(List<StorageDataItemBase> materialList)
		{
			_materialList = materialList;
			base.transform.GetComponent<Animation>().Stop();
			_step = AnimStep.StepNone;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (i >= _materialList.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				StorageDataItemBase storageDataItemBase = _materialList[i];
				int num = 1;
				switch (storageDataItemBase.rarity)
				{
				case 1:
				case 2:
					num = 1;
					break;
				case 3:
					num = 2;
					break;
				case 4:
				case 5:
					num = 3;
					break;
				}
				string prefabPath = "SpriteOutput/MaterialRarityIcons/Metial" + num;
				child.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				child.GetComponent<MonoItemIconButton>().SetupView(storageDataItemBase);
			}
			_step = AnimStep.StepOne;
		}

		public void EatMaterial()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PowerUpAndEvoEffect, "Small"));
		}

		public void EatAll()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PowerUpAndEvoEffect, "EatAll"));
		}
	}
}
