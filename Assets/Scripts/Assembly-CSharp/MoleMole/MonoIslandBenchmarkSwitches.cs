using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoIslandBenchmarkSwitches : MonoBenchmarkSwitches
	{
		public class HideObjectBenchmarkSwitch : BenchmarkSwitch
		{
			private GameObject _obj;

			public HideObjectBenchmarkSwitch(GameObject obj)
				: base("Hide " + obj.name)
			{
				_obj = obj;
				toggler = HideObject;
			}

			public override void SetEnabled()
			{
				toggler(false);
				_toggled = false;
			}

			private void HideObject(bool isHide)
			{
				_obj.SetActive(!isHide);
			}
		}

		public class HideObjectListBenchmarkSwitch : BenchmarkSwitch
		{
			private GameObject[] _objList;

			private int _ix;

			public HideObjectListBenchmarkSwitch(GameObject[] objList)
				: base("Hide Object List")
			{
				_objList = objList;
				_ix = objList.Length;
			}

			public override void SetEnabled()
			{
				HideObjest();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Hide Object: " + ((_ix >= _objList.Length) ? "None" : _objList[_ix].name), GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % (_objList.Length + 1);
					HideObjest();
				}
			}

			private void HideObjest()
			{
				GameObject[] objList = _objList;
				foreach (GameObject gameObject in objList)
				{
					gameObject.SetActive(true);
				}
				if (_ix < _objList.Length)
				{
					_objList[_ix].SetActive(false);
				}
			}
		}

		public GameObject[] hiddenGameObjects;

		private void Start()
		{
			_switches = new List<BenchmarkSwitch>();
			_switches.Add(new BenchmarkSwitch("Post FX", delegate(bool enabled)
			{
				GraphicsSettingUtil.EnablePostFX(enabled);
			}));
			_switches.Add(new BenchmarkSwitch("HDR", GraphicsSettingUtil.EnableHDR));
			_switches.Add(new BenchmarkSwitch("FXAA", GraphicsSettingUtil.EnableFXAA));
			_switches.Add(new BenchmarkSwitch("Distortion", GraphicsSettingUtil.EnableDistortion));
			_switches.Add(new BenchmarkSwitch("Not Fast Mode", delegate(bool enabled)
			{
				Object.FindObjectOfType<PostFXBase>().FastMode = !enabled;
			}));
			_switches.Add(new BufferSizeBenchmarkSwitch());
			_switches.Add(new BenchmarkSwitch("ColorGrading", GraphicsSettingUtil.EnableColorGrading));
			_switches.Add(new BenchmarkSwitch("Reflection", GraphicsSettingUtil.EnableReflection));
			_switches.Add(new BenchmarkSwitch("DynBone", GraphicsSettingUtil.EnableDynamicBone));
			_switches.Add(new BenchmarkSwitch("60FPS", delegate(bool enabled)
			{
				if (enabled)
				{
					Application.targetFrameRate = 60;
				}
				else
				{
					Application.targetFrameRate = 30;
				}
			}));
			if (Singleton<EffectManager>.Instance != null)
			{
				_switches.Add(new BenchmarkSwitch("Effects", delegate(bool enabled)
				{
					Singleton<EffectManager>.Instance.mute = !enabled;
				}));
			}
			_switches.Add(new BenchmarkSwitch("FixedUpdates", delegate(bool enabled)
			{
				Time.fixedDeltaTime = ((!enabled) ? 1f : 0.02f);
			}));
			_switches.Add(new ResolutionBenchmarkSwitch());
			if (Object.FindObjectOfType<PostFXWithResScale>() != null)
			{
				_switches.Add(new ResScaleBenchmarkSwitch());
			}
			_switches.Add(new UseResScaleFXSwitch(_switches, this));
			_switches.Add(new NoFPSLimitSwitch());
			_switches.Add(new BenchmarkSwitch("UI Camera", MakeUICameraEnabledToggler()));
			_switches.Add(new BenchmarkSwitch("UI Camera Clear", MakeUICameraClearToggler()));
			GameObject[] array = hiddenGameObjects;
			foreach (GameObject obj in array)
			{
				_switches.Add(new HideObjectBenchmarkSwitch(obj));
			}
			foreach (BenchmarkSwitch @switch in _switches)
			{
				if (!(@switch.name == "Post FX"))
				{
					@switch.SetEnabled();
				}
			}
		}

		private void OnGUI()
		{
			if (_toggled)
			{
				WidgetSwitches();
				if (GUI.Button(new Rect((float)Screen.width * 0.85f, (float)Screen.height * 0.85f, 60f, 60f), "CLOSE"))
				{
					_toggled = false;
					EasyTouch easyTouch = Object.FindObjectOfType<EasyTouch>();
					if (easyTouch != null)
					{
						easyTouch.enable = true;
					}
					SetEventSystemEnable(true);
				}
				GUI.Label(new Rect(20f, (float)Screen.height * 0.85f, 200f, 60f), "GraphicAPI: " + SystemInfo.graphicsDeviceType);
			}
			else if (GUI.Button(new Rect((float)Screen.width * 0.5f, (float)Screen.height * 0.15f, 80f, 30f), "Benchmark"))
			{
				_toggled = true;
				EasyTouch easyTouch2 = Object.FindObjectOfType<EasyTouch>();
				if (easyTouch2 != null)
				{
					easyTouch2.enable = false;
				}
				SetEventSystemEnable(false);
			}
		}
	}
}
