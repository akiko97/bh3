using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonsterShowPageContext : BasePageContext
	{
		private const string MONSTER_SKILL_PREFAB_PATH = "UI/Menus/Widget/Map/MonsterSkillRow";

		private List<int> _monsterIDList;

		private Dictionary<int, MonsterUIMetaData> _monsterDataDict;

		private Dictionary<int, GameObject> _monsterGameObjectDict;

		private int _currentMonsterIndex;

		public MonsterShowPageContext(List<int> monsterIDList)
		{
			config = new ContextPattern
			{
				contextName = "MonsterShowPageContext",
				viewPrefabPath = "UI/Menus/Page/Map/MonsterShowPage"
			};
			_monsterIDList = monsterIDList;
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("FuncBtn/LastPage").GetComponent<Button>(), OnLastPageButtonClick);
			BindViewCallback(base.view.transform.Find("FuncBtn/NextPage").GetComponent<Button>(), OnNextPageButtonClick);
			BindViewCallback(base.view.transform.Find("BackBtn").GetComponent<Button>(), OnBackBtnClick);
		}

		protected override bool SetupView()
		{
			Init();
			return false;
		}

		public void OnBackBtnClick()
		{
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public void OnLastPageButtonClick()
		{
			_currentMonsterIndex--;
			_currentMonsterIndex += _monsterIDList.Count;
			_currentMonsterIndex %= _monsterIDList.Count;
			ShowMonsterByIndex(_monsterIDList[_currentMonsterIndex]);
		}

		public void OnNextPageButtonClick()
		{
			_currentMonsterIndex++;
			_currentMonsterIndex %= _monsterIDList.Count;
			ShowMonsterByIndex(_monsterIDList[_currentMonsterIndex]);
		}

		private void Init()
		{
			if (_monsterIDList.Count <= 0)
			{
				return;
			}
			_monsterDataDict = new Dictionary<int, MonsterUIMetaData>();
			_monsterGameObjectDict = new Dictionary<int, GameObject>();
			foreach (int monsterID in _monsterIDList)
			{
				MonsterUIMetaData monsterUIMetaDataByKey = MonsterUIMetaDataReader.GetMonsterUIMetaDataByKey(monsterID);
				string prefabPath = monsterUIMetaDataByKey.prefabPath;
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>(prefabPath));
				gameObject.transform.SetParent(base.view.transform.Find("Monster3dModel"), false);
				gameObject.SetActive(false);
				_monsterDataDict.Add(monsterID, monsterUIMetaDataByKey);
				_monsterGameObjectDict.Add(monsterID, gameObject);
			}
			_currentMonsterIndex = 0;
			ShowMonsterByIndex(_monsterIDList[_currentMonsterIndex]);
		}

		private void ShowMonsterByIndex(int index)
		{
			MonsterUIMetaData value;
			_monsterDataDict.TryGetValue(index, out value);
			if (value == null)
			{
				return;
			}
			foreach (KeyValuePair<int, GameObject> item in _monsterGameObjectDict)
			{
				item.Value.SetActive(item.Key == index);
			}
			base.view.transform.Find("Info/Name/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(value.displayTitle);
			base.view.transform.Find("Info/Type/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(value.displayType);
			base.view.transform.Find("Desc/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(value.displayIntroduction);
			base.view.transform.Find("Info/HP/Slider").GetComponent<Slider>().value = value.HP;
			base.view.transform.Find("Info/ATK/Slider").GetComponent<Slider>().value = value.attack;
			base.view.transform.Find("Info/DEF/Slider").GetComponent<Slider>().value = value.defence;
			base.view.transform.Find("Info/SPD/Slider").GetComponent<Slider>().value = value.speed;
			base.view.transform.Find("Info/RND/Slider").GetComponent<Slider>().value = value.range;
			Transform transform = base.view.transform.Find("Skill/SkillListPanel");
			foreach (Transform item2 in transform)
			{
				Object.Destroy(item2.gameObject);
			}
			foreach (int monsterSkillID in value.monsterSkillIDList)
			{
				MonsterSkillMetaData monsterSkillMetaDataByKey = MonsterSkillMetaDataReader.GetMonsterSkillMetaDataByKey(monsterSkillID);
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/MonsterSkillRow"));
				gameObject.transform.SetParent(transform, false);
				gameObject.transform.Find("SkillName/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(monsterSkillMetaDataByKey.displayName);
				gameObject.transform.Find("SkillDescription/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(monsterSkillMetaDataByKey.displayDetail);
			}
		}
	}
}
