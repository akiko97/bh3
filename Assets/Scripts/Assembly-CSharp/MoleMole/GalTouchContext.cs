using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class GalTouchContext : BaseWidgetContext
	{
		private const string FULL_LEVEL_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/Sakura";

		private MonoGalTouchView _galTouchView;

		private GameObject _fullGoodFeelEffect;

		private bool _setup;

		public GalTouchContext()
		{
			config = new ContextPattern
			{
				contextName = "GalTouchContext",
				viewPrefabPath = "UI/Menus/Dialog/Impression/Impression",
				ignoreNotify = false,
				cacheType = ViewCacheType.DontCache
			};
			uiType = UIType.SuspendBar;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.EnterMainPage)
			{
				OnEnterMainPage();
			}
			if (ntf.type == NotifyTypes.ExitMainPage)
			{
				OnExitMainPage();
			}
			return false;
		}

		protected override bool SetupView()
		{
			Singleton<GalTouchModule>.Instance.GalTouchInfoChanged += OnGalTouchInfoChanged;
			_galTouchView = base.view.GetComponent<MonoGalTouchView>();
			if (_galTouchView != null)
			{
				_galTouchView.Upgrade += OnViewUpgrade;
			}
			Singleton<NotifyManager>.Instance.RegisterContext(this);
			_setup = true;
			base.view.SetActive(false);
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Bottom/Button").GetComponent<Button>(), OnAvatarReplaceButtonClick);
		}

		private void OnAvatarReplaceButtonClick()
		{
			if (_galTouchView.shown)
			{
				base.view.SetActive(false);
				Singleton<MainUIManager>.Instance.ShowPage(new AvatarOverviewPageContext
				{
					type = AvatarOverviewPageContext.PageType.GalTouchReplace,
					selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
				});
			}
		}

		private void ActiveFullLevelEffect(bool active)
		{
			if (_fullGoodFeelEffect == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("UI/Menus/Widget/Storage/Sakura");
				if (gameObject != null)
				{
					_fullGoodFeelEffect = Object.Instantiate(gameObject);
				}
			}
			if (_fullGoodFeelEffect != null)
			{
				_fullGoodFeelEffect.SetActive(active);
			}
		}

		public void Close()
		{
			if (_setup)
			{
				Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= OnGalTouchInfoChanged;
				Singleton<NotifyManager>.Instance.RemoveContext(this);
				_setup = false;
			}
		}

		public override void Destroy()
		{
			Close();
			base.Destroy();
		}

		public void OnGalTouchInfoChanged(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType)
		{
			if (!(_galTouchView != null))
			{
				return;
			}
			UpdateHint(limitType);
			int num = GalTouchData.QueryLevelUpFeelNeed(oldHeartLevel);
			float num2 = 0f;
			if (num != 0)
			{
				num2 = (float)oldGoodFeel / (float)num;
			}
			num2 += (float)oldHeartLevel;
			int num3 = GalTouchData.QueryLevelUpFeelNeed(newHeartLevel);
			float num4 = 0f;
			if (num3 != 0)
			{
				num4 = (float)newGoodFeel / (float)num3;
			}
			num4 += (float)newHeartLevel;
			string additionalText = string.Empty;
			int avatarGalTouchBuffId = Singleton<GalTouchModule>.Instance.GetAvatarGalTouchBuffId(Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID());
			if (avatarGalTouchBuffId != 0)
			{
				TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(avatarGalTouchBuffId);
				if (touchBuffItem != null)
				{
					additionalText = LocalizationGeneralLogic.GetText(touchBuffItem.detail, GalTouchBuffData.GetCalculatedParam(touchBuffItem.param1, touchBuffItem.param1Add, newHeartLevel).ToString(), GalTouchBuffData.GetCalculatedParam(touchBuffItem.param2, touchBuffItem.param2Add, newHeartLevel).ToString(), GalTouchBuffData.GetCalculatedParam(touchBuffItem.param3, touchBuffItem.param3Add, newHeartLevel).ToString());
				}
			}
			_galTouchView.Show(num2, num4, newGoodFeel, additionalText);
		}

		private void UpdateHint(GoodFeelLimitType limitType)
		{
			switch (limitType)
			{
			case GoodFeelLimitType.DialyGoodFeel:
				_galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Dialy_Limit"));
				_galTouchView.SetHintVisible(true);
				break;
			case GoodFeelLimitType.Battle:
				_galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Battle_Limit"));
				_galTouchView.SetHintVisible(true);
				break;
			case GoodFeelLimitType.Mission:
				_galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Mission_Limit"));
				_galTouchView.SetHintVisible(true);
				break;
			default:
				_galTouchView.SetHintVisible(false);
				break;
			}
		}

		private void OnViewUpgrade()
		{
			ActiveFullLevelEffect(Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel() >= 5);
		}

		private void OnEnterMainPage()
		{
			ActiveFullLevelEffect(Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel() >= 5);
		}

		private void OnExitMainPage()
		{
			ActiveFullLevelEffect(false);
		}
	}
}
