using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEngine;

namespace MoleMole
{
	public static class Miscs
	{
		public class RenderLineInfo
		{
			private static string _shaderName = "miHoYo/UI/Image Tint Color";

			private Camera _camera;

			private GameObject _lineObj;

			private LineRenderer _lr;

			private Vector3 _screenPos_start;

			private Vector3 _screenPos_end;

			public RenderLineInfo(Camera camera, Vector3 start, Vector3 end, Color color, float width)
			{
				_camera = camera;
				_screenPos_start = start;
				_screenPos_end = end;
				_lineObj = new GameObject();
				_lineObj.AddComponent<LineRenderer>();
				_lr = _lineObj.GetComponent<LineRenderer>();
				_lr.material = new Material(Shader.Find(_shaderName));
				_lr.SetColors(color, color);
				_lr.SetWidth(width, width);
			}

			public void Draw()
			{
				_lineObj.SetActive(true);
				Vector3 screenPos_start = _screenPos_start;
				screenPos_start.x = (float)_camera.pixelWidth * screenPos_start.x;
				screenPos_start.y = (float)_camera.pixelHeight * screenPos_start.y;
				Vector3 screenPos_end = _screenPos_end;
				screenPos_end.x = (float)_camera.pixelWidth * screenPos_end.x;
				screenPos_end.y = (float)_camera.pixelHeight * screenPos_end.y;
				Vector3 position = _camera.ScreenToWorldPoint(screenPos_start);
				Vector3 position2 = _camera.ScreenToWorldPoint(screenPos_end);
				_lineObj.transform.position = position;
				_lr.SetPosition(0, position);
				_lr.SetPosition(1, position2);
			}

			public void Hide()
			{
				_lineObj.SetActive(false);
			}

			public bool IsValid()
			{
				return _lineObj != null;
			}
		}

		public const string HCOIN_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconHC";

		public const string SCOIN_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconSC";

		public const string FRIENDPOINT_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconFP";

		public const string SKILLPOINT_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconSP";

		public const string STAMINA_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconST";

		public const string EXP_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconEXP";

		public const string AVATARFRAGMENT_ICON_PREFAB_PATH = "SpriteOutput/AvatarFragmentIcons";

		public const string AVATARCARD_ICON_PREFAB_PATH = "SpriteOutput/AvatarCardIcons";

		public const string MATERIAL_ICON_PREFAB_PATH = "SpriteOutput/MaterialIcons";

		public const string WEAPON_ICON_PREFAB_PATH = "SpriteOutput/WeaponIcons";

		public const string STIGMATA_ICON_PREFAB_PATH = "SpriteOutput/StigmataIcons";

		public const float EPSILON = 0.0001f;

		public const float DEGREE_LERP_END_DIFF = 2f;

		public const float LENGTH_LERP_END_DIFF = 0.1f;

		public const float VERY_BIG_FLOAT = 999999f;

		public static string[] EMPTY_STRINGS = new string[0];

		private static List<RenderLineInfo> _renderLines = null;

		private static bool _bEnableDraw = false;

		public static string FixNumberStringToLength(uint number, int length)
		{
			if (length <= 0)
			{
				throw new Exception("Invalid Type or State!");
			}
			string text = string.Empty + number;
			while (text.Length < length)
			{
				text = "0" + text;
			}
			return text;
		}

		public static int ExtractOneNumFromString(string Str)
		{
			return int.Parse(Regex.Match(Str, "\\d+").Value);
		}

		public static string LoadTextFileToString(string filePath)
		{
			TextAsset textAsset = LoadResource(filePath, BundleType.DATA_FILE) as TextAsset;
			string text = textAsset.text;
			Resources.UnloadAsset(textAsset);
			return text;
		}

