using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;
using Material = UnityEngine.Material;

namespace MoleMole
{
	public class MonoLevelView : MonoBehaviour
	{
		private const string MATERIAL_GREY_PATH = "Material/LevelGrey";

		private const float ENTER_TIME_LACK_LEVEL_ALPHA = 0.8f;

		private LevelDataItem levelData;

		private LevelBtnClickCallBack btnCallBack;

		private bool _enterTimesEnough;

		private float _timer;

		private float _timerSpan;

		private bool _playAni;

		private Image _prefixLineImage;

		private int _levelNeed;

		private int _playerLevel;

		private int _challengeNumNeed;

		private int _totalFinishChallengeNum;

		private void Update()
		{
			if (_playAni)
			{
				_timer += Time.deltaTime;
				_prefixLineImage.fillAmount = Mathf.Clamp(_timer / _timerSpan, 0f, 1f);
				if (_timer > _timerSpan)
				{
					_playAni = false;
					base.transform.GetComponent<Animation>().Play();
				}
			}
		}

		public void SetupView(LevelDataItem levelData, bool enterTimesEnough, LevelBtnClickCallBack callBack = null, int totalFinishChallengeNum = 0)
		{
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Invalid comparison between Unknown and I4
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Expected I4, but got Unknown
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Invalid comparison between Unknown and I4
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Invalid comparison between Unknown and I4
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Invalid comparison between Unknown and I4
			this.levelData = levelData;
			btnCallBack = callBack;
			_enterTimesEnough = enterTimesEnough;
			_levelNeed = levelData.UnlockPlayerLevel;
			_playerLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			_challengeNumNeed = levelData.UnlockChanllengeNum;
			_totalFinishChallengeNum = totalFinishChallengeNum;
			base.transform.Find("LevelLack").gameObject.SetActive(false);
			base.transform.Find("ChallengeLack").gameObject.SetActive(false);
			base.transform.Find("BattleType").gameObject.SetActive(!levelData.IsNormalBattleType);
			if (!levelData.IsNormalBattleType)
			{
				base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = levelData.GetBattleTypeSprite();
			}
			if (levelData.SectionId > 1)
			{
				string text = "LevelLinesVia/" + levelData.SectionId;
				_prefixLineImage = base.transform.parent.parent.Find(text).GetComponent<Image>();
				float fillAmount = 0f;
				if ((int)levelData.status != 1 && !Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Contains(levelData.levelId))
				{
					fillAmount = 1f;
				}
				_prefixLineImage.fillAmount = fillAmount;
			}
			if (!_enterTimesEnough)
			{
				Material material = Miscs.LoadResource<Material>("Material/LevelGrey");
				base.transform.Find("Active/Pic").GetComponent<Image>().material = material;
				base.transform.Find("Active/Pic").GetComponent<CanvasGroup>().alpha = 0.8f;
			}
			else
			{
				base.transform.Find("Active/Pic").GetComponent<Image>().material = null;
				base.transform.Find("Active/Pic").GetComponent<CanvasGroup>().alpha = 1f;
			}
			base.transform.Find("Story").gameObject.SetActive(false);
			bool active = HasPlot() || HasCg();
			StageType levelType = levelData.LevelType;
			switch ((int)levelType - 1)
			{
			case 0:
			{
				if ((int)levelData.status == 1)
				{
					base.transform.Find("Active").gameObject.SetActive(false);
					base.transform.Find("Unactive").gameObject.SetActive(true);
					base.transform.Find("Unactive/ChapterName").GetComponent<Text>().text = levelData.StageName;
					if (levelData.SectionId != 1)
					{
					}
					break;
				}
				base.transform.Find("Active").gameObject.SetActive(true);
				base.transform.Find("Unactive").gameObject.SetActive(false);
				base.transform.Find("Active/ActivityLevelName").gameObject.SetActive(false);
				base.transform.Find("Active/ChapterName").gameObject.SetActive(true);
				base.transform.Find("Active/ChapterName").GetComponent<Text>().text = levelData.StageName;
				for (int j = 0; j < levelData.challengeList.Count; j++)
				{
					Transform child2 = base.transform.Find("Active/Badges").GetChild(j);
					if (!(child2 == null))
					{
						bool finished2 = levelData.challengeList[j].Finished;
						child2.Find("Achieve").gameObject.SetActive(finished2);
						child2.Find("Unachieve").gameObject.SetActive(!finished2);
					}
				}
				base.transform.Find("Active/Caution").gameObject.SetActive(false);
				base.transform.Find("Active/Pic").GetComponent<Image>().sprite = levelData.GetBriefPicSprite();
				base.transform.Find("Story").gameObject.SetActive(active);
				break;
			}
			case 1:
			case 2:
			case 4:
			{
				base.transform.Find("Active").gameObject.SetActive(true);
				base.transform.Find("Unactive").gameObject.SetActive(false);
				base.transform.Find("Active/ChapterName").gameObject.SetActive(false);
				base.transform.Find("Active/ActivityLevelName").gameObject.SetActive(true);
				base.transform.Find("Active/ActivityLevelName").GetComponent<Text>().text = levelData.StageName;
				for (int i = 0; i < levelData.challengeList.Count; i++)
				{
					Transform child = base.transform.Find("Active/Badges").GetChild(i);
					if (!(child == null))
					{
						bool finished = levelData.challengeList[i].Finished;
						child.Find("Achieve").gameObject.SetActive(finished);
						child.Find("Unachieve").gameObject.SetActive(!finished);
					}
				}
				base.transform.Find("Active/Caution").gameObject.SetActive(false);
				base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = levelData.GetBattleTypeSprite();
				base.transform.Find("Active/Pic").GetComponent<Image>().sprite = levelData.GetBriefPicSprite();
				bool flag = (int)levelData.LevelType == 5;
				base.transform.Find("Active/BG").GetComponent<Image>().color = ((!flag) ? Color.white : MiscData.GetColor("Blue"));
				base.transform.Find("Outter").GetComponent<Image>().color = ((!flag) ? MiscData.GetColor("Blue") : MiscData.GetColor("Purple"));
				break;
			}
			}
			string text2 = null;
			if ((int)levelData.status != 1)
			{
				if (_levelNeed > _playerLevel)
				{
					base.transform.Find("LevelLack").gameObject.SetActive(true);
					base.transform.Find("LevelLack/LevelNeed").GetComponent<Text>().text = "Lv." + _levelNeed;
					Material material2 = Miscs.LoadResource<Material>("Material/LevelGrey");
					base.transform.Find("Active/Pic").GetComponent<Image>().material = material2;
					text2 = "UI_Gen_Select_Negative";
				}
				else if (_challengeNumNeed > _totalFinishChallengeNum)
				{
					base.transform.Find("ChallengeLack").gameObject.SetActive(true);
					base.transform.Find("ChallengeLack/Num").GetComponent<Text>().text = "x" + _challengeNumNeed;
				}
			}
			else
			{
				text2 = "UI_Gen_Select_Negative";
			}
			if (!string.IsNullOrEmpty(text2))
			{
				MonoButtonWwiseEvent monoButtonWwiseEvent = base.transform.Find("Btn").GetComponent<MonoButtonWwiseEvent>();
				if (monoButtonWwiseEvent == null)
				{
					monoButtonWwiseEvent = base.transform.Find("Btn").gameObject.AddComponent<MonoButtonWwiseEvent>();
				}
				if (monoButtonWwiseEvent != null)
				{
					monoButtonWwiseEvent.eventName = text2;
				}
			}
			base.transform.Find("BattleType/Type").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(levelData.BattleTypePath);
			base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(OnBtnClick);
		}

