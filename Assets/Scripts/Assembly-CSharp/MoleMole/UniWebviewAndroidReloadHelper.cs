using UnityEngine;

namespace MoleMole
{
	public class UniWebviewAndroidReloadHelper : MonoBehaviour
	{
		public void OnApplicationPause(bool pause)
		{
			UniWebView component = GetComponent<UniWebView>();
			if (!(component == null))
			{
				if (pause)
				{
					component.Hide();
				}
				else
				{
					component.Show();
				}
			}
		}
	}
}