		public static object ParseJsonParameterValue(JSONNode aNode)
		{
			switch (aNode["Type"])
			{
			case "Float":
				return aNode["Value"].AsFloat;
			case "Int":
				return aNode["Value"].AsInt;
			case "Double":
				return aNode["Value"].AsDouble;
			case "Bool":
				return aNode["Value"].AsBool;
			case "String":
				return aNode["Value"].Value;
			case "Array":
			{
				JSONArray asArray = aNode["Value"].AsArray;
				switch (aNode["ArrayElemType"])
				{
				case "Float":
				{
					float[] array2 = new float[asArray.Count];
					for (int j = 0; j < asArray.Count; j++)
					{
						array2[j] = asArray[j].AsFloat;
					}
					return array2;
				}
				case "Int":
				{
					int[] array5 = new int[asArray.Count];
					for (int m = 0; m < asArray.Count; m++)
					{
						array5[m] = asArray[m].AsInt;
					}
					return array5;
				}
				case "Double":
				{
					double[] array3 = new double[asArray.Count];
					for (int k = 0; k < asArray.Count; k++)
					{
						array3[k] = asArray[k].AsDouble;
					}
					return array3;
				}
				case "Bool":
				{
					bool[] array4 = new bool[asArray.Count];
					for (int l = 0; l < asArray.Count; l++)
					{
						array4[l] = asArray[l].AsBool;
					}
					return array4;
				}
				case "String":
				{
					string[] array = new string[asArray.Count];
					for (int i = 0; i < asArray.Count; i++)
					{
						array[i] = asArray[i].Value;
					}
					return array;
				}
				default:
					throw new Exception("Invalid Type or State!");
				}
			}
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public static void ParseJsonParameterGroup(JSONNode aNode, Hashtable dynamicParamTable)
		{
			ParseJsonParameterGroupWithParamPrefix(aNode, dynamicParamTable, string.Empty);
		}

		public static void ParseJsonParameterGroupWithParamPrefix(JSONNode aNode, Hashtable dynamicParamTable, string paramPrefix)
		{
			for (int i = 0; i < aNode["Parameters"].Count; i++)
			{
				dynamicParamTable.Add(paramPrefix + aNode["Parameters"][i]["Param"].Value, ParseJsonParameterValue(aNode["Parameters"][i]));
			}
		}

		public static float DistancForVec3IgnoreY(Vector3 Vec3A, Vector3 Vec3B)
		{
			Vec3A.y = 0f;
			Vec3B.y = 0f;
			return Vector3.Distance(Vec3A, Vec3B);
		}

		public static float AngleForVec3IgnoreY(Vector3 DirFrom, Vector3 DirTo)
		{
			Vector3 vector = new Vector3(DirFrom.x, 0f, DirFrom.z);
			Vector3 vector2 = new Vector3(DirTo.x, 0f, DirTo.z);
			return Mathf.Acos(Vector3.Dot(vector.normalized, vector2.normalized));
		}

		public static Vector3 LerpAngleForVec3IgnoreY(Vector3 DirFrom, Vector3 DirTo, float ratio)
		{
			Vector3 vector = new Vector3(DirFrom.x, 0f, DirFrom.z);
			Vector3 vector2 = new Vector3(DirTo.x, 0f, DirTo.z);
			vector.Normalize();
			vector2.Normalize();
			float num = Mathf.Atan2(DirFrom.z, DirFrom.x);
			float num2 = Mathf.Atan2(DirTo.z, DirTo.x);
			float f = ratio * num2 + (1f - ratio) * num;
			return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}

		public static float AngleFromToIgnoreY(Vector3 dirFrom, Vector3 dirTo)
		{
			dirFrom.y = 0f;
			dirTo.y = 0f;
			float num = Vector3.Angle(dirFrom, dirTo);
			float num2 = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(dirFrom, dirTo)));
			return num * num2;
		}

		public static float AngleToGround(Vector3 dir)
		{
			Vector3 to = Vector3.ProjectOnPlane(dir, Vector3.up);
			float num = Vector3.Angle(dir, to);
			return (!(dir.y > 0f)) ? (0f - num) : num;
		}

