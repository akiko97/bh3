using UnityEngine;

namespace MoleMole
{
	public class MonoFriendChatList : MonoBehaviour
	{
		public enum Status
		{
			Open = 0,
			Close = 1,
			Opening = 2,
			Closing = 3
		}

		private const float OFFSET_Y = 34.5f;

		public float moveSpeed;

		public float fadeSpeed;

		private float openPosY;

		private float closePosY;

		private RectTransform rectTrans;

		private CanvasGroup canvasGroup;

		public Status status;

		private void Start()
		{
			status = Status.Close;
			rectTrans = base.transform.GetComponent<RectTransform>();
			canvasGroup = base.transform.GetComponent<CanvasGroup>();
			openPosY = 0f;
			closePosY = 0f - rectTrans.sizeDelta.y - 34.5f;
			moveSpeed = 1500f;
		}

		private void SetPosY(float posY)
		{
			base.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(base.transform.GetComponent<RectTransform>().anchoredPosition.x, posY);
		}

		private void SetOpacity(float opacity)
		{
			canvasGroup.alpha = opacity;
		}

		private float GetCurrentPosY()
		{
			return base.transform.GetComponent<RectTransform>().anchoredPosition.y;
		}

		public void OpenFriendChatList()
		{
			status = Status.Opening;
			SetPosY(closePosY);
		}

		public void CloseFriendChatList()
		{
			status = Status.Closing;
			SetPosY(openPosY);
		}

		private void OnOpened()
		{
			CanvasGroup component = base.transform.parent.GetComponent<CanvasGroup>();
			if (component != null)
			{
				base.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
		}

		private void OnClosed()
		{
			CanvasGroup component = base.transform.parent.GetComponent<CanvasGroup>();
			if (component != null)
			{
				base.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
			}
		}

		private void Update()
		{
			if (status == Status.Opening)
			{
				if (GetCurrentPosY() > openPosY)
				{
					status = Status.Open;
					SetPosY(openPosY);
					OnOpened();
				}
				SetPosY(GetCurrentPosY() + moveSpeed * Time.deltaTime);
			}
			if (status == Status.Closing)
			{
				if (GetCurrentPosY() < closePosY)
				{
					status = Status.Close;
					SetPosY(closePosY);
					OnClosed();
				}
				SetPosY(GetCurrentPosY() - moveSpeed * Time.deltaTime);
			}
			float num = (openPosY - GetCurrentPosY()) / (openPosY - closePosY);
			num = Mathf.Clamp(num * num, 0f, 1f);
			float opacity = Mathf.Lerp(0.8f, 0f, num);
			SetOpacity(opacity);
		}
	}
}
