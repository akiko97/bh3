using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarUnlockDialogContext : BaseSequenceDialogContext
	{
		private const float TIMER_SPAN = 1f;

		private int _avatarID;

		private AvatarDataItem _avatarData;

		private bool _enableClipZone;

		private CanvasTimer _timer;

		private SequenceAnimationManager _animationManager;

		public AvatarUnlockDialogContext(int avatarID, bool enableClipZone = false)
		{
			config = new ContextPattern
			{
				contextName = "AvatarUnlockDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarUnlockDialog",
				cacheType = ViewCacheType.DontCache
			};
			_avatarID = avatarID;
			_avatarData = Singleton<AvatarModule>.Instance.GetAvatarByID(_avatarID);
			_enableClipZone = enableClipZone;
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(1f, 0f);
			_timer.timeUpCallback = OnBGClick;
			_timer.StopRun();
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ClipZone").GetComponent<Button>(), OnBGClick);
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager(StartTimer);
			base.view.transform.Find("Dialog/Content/AnimMoveIn1/Portrait").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.IconPath);
			base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/Title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_UnlockNewAvatar");
			base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/ClassName/FirstName").GetComponent<Text>().text = _avatarData.ClassFirstName;
			base.view.transform.Find("Dialog/Content/AnimMoveIn2/NameRow/ClassName/LastName").GetComponent<Text>().text = _avatarData.ClassLastName;
			base.view.transform.Find("Dialog/Content/AnimMoveIn3/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(_avatarData.star);
			base.view.transform.Find("Dialog/Content/AnimMoveIn3/ShortNameRow/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.AttributeIconPath);
			Text component = base.view.transform.Find("Dialog/Content/AnimMoveIn3/ShortNameRow/ShortName").GetComponent<Text>();
			component.text = _avatarData.ShortName;
			component.color = GetFontColorByAttribute(_avatarData.Attribute);
			_animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
			base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(OnDialogBGGrowEnd);
			base.view.transform.Find("ClipZone").gameObject.SetActive(_enableClipZone);
			return false;
		}

		private void OnBGClick()
		{
			Destroy();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}

		private Color GetFontColorByAttribute(int attr)
		{
			Color color = Color.white;
			switch (attr)
			{
			case 1:
				UIUtil.TryParseHexString("#fedf4cFF", out color);
				break;
			case 2:
				UIUtil.TryParseHexString("#fc4399FF", out color);
				break;
			case 3:
				UIUtil.TryParseHexString("#43c6fcFF", out color);
				break;
			}
			return color;
		}

		private void OnDialogBGGrowEnd()
		{
			_animationManager.StartPlay();
		}

		private void StartTimer()
		{
			_timer.StartRun();
		}
	}
}
