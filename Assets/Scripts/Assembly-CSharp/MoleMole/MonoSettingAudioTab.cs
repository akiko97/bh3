using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSettingAudioTab : MonoBehaviour
	{
		public Transform[] BGMVolumeBtns;

		public Transform[] soundEffectVolumeBtns;

		public Transform[] voiceVolumeBtns;

		public Transform[] BGMProcessPiecesGroups;

		public Transform[] soundEffectProcessPiecesGroups;

		public Transform[] voiceProcessPiecesGroups;

		public Transform[] cvLanguageGroups;

		public string[] cvLanguageNames;

		private ConfigAudioSetting _modifiedSettingConfig;

		public void SetupView()
		{
			_modifiedSettingConfig = new ConfigAudioSetting();
			RecoverOriginState();
		}

		public void OnBGMVolumeClick(int index)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_BGM", index);
			_modifiedSettingConfig.BGMVolume = index;
			SetVolumeBtns(BGMVolumeBtns, (int)_modifiedSettingConfig.BGMVolume);
			SetProcessPieces(BGMProcessPiecesGroups, (int)_modifiedSettingConfig.BGMVolume);
		}

		public void OnSoundEffectVolumClick(int index)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_SE", index);
			_modifiedSettingConfig.SoundEffectVolume = index;
			SetVolumeBtns(soundEffectVolumeBtns, (int)_modifiedSettingConfig.SoundEffectVolume);
			SetProcessPieces(soundEffectProcessPiecesGroups, (int)_modifiedSettingConfig.SoundEffectVolume);
		}

		public void OnVoiceVolumClick(int index)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam("Vol_Voice", index);
			_modifiedSettingConfig.VoiceVolume = index;
			SetVolumeBtns(voiceVolumeBtns, (int)_modifiedSettingConfig.VoiceVolume);
			SetProcessPieces(voiceProcessPiecesGroups, (int)_modifiedSettingConfig.VoiceVolume);
		}

		public void OnCVLanguageBtnClick(int index)
		{
			if (index >= 0 && index < cvLanguageNames.Length && !(_modifiedSettingConfig.CVLanguage == cvLanguageNames[index]))
			{
				_modifiedSettingConfig.CVLanguage = cvLanguageNames[index];
				SetLanguageBtns();
			}
		}

		public void OnNoSaveBtnClick()
		{
			RecoverOriginState();
		}

		public void OnSaveBtnClick()
		{
			AudioSettingData.SavePersonalConfig(_modifiedSettingConfig);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess")));
		}

		public bool CheckNeedSave()
		{
			return !AudioSettingData.IsValueEqualToPersonalAudioConfig(_modifiedSettingConfig);
		}

		private void RecoverOriginState()
		{
			AudioSettingData.ApplySettingConfig();
			AudioSettingData.CopyPersonalAudioConfig(ref _modifiedSettingConfig);
			SetVolumeBtns(BGMVolumeBtns, (int)_modifiedSettingConfig.BGMVolume);
			SetVolumeBtns(soundEffectVolumeBtns, (int)_modifiedSettingConfig.SoundEffectVolume);
			SetVolumeBtns(voiceVolumeBtns, (int)_modifiedSettingConfig.VoiceVolume);
			SetProcessPieces(BGMProcessPiecesGroups, (int)_modifiedSettingConfig.BGMVolume);
			SetProcessPieces(soundEffectProcessPiecesGroups, (int)_modifiedSettingConfig.SoundEffectVolume);
			SetProcessPieces(voiceProcessPiecesGroups, (int)_modifiedSettingConfig.VoiceVolume);
			SetLanguageBtns();
		}

		private void SetVolumeBtns(Transform[] volumeBtns, int volume)
		{
			for (int i = 0; i < volume; i++)
			{
				Transform transform = volumeBtns[i];
				transform.GetComponent<Image>().color = MiscData.GetColor("Blue");
				transform.Find("Check").gameObject.SetActive(false);
				transform.Find("Text").gameObject.SetActive(true);
			}
			volumeBtns[volume].GetComponent<Image>().color = MiscData.GetColor("Blue");
			volumeBtns[volume].Find("Check").gameObject.SetActive(true);
			volumeBtns[volume].Find("Text").gameObject.SetActive(false);
			for (int j = volume + 1; j < volumeBtns.Length; j++)
			{
				Transform transform2 = volumeBtns[j];
				transform2.GetComponent<Image>().color = MiscData.GetColor("TextGrey");
				transform2.Find("Check").gameObject.SetActive(false);
				transform2.Find("Text").gameObject.SetActive(true);
			}
		}

		private void SetProcessPieces(Transform[] processPiecesGroups, int volume)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < volume; i++)
			{
				Transform transform = processPiecesGroups[i];
				foreach (Transform item in transform)
				{
					list.Add(item);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Transform transform2 = list[j];
				transform2.GetComponent<Image>().color = Color.Lerp(MiscData.GetColor("AudioSettingPieceYellow"), MiscData.GetColor("Blue"), (float)j / (float)list.Count);
			}
			for (int k = volume; k < processPiecesGroups.Length; k++)
			{
				Transform transform3 = processPiecesGroups[k];
				foreach (Transform item2 in transform3)
				{
					item2.GetComponent<Image>().color = MiscData.GetColor("TextGrey");
				}
			}
		}

		private void SetLanguageBtns()
		{
			int i = 0;
			for (int num = cvLanguageGroups.Length; i < num && i < cvLanguageNames.Length; i++)
			{
				Transform transform = cvLanguageGroups[i].Find("Check");
				Transform transform2 = cvLanguageGroups[i].Find("Blue");
				Transform transform3 = cvLanguageGroups[i].Find("Grey");
				if (!(transform == null))
				{
					bool flag = cvLanguageNames[i] == _modifiedSettingConfig.CVLanguage;
					transform.gameObject.SetActive(flag);
					transform2.gameObject.SetActive(flag);
					transform3.gameObject.SetActive(!flag);
				}
			}
		}
	}
}
