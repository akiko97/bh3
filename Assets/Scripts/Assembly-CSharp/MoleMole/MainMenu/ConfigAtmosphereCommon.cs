using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class ConfigAtmosphereCommon
	{
		[Range(0f, 100f)]
		public float PlaybackSpeed;

		[Tooltip("The max time (in second) in which transit from a frame to next ")]
		public float TransitionTime;

		public ConfigCloudScene[] SceneList;

		public Texture Tex;

		public Texture SecondTex;

		private int _sceneId;

		public int SceneId
		{
			get
			{
				return _sceneId;
			}
			set
			{
				_sceneId = value;
			}
		}

		public string ScneneName
		{
			get
			{
				if (SceneList.Length == 0)
				{
					return string.Empty;
				}
				return SceneList[_sceneId].Name;
			}
		}

		public string[] SceneNameList
		{
			get
			{
				string[] array = new string[SceneList.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = SceneList[i].Name;
				}
				return array;
			}
		}

		public void InitAfterLoad()
		{
		}

		public void UpdateSceneRandomly()
		{
			if (SceneList.Length != 0)
			{
				int[] array = new int[SceneList.Length];
				for (int i = 0; i < SceneList.Length; i++)
				{
					array[i] = SceneList[i].ChooseRate;
				}
				_sceneId = ConfigAtmosphereUtil.ChooseRandomly(array);
			}
		}

		public int GetSceneIdRandomly()
		{
			if (SceneList.Length == 0)
			{
				return 0;
			}
			int[] array = new int[SceneList.Length];
			for (int i = 0; i < SceneList.Length; i++)
			{
				array[i] = SceneList[i].ChooseRate;
			}
			return ConfigAtmosphereUtil.ChooseRandomly(array);
		}

		public void UpdateSceneNameNext()
		{
			_sceneId++;
			if (_sceneId >= SceneList.Length)
			{
				_sceneId = 0;
			}
		}
	}
}
