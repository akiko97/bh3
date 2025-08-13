using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarStar : MonoBehaviour
	{
		private const int MAX_STAR = 5;

		public int star;

		private Image _image;

		public void Awake()
		{
			_image = base.transform.Find("Image").GetComponent<Image>();
		}

		public void SetupView(int star)
		{
			this.star = star;
			if (_image == null)
			{
				_image = base.transform.Find("Image").GetComponent<Image>();
			}
			_image.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[this.star]);
		}
	}
}
