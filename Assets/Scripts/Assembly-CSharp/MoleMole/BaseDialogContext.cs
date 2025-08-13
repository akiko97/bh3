using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseDialogContext : BaseContext
	{
		public BasePageContext pageContext;

		public BaseDialogContext()
		{
			uiType = UIType.Dialog;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent)
		{
			base.StartUp(canvasTrans, viewParent);
			base.view.transform.SetAsLastSibling();
			CheckAndSetNewbieDialogAvailable();
		}

		public override void Destroy()
		{
			if (pageContext != null)
			{
				pageContext.dialogContextList.Remove(this);
			}
			base.Destroy();
			if (!(this is NewbieDialogContext))
			{
				CheckAndSetNewbieDialogAvailable();
			}
			Singleton<MainUIManager>.Instance.LockUI(false);
		}

		public virtual bool NeedRecoverWhenPageStartUp()
		{
			return true;
		}

		public override void SetActive(bool enabled)
		{
			base.SetActive(enabled);
			if (!(this is NewbieDialogContext))
			{
				CheckAndSetNewbieDialogAvailable();
			}
		}

		private void CheckAndSetNewbieDialogAvailable()
		{
			NewbieDialogContext newbieDialogContext = null;
			if (this is NewbieDialogContext)
			{
				newbieDialogContext = (NewbieDialogContext)this;
			}
			else if (pageContext != null)
			{
				newbieDialogContext = pageContext.TryToGetNewbieDialogContext();
			}
			if (newbieDialogContext == null || newbieDialogContext.view == null)
			{
				return;
			}
			Transform transform = null;
			if (pageContext.dialogContextList.Count > 0)
			{
				List<BaseDialogContext> list = new List<BaseDialogContext>();
				list.AddRange(pageContext.dialogContextList);
				list.Reverse();
				foreach (BaseDialogContext item in list)
				{
					if (item != null && item.view != null && item.view.transform != null && item.view.transform.gameObject != null)
					{
						transform = item.view.transform;
						break;
					}
				}
			}
			if (newbieDialogContext.view.transform != transform)
			{
				newbieDialogContext.SetAvailable(false);
				return;
			}
			if (!pageContext.CheckHasDialogExceptNewbie())
			{
				newbieDialogContext.SetAvailable(true);
				return;
			}
			if (newbieDialogContext.referredContext != null)
			{
				BaseDialogContext baseDialogContext = null;
				List<BaseDialogContext> list2 = new List<BaseDialogContext>();
				list2.AddRange(pageContext.dialogContextList);
				list2.Reverse();
				foreach (BaseDialogContext item2 in list2)
				{
					if (item2 is NewbieDialogContext)
					{
						continue;
					}
					baseDialogContext = item2;
					break;
				}
				if (baseDialogContext == newbieDialogContext.referredContext)
				{
					newbieDialogContext.SetAvailable(true);
					return;
				}
			}
			newbieDialogContext.SetAvailable(false);
		}
	}
}
