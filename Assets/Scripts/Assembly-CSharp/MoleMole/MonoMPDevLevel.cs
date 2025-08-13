using System.Collections;
using System.Linq;
using FlatBuffers;
using MoleMole.MPProtocol;
using UnityEngine;

namespace MoleMole
{
	public class MonoMPDevLevel : MonoMPLevelV1
	{
		private const int MAX_PLAYER_COUNT = 4;

		public bool usePresetData;

		public MPStageData stageData;

		public MPAvatarDataItem[] avatarDataLs;

		public MPMode mpMode;

		private bool _established;

		private MonoDebugMP _debugMP;

		private GUIStyle _style;

		private MPMode _mpMode;

		private int _playerCount;

		private Popup _stagePop;

		private Popup[] _avatarPops;

		private Popup _modePop;

		private GUIContent[] _whiteListAvatars;

		private GUIContent[] _whiteListStages;

		private GUIContent[] _mpModes;

		private new void Awake()
		{
			Screen.sleepTimeout = -1;
			GlobalVars.DISABLE_NETWORK_DEBUG = true;
			MainUIData.USE_VIEW_CACHING = false;
			GeneralLogicManager.InitAll();
			GlobalDataManager.Refresh();
			Singleton<LevelScoreManager>.Create();
			Singleton<LevelScoreManager>.Instance.luaFile = "Lua/Levels/Common/Level 0.lua";
			Object.FindObjectOfType<MonoDebugMP>().onPeerReady = OnPeerReady;
			base.Awake();
			Singleton<LevelManager>.Instance.levelActor.AddPlugin(new MPDevLevelActorPlugin(this));
			_established = false;
			InitGUI();
		}

		private void OnPeerReady(MPPeer peer)
		{
			_established = true;
			if (!usePresetData)
			{
				SyncSelectionToData();
			}
			Singleton<MPManager>.Instance.Setup(peer);
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true);
			LevelIdentity identity = Singleton<MPManager>.Instance.GetIdentity<LevelIdentity>(562036737u);
			if (identity.isAuthority)
			{
				MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_CreateStageFullData>();
				Offset<MoleMole.MPProtocol.MPStageData> stageDataOffset = MPMappings.Serialize(pc.builder, stageData);
				Offset<MoleMole.MPProtocol.MPAvatarDataItem>[] array = new Offset<MoleMole.MPProtocol.MPAvatarDataItem>[avatarDataLs.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = MPMappings.Serialize(pc.builder, avatarDataLs[i]);
				}
				VectorOffset avatarsOffset = Packet_Level_CreateStageFullData.CreateAvatarsVector(pc.builder, array);
				Packet_Level_CreateStageFullData.StartPacket_Level_CreateStageFullData(pc.builder);
				Packet_Level_CreateStageFullData.AddStageData(pc.builder, stageDataOffset);
				Packet_Level_CreateStageFullData.AddAvatars(pc.builder, avatarsOffset);
				Packet_Level_CreateStageFullData.AddMpMode(pc.builder, mpMode);
				pc.Finish(Packet_Level_CreateStageFullData.EndPacket_Level_CreateStageFullData(pc.builder));
				identity.DebugCreateStageWithFullDataSync(pc);
			}
		}

		private new void Start()
		{
			Singleton<LevelManager>.Instance.InitAtStart();
			StartCoroutine(EndOfFrameHideUI());
		}

		private IEnumerator EndOfFrameHideUI()
		{
			yield return new WaitForEndOfFrame();
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false);
		}

		public new void OnDestroy()
		{
			Singleton<LevelManager>.Instance.Destroy();
			Singleton<LevelManager>.Destroy();
		}

		private void InitGUI()
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
			_whiteListAvatars = new string[3] { "Kiana_C2_PT", "Kiana_C1_FX", "Mei_C2_CK" }.Select((string x) => new GUIContent(x)).ToArray();
			_whiteListStages = new string[1] { "St_Freyja_05" }.Select((string x) => new GUIContent(x)).ToArray();
			_mpModes = new GUIContent[3]
			{
				new GUIContent(MPMode.Normal.ToString()),
				new GUIContent(MPMode.PvP_SendNoReceive.ToString()),
				new GUIContent(MPMode.PvP_ReceiveNoSend.ToString())
			};
			_stagePop = new Popup();
			_avatarPops = new Popup[4];
			for (int num = 0; num < 4; num++)
			{
				_avatarPops[num] = new Popup();
			}
			_modePop = new Popup();
			_debugMP = Object.FindObjectOfType<MonoDebugMP>();
			_playerCount = _debugMP.WaitForPlayerCount;
		}

		private void OnGUI()
		{
			if (!_established)
			{
				PreEstablishedGUIUpdate();
			}
		}

		private void PreEstablishedGUIUpdate()
		{
			if (usePresetData)
			{
				if (GUI.Button(new Rect(50f, 50f, 200f, 100f), "Override Preset Data"))
				{
					usePresetData = false;
				}
				return;
			}
			GUILayout.BeginArea(new Rect(50f, 50f, 300f, 400f));
			if (GUILayout.Button("Player Count: " + _playerCount))
			{
				int num = Mathf.Clamp((_playerCount + 1) % 4, 2, 4);
				if (num != _playerCount)
				{
					_debugMP.WaitForPlayerCount = num;
				}
				_playerCount = num;
			}
			_stagePop.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _whiteListStages, _style, _style);
			for (int i = 0; i < _playerCount; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("p" + (i + 1), GUILayout.ExpandWidth(false));
				_avatarPops[i].List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _whiteListAvatars, _style, _style);
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Mode", GUILayout.ExpandWidth(false));
			_modePop.List(GUILayoutUtility.GetRect(20f, _style.lineHeight * 1.5f), _mpModes, _style, _style);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void SyncSelectionToData()
		{
			mpMode = (MPMode)_modePop.GetSelectedItemIndex();
			stageData = new MPStageData();
			stageData.stageName = _whiteListStages[_stagePop.GetSelectedItemIndex()].text;
			avatarDataLs = new MPAvatarDataItem[_playerCount];
			for (int i = 0; i < _playerCount; i++)
			{
				AvatarMetaData avatarMetaDataByRegistryKey = Singleton<AvatarModule>.Instance.GetAvatarMetaDataByRegistryKey(_whiteListAvatars[_avatarPops[i].GetSelectedItemIndex()].text);
				MPAvatarDataItem mPAvatarDataItem = new MPAvatarDataItem();
				avatarDataLs[i] = mPAvatarDataItem;
				mPAvatarDataItem.avatarID = avatarMetaDataByRegistryKey.avatarID;
				mPAvatarDataItem.level = 10;
			}
		}
	}
}
