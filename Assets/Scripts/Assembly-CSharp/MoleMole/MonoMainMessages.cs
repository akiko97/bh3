using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMainMessages : MonoBehaviour
	{
		public enum Status
		{
			Move = 0,
			Hold = 1,
			Idle = 2
		}

		private const float DEFAULT_HOLD_HEIGHT = 10f;

		public GameObject messageGO;

		private Transform _preMessage;

		private Transform _currentMessage;

		public float moveSpeed;

		public float fadeSpeed;

		private float _messageHoldTimeSpan;

		private float _messageHoldTimer;

		private Status status;

		private MainPageContext.PopShowMessage _currentMessageData;

		private Action _moveEndCallBack;

		private void Start()
		{
			_preMessage = null;
			_currentMessage = null;
			_currentMessageData = null;
			_messageHoldTimeSpan = MiscData.Config.ChatConfig.PopMessageHoldDuration;
			status = Status.Idle;
		}

		private void Update()
		{
			switch (status)
			{
			case Status.Move:
				if (_preMessage != null)
				{
					RectTransform component = _preMessage.GetComponent<RectTransform>();
					component.anchoredPosition = new Vector2(component.anchoredPosition.x, component.anchoredPosition.y + moveSpeed * Time.deltaTime);
					float alpha = component.GetComponent<CanvasGroup>().alpha;
					alpha = Mathf.Clamp(alpha - fadeSpeed * Time.deltaTime, 0f, 1f);
					component.GetComponent<CanvasGroup>().alpha = alpha;
					if (alpha < 0.1f)
					{
						UnityEngine.Object.Destroy(_preMessage.gameObject);
					}
				}
				if (_currentMessage != null)
				{
					RectTransform component2 = _currentMessage.GetComponent<RectTransform>();
					component2.anchoredPosition = new Vector2(component2.anchoredPosition.x, component2.anchoredPosition.y + moveSpeed * Time.deltaTime);
					if (component2.anchoredPosition.y >= 10f)
					{
						component2.anchoredPosition = new Vector2(component2.anchoredPosition.x, 10f);
						if (_preMessage != null)
						{
							UnityEngine.Object.Destroy(_preMessage.gameObject);
						}
						status = Status.Hold;
						_messageHoldTimer = 0f;
					}
				}
				if (_preMessage == null && _currentMessage == null)
				{
					status = Status.Idle;
				}
				break;
			case Status.Hold:
				_messageHoldTimer += Time.deltaTime;
				if (_messageHoldTimer >= _messageHoldTimeSpan)
				{
					status = Status.Move;
					_preMessage = _currentMessage;
					_preMessage.name = "preMessage";
					_currentMessage = null;
					_currentMessageData = null;
					if (_moveEndCallBack != null)
					{
						_moveEndCallBack();
					}
				}
				break;
			}
		}

		public void ShowMessage(MainPageContext.PopShowMessage message, Action moveEndCallBack = null)
		{
			if (_currentMessage != null)
			{
				_preMessage = _currentMessage;
				_preMessage.name = "preMessage";
			}
			_currentMessage = UnityEngine.Object.Instantiate(messageGO).transform;
			_currentMessage.SetParent(base.transform, false);
			_currentMessageData = message;
			string prefabPath = "SpriteOutput/ChatTypeBackground/PicChatWorldBg";
			if (_currentMessageData != null && _currentMessageData.source != MainPageContext.MessageSource.World)
			{
				if (_currentMessageData.source == MainPageContext.MessageSource.System)
				{
					prefabPath = "SpriteOutput/ChatTypeBackground/PicChatSystemBg";
				}
				else if (_currentMessageData.source == MainPageContext.MessageSource.Guild)
				{
					prefabPath = "SpriteOutput/ChatTypeBackground/PicChatGuildBg";
				}
				else if (_currentMessageData.source == MainPageContext.MessageSource.Friend)
				{
					prefabPath = "SpriteOutput/ChatTypeBackground/PicChatFriendBg";
				}
			}
			_currentMessage.transform.Find("BG/Panel").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
			RectTransform component = _currentMessage.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(component.anchoredPosition.x, 0f - (component.rect.height + 10f));
			component.transform.Find("Content/Text").GetComponent<Text>().text = message.message;
			component.transform.Find("Content/Text").GetComponent<Text>().supportRichText = true;
			component.transform.name = "currentMessage";
			_moveEndCallBack = moveEndCallBack;
			status = Status.Move;
		}

		public void OnMessagePanelClick()
		{
			if (!(_currentMessage != null) || _currentMessageData == null)
			{
				return;
			}
			if (_currentMessageData.source == MainPageContext.MessageSource.Friend)
			{
				if (_currentMessageData.talkingUid != 0)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext((int)_currentMessageData.talkingUid));
				}
			}
			else if (_currentMessageData.source == MainPageContext.MessageSource.World)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext());
			}
			else if (_currentMessageData.source != MainPageContext.MessageSource.Guild && _currentMessageData.source == MainPageContext.MessageSource.System)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext());
			}
		}
	}
}
