using UnityEngine;

namespace MoleMole
{
	public class MonoNewbieOriginTransformHelper : MonoBehaviour
	{
		public NewbieDialogContext newbieDialogContext;

		private void OnEnable()
		{
			if (newbieDialogContext != null && !newbieDialogContext.IsActive)
			{
				newbieDialogContext.SetActive(true);
			}
		}

		private void OnDisable()
		{
			if (newbieDialogContext != null && newbieDialogContext.IsActive)
			{
				newbieDialogContext.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			if (newbieDialogContext != null && newbieDialogContext.IsActive)
			{
				newbieDialogContext.SetActive(false);
			}
		}
	}
}
