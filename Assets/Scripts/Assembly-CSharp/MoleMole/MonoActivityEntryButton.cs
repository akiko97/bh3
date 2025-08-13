using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoActivityEntryButton : MonoBehaviour
	{
		private ActivityDataItemBase _activityData;

		public bool selected { get; private set; }

		private void Awake()
		{
			base.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void Update()
		{
			Transform transform = base.transform.Find("LeftTime");
			if (transform.gameObject.activeSelf)
			{
				switch (_activityData.GetStatus())
				{
				case ActivityDataItemBase.Status.WaitToStart:
				{
					string label2;
					transform.Find("TimeValue").GetComponent<Text>().text = Miscs.GetTimeSpanToShow(_activityData.beginTime, out label2).ToString();
					transform.Find("Label").GetComponent<Text>().text = label2;
					break;
				}
				case ActivityDataItemBase.Status.InProgress:
				{
					string label;
					transform.Find("TimeValue").GetComponent<Text>().text = Miscs.GetTimeSpanToShow(_activityData.endTime, out label).ToString();
					transform.Find("Label").GetComponent<Text>().text = label;
					break;
				}
				default:
					transform.gameObject.SetActive(false);
					break;
				}
			}
		}

		public void SetupView(ActivityDataItemBase activityData)
		{
			_activityData = activityData;
			base.transform.Find("Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_activityData.GetActivityEnterImgPath());
			base.transform.Find("LeftTime").gameObject.SetActive(false);
			base.transform.Find("Icon/Status/Over").gameObject.SetActive(false);
			base.transform.Find("Icon/Status/Lock").gameObject.SetActive(false);
			base.transform.Find("Icon/Status/WaitToStart").gameObject.SetActive(false);
			string text = null;
			switch (_activityData.GetStatus())
			{
			case ActivityDataItemBase.Status.Over:
				base.transform.Find("Icon/Status/Over").gameObject.SetActive(true);
				break;
			case ActivityDataItemBase.Status.Locked:
				base.transform.Find("Icon/Status/Lock").gameObject.SetActive(true);
				text = "UI_Gen_Select_Negative";
				break;
			case ActivityDataItemBase.Status.WaitToStart:
				base.transform.Find("Icon/Status/WaitToStart").gameObject.SetActive(true);
				base.transform.Find("LeftTime").gameObject.SetActive(true);
				break;
			case ActivityDataItemBase.Status.InProgress:
				base.transform.Find("LeftTime").gameObject.SetActive(!(activityData is EndlessActivityDataItem));
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MonoButtonWwiseEvent monoButtonWwiseEvent = base.transform.Find("Button").GetComponent<MonoButtonWwiseEvent>();
				if (monoButtonWwiseEvent == null)
				{
					monoButtonWwiseEvent = base.transform.Find("Button").gameObject.AddComponent<MonoButtonWwiseEvent>();
				}
				if (monoButtonWwiseEvent != null)
				{
					monoButtonWwiseEvent.eventName = text;
				}
			}
		}

		public void OnClick()
		{
			MonoChapterScroller component = base.transform.parent.parent.GetComponent<MonoChapterScroller>();
			if (component.IsCenter(base.transform))
			{
				switch (_activityData.GetStatus())
				{
				case ActivityDataItemBase.Status.InProgress:
					if (_activityData is WeekDayActivityDataItem)
					{
						Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(_activityData as WeekDayActivityDataItem));
					}
					else if (_activityData is EndlessActivityDataItem)
					{
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RequestEnterEndlessActivity));
					}
					break;
				case ActivityDataItemBase.Status.Locked:
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", _activityData.GetMinPlayerLevelLimit())));
					break;
				case ActivityDataItemBase.Status.WaitToStart:
					break;
				}
			}
			else
			{
				component.ClickToChangeCenter(base.transform);
			}
		}

		public void UpdateView(bool isSelected)
		{
			selected = isSelected;
			base.transform.Find("Icon/Mask").gameObject.SetActive(!isSelected);
			base.transform.Find("Arrow").gameObject.SetActive(isSelected);
			base.transform.Find("Frame").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Top").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Bottom").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Left").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Right").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
		}
	}
}
