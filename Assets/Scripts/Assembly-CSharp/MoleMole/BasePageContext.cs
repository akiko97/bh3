using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public abstract class BasePageContext : BaseContext
	{
		public List<BaseDialogContext> dialogContextList;

		protected bool showSpaceShip;

		public BasePageContext()
		{
			uiType = UIType.Page;
			dialogContextList = new List<BaseDialogContext>();
			showSpaceShip = false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			if (dialogContextList.Count > 0)
			{
				List<BaseDialogContext> list = new List<BaseDialogContext>();
				foreach (BaseDialogContext dialogContext in dialogContextList)
				{
					if (!dialogContext.NeedRecoverWhenPageStartUp())
					{
						list.Add(dialogContext);
					}
				}
				foreach (BaseDialogContext item in list)
				{
					item.Destroy();
				}
			}
			HandleBackButton();
			HandleSpaceShip();
			base.StartUp(canvasTrans, viewParent);
			if (dialogContextList.Count > 0)
			{
				List<BaseDialogContext> list2 = new List<BaseDialogContext>(dialogContextList);
				foreach (BaseDialogContext item2 in list2)
				{
					if (!(item2 is NewbieDialogContext))
					{
						item2.StartUp(canvasTrans, base.view.transform.parent);
					}
				}
			}
			if (spaceShipVisible() && Singleton<PlayerModule>.Instance != null && Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasLandedMainPage)
			{
				UIUtil.SpaceshipCheckWeather();
			}
		}

		public override void SetActive(bool enabled)
		{
			BaseDialogContext[] array = dialogContextList.ToArray();
			foreach (BaseDialogContext baseDialogContext in array)
			{
				baseDialogContext.SetActive(enabled);
			}
			base.SetActive(enabled);
		}

		public override void Destroy()
		{
			if (dialogContextList.Count > 0)
			{
				BaseDialogContext[] array = dialogContextList.ToArray();
				foreach (BaseDialogContext baseDialogContext in array)
				{
					baseDialogContext.Destroy();
				}
			}
			base.Destroy();
		}

		public virtual void OnLandedFromBackPage()
		{
			HandleBackButton();
			HandleSpaceShip();
		}

		private void HandleBackButton()
		{
			bool flag = config.contextName != "MainPageContext";
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetBackButtonActive, flag));
		}

		private void HandleSpaceShip()
		{
			if (showSpaceShip)
			{
				MonoMainCanvas monoMainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
				if (monoMainCanvas != null)
				{
					monoMainCanvas.InitMainPageContexts();
				}
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(showSpaceShip, false)));
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipLight, config.contextName == "MainPageContext"));
		}

		public virtual void BackPage()
		{
			Singleton<MainUIManager>.Instance.BackPage();
		}

		public virtual void BackToMainMenuPage()
		{
			Singleton<MainUIManager>.Instance.BackToMainMenuPage();
		}

		public NewbieDialogContext TryToGetNewbieDialogContext()
		{
			foreach (BaseDialogContext dialogContext in dialogContextList)
			{
				if (dialogContext != null && dialogContext is NewbieDialogContext && dialogContext.view != null && dialogContext.view.transform != null && dialogContext.view.transform.gameObject != null)
				{
					return (NewbieDialogContext)dialogContext;
				}
			}
			return null;
		}

		public bool CheckHasDialogExceptNewbie(bool ignoreDialogInPage = false)
		{
			foreach (BaseDialogContext dialogContext in dialogContextList)
			{
				if (dialogContext != null && !(dialogContext is NewbieDialogContext) && dialogContext.view != null && dialogContext.view.transform != null && dialogContext.view.transform.gameObject != null)
				{
					if (!ignoreDialogInPage)
					{
						return true;
					}
					if (dialogContext.view.transform.root == dialogContext.view.transform.parent)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool spaceShipVisible()
		{
			return showSpaceShip;
		}

		public void Clear()
		{
			foreach (BaseDialogContext dialogContext in dialogContextList)
			{
				dialogContext.Destroy();
			}
			dialogContextList.Clear();
			Destroy();
		}
	}
}
