using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoVentureDispatchAvatar : MonoBehaviour
	{
		private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";

		private AvatarDataItem _avatarData;

		private int _index;

		private VentureDataItem _ventureData;

		public void SetupView(int index, VentureDataItem ventureData)
		{
			_index = index;
			_ventureData = ventureData;
			if (_ventureData.selectedAvatarList.Count >= index)
			{
				_avatarData = Singleton<AvatarModule>.Instance.GetAvatarByID(_ventureData.selectedAvatarList[index - 1]);
			}
			else
			{
				_avatarData = null;
			}
			base.transform.Find("Content").gameObject.SetActive(_avatarData != null);
			if (_avatarData != null)
			{
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[_avatarData.Attribute]);
				SetupAvatar();
			}
			else
			{
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
			}
		}

		public void OnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new DispatchAvatarDialogContext(_ventureData, _index));
		}

		private void SetupAvatar()
		{
			base.transform.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(_avatarData.star);
			base.transform.Find("Content/LVNum").GetComponent<Text>().text = _avatarData.level.ToString();
			base.transform.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.AvatarTachie);
		}
	}
}
