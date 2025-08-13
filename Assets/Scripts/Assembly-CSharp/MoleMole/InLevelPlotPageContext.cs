using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class InLevelPlotPageContext : BasePageContext
	{
		private MonoStoryScreen _storyScreen;

		private int _plotID;

		private List<DialogMetaData> _dialogList;

		private int _dialogIndex = -1;

		private PlotMetaData _plotData;

		private Coroutine _currentChatCoroutine;

		private int _currentChatIndex;

		private DialogDataItem _currentDialogDataItem;

		public InLevelPlotPageContext(MonoStoryScreen storyScreen, int plotID, GameObject view = null)
		{
			config = new ContextPattern
			{
				contextName = "InLevelPlotPageContext",
				viewPrefabPath = "UI/Menus/Page/InLevel/InLevelPlotPage"
			};
			base.view = view;
			_storyScreen = storyScreen;
			_plotID = plotID;
			_dialogList = new List<DialogMetaData>();
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.OnPlotFinished)
			{
				return OnPlotFinished();
			}
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
		}

		public override void Destroy()
		{
			base.Destroy();
			_dialogList.Clear();
			_dialogIndex = -1;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("PlotDialog/Button").GetComponent<Button>(), OnSkipBtnClick);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
		}

		protected override bool SetupView()
		{
			ReadPlotData();
			bool flag = false;
			if (Singleton<LevelModule>.Instance.ContainLevelById(_plotData.levelID))
			{
				flag = true;
				bool flag2 = Singleton<LevelPlotModule>.Instance.IsPlotFinished(_plotID);
				flag = flag && flag2;
			}
			else
			{
				flag = true;
			}
			SetSkipButtonEnabled(flag);
			MonoStoryScreen storyScreen = _storyScreen;
			storyScreen.onOpenAnimationChange = (Action<bool>)Delegate.Combine(storyScreen.onOpenAnimationChange, new Action<bool>(OnOpenAnimChange));
			_storyScreen.Typewritter.myEvent.AddListener(OnChatFinished);
			return false;
		}

		public void OnSkipBtnClick()
		{
			ShowConfirmSkipDialog();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			if (!_storyScreen.IsDialogProcessingOpen())
			{
				if (_storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Displaying)
				{
					SkipDialog();
				}
				else if (_storyScreen.StoryDialogState == MonoStoryScreen.DialogState.DialogEnd)
				{
					ShowNextDialog();
				}
				else if (_storyScreen.StoryDialogState == MonoStoryScreen.DialogState.ChatEnd)
				{
					ShowNextChat();
				}
			}
		}

		private void ReadPlotData()
		{
			PlotMetaData plotMetaData = (_plotData = PlotMetaDataReader.GetPlotMetaDataByKey(_plotID));
			for (int i = plotMetaData.startDialogID; i <= plotMetaData.endDialogID; i++)
			{
				DialogMetaData dialogMetaDataByKey = DialogMetaDataReader.GetDialogMetaDataByKey(i);
				_dialogList.Add(dialogMetaDataByKey);
			}
		}

		private void OnOpenAnimChange(bool openState)
		{
			if (openState)
			{
				ShowNextDialog();
				MonoStoryScreen storyScreen = _storyScreen;
				storyScreen.onOpenAnimationChange = (Action<bool>)Delegate.Remove(storyScreen.onOpenAnimationChange, new Action<bool>(OnOpenAnimChange));
			}
		}

		private void OnChatFinished()
		{
			if (_currentChatIndex < _currentDialogDataItem.plotChatNodeList.Count)
			{
				_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
			}
			else
			{
				_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
			}
		}

		private void ShowNextDialog()
		{
			_dialogIndex++;
			if (_currentChatCoroutine != null)
			{
				Singleton<LevelManager>.Instance.levelEntity.StopCoroutine(_currentChatCoroutine);
			}
			if (_dialogIndex >= 0 && _dialogIndex < _dialogList.Count)
			{
				DialogMetaData dialogMetaData = _dialogList[_dialogIndex];
				_currentDialogDataItem = new DialogDataItem(dialogMetaData);
				int avatarID = dialogMetaData.avatarID;
				MonoStoryScreen.SelectScreenSide screenSide = (MonoStoryScreen.SelectScreenSide)dialogMetaData.screenSide;
				_storyScreen.RefreshAvatar3dModel(avatarID, screenSide);
				_storyScreen.RefreshCurrentSpeakerWidgets(_currentDialogDataItem);
				_currentChatIndex = 0;
				if (_currentDialogDataItem.plotChatNodeList.Count > 1)
				{
					_currentChatCoroutine = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(WaitAllChatsDone());
				}
				else
				{
					RefreshChatMsg(_currentDialogDataItem.plotChatNodeList[_currentChatIndex].chatContent, _currentDialogDataItem.plotChatNodeList[_currentChatIndex].chatDuration);
					_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
					_currentChatIndex++;
				}
				Singleton<WwiseAudioManager>.Instance.Post(_currentDialogDataItem.audio);
			}
			else
			{
				QuitPlot();
			}
		}

		private IEnumerator WaitAllChatsDone()
		{
			while (_currentChatIndex < _currentDialogDataItem.plotChatNodeList.Count)
			{
				if (_storyScreen.StoryDialogState == MonoStoryScreen.DialogState.ChatEnd || _storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Default)
				{
					RefreshChatMsg(_currentDialogDataItem.plotChatNodeList[_currentChatIndex].chatContent, _currentDialogDataItem.plotChatNodeList[_currentChatIndex].chatDuration);
					float duration = _currentDialogDataItem.plotChatNodeList[_currentChatIndex].chatDuration;
					_currentChatIndex++;
					if (_currentChatIndex >= _currentDialogDataItem.plotChatNodeList.Count)
					{
						_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
						yield break;
					}
					_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
					yield return new WaitForSeconds(duration);
				}
				_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
			}
			_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
		}

		private void RefreshChatMsg(string chatContentKey, float chatDuration)
		{
			string text = LocalizationGeneralLogic.GetText(chatContentKey);
			_storyScreen.SetDisplayText(text);
		}

		private void ShowNextChat()
		{
			if (_currentChatIndex < _currentDialogDataItem.plotChatNodeList.Count)
			{
				if (_currentChatCoroutine != null)
				{
					Singleton<LevelManager>.Instance.levelEntity.StopCoroutine(_currentChatCoroutine);
				}
				_currentChatCoroutine = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(WaitAllChatsDone());
			}
		}

		private void ShowConfirmSkipDialog()
		{
			GeneralDialogContext generalDialogContext = new GeneralDialogContext();
			generalDialogContext.type = GeneralDialogContext.ButtonType.DoubleButton;
			generalDialogContext.title = LocalizationGeneralLogic.GetText("Skip_Confirm_Title");
			generalDialogContext.desc = LocalizationGeneralLogic.GetText("Skip_Confirm_Content");
			generalDialogContext.notDestroyAfterTouchBG = true;
			generalDialogContext.notDestroyAfterCallback = false;
			generalDialogContext.buttonCallBack = delegate(bool confirmed)
			{
				if (confirmed)
				{
					SkipPlot();
				}
			};
			GeneralDialogContext dialogContext = generalDialogContext;
			Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
		}

		private void SetSkipButtonEnabled(bool enabled)
		{
			base.view.transform.Find("PlotDialog/Button").gameObject.SetActive(enabled);
		}

		private void SkipDialog()
		{
			if (_storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Displaying)
			{
				_storyScreen.SkipDialog();
				if (_currentChatIndex < _currentDialogDataItem.plotChatNodeList.Count)
				{
					_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
				}
				else
				{
					_storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
				}
			}
		}

		private void SkipPlot()
		{
			QuitPlot();
		}

		private void QuitPlot()
		{
			MainCameraStoryState storyState = Singleton<CameraManager>.Instance.GetMainCamera().storyState;
			if (storyState != null && storyState.active)
			{
				storyState.StartQuit();
				SetThisPlotIDFinished();
				Singleton<WwiseAudioManager>.Instance.Post("UI_StoryScreen_Close");
			}
		}

		private void SetThisPlotIDFinished()
		{
			if (Singleton<LevelPlotModule>.Instance.GetUnFinishedPlotIDList(_plotData.levelID).Contains(_plotData.plotID))
			{
				Singleton<LevelPlotModule>.Instance.MarkPlotIDFinish(_plotID);
			}
		}

		private bool OnPlotFinished()
		{
			Singleton<MainUIManager>.Instance.BackPage();
			Destroy();
			return false;
		}

		private bool OnSocketConnect()
		{
			return false;
		}

		private bool OnSocketDisconnect()
		{
			return false;
		}
	}
}