		public static float SignedAngleDiff(float from, float to)
		{
			float num;
			for (num = to - from; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			return num;
		}

		public static float AbsAngleDiff(float from, float to)
		{
			return Mathf.Abs(SignedAngleDiff(from, to));
		}

		public static float NormalizedRotateAngle(float from, float to)
		{
			float num = SignedAngleDiff(from, to);
			return from + num;
		}

		public static float NormalizedClamp(float value, float min, float max)
		{
			value = Mathf.Clamp(value, min, max);
			return (value - min) / (max - min);
		}

		public static int RightMost1BitIndex(int x)
		{
			int num = -1;
			while (x != 0)
			{
				num++;
				x >>= 1;
			}
			return num;
		}

		public static bool IsFloatInRange(float value, float lower, float upper)
		{
			return value > lower && value < upper;
		}

		public static bool ArrayContains<T>(T[] array, T element)
		{
			return Array.IndexOf(array, element) != -1;
		}

		public static void ArrayAppend<T>(ref T[] array, T element)
		{
			T[] array2 = new T[array.Length + 1];
			Array.Copy(array, array2, array.Length);
			array2[array2.Length - 1] = element;
			array = array2;
		}

		public static void ArrayClearOutNulls<T>(ref T[] array)
		{
			List<T> list = new List<T>();
			T[] array2 = array;
			foreach (T val in array2)
			{
				if (val != null)
				{
					list.Add(val);
				}
			}
			array = list.ToArray();
		}

		public static int ArrayRefCountOf<T>(T[] arr, T element) where T : class
		{
			int num = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				if (object.ReferenceEquals(element, arr[i]))
				{
					num++;
				}
			}
			return num;
		}

		public static Sprite GetSpriteByPrefab(string prefabPath)
		{
			Sprite sprite = LoadResource<Sprite>(prefabPath);
			if (sprite != null)
			{
				return sprite;
			}
			GameObject gameObject = LoadResource<GameObject>(prefabPath);
			if (gameObject == null)
			{
				gameObject = LoadResource<GameObject>("SpriteOutput/SpecialIcons/ItemEmpty");
			}
			return gameObject.GetComponent<SpriteRenderer>().sprite;
		}

