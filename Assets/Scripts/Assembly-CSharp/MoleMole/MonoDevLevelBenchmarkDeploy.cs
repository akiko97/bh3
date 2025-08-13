using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoleMole
{
	public class MonoDevLevelBenchmarkDeploy : MonoBehaviour
	{
		private const string AI_PREFIX = "AI/Avatar/";

		private GUIStyle _style;

		private Popup _widgetAvatarType = new Popup();

		private GUIContent[] _avatarTypes;

		private Popup _widgetAIType = new Popup();

		private GUIContent[] _aiTypes;

		private Popup _widgetMonsterNames = new Popup();

		private GUIContent[] _monsterNames;

		private Popup _widgetMonsterTypes = new Popup();

		private GUIContent[][] _monsterTypes;

		private Popup _widgetStages = new Popup();

		private GUIContent[] _stages;

		private Popup _widgetLevels = new Popup();

		private GUIContent[] _levels;

		private bool _multiMode;

		private void Awake()
		{
			_style = new GUIStyle();
			_style.normal.textColor = Color.white;
			_style.hover.textColor = Color.white;
			_style.focused.textColor = Color.gray;
			_style.active.textColor = Color.white;
			_style.onNormal.textColor = Color.white;
			_style.onHover.textColor = Color.white;
			_style.onFocused.textColor = Color.gray;
			_style.onActive.textColor = Color.white;
		}

		private void Start()
		{
			MainUIData.USE_VIEW_CACHING = false;
			GeneralLogicManager.InitAll();
			GlobalDataManager.Refresh();
			_avatarTypes = AvatarData.GetAllAvatarData().Keys.Select((string x) => new GUIContent(x)).ToArray();
			_aiTypes = new string[6] { "AvatarAutoBattleBehavior_Attack", "AvatarAutoBattleBehavior_AlwaysSkill", "AvatarAutoBattleBehavior_BranchAttack_Kiana_ATK02", "AvatarAutoBattleBehavior_BranchAttack_Kiana_ATK03", "AvatarAutoBattleBehavior_BranchAttack_Mei_ATK01", "AvatarAutoBattleBehavior_Empty" }.Select((string x) => new GUIContent(x)).ToArray();
			_monsterNames = (from x in MonsterData.GetAllMonsterConfigMetaData()
				group x by x.monsterName into g
				select g.First() into x
				select new GUIContent(x.monsterName)).ToArray();
			_monsterTypes = new GUIContent[_monsterNames.Length][];
			int ix;
			for (ix = 0; ix < _monsterTypes.Length; ix++)
			{
				_monsterTypes[ix] = (from x in MonsterData.GetAllMonsterConfigMetaData()
					where x.monsterName == _monsterNames[ix].text
					select new GUIContent(x.typeName)).ToArray();
			}
			_stages = StageData.GetAllStageEntries().Keys.Select((string x) => new GUIContent(x)).ToArray();
			_levels = new string[3] { "Common/Level 0.lua", "Common/Level Keith.lua", "Common/Level Benchmark Baseline.lua" }.Select((string x) => new GUIContent(x)).ToArray();
			DevLevelConfigData.avatarDevDatas.Clear();
			DevLevelConfigData.monsterDevDatas.Clear();
			DevLevelConfigData.stageDevData = new DevStageData();
		}

		private void Update()
		{
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(50f, 0f, Screen.width - 100, Screen.height));
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Avatar : ", GUILayout.ExpandWidth(false));
			_widgetAvatarType.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _avatarTypes, _style, _style);
			GUILayout.Label("AI : ", GUILayout.ExpandWidth(false));
			_widgetAIType.List(GUILayoutUtility.GetRect(50f, _style.lineHeight * 1.5f), _aiTypes, _style, _style);
			if (GUILayout.Button("Add", GUILayout.ExpandWidth(false), GUILayout.Width(100f)) && DevLevelConfigData.avatarDevDatas.Count < 3)
			{
				string text = _avatarTypes[_widgetAvatarType.GetSelectedItemIndex()].text;
				DevLevelConfigData.avatarDevDatas.Add(new DevAvatarData
				{
					avatarType = text,
					avatarTestSkills = new string[2] { "Test_UnlockAllAniSkill", "Test_Undamagable" },
					avatarAI = "AI/Avatar/" + _aiTypes[_widgetAIType.GetSelectedItemIndex()].text,
					avatarWeapon = WeaponData.GetFirstWeaponIDForRole(AvatarData.GetAvatarConfig(text).CommonArguments.RoleName),
					avatarLevel = 1,
					avatarWeaponLevel = 1,
					avatarStigmata = new int[3] { -1, -1, -1 }
				});
			}
			GUILayout.EndHorizontal();
			for (int i = 0; i < DevLevelConfigData.avatarDevDatas.Count; i++)
			{
				DevAvatarData devAvatarData = DevLevelConfigData.avatarDevDatas[i];
				GUILayout.Label(string.Format("{0}: {1}", devAvatarData.avatarType, devAvatarData.avatarAI));
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Monster: ", GUILayout.ExpandWidth(false));
			int num = _widgetMonsterNames.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _monsterNames, _style, _style);
			GUILayout.Label("Type: ", GUILayout.ExpandWidth(false));
			_widgetMonsterTypes.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _monsterTypes[num], _style, _style);
			if (GUILayout.Button("Add", GUILayout.ExpandWidth(false), GUILayout.Width(100f)))
			{
				DevLevelConfigData.monsterDevDatas.Add(new DevMonsterData
				{
					monsterName = _monsterNames[_widgetMonsterNames.GetSelectedItemIndex()].text,
					typeName = _monsterTypes[_widgetMonsterNames.GetSelectedItemIndex()][_widgetMonsterTypes.GetSelectedItemIndex()].text,
					abilities = new string[1] { "Test_Undamagable" },
					level = 1
				});
			}
			GUILayout.EndHorizontal();
			for (int j = 0; j < DevLevelConfigData.monsterDevDatas.Count; j++)
			{
				DevMonsterData devMonsterData = DevLevelConfigData.monsterDevDatas[j];
				GUILayout.Label(string.Format("{0}, {1}", devMonsterData.monsterName, devMonsterData.typeName));
			}
			GUILayout.BeginHorizontal();
			DevLevelConfigData.stageDevData.stageName = _stages[_widgetStages.GetSelectedItemIndex()].text;
			GUILayout.Label("Level: ", GUILayout.ExpandWidth(false));
			_widgetLevels.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _levels, _style, _style);
			DevLevelConfigData.LEVEL_PATH = "Lua/Levels/" + _levels[_widgetLevels.GetSelectedItemIndex()].text;
			_multiMode = GUILayout.Toggle(_multiMode, "Multi Mode?", GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			_widgetStages.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _stages, _style, _style, 3);
			GUILayout.Space(50f);
			if (GUILayout.Button("Start", GUILayout.Height(_style.lineHeight * 3f)))
			{
				LoadDevLevel();
			}
			if (GUILayout.Button("Render Scene", GUILayout.Height(_style.lineHeight * 3f)))
			{
				SceneManager.LoadScene("RenderScene");
			}
			GUILayout.EndArea();
		}

		private void LoadDevLevel()
		{
			DevLevelConfigData.LEVEL_MODE = (_multiMode ? LevelActor.Mode.Multi : LevelActor.Mode.Single);
			DevLevelConfigData.configFromScene = true;
			DevLevelConfigData.isBenchmark = true;
			SceneManager.LoadScene("DevLevel");
		}
	}
}
