using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.MainMenu
{
	public class ConfigAtmosphereSeries : ScriptableObject
	{
		[HideInInspector]
		public SortedList<float, ConfigAtmosphere> SortedConfigList;

		private static readonly float MIN_KEY_INTERVAL = 0.1f;

		public ConfigAtmosphereCommon Common;

		public ConfigAtmosphere[] ConfigList;

		private string _path;

		private bool _fistEvaluate = true;

		private bool _inTransition;

		private float _beginTime;

		private float _duration;

		private int _lastKey;

		private int _beginKey;

		public string Path
		{
			get
			{
				return _path;
			}
		}

		public ConfigAtmosphereSeries()
		{
			SortedConfigList = new SortedList<float, ConfigAtmosphere>();
		}

		public bool IsValid()
		{
			return SortedConfigList != null && SortedConfigList.Count != 0;
		}

		public static ConfigAtmosphereSeries LoadFromFile(string path)
		{
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigUtil.LoadConfig<ConfigAtmosphereSeries>(path);
			configAtmosphereSeries.InitAfterLoad();
			configAtmosphereSeries._path = path;
			return configAtmosphereSeries;
		}

		public static ConfigAtmosphereSeries LoadFromFileAndDetach(string path)
		{
			ConfigAtmosphereSeries original = ConfigUtil.LoadConfig<ConfigAtmosphereSeries>(path);
			original = Object.Instantiate(original);
			original.InitAfterLoad();
			original._path = path;
			return original;
		}

		public void InitAfterLoad()
		{
			Common.InitAfterLoad();
			if (ConfigList != null && ConfigList.Length != 0)
			{
				ConfigAtmosphere[] configList = ConfigList;
				foreach (ConfigAtmosphere configAtmosphere in configList)
				{
					configAtmosphere.InitAfterLoad();
					SortedConfigList.Add(configAtmosphere.FrameTime, configAtmosphere);
				}
				ConfigList = new ConfigAtmosphere[SortedConfigList.Count];
				SortedConfigList.Values.CopyTo(ConfigList, 0);
			}
		}

		public void Add(ConfigAtmosphere config)
		{
			SortedConfigList.Add(config.FrameTime, config);
			ConfigList = new ConfigAtmosphere[SortedConfigList.Count];
			SortedConfigList.Values.CopyTo(ConfigList, 0);
		}

		public void Delete(int key)
		{
			SortedConfigList.RemoveAt(key);
			ConfigList = new ConfigAtmosphere[SortedConfigList.Count];
			SortedConfigList.Values.CopyTo(ConfigList, 0);
		}

		public int GetSceneIdRandomly()
		{
			return Common.GetSceneIdRandomly();
		}

		public void SetSceneId(int id)
		{
			Common.SceneId = id;
		}

		public bool Evaluate(float time, out ConfigAtmosphere config)
		{
			int num = KeyBeforeTime(time);
			if (_fistEvaluate)
			{
				_fistEvaluate = false;
				config = Value(num);
				_lastKey = num;
				return true;
			}
			if (!_inTransition)
			{
				if (num == _lastKey)
				{
					config = Value(num);
					_lastKey = num;
					return false;
				}
				_inTransition = true;
				_beginTime = time;
				_duration = 0f;
				_beginKey = _lastKey;
				_lastKey = num;
			}
			else if (num != _lastKey)
			{
				_lastKey = num;
				_beginTime = time - _duration;
			}
			else
			{
				_duration = time - _beginTime;
			}
			float num2 = _duration * 3600f / Common.TransitionTime;
			if (num2 > 1f)
			{
				_inTransition = false;
				config = Value(num);
				_lastKey = num;
			}
			else
			{
				config = ConfigAtmosphere.Lerp(Value(_beginKey), Value(num), num2);
			}
			return true;
		}

		public ConfigAtmosphere Evaluate(float time, bool isEditorMode)
		{
			if (SortedConfigList == null)
			{
				return null;
			}
			float tStart;
			float tEnd;
			if (!GetTimeRange(time, out tStart, out tEnd))
			{
				return null;
			}
			float num = time - tStart;
			float num2 = tEnd - tStart;
			if (num < 0f)
			{
				num += 24f;
			}
			if (num2 < 0f)
			{
				num2 += 24f;
			}
			float t;
			if (isEditorMode)
			{
				t = num / num2;
				_lastKey = KeyAtTime(tStart);
				return ConfigAtmosphere.Lerp(SortedConfigList[tStart], SortedConfigList[tEnd], t);
			}
			num2 = Mathf.Min(num2, Common.TransitionTime / 3600f);
			int num3 = KeyBeforeTime(tStart - MIN_KEY_INTERVAL);
			t = ((!(num2 < float.Epsilon)) ? Mathf.Clamp01(num / num2) : 1f);
			_lastKey = num3;
			return ConfigAtmosphere.Lerp(Value(num3), SortedConfigList[tStart], t);
		}

		public ConfigAtmosphere Value(int key)
		{
			return ConfigList[key];
		}

		public int KeyCount()
		{
			return SortedConfigList.Count;
		}

		public int KeyBeforeTime(float time)
		{
			if (SortedConfigList.Keys.Count == 0)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i < SortedConfigList.Keys.Count; i++)
			{
				if (SortedConfigList.Keys[i] > time)
				{
					num = i;
					break;
				}
			}
			if (num == -1 || num == 0)
			{
				return SortedConfigList.Keys.Count - 1;
			}
			return num - 1;
		}

		public int KeyAtTime(float time)
		{
			if (!IsValid())
			{
				return -1;
			}
			int num = KeyBeforeTime(time);
			if (num == -1)
			{
				return -1;
			}
			int num2 = num + 1;
			if (num2 == SortedConfigList.Count)
			{
				num2 = 0;
			}
			float num3 = TimeAtKey(num);
			float num4 = TimeAtKey(num2);
			float num5 = Mathf.Abs(time - num3);
			float num6 = Mathf.Abs(num4 - time);
			int result;
			float num7;
			if (num5 < num6)
			{
				result = num;
				num7 = num5;
			}
			else
			{
				result = num2;
				num7 = num6;
			}
			if (num7 < MIN_KEY_INTERVAL)
			{
				return result;
			}
			return -1;
		}

		public float TimeAtKey(int key)
		{
			return SortedConfigList.Keys[key];
		}

		private bool GetTimeRange(float t, out float tStart, out float tEnd)
		{
			tStart = 0f;
			tEnd = 0f;
			bool flag = false;
			bool flag2 = false;
			foreach (float key in SortedConfigList.Keys)
			{
				float num = key;
				if (num > t)
				{
					tEnd = num;
					flag2 = true;
					break;
				}
				flag = true;
				tStart = num;
			}
			if (!flag && !flag2)
			{
				return false;
			}
			if (!flag)
			{
				tStart = SortedConfigList.Keys[SortedConfigList.Keys.Count - 1];
			}
			if (!flag2)
			{
				tEnd = SortedConfigList.Keys[0];
			}
			return true;
		}
	}
}
