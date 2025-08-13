using System;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	public class MonoTheLevelV1 : MonoBehaviour
	{
		protected virtual void CreateLevelManager()
		{
			Singleton<LevelManager>.Create();
			MonoLevelEntity monoLevelEntity = (Singleton<LevelManager>.Instance.levelEntity = base.gameObject.AddComponent<MonoLevelEntity>());
			monoLevelEntity.Init(562036737u);
			Singleton<LevelManager>.Instance.levelActor = Singleton<EventManager>.Instance.CreateActor<LevelActor>(monoLevelEntity);
			Singleton<LevelManager>.Instance.levelActor.PostInit();
		}

		public void Awake()
		{
			CreateLevelManager();
			Singleton<LevelManager>.Instance.InitAtAwake();
		}

		public void Start()
		{
			Singleton<LevelManager>.Instance.InitAtStart();
			GraphicsSettingData.ApplySettingConfig();
			AudioSettingData.ApplySettingConfig();
		}

		public void Update()
		{
			Singleton<LevelManager>.Instance.Core();
		}

		public void OnDestroy()
		{
			if (Singleton<WwiseAudioManager>.Instance != null)
			{
				Singleton<WwiseAudioManager>.Instance.PopSoundBankScale();
				Singleton<MainMenuBGM>.Instance.TryEnterMainMenu();
			}
			Singleton<LevelManager>.Instance.Destroy();
			Singleton<LevelManager>.Destroy();
		}

		private void OnApplicationQuit()
		{
			if (Singleton<LevelManager>.Instance != null)
			{
				Singleton<LevelManager>.Instance.levelActor.SuddenLevelEnd();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void AttachLabelToTransform(Transform target, Vector3 offset, Func<string> textCallback)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public void PopupLabelToTransform(Transform target, Vector3 offset, string text, float duration = 2f)
		{
		}
	}
}
