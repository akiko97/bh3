using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoMemoryProfiler : MonoBehaviour
	{
		public enum E_AssetDetail
		{
			None = 0,
			Textures = 1,
			RenderTextures = 2,
			Meshes = 3,
			AnimationClips = 4,
			Meterials = 5
		}

		private static MonoMemoryProfiler _profiler;

		private uint _memory_texture;

		private uint _memory_renderTexture;

		private uint _memory_monoHeap;

		private uint _memory_monoUsed;

		private uint _memory_mesh;

		private uint _memory_animationClip;

		private uint _memory_material;

		private uint _memory_gameobject;

		private int _num_gameobject;

		private AssetList _assetList;

		private int _width = 600;

		private int _heigth = 800;

		private int _top = 80;

		private Vector2 _scale = new Vector2(2f, 2f);

		private int _leftOffset;

		private int _moveStep = 10;

		private Vector2 _pivot;

		private static float _autoSampleInterval = 2f;

		private float _timeEclapse;

		private E_AssetDetail _assetDetail;

		public static void CreateMemoryProfiler()
		{
			if (!(_profiler != null))
			{
				GameObject gameObject = new GameObject("MemoryProfilerGUI");
				gameObject.AddComponent<MonoMemoryProfiler>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_profiler = gameObject.GetComponent<MonoMemoryProfiler>();
			}
		}

		private void Start()
		{
			_assetList = new AssetList();
			_assetDetail = E_AssetDetail.Textures;
			DoProfiler();
		}

		private void OnGUI()
		{
			_pivot.x = Screen.width / 2;
			_pivot.y = 0f;
			GUIUtility.ScaleAroundPivot(_scale, _pivot);
			GUI.backgroundColor = Color.black;
			float value = (Screen.width - _width) / 2 + _leftOffset;
			value = Mathf.Clamp(value, 0f, Screen.width - _width);
			GUILayout.BeginArea(new Rect(value, _top, _width, _heigth));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (GUILayout.Button("Memory Profiler"))
			{
				DoDestroy();
			}
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button(string.Format("Normal Texture: {0} MB", _memory_texture)))
			{
				_assetDetail = E_AssetDetail.Textures;
			}
			if (GUILayout.Button(string.Format("Render Texture: {0} MB", _memory_renderTexture)))
			{
				_assetDetail = E_AssetDetail.RenderTextures;
			}
			if (GUILayout.Button(string.Format("Mesh:           {0} MB", _memory_mesh)))
			{
				_assetDetail = E_AssetDetail.Meshes;
			}
			if (GUILayout.Button(string.Format("AnimationClip:  {0} MB", _memory_animationClip)))
			{
				_assetDetail = E_AssetDetail.AnimationClips;
			}
			if (GUILayout.Button(string.Format("Material:       {0} MB", _memory_material)))
			{
				_assetDetail = E_AssetDetail.Meterials;
			}
			GUI.backgroundColor = Color.yellow;
			GUILayout.Button(string.Format("Mono Heap:      {0} MB", _memory_monoHeap));
			GUILayout.Button(string.Format("Mono Used:      {0} MB", _memory_monoUsed));
			GUILayout.Button(string.Format("{0} GameObjects: {1} MB", _num_gameobject, _memory_gameobject));
			GUI.backgroundColor = Color.black;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Left"))
			{
				_leftOffset -= _moveStep;
			}
			if (GUILayout.Button("Right"))
			{
				_leftOffset += _moveStep;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			GUI.backgroundColor = Color.gray;
			if (_assetDetail != E_AssetDetail.None)
			{
				foreach (AssetList.AssetItem item in _assetList.GetList())
				{
					GUILayout.Button(string.Format("{0} KB  {1}", ToKiloBytes(item._size), item._name));
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void Update()
		{
			_timeEclapse += Time.deltaTime;
			if (_timeEclapse > _autoSampleInterval)
			{
				_timeEclapse = 0f;
				DoProfiler();
			}
		}

		private void DoProfiler()
		{
			_assetList.Clear();
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Texture));
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Texture texture = (Texture)array2[i];
				uint runtimeMemorySize = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(texture);
				if (texture is RenderTexture)
				{
					_memory_renderTexture += runtimeMemorySize;
					if (_assetDetail == E_AssetDetail.RenderTextures)
					{
						_assetList.TryAdd(runtimeMemorySize, texture.name);
					}
				}
				else
				{
					_memory_texture += runtimeMemorySize;
					if (_assetDetail == E_AssetDetail.Textures)
					{
						_assetList.TryAdd(runtimeMemorySize, texture.name);
					}
				}
			}
			UnityEngine.Object[] array3 = Resources.FindObjectsOfTypeAll(typeof(Mesh));
			UnityEngine.Object[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				Mesh mesh = (Mesh)array4[j];
				uint runtimeMemorySize2 = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(mesh);
				_memory_mesh += runtimeMemorySize2;
				if (_assetDetail == E_AssetDetail.Meshes)
				{
					_assetList.TryAdd(runtimeMemorySize2, mesh.name);
				}
			}
			UnityEngine.Object[] array5 = Resources.FindObjectsOfTypeAll(typeof(AnimationClip));
			UnityEngine.Object[] array6 = array5;
			for (int k = 0; k < array6.Length; k++)
			{
				AnimationClip animationClip = (AnimationClip)array6[k];
				uint runtimeMemorySize3 = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(animationClip);
				_memory_animationClip += runtimeMemorySize3;
				if (_assetDetail == E_AssetDetail.AnimationClips)
				{
					_assetList.TryAdd(runtimeMemorySize3, animationClip.name);
				}
			}
			UnityEngine.Object[] array7 = Resources.FindObjectsOfTypeAll(typeof(Material));
			UnityEngine.Object[] array8 = array7;
			for (int l = 0; l < array8.Length; l++)
			{
				Material material = (Material)array8[l];
				uint runtimeMemorySize4 = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(material);
				_memory_material += runtimeMemorySize4;
				if (_assetDetail == E_AssetDetail.Meterials)
				{
					_assetList.TryAdd(runtimeMemorySize4, material.name);
				}
			}
			_assetList.Sort();
			_num_gameobject = 0;
			UnityEngine.Object[] array9 = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			UnityEngine.Object[] array10 = array9;
			for (int m = 0; m < array10.Length; m++)
			{
				GameObject o = (GameObject)array10[m];
				_num_gameobject++;
				uint runtimeMemorySize5 = (uint)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(o);
				_memory_gameobject += runtimeMemorySize5;
			}
			_memory_texture = ToMegaBytes(_memory_texture);
			_memory_renderTexture = ToMegaBytes(_memory_renderTexture);
			_memory_mesh = ToMegaBytes(_memory_mesh);
			_memory_animationClip = ToMegaBytes(_memory_animationClip);
			_memory_material = ToMegaBytes(_memory_material);
			_memory_monoHeap = ToMegaBytes(UnityEngine.Profiling.Profiler.GetMonoHeapSize());
			_memory_monoUsed = ToMegaBytes(UnityEngine.Profiling.Profiler.GetMonoUsedSize());
			_memory_gameobject = ToMegaBytes(_memory_gameobject);
		}

		private void DoDestroy()
		{
			if (!(_profiler == null))
			{
				UnityEngine.Object.Destroy(_profiler.gameObject);
				_profiler = null;
			}
		}

		private uint ToMegaBytes(uint _bytes)
		{
			return _bytes / 1024 / 1024;
		}

		private uint ToKiloBytes(uint _bytes)
		{
			return _bytes / 1024;
		}

		public static bool ParseCommand(string cmd)
		{
			if (!cmd.StartsWith("mem", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			_autoSampleInterval = 2f;
			if (cmd.Length > 3)
			{
				string s = cmd.Substring(3).Trim();
				float result;
				if (float.TryParse(s, out result))
				{
					_autoSampleInterval = result;
				}
			}
			return true;
		}
	}
}
