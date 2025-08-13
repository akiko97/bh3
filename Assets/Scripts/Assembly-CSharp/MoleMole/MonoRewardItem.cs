using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoRewardItem : MonoBehaviour
	{
		private RewardUIData _rewardData;

		public void SetupView(RewardUIData rewardData)
		{
			_rewardData = rewardData;
			if (rewardData == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.transform.Find("Number").GetComponent<Text>().text = _rewardData.value.ToString();
			base.transform.Find("Icon").GetComponent<Image>().sprite = _rewardData.GetIconSprite();
		}
	}
}
