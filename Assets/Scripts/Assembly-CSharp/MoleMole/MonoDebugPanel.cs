using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDebugPanel : MonoBehaviour
	{
		private enum MonsterColType
		{
			CATEGORY = 0,
			NAME = 1,
			TYPE = 2
		}

		private int monsterNumber = 1;

		private int monsterLevel = 1;

		private uint uniqueMonsterID;

		private bool isElite;

		private bool isStationary;

		private float toggleCategoryListPanelOriginHeight;

		private float toggleNameListPanelOriginHeight;

		private float toggleTypeListPanelOriginHeight;

		private List<string> _selectedMonsterCategoryList;

		private List<string> _selectedMonsterNameList;

		private List<string> _selectedMonsterTypeList;

		private string _selectedMonsterCategory;

		private string _selectedMonsterName;

		private string _selectedMonsterType;

		private List<MonsterConfigMetaData> _monsterMetaDataList;

		private Dictionary<uint, float> avatarMaxHPDict;

		private Dictionary<uint, float> avatarMaxSPDict;

		private void Awake()
		{
			avatarMaxHPDict = new Dictionary<uint, float>();
			avatarMaxSPDict = new Dictionary<uint, float>();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void OnShowPanelButtonClick()
		{
			SetupView();
			base.transform.gameObject.SetActive(true);
		}

		public void OnConfirmButtonClick()
		{
			_selectedMonsterCategoryList.Clear();
			_selectedMonsterNameList.Clear();
			_selectedMonsterTypeList.Clear();
			string text = base.transform.Find("ConfirmPanel/MonsterNumber/Text").GetComponent<Text>().text;
			if (text == string.Empty)
			{
				text = "1";
			}
			if (!int.TryParse(text, out monsterNumber))
			{
				monsterNumber = 1;
			}
			string text2 = base.transform.Find("ConfirmPanel/MonsterLevel/Text").GetComponent<Text>().text;
			if (text2 == string.Empty)
			{
				text2 = "1";
			}
			if (!int.TryParse(text2, out monsterLevel))
			{
				monsterNumber = 1;
			}
			string text3 = base.transform.Find("ConfirmPanel/UniqueID/Text").GetComponent<Text>().text;
			if (text3 == string.Empty)
			{
				text3 = "0";
			}
			if (!uint.TryParse(text3, out uniqueMonsterID))
			{
				uniqueMonsterID = 0u;
			}
			isElite = base.transform.Find("ConfirmPanel/EliteToggle").GetComponent<Toggle>().isOn;
			if (uniqueMonsterID != 0 || (!string.IsNullOrEmpty(_selectedMonsterCategory) && !string.IsNullOrEmpty(_selectedMonsterName) && !string.IsNullOrEmpty(_selectedMonsterType)))
			{
				Vector3 xZPosition = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
				for (int i = 0; i < monsterNumber; i++)
				{
					uint runtimeID = Singleton<MonsterManager>.Instance.CreateMonster(_selectedMonsterName, _selectedMonsterType, monsterLevel, true, xZPosition, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), isElite, uniqueMonsterID);
					if (isElite)
					{
						MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(runtimeID);
						if (_selectedMonsterCategory == "DeadGal" || _selectedMonsterCategory == "Ulysses" || _selectedMonsterCategory == "Robot")
						{
							actor.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig("Elite_EliteShield", _selectedMonsterName));
						}
					}
				}
				isStationary = base.transform.Find("ConfirmPanel/MonsterStationaryToggle").GetComponent<Toggle>().isOn;
				if (isStationary)
				{
					OnMonsterStationaryToggleValueChanged(isStationary);
				}
			}
			base.transform.gameObject.SetActive(false);
		}

		public void OnKillAllMonsterButtonClick()
		{
			MonsterActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4);
			foreach (MonsterActor monsterActor in actorByCategory)
			{
				if ((bool)monsterActor.isAlive)
				{
					monsterActor.ForceKill();
				}
			}
			Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
		}

		private void ClearMonsterCol(MonsterColType colType)
		{
			ClearMonsterColData(colType);
			ClearMonsterColItem(colType);
		}

		private void ClearMonsterColData(MonsterColType colType)
		{
			switch (colType)
			{
			case MonsterColType.CATEGORY:
				_selectedMonsterCategoryList.Clear();
				_selectedMonsterCategory = string.Empty;
				break;
			case MonsterColType.NAME:
				_selectedMonsterNameList.Clear();
				_selectedMonsterName = string.Empty;
				break;
			case MonsterColType.TYPE:
				_selectedMonsterTypeList.Clear();
				_selectedMonsterType = string.Empty;
				break;
			}
		}

		private void ClearMonsterColItem(MonsterColType colType)
		{
			switch (colType)
			{
			case MonsterColType.CATEGORY:
			{
				Transform transform2 = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
				for (int j = 0; j < transform2.childCount; j++)
				{
					Object.Destroy(transform2.GetChild(j).gameObject);
				}
				break;
			}
			case MonsterColType.NAME:
			{
				Transform transform3 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
				for (int k = 0; k < transform3.childCount; k++)
				{
					Object.Destroy(transform3.GetChild(k).gameObject);
				}
				break;
			}
			case MonsterColType.TYPE:
			{
				Transform transform = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
				for (int i = 0; i < transform.childCount; i++)
				{
					Object.Destroy(transform.GetChild(i).gameObject);
				}
				break;
			}
			}
		}

		private void ClearMonsterNameColume()
		{
			_selectedMonsterNameList.Clear();
			_selectedMonsterName = string.Empty;
			Transform transform = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
			for (int i = 0; i < transform.childCount; i++)
			{
				Object.Destroy(transform.GetChild(i).gameObject);
			}
		}

		private void ClearMonsterTypeColume()
		{
			_selectedMonsterTypeList.Clear();
			_selectedMonsterType = string.Empty;
			Transform transform = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
			for (int i = 0; i < transform.childCount; i++)
			{
				Object.Destroy(transform.GetChild(i).gameObject);
			}
		}

		public void SetupView()
		{
			_selectedMonsterCategoryList = new List<string>();
			_selectedMonsterTypeList = new List<string>();
			_selectedMonsterNameList = new List<string>();
			toggleCategoryListPanelOriginHeight = base.transform.Find("MonsterCategoryListPanel").GetComponent<RectTransform>().rect.height;
			toggleNameListPanelOriginHeight = base.transform.Find("MonsterNameListPanel").GetComponent<RectTransform>().rect.height;
			toggleTypeListPanelOriginHeight = base.transform.Find("MonsterTypeListPanel").GetComponent<RectTransform>().rect.height;
			Transform transform = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
			Transform transform2 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
			Transform transform3 = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleCategoryListPanelOriginHeight);
			transform2.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleNameListPanelOriginHeight);
			transform3.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleTypeListPanelOriginHeight);
			ClearMonsterCol(MonsterColType.CATEGORY);
			ClearMonsterCol(MonsterColType.NAME);
			ClearMonsterCol(MonsterColType.TYPE);
			_monsterMetaDataList = MonsterData.GetAllMonsterConfigMetaData();
			BuildMonsterColData(MonsterColType.CATEGORY);
			BuildMonsterColItem(MonsterColType.CATEGORY);
		}

		private void BuildMonsterColData(MonsterColType colType)
		{
			switch (colType)
			{
			case MonsterColType.CATEGORY:
			{
				foreach (MonsterConfigMetaData monsterMetaData in _monsterMetaDataList)
				{
					if (!_selectedMonsterCategoryList.Contains(monsterMetaData.categoryName))
					{
						_selectedMonsterCategoryList.Add(monsterMetaData.categoryName);
					}
				}
				break;
			}
			case MonsterColType.NAME:
			{
				foreach (MonsterConfigMetaData monsterMetaData2 in _monsterMetaDataList)
				{
					if (monsterMetaData2.categoryName == _selectedMonsterCategory && !_selectedMonsterNameList.Contains(monsterMetaData2.monsterName))
					{
						_selectedMonsterNameList.Add(monsterMetaData2.monsterName);
					}
				}
				break;
			}
			case MonsterColType.TYPE:
			{
				foreach (MonsterConfigMetaData monsterMetaData3 in _monsterMetaDataList)
				{
					if (monsterMetaData3.categoryName == _selectedMonsterCategory && monsterMetaData3.monsterName == _selectedMonsterName && !_selectedMonsterTypeList.Contains(monsterMetaData3.typeName))
					{
						_selectedMonsterTypeList.Add(monsterMetaData3.typeName);
					}
				}
				break;
			}
			}
		}

		private void BuildMonsterColItem(MonsterColType colType)
		{
			Transform transform = base.transform.Find("MonsterCategoryListPanel/MonsterToggleListPanel");
			if (_selectedMonsterCategoryList.Count > 5)
			{
				transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleCategoryListPanelOriginHeight * (float)_selectedMonsterCategoryList.Count / 5f);
			}
			Transform transform2 = base.transform.Find("MonsterNameListPanel/MonsterToggleListPanel");
			if (_selectedMonsterNameList.Count > 5)
			{
				transform2.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleNameListPanelOriginHeight * (float)_selectedMonsterNameList.Count / 5f);
			}
			Transform transform3 = base.transform.Find("MonsterTypeListPanel/MonsterToggleListPanel");
			if (_selectedMonsterTypeList.Count > 5)
			{
				transform3.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, toggleTypeListPanelOriginHeight * (float)_selectedMonsterTypeList.Count / 5f);
			}
			int num = 0;
			switch (colType)
			{
			case MonsterColType.CATEGORY:
			{
				foreach (string selectedMonsterCategory in _selectedMonsterCategoryList)
				{
					num++;
					Transform transform6 = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle")).transform;
					string text3 = string.Format("{0}", selectedMonsterCategory);
					transform6.Find("Label").GetComponent<Text>().text = text3;
					transform6.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, text3, MonoMonsterToggle.ToggleColumn.CATEGORY);
					transform6.SetParent(transform, false);
					transform6.GetComponent<Toggle>().group = transform.GetComponent<ToggleGroup>();
					transform6.GetComponent<Toggle>().onValueChanged.AddListener(transform6.GetComponent<MonoMonsterToggle>().OnToggleValueChanged);
				}
				break;
			}
			case MonsterColType.NAME:
			{
				foreach (string selectedMonsterName in _selectedMonsterNameList)
				{
					num++;
					Transform transform5 = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle")).transform;
					string text2 = string.Format("{0}", selectedMonsterName);
					transform5.Find("Label").GetComponent<Text>().text = text2;
					transform5.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, text2, MonoMonsterToggle.ToggleColumn.NAME);
					transform5.SetParent(transform2, false);
					transform5.GetComponent<Toggle>().group = transform2.GetComponent<ToggleGroup>();
					transform5.GetComponent<Toggle>().onValueChanged.AddListener(transform5.GetComponent<MonoMonsterToggle>().OnToggleValueChanged);
				}
				break;
			}
			case MonsterColType.TYPE:
			{
				foreach (string selectedMonsterType in _selectedMonsterTypeList)
				{
					num++;
					Transform transform4 = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/MonsterDebugToggle")).transform;
					string text = string.Format("{0}", selectedMonsterType);
					transform4.Find("Label").GetComponent<Text>().text = text;
					transform4.GetComponent<MonoMonsterToggle>().SetMonsterToggleValue(this, text, MonoMonsterToggle.ToggleColumn.TYPE);
					transform4.SetParent(transform3, false);
					transform4.GetComponent<Toggle>().group = transform3.GetComponent<ToggleGroup>();
					transform4.GetComponent<Toggle>().onValueChanged.AddListener(transform4.GetComponent<MonoMonsterToggle>().OnToggleValueChanged);
				}
				break;
			}
			}
		}

		public void OnMonsterCategoryToggleValueChanged(Toggle toggle)
		{
			string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
			_selectedMonsterCategory = text;
			ClearMonsterCol(MonsterColType.NAME);
			ClearMonsterCol(MonsterColType.TYPE);
			BuildMonsterColData(MonsterColType.NAME);
			BuildMonsterColItem(MonsterColType.NAME);
		}

		public void OnMonsterNameToggleValueChanged(Toggle toggle)
		{
			string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
			_selectedMonsterName = text;
			ClearMonsterCol(MonsterColType.TYPE);
			BuildMonsterColData(MonsterColType.TYPE);
			BuildMonsterColItem(MonsterColType.TYPE);
		}

		public void OnMonsterTypeToggleValueChanged(Toggle toggle)
		{
			string text = toggle.gameObject.transform.Find("Label").GetComponent<Text>().text;
			_selectedMonsterType = text;
		}

		public void OnMonsterStationaryToggleValueChanged(bool isOn)
		{
			isStationary = isOn;
			if (isOn)
			{
				if (Singleton<MonsterManager>.Instance.GetAllMonsters().Count <= 0)
				{
					return;
				}
				{
					foreach (BaseMonoMonster allMonster in Singleton<MonsterManager>.Instance.GetAllMonsters())
					{
						MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(allMonster.GetRuntimeID());
						actor.baseMaxHP = (actor.HP = (actor.maxHP = float.MaxValue));
						BehaviorTree component = allMonster.GetComponent<BehaviorTree>();
						if (component != null)
						{
							component.enabled = false;
						}
					}
					return;
				}
			}
			foreach (BaseMonoMonster allMonster2 in Singleton<MonsterManager>.Instance.GetAllMonsters())
			{
				MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(allMonster2.GetRuntimeID());
				NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(monsterLevel);
				actor2.baseMaxHP = (actor2.maxHP = (actor2.HP = actor2.config.CommonArguments.HP * nPCLevelMetaDataByKey.HPRatio));
				actor2.defense = actor2.config.CommonArguments.Defence * nPCLevelMetaDataByKey.DEFRatio;
				actor2.attack = actor2.config.CommonArguments.Attack * nPCLevelMetaDataByKey.ATKRatio;
				actor2.PushProperty("Actor_ResistAllElementAttackRatio", nPCLevelMetaDataByKey.ElementalResistRatio);
				if (isElite)
				{
					actor2.baseMaxHP = (actor2.maxHP = (actor2.HP = (float)actor2.maxHP * actor2.config.EliteArguments.HPRatio));
					actor2.defense = (float)actor2.defense * actor2.config.EliteArguments.DefenseRatio;
					actor2.attack = (float)actor2.attack * actor2.config.EliteArguments.AttackRatio;
				}
				BehaviorTree component2 = allMonster2.GetComponent<BehaviorTree>();
				if (component2 != null)
				{
					component2.enabled = true;
				}
			}
		}

		public void OnAvatarPowerfulToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				foreach (BaseMonoAvatar allAvatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
				{
					AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allAvatar.GetRuntimeID());
					foreach (AvatarActor.SKillInfo skillInfo2 in actor.skillInfoList)
					{
						skillInfo2.CD = 0f;
					}
					avatarMaxHPDict[allAvatar.GetRuntimeID()] = actor.maxHP;
					avatarMaxSPDict[allAvatar.GetRuntimeID()] = actor.maxSP;
					actor.maxHP = 999999f;
					actor.maxSP = 999999f;
					DelegateUtils.UpdateField(ref actor.HP, 999999f, 0f, actor.onHPChanged);
					DelegateUtils.UpdateField(ref actor.SP, 999999f, 0f, actor.onSPChanged);
				}
				Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
				return;
			}
			foreach (BaseMonoAvatar allAvatar2 in Singleton<AvatarManager>.Instance.GetAllAvatars())
			{
				uint runtimeID = allAvatar2.GetRuntimeID();
				AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
				foreach (string key in actor2.config.Skills.Keys)
				{
					ConfigAvatarSkill configAvatarSkill = actor2.config.Skills[key];
					string skillNameByAnimEventID = actor2.GetSkillNameByAnimEventID(key);
					AvatarActor.SKillInfo skillInfo = actor2.GetSkillInfo(skillNameByAnimEventID);
					if (skillInfo != null)
					{
						skillInfo.CD = Mathf.Max(0f, configAvatarSkill.SkillCD + actor2.Evaluate(configAvatarSkill.SkillCDDelta));
					}
				}
				if (avatarMaxHPDict.ContainsKey(runtimeID))
				{
					actor2.maxHP = avatarMaxHPDict[runtimeID];
					DelegateUtils.UpdateField(ref actor2.HP, avatarMaxHPDict[runtimeID], 0f, actor2.onHPChanged);
				}
				if (avatarMaxSPDict.ContainsKey(runtimeID))
				{
					actor2.maxSP = avatarMaxSPDict[runtimeID];
					DelegateUtils.UpdateField(ref actor2.SP, avatarMaxSPDict[runtimeID], 0f, actor2.onSPChanged);
				}
			}
		}

		public void DebugRemoveGameLogic()
		{
			StartCoroutine(DestructionIter());
		}

		private IEnumerator DestructionIter()
		{
			yield return new WaitForEndOfFrame();
			Singleton<LevelManager>.Instance.levelEntity.StopAllCoroutines();
			Singleton<EventManager>.Instance.DropEventsAndStop();
			Singleton<LevelDesignManager>.Instance.StopLevelDesign();
			yield return null;
			Rigidbody[] array = Object.FindObjectsOfType<Rigidbody>();
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.velocity = Vector3.zero;
			}
			Object.Destroy(Object.FindObjectOfType<MonoInLevelUICanvas>().gameObject);
			DestroyAll<BehaviorTree>();
			DestroyAll<BaseMonoEffectPlugin>();
			DestroyAll<BaseMonoEntity>();
			DestroyAll<Animator>();
			DestroyAll<MonoAuxObject>();
			DestroyAll<MonoTheLevelV1>();
		}

		private void DestroyAll<T>() where T : Component
		{
			T[] array = Object.FindObjectsOfType<T>();
			foreach (T obj in array)
			{
				Object.Destroy(obj);
			}
		}

		public void TogglePostFX()
		{
			PostFX postFX = Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.enabled = !postFX.enabled;
			}
		}

		public void ToggleBenchmark(bool isOn)
		{
			MonoBenchmarkSwitches monoBenchmarkSwitches = Object.FindObjectOfType<MonoBenchmarkSwitches>();
			if (monoBenchmarkSwitches == null)
			{
				if (isOn)
				{
					new GameObject("__benchmark", typeof(MonoBenchmarkSwitches));
				}
			}
			else
			{
				monoBenchmarkSwitches.gameObject.SetActive(isOn);
			}
		}

		public void ToggleFXAA(bool isOn)
		{
			PostFX postFX = Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.FXAA = isOn;
			}
		}

		public void ToggleDistortion(bool isOn)
		{
			PostFX postFX = Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.UseDistortion = isOn;
			}
		}

		public void ToggleDistortionDepth(bool isOn)
		{
			PostFX postFX = Object.FindObjectOfType<PostFX>();
			if (postFX != null)
			{
				postFX.UseDepthTest = isOn;
			}
		}

		public void Toggle60Frames(bool is60)
		{
			if (is60)
			{
				Application.targetFrameRate = 60;
			}
			else
			{
				Application.targetFrameRate = 30;
			}
		}

		public void ToggleAlwaysLastKill(bool isOn)
		{
			MainCameraActor actor = Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID());
			if (isOn)
			{
				actor.SetupLastKillCloseUp();
				CameraActorLastKillCloseUpPlugin plugin = actor.GetPlugin<CameraActorLastKillCloseUpPlugin>();
				plugin.AlwaysTrigger = true;
			}
			else if (actor.HasPlugin<CameraActorLastKillCloseUpPlugin>())
			{
				actor.RemovePlugin<CameraActorLastKillCloseUpPlugin>();
			}
		}

		public void OnEndLevel()
		{
			Singleton<LevelScoreManager>.Instance.useDebugFunction = true;
			base.transform.gameObject.SetActive(false);
			Singleton<LevelManager>.Instance.SetPause(true);
			Singleton<LevelManager>.Instance.levelActor.SuddenLevelEnd();
			Singleton<MainUIManager>.Instance.ShowPage(new LevelEndPageContext(EvtLevelState.LevelEndReason.EndWin, true));
			Singleton<WwiseAudioManager>.Instance.SetSwitch("Level_Result", "Win");
			Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off");
			Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_End");
		}
	}
}
