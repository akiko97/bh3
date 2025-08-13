using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoVentureConditionRow : MonoBehaviour
	{
		private VentureDataItem _ventureData;

		private int _index;

		private VentureCondition _condition;

		public void SetupView(int index, VentureDataItem ventureData = null)
		{
			_ventureData = ventureData;
			_index = index;
			_condition = ventureData.GetVentureCondition(_index);
			base.transform.gameObject.SetActive(_condition != null);
			if (_condition != null)
			{
				bool flag = _ventureData.IsConditionMatch(_condition);
				base.transform.Find("Check").gameObject.SetActive(flag);
				base.transform.Find("UnCheck").gameObject.SetActive(!flag);
				base.transform.Find("Check/Desc").GetComponent<Text>().text = _condition.desc;
				base.transform.Find("UnCheck/Desc").GetComponent<Text>().text = _condition.desc;
			}
		}
	}
}
