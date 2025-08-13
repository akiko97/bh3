using System;

namespace MoleMole
{
	public class BaseSequenceDialogContext : BaseDialogContext
	{
		private Action OnDestroy;

		public void SetDestroyCallBack(Action callBack)
		{
			OnDestroy = callBack;
		}

		public override void Destroy()
		{
			if (OnDestroy != null)
			{
				OnDestroy();
			}
			base.Destroy();
		}
	}
}