		protected void BindViewCallback(Transform trans, EventTriggerType eventType, Action<BaseEventData> callback)
		{
			MonoEventTrigger monoEventTrigger = trans.gameObject.GetComponent<MonoEventTrigger>();
			if (monoEventTrigger == null)
			{
				monoEventTrigger = trans.gameObject.AddComponent<MonoEventTrigger>();
			}
			monoEventTrigger.ClearTriggers();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventType;
			entry.callback.AddListener(delegate(BaseEventData evtData)
			{
				callback(evtData);
			});
			monoEventTrigger.AddTrigger(entry);
		}

		private void OnBtnClick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)levelData.status != 1)
			{
				if (!_enterTimesEnough)
				{
					EnterTimesLack();
				}
				else if (_levelNeed > _playerLevel)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", _levelNeed)));
				}
				else if (btnCallBack != null)
				{
					btnCallBack(levelData);
				}
			}
		}

		public void SetBGColor(string colorCode)
		{
			base.transform.Find("Active/BG").GetComponent<Image>().color = UIUtil.SetupColor(colorCode);
		}

		private void EnterTimesLack()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_LevelEnterTimeLack")));
		}

		public void PlayNewUnlockAnimation(float timerSpan = 0.5f)
		{
			if (_prefixLineImage != null)
			{
				_timer = 0f;
				_timerSpan = timerSpan;
				_playAni = true;
			}
			else
			{
				base.transform.GetComponent<Animation>().Play();
			}
		}

		private bool HasPlot()
		{
			List<PlotMetaData> itemList = PlotMetaDataReader.GetItemList();
			PlotMetaData plotMetaData = itemList.Find((PlotMetaData x) => x.levelID == levelData.levelId);
			return plotMetaData != null;
		}

		private bool HasCg()
		{
			List<CgDataItem> cgDataItemList = Singleton<CGModule>.Instance.GetCgDataItemList();
			CgDataItem cgDataItem = cgDataItemList.Find((CgDataItem x) => x.levelID == levelData.levelId);
			return cgDataItem != null;
		}
	}
}
