using UnityEngine;

namespace MoleMole
{
	public class GalTouchTestDrive : MonoBehaviour
	{
		public Transform camPosTrans0;

		public Transform camPosTrans1;

		public Transform camFocTrans0;

		public Transform camFocTrans1;

		public float camFactor;

		public string[] avatarNames = new string[12]
		{
			"Kiana_C1", "Kiana_C2", "Kiana_C3", "Kiana_C4", "Mei_C1", "Mei_C2", "Mei_C3", "Mei_C4", "Bronya_C1", "Bronya_C2",
			"Bronya_C3", "Bronya_C4"
		};

		public int[] avatarIds = new int[12]
		{
			101, 102, 103, 104, 201, 202, 203, 204, 301, 302,
			303, 304
		};

		public GameObject[] avatarObjects;

		private int _currentAvatarIndex;

		private GalTouchTestAvatar _curAvatar;

		private void Awake()
		{
			GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
			TouchPatternData.ReloadFromFile();
		}

		private void Start()
		{
			InitWwise();
			SwitchAvatarGameObject();
		}

		private void SwitchAvatarGameObject()
		{
			if (_currentAvatarIndex < avatarObjects.Length)
			{
				int i = 0;
				for (int num = avatarObjects.Length; i < num; i++)
				{
					avatarObjects[i].SetActive(i == _currentAvatarIndex);
				}
				_curAvatar = avatarObjects[_currentAvatarIndex].GetComponent<GalTouchTestAvatar>();
			}
		}

		private void Update()
		{
			Vector3 position = Vector3.Lerp(camPosTrans0.position, camPosTrans1.position, camFactor);
			Vector3 worldPosition = Vector3.Lerp(camFocTrans0.position, camFocTrans1.position, camFactor);
			Camera.main.transform.position = position;
			Camera.main.transform.LookAt(worldPosition);
		}

		private void OnGUI()
		{
			camFactor = GUILayout.HorizontalScrollbar(camFactor, 0.01f, 0f, 1f);
			OnAvatarSelectGUI();
			OnAvatarSettingGUI();
		}

		private void OnAvatarSelectGUI()
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("<", GUILayout.Width(30f), GUILayout.Height(30f)))
			{
				_currentAvatarIndex = Mathf.Clamp(_currentAvatarIndex - 1, 0, avatarNames.Length - 1);
				SwitchAvatarGameObject();
			}
			GUILayout.Label(avatarNames[_currentAvatarIndex], GUILayout.Width(100f), GUILayout.Height(30f));
			if (GUILayout.Button(">", GUILayout.Width(30f), GUILayout.Height(30f)))
			{
				_currentAvatarIndex = Mathf.Clamp(_currentAvatarIndex + 1, 0, avatarNames.Length - 1);
				SwitchAvatarGameObject();
			}
			GUILayout.EndHorizontal();
		}

		private void OnAvatarSettingGUI()
		{
			if (!(_curAvatar == null))
			{
				GUILayout.Label(string.Format("AvatarID : {0}", avatarIds[_currentAvatarIndex].ToString()));
				GUILayout.BeginHorizontal();
				GUILayout.Label("HeartLevel : ");
				GUILayout.TextField(_curAvatar.heartLevel.ToString());
				if (GUILayout.Button("-"))
				{
					_curAvatar.heartLevel = Mathf.Clamp(_curAvatar.heartLevel - 1, 1, 4);
					_curAvatar.ResetGalTouchSystem();
				}
				if (GUILayout.Button("+"))
				{
					_curAvatar.heartLevel = Mathf.Clamp(_curAvatar.heartLevel + 1, 1, 4);
					_curAvatar.ResetGalTouchSystem();
				}
				GUILayout.EndHorizontal();
			}
		}

		private void InitWwise()
		{
			Singleton<WwiseAudioManager>.Create();
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[1] { "BK_MainMenu" });
		}
	}
}
