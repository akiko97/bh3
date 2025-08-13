using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class ResetTechTreeDialogContext : BaseSequenceDialogContext
	{
		private CabinDataItemBase _cabinData;

		private int _scoin_need;

		public ResetTechTreeDialogContext(CabinDataItemBase data)
		{
			config = new ContextPattern
			{
				contextName = "ResetTechTreeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ResetTechTreeDialog"
			};
			_cabinData = data;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>(), OnReset);
		}

		protected override bool SetupView()
		{
			_scoin_need = _cabinData.GetResetScoin();
			InitView();
			base.view.transform.Find("Dialog/Content/Info/Scoin").GetComponent<Text>().text = _scoin_need.ToString();
			int scoin = Singleton<PlayerModule>.Instance.playerData.scoin;
			if (scoin >= _scoin_need)
			{
				base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>().interactable = true;
			}
			else
			{
				base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>().interactable = false;
				base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(true);
			}
			return false;
		}

		private void InitView()
		{
			base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
		}

		private void OnReset()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (_scoin_need > 0)
			{
				Singleton<NetworkManager>.Instance.RequestResetCabinTech(_cabinData.cabinType);
			}
			Close();
		}

		private void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		private void Close()
		{
			Destroy();
		}
	}
}
