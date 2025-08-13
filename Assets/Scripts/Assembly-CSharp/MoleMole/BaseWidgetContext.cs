using UnityEngine;

namespace MoleMole
{
	public abstract class BaseWidgetContext : BaseContext
	{
		public BaseWidgetContext()
		{
			uiType = UIType.Root;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent)
		{
			base.StartUp(canvasTrans, viewParent);
		}
	}
}
