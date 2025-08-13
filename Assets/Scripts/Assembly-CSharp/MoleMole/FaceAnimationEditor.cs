using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class FaceAnimationEditor : MonoBehaviour
	{
		public enum FacePartType
		{
			LeftEye = 0,
			RightEye = 1,
			Mouth = 2
		}

		public delegate void ChangedHandler(bool changed);

		public Transform camPosTrans0;

		public Transform camPosTrans1;

		public Transform camFocTrans0;

		public Transform camFocTrans1;

		public float camFactor;

		public float rotation;

		public float angleOfPitch;

		public float heightOffset;

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

		public TestMatInfoProvider kianaImageMap_L;

		public TestMatInfoProvider meiImageMap_L;

		public TestMatInfoProvider bronyaImageMap_L;

		public TestMatInfoProvider kianaImageMap_R;

		public TestMatInfoProvider meiImageMap_R;

		public TestMatInfoProvider bronyaImageMap_R;

		public TestMatInfoProvider kianaImageMap_M;

		public TestMatInfoProvider meiImageMap_M;

		public TestMatInfoProvider bronyaImageMap_M;

		public GameObject[] avatarObjects;

		private FaceAnimationEditAvatar _curAvatar;

		private int _currentAvatarIndex;

		public ConfigFaceAnimation _kianaConfig;

		public ConfigFaceAnimation _meiConfig;

		public ConfigFaceAnimation _bronyaConfig;

		private ConfigFaceAnimation _currentConfig;

		private int _currentFaceAnimationItemIndex = -1;

		private FaceAnimationItem _currentFaceAnimationItem;

		public float faceAnimationPanelHeight = 100f;

		public float selectPanelWidth = 100f;

		public float blockEditPanelWidth = 100f;

		public float camFactorViewHeight = 100f;

		public float timePerFrameMin = 0.001f;

		public float timePerFrameMax = 5f;

		public float frameWidth = 30f;

		public float frameButtonFactor = 0.8f;

		public Material kianaLeftEyeMaterial;

		public Material kianaRightEyeMaterial;

		public Material kianaMouthMaterial;

		public Material meiLeftEyeMaterial;

		public Material meiRightEyeMaterial;

		public Material meiMouthMaterial;

		public Material bronyaLeftEyeMaterial;

		public Material bronyaRightEyeMaterial;

		public Material bronyaMouthMaterial;

		public string kianaLeftEyeOriginName;

		public string kianaRightEyeOriginName;

		public string kianaMouthOriginName;

		public string meiLeftEyeOriginName;

		public string meiRightEyeOriginName;

		public string meiMouthOriginName;

		public string bronyaLeftEyeOriginName;

		public string bronyaRightEyeOriginName;

		public string bronyaMouthOriginName;

		public AtlasMatInfoProvider kianaEyeAtlas;

		public AtlasMatInfoProvider kianaMouthAtlas;

		public AtlasMatInfoProvider meiEyeAtlas;

		public AtlasMatInfoProvider meiMouthAtlas;

		public AtlasMatInfoProvider bronyaEyeAtlas;

		public AtlasMatInfoProvider bronyaMouthAtlas;

		private FacePartType _currentPart;

		private float _normalizedTime;

		private bool _playing;

		private Animator _animator;

		private Vector2 _faceAnimationListScrollPos = Vector2.zero;

		private Vector2 _blockEditViewScrollPos = Vector2.zero;

		private Vector2 _clipboardScrollPos = Vector2.zero;

		private Vector2 _contentViewScrollPos = Vector2.zero;

		private bool _useAnimator;

		private string _animatorStateName = string.Empty;

		private int _heartLevel = 1;

		private int _action = 1;

		private int _eyeCopyDirection;

		private int selectIndex;

		private int selectCount;

		private List<FaceAnimationFrameBlock> _clipBoard = new List<FaceAnimationFrameBlock>();

		private bool _changed;

		public bool running { get; set; }

		public bool openned { get; set; }

		public string currentFilePath { get; set; }

		public bool changed
		{
			get
			{
				return _changed;
			}
			set
			{
				bool flag = _changed;
				_changed = value;
				if (flag != _changed && this.Changed != null)
				{
					this.Changed(_changed);
				}
			}
		}

		public event ChangedHandler Changed;

		private void Awake()
		{
			GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
			TouchPatternData.ReloadFromFile();
			running = true;
			InitWwise();
		}

		private void Start()
		{
			Init();
		}

		public void Init()
		{
			InitConfigs();
			InitAvatars();
			SwitchAvatarGameObject();
			heightOffset = base.transform.position.y;
		}

		public void Refresh()
		{
			InitAvatars();
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
				_curAvatar = avatarObjects[_currentAvatarIndex].GetComponent<FaceAnimationEditAvatar>();
				_currentConfig = GetAvatarConfigById(_curAvatar.avatarId);
				if (_currentConfig != null && _currentConfig.items.Length > 0)
				{
					_currentFaceAnimationItemIndex = 0;
				}
				else
				{
					_currentFaceAnimationItemIndex = -1;
				}
				if (_currentFaceAnimationItemIndex >= 0)
				{
					_curAvatar.PrepareFaceAnimation(_currentConfig.items[_currentFaceAnimationItemIndex].name);
				}
				_animator = _curAvatar.gameObject.GetComponent<Animator>();
				_curAvatar.RebuildFaceAnimation();
			}
		}

		private void Update()
		{
			Vector3 position = Vector3.Lerp(camPosTrans0.position, camPosTrans1.position, camFactor);
			Vector3 worldPosition = Vector3.Lerp(camFocTrans0.position, camFocTrans1.position, camFactor);
			Camera.main.transform.position = position;
			Camera.main.transform.LookAt(worldPosition);
			if (_curAvatar == null || _currentConfig == null || _currentFaceAnimationItemIndex < 0)
			{
				return;
			}
			float num = _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame * (float)_currentConfig.items[_currentFaceAnimationItemIndex].length;
			float num2 = _normalizedTime * num;
			if (_playing)
			{
				num2 += Time.deltaTime;
				if (num > 0f && num2 > num)
				{
					if (_useAnimator)
					{
						if (_animator == null || _animator.GetCurrentAnimatorStateInfo(0).IsName("StandBy"))
						{
							_playing = false;
						}
					}
					else
					{
						_playing = false;
					}
				}
				_normalizedTime = ((!_playing) ? 0f : (num2 / num));
			}
			if (_curAvatar != null)
			{
				_curAvatar.SetAnimationTime(num2);
			}
			UpdateHotkey();
		}

		private void OnGUI()
		{
			DrawCamFactor();
			DrawAnimationSelectGUI();
			if (_currentFaceAnimationItemIndex >= 0 && _currentFaceAnimationItemIndex < _currentConfig.items.Length)
			{
				GUI.enabled = openned;
				DrawAnimationPanelGUI();
				DrawAnimationBlocksView();
				GUI.enabled = true;
			}
		}

		private void DrawCamFactor()
		{
			Color backgroundColor = GUI.backgroundColor;
			Color color = GUI.color;
			GUI.backgroundColor = Color.cyan;
			Rect rect = new Rect(selectPanelWidth + 10f, 5f, (float)Screen.width - selectPanelWidth - blockEditPanelWidth - 15f, camFactorViewHeight);
			GUI.Box(rect, string.Empty);
			GUILayout.BeginArea(rect);
			if (!openned)
			{
				Color color2 = GUI.color;
				GUI.color = Color.red;
				GUILayout.Label("View Mode");
				GUI.color = color2;
			}
			else
			{
				GUILayout.Label(string.Format("File : {0}", currentFilePath));
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("Distance:({0})", ((int)(camFactor * 100f)).ToString(), GUILayout.Width(60f)));
			camFactor = GUILayout.HorizontalSlider(camFactor, 0f, 1f);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("Rotation:"));
			rotation = GUILayout.HorizontalSlider(rotation, -100f, 100f);
			base.transform.rotation = Quaternion.Euler(angleOfPitch, rotation, 0f);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("Heigth:"));
			heightOffset = GUILayout.HorizontalSlider(heightOffset, 1f, 2f);
			base.transform.localPosition = new Vector3(0f, heightOffset, 0f);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}

		private void DrawAnimationSelectGUI()
		{
			Color backgroundColor = GUI.backgroundColor;
			Color color = GUI.color;
			GUI.backgroundColor = Color.cyan;
			Rect rect = new Rect(5f, 5f, selectPanelWidth, (float)Screen.height - 15f - faceAnimationPanelHeight);
			GUI.Box(rect, string.Empty);
			GUILayout.BeginArea(rect);
			int currentAvatarIndex = _currentAvatarIndex;
			_currentAvatarIndex = DrawSwitchView(avatarNames, _currentAvatarIndex);
			if (currentAvatarIndex != _currentAvatarIndex)
			{
				SwitchAvatarGameObject();
			}
			if (_playing)
			{
				if (GUILayout.Button("Stop") && _currentFaceAnimationItemIndex >= 0)
				{
					if (_animator != null)
					{
						_animator.Play("StandBy");
					}
					_playing = false;
					_normalizedTime = 0f;
					Singleton<WwiseAudioManager>.Instance.StopAll();
				}
			}
			else if (GUILayout.Button("Play") && _currentFaceAnimationItemIndex >= 0 && (float)_currentConfig.items[_currentFaceAnimationItemIndex].length * _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame > 0f)
			{
				_playing = true;
				_normalizedTime = 0f;
				if (_useAnimator && _animator != null && !string.IsNullOrEmpty(_animatorStateName))
				{
					_animator.Play(_animatorStateName);
				}
			}
			_useAnimator = GUILayout.Toggle(_useAnimator, "With Animation");
			GUI.enabled = _useAnimator;
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("-"))
			{
				_heartLevel = Mathf.Clamp(_heartLevel - 1, 1, 4);
			}
			GUILayout.TextField(_heartLevel.ToString());
			if (GUILayout.Button("+"))
			{
				_heartLevel = Mathf.Clamp(_heartLevel + 1, 1, 4);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("-"))
			{
				_action = Mathf.Clamp(_action - 1, 1, 9);
			}
			GUILayout.TextField(_action.ToString());
			if (GUILayout.Button("+"))
			{
				_action = Mathf.Clamp(_action + 1, 1, 9);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			string arg = string.Empty;
			if (_curAvatar.avatarId / 100 == 1)
			{
				arg = "Kia";
			}
			else if (_curAvatar.avatarId / 100 == 2)
			{
				arg = "Mei";
			}
			else if (_curAvatar.avatarId / 100 == 3)
			{
				arg = "Bro";
			}
			_animatorStateName = string.Format("Gal_{0}_A_{1}_{2}", arg, _heartLevel.ToString(), _action.ToString());
			GUILayout.TextField(_animatorStateName);
			GUI.enabled = true;
			_faceAnimationListScrollPos = GUILayout.BeginScrollView(_faceAnimationListScrollPos, GUILayout.Height(150f));
			int i = 0;
			for (int num = _currentConfig.items.Length; i < num; i++)
			{
				string text = _currentConfig.items[i].name;
				if (GUILayout.Toggle(i == _currentFaceAnimationItemIndex, text) && i != _currentFaceAnimationItemIndex)
				{
					_currentFaceAnimationItemIndex = i;
					_curAvatar.PrepareFaceAnimation(_currentConfig.items[_currentFaceAnimationItemIndex].name);
					_playing = false;
					_normalizedTime = 0f;
				}
			}
			GUILayout.EndScrollView();
			bool flag = GUI.enabled;
			GUI.enabled = openned;
			if (GUILayout.Button("Add"))
			{
				List<FaceAnimationItem> list = new List<FaceAnimationItem>(_currentConfig.items);
				FaceAnimationItem faceAnimationItem = new FaceAnimationItem();
				faceAnimationItem.length = 1;
				faceAnimationItem.timePerFrame = 0.016f;
				faceAnimationItem.name = string.Format("anim[{0}]", list.Count);
				faceAnimationItem.leftEyeBlocks = new FaceAnimationFrameBlock[0];
				faceAnimationItem.rightEyeBlocks = new FaceAnimationFrameBlock[0];
				faceAnimationItem.mouthBlocks = new FaceAnimationFrameBlock[0];
				list.Add(faceAnimationItem);
				_currentConfig.items = list.ToArray();
				_curAvatar.RebuildFaceAnimation();
				changed = true;
			}
			if (_currentFaceAnimationItemIndex >= 0 && GUILayout.Button("Remove"))
			{
				List<FaceAnimationItem> list2 = new List<FaceAnimationItem>(_currentConfig.items);
				list2.RemoveAt(_currentFaceAnimationItemIndex);
				_currentFaceAnimationItemIndex = ((list2.Count != 0) ? Mathf.Clamp(_currentFaceAnimationItemIndex, 0, list2.Count - 1) : (-1));
				_currentConfig.items = list2.ToArray();
				changed = true;
			}
			if (_currentFaceAnimationItemIndex >= 0 && GUILayout.Button("Insert"))
			{
				List<FaceAnimationItem> list3 = new List<FaceAnimationItem>(_currentConfig.items);
				FaceAnimationItem faceAnimationItem2 = new FaceAnimationItem();
				faceAnimationItem2.length = 1;
				faceAnimationItem2.timePerFrame = 0.016f;
				faceAnimationItem2.name = string.Format("anim[{0}]", _currentFaceAnimationItemIndex);
				faceAnimationItem2.leftEyeBlocks = new FaceAnimationFrameBlock[0];
				faceAnimationItem2.rightEyeBlocks = new FaceAnimationFrameBlock[0];
				faceAnimationItem2.mouthBlocks = new FaceAnimationFrameBlock[0];
				list3.Insert(_currentFaceAnimationItemIndex, faceAnimationItem2);
				_currentConfig.items = list3.ToArray();
				_curAvatar.RebuildFaceAnimation();
				changed = true;
			}
			GUILayout.EndArea();
			GUI.enabled = flag;
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}

		private void DrawAnimationPanelGUI()
		{
			Color backgroundColor = GUI.backgroundColor;
			Color color = GUI.color;
			GUI.backgroundColor = Color.cyan;
			Rect rect = new Rect(5f, (float)Screen.height - faceAnimationPanelHeight - 5f, (float)Screen.width - 10f, faceAnimationPanelHeight);
			GUI.Box(rect, string.Empty);
			if (_currentFaceAnimationItemIndex >= 0)
			{
				GUILayout.BeginArea(rect);
				Rect screenRect = new Rect(5f, 5f, selectPanelWidth, faceAnimationPanelHeight);
				GUILayout.BeginArea(screenRect);
				GUILayout.BeginHorizontal(GUILayout.Width(selectPanelWidth - 10f));
				GUILayout.Label("Name:");
				string text = GUILayout.TextField(_currentConfig.items[_currentFaceAnimationItemIndex].name);
				if (text != _currentConfig.items[_currentFaceAnimationItemIndex].name)
				{
					_currentConfig.items[_currentFaceAnimationItemIndex].name = text;
					_curAvatar.RebuildFaceAnimation();
					_curAvatar.PrepareFaceAnimation(text);
					changed = true;
				}
				GUILayout.EndHorizontal();
				float timePerFrame = _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame;
				_currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame = GUILayout.HorizontalSlider(_currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame, timePerFrameMin, timePerFrameMax, GUILayout.Width(selectPanelWidth - 10f));
				float result = _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame * 60f;
				GUILayout.BeginHorizontal();
				GUILayout.Label(string.Format("RF/AF"));
				string s = GUILayout.TextField(result.ToString());
				float.TryParse(s, out result);
				_currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame = result * 1f / 60f;
				if (timePerFrame != _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame)
				{
					changed = true;
				}
				GUILayout.EndHorizontal();
				_curAvatar.SetAnimationTimePerFrame(_currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame);
				GUILayout.BeginHorizontal(GUILayout.Width(selectPanelWidth - 10f));
				GUILayout.Label("Frames:");
				int length = _currentConfig.items[_currentFaceAnimationItemIndex].length;
				string s2 = GUILayout.TextField(_currentConfig.items[_currentFaceAnimationItemIndex].length.ToString());
				if (int.TryParse(s2, out _currentConfig.items[_currentFaceAnimationItemIndex].length) && length != _currentConfig.items[_currentFaceAnimationItemIndex].length)
				{
					_curAvatar.RebuildFaceAnimation();
					changed = true;
				}
				GUILayout.EndHorizontal();
				if (_eyeCopyDirection == 1)
				{
					GUILayout.Label("L => R Ready");
				}
				else if (_eyeCopyDirection == 2)
				{
					GUILayout.Label("R => L Ready");
				}
				GUILayout.EndArea();
				Rect screenRect2 = new Rect(5f + selectPanelWidth, 5f, (float)Screen.width - selectPanelWidth - 20f - blockEditPanelWidth, faceAnimationPanelHeight);
				GUILayout.BeginArea(screenRect2);
				DrawAnimationContentEditPanel();
				GUILayout.EndArea();
				Rect screenRect3 = new Rect((float)Screen.width - 5f - blockEditPanelWidth, 5f, blockEditPanelWidth, faceAnimationPanelHeight);
				GUILayout.BeginArea(screenRect3);
				_clipboardScrollPos = GUILayout.BeginScrollView(_clipboardScrollPos);
				DrawClipboard();
				GUILayout.EndScrollView();
				GUILayout.EndArea();
				GUILayout.EndArea();
			}
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}

		private void DrawAnimationBlocksView()
		{
			Color backgroundColor = GUI.backgroundColor;
			Color color = GUI.color;
			GUI.backgroundColor = Color.cyan;
			Rect rect = new Rect((float)Screen.width - blockEditPanelWidth, 5f, blockEditPanelWidth - 5f, (float)Screen.height - faceAnimationPanelHeight - 15f);
			GUI.Box(rect, string.Empty);
			GUILayout.BeginArea(rect);
			if (_currentPart == FacePartType.LeftEye)
			{
				GUILayout.Label("<Left Eye>");
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				GUILayout.Label("<Right Eye>");
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				GUILayout.Label("<Mouth>");
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Length");
			GUILayout.Label("Name");
			GUILayout.Label(string.Empty, GUILayout.Width(30f));
			GUILayout.EndHorizontal();
			_blockEditViewScrollPos = GUILayout.BeginScrollView(_blockEditViewScrollPos);
			if (_currentFaceAnimationItemIndex < 0)
			{
				return;
			}
			FaceAnimationFrameBlock[] array = null;
			if (_currentPart == FacePartType.LeftEye)
			{
				array = _currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks;
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				array = _currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks;
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				array = _currentConfig.items[_currentFaceAnimationItemIndex].mouthBlocks;
			}
			if (array != null)
			{
				int num = -1;
				int num2 = 1;
				int i = 0;
				for (int num3 = array.Length; i < num3; i++)
				{
					int frameLength = array[i].frameLength;
					GUILayout.BeginHorizontal();
					Color color2 = GUI.color;
					int num4 = ((selectCount < 1) ? 1 : selectCount);
					if (i >= selectIndex && i < selectIndex + num4)
					{
						GUI.color = Color.red;
					}
					if (GUILayout.Toggle(selectIndex == i, num2.ToString(), GUILayout.Width(35f)))
					{
						selectIndex = i;
					}
					GUI.color = color2;
					num2 += frameLength;
					string s = GUILayout.TextField(array[i].frameLength.ToString(), GUILayout.Width(20f));
					int.TryParse(s, out array[i].frameLength);
					string frameKey = array[i].frameKey;
					if (GUILayout.Button("-", GUILayout.Width(20f)))
					{
						int num5 = MapFromNameToIndex(array[i].frameKey);
						int imageMapLength = GetImageMapLength();
						num5 = Mathf.Clamp(num5 - 1, 0, imageMapLength - 1);
						string text = MapFromIndexToName(num5);
						if (text == null)
						{
							text = "<Unnamed>";
						}
						array[i].frameKey = text;
					}
					array[i].frameKey = GUILayout.TextField(array[i].frameKey);
					if (GUILayout.Button("+", GUILayout.Width(20f)))
					{
						int num6 = MapFromNameToIndex(array[i].frameKey);
						int imageMapLength2 = GetImageMapLength();
						num6 = Mathf.Clamp(num6 + 1, 0, imageMapLength2 - 1);
						string text2 = MapFromIndexToName(num6);
						if (text2 == null)
						{
							text2 = "<Unnamed>";
						}
						array[i].frameKey = text2;
					}
					if (frameLength != array[i].frameLength || frameKey != array[i].frameKey)
					{
						_curAvatar.RebuildFaceAnimation();
						changed = true;
					}
					if (GUILayout.Button("X", GUILayout.Width(30f)))
					{
						num = i;
					}
					GUILayout.EndHorizontal();
				}
				if (num >= 0)
				{
					List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(array);
					list.RemoveAt(num);
					if (_currentPart == FacePartType.LeftEye)
					{
						_currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks = list.ToArray();
					}
					else if (_currentPart == FacePartType.RightEye)
					{
						_currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks = list.ToArray();
					}
					else if (_currentPart == FacePartType.Mouth)
					{
						_currentConfig.items[_currentFaceAnimationItemIndex].mouthBlocks = list.ToArray();
					}
					_curAvatar.RebuildFaceAnimation();
					changed = true;
				}
			}
			GUILayout.EndScrollView();
			if (GUILayout.Button("Add"))
			{
				List<FaceAnimationFrameBlock> list2 = new List<FaceAnimationFrameBlock>(array);
				FaceAnimationFrameBlock faceAnimationFrameBlock = new FaceAnimationFrameBlock();
				faceAnimationFrameBlock.frameLength = 1;
				faceAnimationFrameBlock.frameKey = "origin";
				list2.Add(faceAnimationFrameBlock);
				if (_currentPart == FacePartType.LeftEye)
				{
					_currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks = list2.ToArray();
				}
				else if (_currentPart == FacePartType.RightEye)
				{
					_currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks = list2.ToArray();
				}
				else if (_currentPart == FacePartType.Mouth)
				{
					_currentConfig.items[_currentFaceAnimationItemIndex].mouthBlocks = list2.ToArray();
				}
				_curAvatar.RebuildFaceAnimation();
				changed = true;
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("SelectCount");
			string s2 = GUILayout.TextField(selectCount.ToString());
			int.TryParse(s2, out selectCount);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Copy(Z+C)"))
			{
				Copy();
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Paste(Z+V)"))
			{
				PasteIndex();
			}
			if (GUILayout.Button("PasteTail(Z+B)"))
			{
				PasteTail();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}

		private void DrawAnimationContentEditPanel()
		{
			_contentViewScrollPos = GUILayout.BeginScrollView(_contentViewScrollPos);
			_normalizedTime = GUILayout.HorizontalSlider(_normalizedTime, 0f, 0.9999f, GUILayout.Width((float)_currentConfig.items[_currentFaceAnimationItemIndex].length * frameWidth));
			float num = _currentConfig.items[_currentFaceAnimationItemIndex].timePerFrame * (float)_currentConfig.items[_currentFaceAnimationItemIndex].length;
			GUILayout.Label((_normalizedTime * num).ToString());
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Empty, GUILayout.Width(frameWidth * frameButtonFactor * 0.3f));
			int i = 0;
			for (int length = _currentConfig.items[_currentFaceAnimationItemIndex].length; i < length; i++)
			{
				GUILayout.Label((i + 1).ToString(), GUILayout.Width(frameWidth * frameButtonFactor));
			}
			GUILayout.EndHorizontal();
			TestMatInfoProvider provider = kianaImageMap_L;
			TestMatInfoProvider provider2 = kianaImageMap_R;
			TestMatInfoProvider provider3 = kianaImageMap_M;
			if (_curAvatar.avatarId / 100 == 2)
			{
				provider = meiImageMap_L;
				provider2 = meiImageMap_R;
				provider3 = meiImageMap_M;
			}
			else if (_curAvatar.avatarId / 100 == 3)
			{
				provider = bronyaImageMap_L;
				provider2 = bronyaImageMap_R;
				provider3 = bronyaImageMap_M;
			}
			if (DrawBlockGraphArray(_currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks, provider, _currentConfig.items[_currentFaceAnimationItemIndex].length, (float)Screen.width - selectPanelWidth - 20f - blockEditPanelWidth, Color.red, Color.yellow, _currentPart != FacePartType.LeftEye))
			{
				_currentPart = FacePartType.LeftEye;
			}
			if (DrawBlockGraphArray(_currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks, provider2, _currentConfig.items[_currentFaceAnimationItemIndex].length, (float)Screen.width - selectPanelWidth - 20f - blockEditPanelWidth, Color.red, Color.yellow, _currentPart != FacePartType.RightEye))
			{
				_currentPart = FacePartType.RightEye;
			}
			if (DrawBlockGraphArray(_currentConfig.items[_currentFaceAnimationItemIndex].mouthBlocks, provider3, _currentConfig.items[_currentFaceAnimationItemIndex].length, (float)Screen.width - selectPanelWidth - 20f - blockEditPanelWidth, Color.red, Color.yellow, _currentPart != FacePartType.Mouth))
			{
				_currentPart = FacePartType.Mouth;
			}
			GUILayout.EndScrollView();
		}

		private void DrawClipboard()
		{
			GUILayout.Label("<Clipboard>");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Length");
			GUILayout.Label("Index");
			GUILayout.EndHorizontal();
			int i = 0;
			for (int count = _clipBoard.Count; i < count; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.TextField(_clipBoard[i].frameLength.ToString());
				GUILayout.TextField(_clipBoard[i].frameKey);
				GUILayout.EndHorizontal();
			}
		}

		private int DrawSwitchView(string[] strings, int index, float width = 90f)
		{
			int num = index;
			bool flag = GUI.enabled;
			GUI.enabled = !_playing;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("<", GUILayout.Width(30f), GUILayout.Height(30f)))
			{
				num = Mathf.Clamp(--num, 0, strings.Length - 1);
			}
			GUILayout.Label(strings[index], GUILayout.Width(width), GUILayout.Height(30f));
			if (GUILayout.Button(">", GUILayout.Width(30f), GUILayout.Height(30f)))
			{
				num = Mathf.Clamp(++num, 0, strings.Length - 1);
			}
			GUILayout.EndHorizontal();
			GUI.enabled = flag;
			return num;
		}

		private void InitConfigs()
		{
			_kianaConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Kiana");
			_meiConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
			_bronyaConfig = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
		}

		private void InitAvatars()
		{
			int i = 0;
			for (int num = avatarObjects.Length; i < num; i++)
			{
				FaceAnimationEditAvatar component = avatarObjects[i].GetComponent<FaceAnimationEditAvatar>();
				if (component != null)
				{
					ConfigFaceAnimation avatarConfigById = GetAvatarConfigById(component.avatarId);
					component.SetupFaceAnimation(avatarConfigById);
				}
			}
		}

		private void InitWwise()
		{
			Singleton<WwiseAudioManager>.Create();
			Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(new string[1] { "BK_MainMenu" });
		}

		private ConfigFaceAnimation GetAvatarConfigById(int id)
		{
			ConfigFaceAnimation result = null;
			switch (id / 100)
			{
			case 1:
				result = _kianaConfig;
				break;
			case 2:
				result = _meiConfig;
				break;
			case 3:
				result = _bronyaConfig;
				break;
			}
			return result;
		}

		private string[] GetFaceAnimationItemNameArray()
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int num = _currentConfig.items.Length; i < num; i++)
			{
				list.Add(_currentConfig.items[i].name);
			}
			return list.ToArray();
		}

		private bool DrawBlockGraphArray(FaceAnimationFrameBlock[] blocks, TestMatInfoProvider provider, int totalLength, float width, Color color0, Color color1, bool halfColor = false)
		{
			bool flag = false;
			Color backgroundColor = GUI.backgroundColor;
			GUILayout.BeginHorizontal();
			int num = 0;
			int i = 0;
			for (int num2 = blocks.Length; i < num2; i++)
			{
				GUI.backgroundColor = ((i % 2 != 0) ? color1 : color0);
				if (halfColor)
				{
					GUI.backgroundColor *= 0.5f;
				}
				FaceAnimationFrameBlock faceAnimationFrameBlock = blocks[i];
				int j = 0;
				for (int frameLength = faceAnimationFrameBlock.frameLength; j < frameLength; j++)
				{
					int num3 = 0;
					string[] matInfoNames = provider.GetMatInfoNames();
					int k = 0;
					for (int num4 = matInfoNames.Length; k < num4; k++)
					{
						if (faceAnimationFrameBlock.frameKey == matInfoNames[k])
						{
							num3 = k;
							break;
						}
					}
					flag |= GUILayout.Button(num3.ToString(), GUILayout.Width(frameWidth * frameButtonFactor));
					num++;
					if (num >= totalLength)
					{
						break;
					}
				}
				if (num >= totalLength)
				{
					break;
				}
			}
			if (num < totalLength)
			{
				GUI.backgroundColor = Color.white;
				if (halfColor)
				{
					GUI.backgroundColor *= 0.5f;
				}
				int l = 0;
				for (int num5 = totalLength - num; l < num5; l++)
				{
					flag |= GUILayout.Button("0", GUILayout.Width(frameWidth * frameButtonFactor));
				}
			}
			GUILayout.EndHorizontal();
			GUI.backgroundColor = backgroundColor;
			return flag;
		}

		private int MapFromNameToIndex(string name)
		{
			TestMatInfoProvider testMatInfoProvider = null;
			if (_currentPart == FacePartType.LeftEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_L;
				}
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_R;
				}
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_M;
				}
			}
			if (testMatInfoProvider == null)
			{
				return 0;
			}
			string[] matInfoNames = testMatInfoProvider.GetMatInfoNames();
			int i = 0;
			for (int num = matInfoNames.Length; i < num; i++)
			{
				if (name == matInfoNames[i])
				{
					return i;
				}
			}
			return 0;
		}

		private string MapFromIndexToName(int index)
		{
			TestMatInfoProvider testMatInfoProvider = null;
			if (_currentPart == FacePartType.LeftEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_L;
				}
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_R;
				}
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_M;
				}
			}
			if (testMatInfoProvider == null)
			{
				return null;
			}
			string[] matInfoNames = testMatInfoProvider.GetMatInfoNames();
			if (matInfoNames != null && index >= 0 && index < matInfoNames.Length)
			{
				return matInfoNames[index];
			}
			return null;
		}

		private int GetImageMapLength()
		{
			TestMatInfoProvider testMatInfoProvider = null;
			if (_currentPart == FacePartType.LeftEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_L;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_L;
				}
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_R;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_R;
				}
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				if (_curAvatar.avatarId / 100 == 1)
				{
					testMatInfoProvider = kianaImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 2)
				{
					testMatInfoProvider = meiImageMap_M;
				}
				else if (_curAvatar.avatarId / 100 == 3)
				{
					testMatInfoProvider = bronyaImageMap_M;
				}
			}
			if (testMatInfoProvider == null)
			{
				return -1;
			}
			return testMatInfoProvider.GetMatInfoNames().Length;
		}

		private void Copy()
		{
			int i = selectIndex;
			FaceAnimationFrameBlock[] currentBlocks = GetCurrentBlocks();
			if (currentBlocks != null)
			{
				_clipBoard.Clear();
				for (int num = ((selectCount < 1) ? 1 : selectCount); i < selectIndex + num && i >= 0 && i < currentBlocks.Length; i++)
				{
					_clipBoard.Add(new FaceAnimationFrameBlock
					{
						frameLength = currentBlocks[i].frameLength,
						frameKey = currentBlocks[i].frameKey
					});
				}
			}
		}

		private void PasteIndex()
		{
			FaceAnimationFrameBlock[] currentBlocks = GetCurrentBlocks();
			if (currentBlocks != null)
			{
				List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(currentBlocks);
				for (int num = _clipBoard.Count - 1; num >= 0; num--)
				{
					list.Insert(selectIndex, new FaceAnimationFrameBlock
					{
						frameLength = _clipBoard[num].frameLength,
						frameKey = _clipBoard[num].frameKey
					});
				}
				FaceAnimationItem faceAnimationItem = _currentConfig.items[_currentFaceAnimationItemIndex];
				if (_currentPart == FacePartType.LeftEye)
				{
					faceAnimationItem.leftEyeBlocks = list.ToArray();
				}
				if (_currentPart == FacePartType.RightEye)
				{
					faceAnimationItem.rightEyeBlocks = list.ToArray();
				}
				if (_currentPart == FacePartType.Mouth)
				{
					faceAnimationItem.mouthBlocks = list.ToArray();
				}
				_curAvatar.RebuildFaceAnimation();
				changed = true;
			}
		}

		private void PasteTail()
		{
			FaceAnimationFrameBlock[] currentBlocks = GetCurrentBlocks();
			if (currentBlocks != null)
			{
				List<FaceAnimationFrameBlock> list = new List<FaceAnimationFrameBlock>(currentBlocks);
				for (int i = 0; i < _clipBoard.Count; i++)
				{
					list.Add(new FaceAnimationFrameBlock
					{
						frameLength = _clipBoard[i].frameLength,
						frameKey = _clipBoard[i].frameKey
					});
				}
				FaceAnimationItem faceAnimationItem = _currentConfig.items[_currentFaceAnimationItemIndex];
				if (_currentPart == FacePartType.LeftEye)
				{
					faceAnimationItem.leftEyeBlocks = list.ToArray();
				}
				if (_currentPart == FacePartType.RightEye)
				{
					faceAnimationItem.rightEyeBlocks = list.ToArray();
				}
				if (_currentPart == FacePartType.Mouth)
				{
					faceAnimationItem.mouthBlocks = list.ToArray();
				}
				_curAvatar.RebuildFaceAnimation();
				changed = true;
			}
		}

		private FaceAnimationFrameBlock[] GetCurrentBlocks()
		{
			if (_currentFaceAnimationItemIndex < 0 || _currentFaceAnimationItemIndex >= _currentConfig.items.Length)
			{
				return null;
			}
			FaceAnimationFrameBlock[] result = null;
			if (_currentPart == FacePartType.LeftEye)
			{
				result = _currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks;
			}
			else if (_currentPart == FacePartType.RightEye)
			{
				result = _currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks;
			}
			else if (_currentPart == FacePartType.Mouth)
			{
				result = _currentConfig.items[_currentFaceAnimationItemIndex].mouthBlocks;
			}
			return result;
		}

		private FaceAnimationFrameBlock[] CreateInverseEyeBlocks(FaceAnimationFrameBlock[] src, char replaceFrom, char replaceTo)
		{
			FaceAnimationFrameBlock[] array = new FaceAnimationFrameBlock[src.Length];
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int frameLength = src[i].frameLength;
				string frameKey = src[i].frameKey.Replace(replaceFrom, replaceTo);
				array[i] = new FaceAnimationFrameBlock
				{
					frameLength = frameLength,
					frameKey = frameKey
				};
			}
			return array;
		}

		private void UpdateHotkey()
		{
			if (!openned)
			{
				return;
			}
			if (Input.GetKey(KeyCode.Z))
			{
				if (Input.GetKeyDown(KeyCode.C))
				{
					Copy();
				}
				if (Input.GetKeyDown(KeyCode.V))
				{
					PasteIndex();
				}
				if (Input.GetKeyDown(KeyCode.B))
				{
					PasteTail();
				}
				if (Input.GetKeyDown(KeyCode.R))
				{
					if (_eyeCopyDirection == 0)
					{
						_eyeCopyDirection = 2;
					}
					else if (_eyeCopyDirection == 1)
					{
						_eyeCopyDirection = 0;
						_currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks = CreateInverseEyeBlocks(_currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks, 'L', 'R');
					}
				}
				if (Input.GetKeyDown(KeyCode.L))
				{
					if (_eyeCopyDirection == 0)
					{
						_eyeCopyDirection = 1;
					}
					else if (_eyeCopyDirection == 2)
					{
						_eyeCopyDirection = 0;
						_currentConfig.items[_currentFaceAnimationItemIndex].leftEyeBlocks = CreateInverseEyeBlocks(_currentConfig.items[_currentFaceAnimationItemIndex].rightEyeBlocks, 'R', 'L');
					}
				}
			}
			else
			{
				_eyeCopyDirection = 0;
			}
		}
	}
}
