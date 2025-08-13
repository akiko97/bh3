using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class DragObject : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
	{
		private enum Status
		{
			Default = 0,
			Drag = 1,
			BoundBack = 2,
			Identify = 3
		}

		private const float FADE_OUT_TIME = 0.3f;

		private const float BOUND_BACK_TIME = 0.2f;

		private const float dragObjectBeginY = 241f;

		private const float maxDeltaY = 300f;

		private const float defaultMaskHeight = 609f;

		private const float maskHeightOrig = 720f;

		private const float maxDragSpeed = 600f;

		private const float _acceleration = 400f;

		public Transform pageTrans;

		public RectTransform dragObject;

		public RectTransform maskRect;

		public RectTransform[] dragSequence;

		public string successAudioName;

		private Status _status;

		private float _fadeOutTimer;

		private float _boundBackDelta;

		private float _boundBackTimer;

		private float dragSpeed;

		private Vector2 _targetPos;

		private Vector2 _beginPoint;

		private Vector2 _beginDragPoint;

		private float[] _sequenceOffset;

		private StorageDataItemBase _item;

		private bool _isIdentifySucc;

		public void Init(StorageDataItemBase item)
		{
			_item = item;
			_isIdentifySucc = false;
			_status = Status.Default;
			SetInfoActive(true);
			pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(false);
			maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720f);
			dragObject.anchoredPosition = new Vector2(dragObject.anchoredPosition.x, 241f);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, true));
		}

		public void Update()
		{
			if (_status == Status.Drag)
			{
				BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
				if (mainCanvas == null)
				{
					return;
				}
				float scaleFactor = mainCanvas.GetComponent<Canvas>().scaleFactor;
				float num = (_targetPos.y - _beginDragPoint.y) / scaleFactor - (dragObject.anchoredPosition.y - _beginPoint.y);
				if (num < 0f)
				{
					dragSpeed += Time.unscaledDeltaTime * 400f;
					dragSpeed = Mathf.Min(600f, dragSpeed);
				}
				else
				{
					dragSpeed = 0f;
				}
				float num2 = Mathf.Max(num, (0f - dragSpeed) * Time.unscaledDeltaTime) + dragObject.anchoredPosition.y;
				if (num2 > 241f)
				{
					num2 = 241f;
				}
				dragObject.anchoredPosition = new Vector2(dragObject.anchoredPosition.x, num2);
				for (int i = 0; i < _sequenceOffset.Length; i++)
				{
					float num3 = ((!(num < 0f)) ? 0f : ((1f - 1f / Mathf.Pow(2f, i + 1)) * num));
					dragSequence[i].anchoredPosition = new Vector2(dragSequence[i].anchoredPosition.x, _sequenceOffset[i] + num3);
				}
				UpdateImageByDelta(num2 - 241f);
				if (num2 <= -59f)
				{
					Singleton<NetworkManager>.Instance.RequestIdentifyStigmataAffix(_item);
					_status = Status.Identify;
					_fadeOutTimer = 0f;
					pageTrans.Find("Info/Figure/IdentifySuccEffect").GetComponent<ParticleSystem>().Play();
					if (!string.IsNullOrEmpty(successAudioName))
					{
						Singleton<WwiseAudioManager>.Instance.Post(successAudioName);
					}
				}
			}
			else if (_status == Status.Identify)
			{
				_fadeOutTimer += Time.unscaledDeltaTime;
				if (_fadeOutTimer <= 0.3f)
				{
					float num4 = 0f - Mathf.Lerp(300f, 720f, _fadeOutTimer / 0.3f);
					dragObject.anchoredPosition = new Vector2(dragObject.anchoredPosition.x, 241f + num4);
					for (int j = 0; j < _sequenceOffset.Length; j++)
					{
						dragSequence[j].anchoredPosition = new Vector2(dragSequence[j].anchoredPosition.x, _sequenceOffset[j]);
					}
					UpdateImageByDelta(num4);
				}
				else if (_isIdentifySucc)
				{
					Reset();
					Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshStigmataDetailView));
				}
			}
			else
			{
				if (_status != Status.BoundBack)
				{
					return;
				}
				_boundBackTimer += Time.unscaledDeltaTime;
				if (_boundBackTimer <= 0.2f)
				{
					float num5 = 0f - Mathf.Lerp(_boundBackDelta, 0f, _boundBackTimer / 0.2f);
					dragObject.anchoredPosition = new Vector2(dragObject.anchoredPosition.x, 241f + num5);
					for (int k = 0; k < _sequenceOffset.Length; k++)
					{
						dragSequence[k].anchoredPosition = new Vector2(dragSequence[k].anchoredPosition.x, _sequenceOffset[k]);
					}
					UpdateImageByDelta(num5);
				}
				else
				{
					Reset();
				}
			}
		}

		public void OnIdentifyStigmataAffixSucc()
		{
			_isIdentifySucc = true;
		}

		public void OnBeginDrag(PointerEventData pointerEventData)
		{
			_beginPoint = dragObject.anchoredPosition;
			_beginDragPoint = pointerEventData.position;
			_sequenceOffset = new float[dragSequence.Length];
			for (int i = 0; i < dragSequence.Length; i++)
			{
				_sequenceOffset[i] = dragSequence[i].anchoredPosition.y;
			}
			SetInfoActive(false);
			pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(true);
			pageTrans.Find("Info/Figure/PrefContainer").gameObject.SetActive(true);
			UpdateImageByDelta(0f);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, false));
			_status = Status.Drag;
			_targetPos = pointerEventData.position;
			dragSpeed = 0f;
		}

		public void OnDrag(PointerEventData pointerEventData)
		{
			if (_status == Status.Drag)
			{
				_targetPos = pointerEventData.position;
			}
		}

		public void OnEndDrag(PointerEventData pointerEventData)
		{
			if (_status == Status.Drag)
			{
				float num = pointerEventData.position.y - _beginDragPoint.y;
				if (num < 0f)
				{
					_status = Status.BoundBack;
					_boundBackDelta = Mathf.Abs(dragObject.anchoredPosition.y - 241f);
					_boundBackTimer = 0f;
				}
				else
				{
					Reset();
				}
			}
		}

		private void Reset()
		{
			_status = Status.Default;
			dragObject.anchoredPosition = new Vector2(dragObject.anchoredPosition.x, 241f);
			SetInfoActive(true);
			pageTrans.Find("Info/IdentifyNotice").gameObject.SetActive(false);
			pageTrans.Find("Info/Figure/PrefContainer").gameObject.SetActive(false);
			maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720f);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetPlayerStatusWidgetDisplay, true));
		}

		public void SetInfoActive(bool active)
		{
			List<string> list = new List<string>();
			list.Add("Info/Content");
			list.Add("Attributes");
			list.Add("Skills");
			list.Add("Lv");
			list.Add("ActionBtns");
			list.Add("Info/IdentifyBtn/Text");
			List<string> list2 = list;
			int i = 0;
			for (int count = list2.Count; i < count; i++)
			{
				pageTrans.Find(list2[i]).gameObject.SetActive(active);
			}
		}

		public void UpdateImageByDelta(float delta)
		{
			maskRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 609f + delta);
		}
	}
}