		public static Sprite GetItemSpriteByPrefab(int id)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id);
			string iconPath = dummyStorageDataItem.GetIconPath();
			return GetSpriteByPrefab(iconPath);
		}

		public static IEnumerator LoadAsyncAsset(string path, Action<object> callback)
		{
			ResourceRequest resReq = Resources.LoadAsync(path);
			while (!resReq.isDone)
			{
				yield return 0;
			}
			if (callback != null)
			{
				callback(resReq.asset);
			}
		}

		public static string GetBeforeTimeToShow(DateTime time)
		{
			TimeSpan timeSpan = TimeUtil.Now - time;
			if (timeSpan.TotalMinutes < 60.0)
			{
				return LocalizationGeneralLogic.GetText("Menu_Desc_TimeMinutesBefore", timeSpan.Minutes);
			}
			if (timeSpan.TotalHours < 24.0)
			{
				return LocalizationGeneralLogic.GetText("Menu_Desc_TimeHoursBefore", timeSpan.Hours);
			}
			if (timeSpan.TotalDays < 7.0)
			{
				return LocalizationGeneralLogic.GetText("Menu_Desc_TimeDaysBefore", timeSpan.Days);
			}
			return time.ToString("yyyy-MM-dd");
		}

		public static int GetTimeSpanToShow(DateTime time, out string label)
		{
			DateTime now = TimeUtil.Now;
			TimeSpan timeSpan = ((!(now > time)) ? (time - now) : (now - time));
			if (timeSpan.TotalMinutes < 60.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Minute");
				return timeSpan.Minutes;
			}
			if (timeSpan.TotalHours < 24.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Hour");
				return timeSpan.Hours;
			}
			if (timeSpan.TotalDays < 7.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Day");
				return timeSpan.Days;
			}
			label = string.Empty;
			return 0;
		}

		public static int GetDiffTimeToShow(DateTime from, DateTime to, out string label)
		{
			TimeSpan timeSpan = to - from;
			if (timeSpan.TotalMinutes < 60.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Minute");
				return timeSpan.Minutes;
			}
			if (timeSpan.TotalHours < 24.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Hour");
				return timeSpan.Hours;
			}
			if (timeSpan.TotalDays < 7.0)
			{
				label = LocalizationGeneralLogic.GetText("Menu_Desc_Day");
				return timeSpan.Days;
			}
			label = string.Empty;
			return 0;
		}

		public static string GetTimeString(DateTime time)
		{
			return time.ToString("yyyy-MM-dd HH:mm");
		}

		public static DateTime GetDateTimeFromTimeStamp(uint timeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();
		}

		public static uint GetTimeStampFromDateTime(DateTime datetime)
		{
			return (uint)(datetime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime()).TotalSeconds;
		}

		public static void PrettyPrintLayerMask(LayerMask mask)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(mask.value + "\n");
			for (int i = 0; i < 32; i++)
			{
				if (mask.ContainsLayer(i))
				{
					stringBuilder.Append(LayerMask.LayerToName(i) + "\n");
				}
			}
		}

		public static Color GetDifficultyColor(LevelDiffculty difficulty)
		{
			switch (difficulty)
			{
			case LevelDiffculty.Normal:
				return MiscData.GetColor("ChapterDifficultyNormal");
			case LevelDiffculty.Hard:
				return MiscData.GetColor("ChapterDifficultyHard");
			case LevelDiffculty.Hell:
				return MiscData.GetColor("ChapterDifficultyHell");
			default:
				return MiscData.GetColor("ChapterDifficultyNormal");
			}
		}

		public static string GetDifficultyDesc(LevelDiffculty difficulty)
		{
			switch (difficulty)
			{
			case LevelDiffculty.Normal:
				return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyNormal");
			case LevelDiffculty.Hard:
				return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyHard");
			case LevelDiffculty.Hell:
				return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyHell");
			default:
				return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyNormal");
			}
		}

		public static bool IsPosition2InRect(Vector2 position, Rect rect)
		{
			return position.x >= rect.xMin && position.x <= rect.xMax && position.y >= rect.yMin && position.y <= rect.yMax;
		}

		public static IEnumerator WWWRequestWithTimeOut(string url, Action<string> callback, Action timeOutCallback, float timeoutSecond = 5f, byte[] postData = null, Dictionary<string, string> headers = null, bool needDispose = true)
		{
			float timer = 0f;
			bool timeout = false;
			WWW www = new WWW(url, postData, headers);
			while (!www.isDone)
			{
				if (timer > timeoutSecond)
				{
					timeout = true;
					break;
				}
				timer += Time.deltaTime;
				yield return null;
			}
			if (string.IsNullOrEmpty(www.error))
			{
				if (timeout)
				{
					if (timeOutCallback != null)
					{
						timeOutCallback();
					}
				}
				else if (callback != null)
				{
					callback(www.text);
				}
			}
			if (needDispose)
			{
				www.Dispose();
			}
		}

		public static IEnumerator WWWRequestWithRetry(string url, Action<string> callback, Action timeOutCallback, float timeoutSecond = 5f, int retryTime = 3, byte[] postData = null, Dictionary<string, string> headers = null)
		{
			int counter = 0;
			while (counter < retryTime)
			{
				counter++;
				float timer = 0f;
				bool timeout = false;
				WWW localWWW = new WWW(url, postData, headers);
				while (!localWWW.isDone)
				{
					if (timer > timeoutSecond)
					{
						timeout = true;
						break;
					}
					timer += Time.deltaTime;
					yield return null;
				}
				if (!string.IsNullOrEmpty(localWWW.error) || timeout)
				{
					string warningMsg = ((!timeout) ? localWWW.error : "timeout");
					if (counter >= retryTime && timeOutCallback != null)
					{
						timeOutCallback();
					}
					continue;
				}
				if (callback != null)
				{
					callback(localWWW.text);
				}
				break;
			}
		}

		public static string Truncate(string str, int n = 10)
		{
			return (str.Length <= n) ? str : str.Substring(0, n);
		}

		public static string GetDebugActorName(BaseActor actor)
		{
			if (actor == null)
			{
				return "<!null>";
			}
			if (!actor.IsActive())
			{
				if (actor.gameObject != null)
				{
					return string.Format("<!inactive {0}({1:x})>", Truncate(actor.gameObject.name), actor.runtimeID);
				}
				return string.Format("<!dead:{0}>", actor.runtimeID);
			}
			return string.Format("<{0}({1:x})>", Truncate(actor.gameObject.name), actor.runtimeID);
		}

		public static string GetDebugEntityName(BaseMonoEntity entity)
		{
			if (entity == null)
			{
				return "<!null>";
			}
			return string.Format("<{0}({1:x})>", Truncate(entity.gameObject.name), entity.GetRuntimeID());
		}

		public static string GetAnimIDAttackPropertyOutput(BaseActor actor, string animEventID)
		{
			if (actor == null)
			{
				return string.Format("<!null attack proeprty {0} on {1}>", GetDebugActorName(actor), animEventID);
			}
			if (actor is AvatarActor)
			{
				return SharedAnimEventData.ResolveAnimEvent(((AvatarActor)actor).config, animEventID).AttackProperty.GetDebugOutput();
			}
			if (actor is MonsterActor)
			{
				return SharedAnimEventData.ResolveAnimEvent(((MonsterActor)actor).config, animEventID).AttackProperty.GetDebugOutput();
			}
			if (actor is PropObjectActor)
			{
				return SharedAnimEventData.ResolveAnimEvent(((PropObjectActor)actor).config, animEventID).AttackProperty.GetDebugOutput();
			}
			return string.Format("<!null attack proeprty {0} on {1}>", GetDebugActorName(actor), animEventID);
		}

		public static T LoadResource<T>(string path, BundleType type = BundleType.RESOURCE_FILE) where T : UnityEngine.Object
		{
			if (Singleton<AssetBundleManager>.Instance == null)
			{
				return Resources.Load<T>(path);
			}
			switch (type)
			{
			case BundleType.DATA_FILE:
				return Singleton<AssetBundleManager>.Instance.LoadData<T>(path);
			case BundleType.RESOURCE_FILE:
				return Singleton<AssetBundleManager>.Instance.LoadRes<T>(path);
			default:
				return (T)null;
			}
		}

		public static AsyncAssetRequst LoadResourceAsync(string path, BundleType type = BundleType.RESOURCE_FILE)
		{
			if (Singleton<AssetBundleManager>.Instance == null)
			{
				ResourceRequest operation = Resources.LoadAsync(path);
				return new AsyncAssetRequst(operation);
			}
			switch (type)
			{
			case BundleType.DATA_FILE:
				return Singleton<AssetBundleManager>.Instance.LoadDataAsync(path);
			case BundleType.RESOURCE_FILE:
				return Singleton<AssetBundleManager>.Instance.LoadResAsync(path);
			default:
				return null;
			}
		}

		public static UnityEngine.Object LoadResource(string path, BundleType type = BundleType.RESOURCE_FILE)
		{
			return LoadResource<UnityEngine.Object>(path, type);
		}

		public static Color ParseColor(string hexString)
		{
			Color color = Color.white;
			if (!ColorUtility.TryParseHtmlString(hexString, out color))
			{
			}
			return color;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = UnityEngine.Random.Range(0, num);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}

		public static string GetBaseName(string path)
		{
			int num = path.LastIndexOf('/');
			return (num != -1) ? path.Substring(num + 1) : path;
		}

		public static bool IsAlmostZero(float f)
		{
			return f < 0.0001f && f > -0.0001f;
		}

		public static void SetPitch(Transform tran, float pitch)
		{
			Vector3 eulerAngles = tran.eulerAngles;
			eulerAngles.x = pitch;
			tran.eulerAngles = eulerAngles;
		}

		public static void TriggerDrawLine()
		{
			_bEnableDraw = !_bEnableDraw;
			if (!_bEnableDraw)
			{
				ClearLine_GameView();
			}
		}

		public static void DrawLine_GameView()
		{
			if (!_bEnableDraw)
			{
				return;
			}
			if (_renderLines == null)
			{
				_renderLines = new List<RenderLineInfo>();
			}
			if (_renderLines.Count <= 0)
			{
				RenderLineInfo item = new RenderLineInfo(Camera.main, new Vector3(0f, 0.5f, 10f), new Vector3(1f, 0.5f, 10f), Color.red, 0.02f);
				RenderLineInfo item2 = new RenderLineInfo(Camera.main, new Vector3(0.5f, 1f, 10f), new Vector3(0.5f, 0f, 10f), Color.red, 0.02f);
				_renderLines.Add(item);
				_renderLines.Add(item2);
			}
			bool flag = false;
			foreach (RenderLineInfo renderLine in _renderLines)
			{
				if (renderLine.IsValid())
				{
					renderLine.Draw();
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				_renderLines.Clear();
			}
		}

		public static void ClearLine_GameView()
		{
			foreach (RenderLineInfo renderLine in _renderLines)
			{
				renderLine.Hide();
			}
		}

		public static bool WildcardMatch(string wildcard, string input, bool ignoreCase = false)
		{
			int num = 0;
			int num2 = 0;
			int i = 0;
			int num3 = 0;
			while (num < input.Length && i < wildcard.Length && wildcard[i] != '*')
			{
				if (wildcard[i] != '?' && string.Compare(input, num, wildcard, i, 1, ignoreCase) != 0)
				{
					return false;
				}
				num++;
				i++;
			}
			while (num < input.Length)
			{
				if (i < wildcard.Length && wildcard[i] == '*')
				{
					if (++i >= wildcard.Length)
					{
						return true;
					}
					num2 = num + 1;
					num3 = i;
				}
				else if (i < wildcard.Length && (string.Compare(input, num, wildcard, i, 1, ignoreCase) == 0 || wildcard[i] == '?'))
				{
					num++;
					i++;
				}
				else
				{
					num = num2++;
					i = num3;
				}
			}
			for (; i < wildcard.Length && wildcard[i] == '*'; i++)
			{
			}
			return i >= wildcard.Length;
		}

		[RuntimeInitializeOnLoadMethod]
		public static void CreateExceptionWarningLogger()
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<MonoDeviceDebugGUI>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		public static string ListToString(List<int> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int item in list)
			{
				stringBuilder.Append(item.ToString());
				stringBuilder.Append(", ");
			}
			return stringBuilder.ToString();
		}

		public static Transform FindFirstChildGivenLayerAndCollider(Transform start, int layer)
		{
			Transform transform = null;
			foreach (Transform item in start)
			{
				if (item.gameObject.layer == layer && item.gameObject.GetComponent<Collider>() != null)
				{
					return item;
				}
				transform = FindFirstChildGivenLayerAndCollider(item, layer);
				if (transform != null)
				{
					return transform;
				}
			}
			return transform;
		}

		public static bool CheckOutsideWallAndDrag(Transform transform)
		{
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (localAvatar != null)
			{
				bool flag = false;
				Vector3 vector = new Vector3(localAvatar.transform.position.x, 0.1f, localAvatar.transform.position.z);
				Vector3 vector2 = new Vector3(transform.position.x, 0.1f, transform.position.z);
				RaycastHit hitInfo;
				if (Physics.Linecast(vector, vector2, out hitInfo, 1 << InLevelData.OBSTACLE_COLLIDER_LAYER))
				{
					flag = true;
				}
				if (!flag && Physics.Linecast(vector, vector2, out hitInfo, 1 << InLevelData.STAGE_COLLIDER_LAYER) && !Physics.Linecast(vector2, vector, 1 << InLevelData.STAGE_COLLIDER_LAYER))
				{
					flag = true;
				}
				if (flag)
				{
					Vector3 point = hitInfo.point;
					Vector3 normalized = (vector - vector2).normalized;
					CapsuleCollider componentInChildren = transform.GetComponentInChildren<CapsuleCollider>();
					float num = 0f;
					num = ((!(componentInChildren != null)) ? 0.5f : (componentInChildren.radius + 0.1f));
					Vector3 vector3 = point + normalized * num;
					transform.position = new Vector3(vector3.x, transform.position.y, vector3.z);
				}
				return flag;
			}
			return false;
		}
	}
}
