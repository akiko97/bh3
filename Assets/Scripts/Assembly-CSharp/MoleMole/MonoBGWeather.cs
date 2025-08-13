using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoBGWeather : MonoBehaviour
	{
		private void Start()
		{
			int hour = TimeUtil.Now.Hour;
			string prefabPath = ((!(TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)) ? MiscData.Config.WeatherBgPath[hour] : ((hour < 6 || hour > 21) ? MiscData.Config.WeatherBgPath[25] : MiscData.Config.WeatherBgPath[24]));
			base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
		}
	}
}
