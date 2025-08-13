using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarDamageText : MonoBehaviour
	{
		public enum Type
		{
			Up = 0,
			Down = 1
		}

		private Text _upText;

		private Text _downText;

		private Animation _animation;

		public void SetupView(Type type, float damage, Vector3 pos)
		{
			Init();
			base.transform.position = pos;
			base.transform.SetLocalPositionZ(0f);
			if (type == Type.Up)
			{
				_upText.gameObject.SetActive(true);
				_downText.gameObject.SetActive(false);
				_upText.text = string.Format("+{0}", UIUtil.FloorToIntCustom(0f - damage));
				_animation.Play("DisplayAvatarHPUp");
			}
			else
			{
				_downText.gameObject.SetActive(true);
				_upText.gameObject.SetActive(false);
				_downText.text = string.Format("-{0}", UIUtil.FloorToIntCustom(damage));
				_animation.Play("DisplayAvatarHPDown");
			}
		}

		private void Init()
		{
			_animation = GetComponent<Animation>();
			_upText = base.transform.Find("UpText").GetComponent<Text>();
			_downText = base.transform.Find("DownText").GetComponent<Text>();
		}

		private void Update()
		{
			if (!_animation.isPlaying)
			{
				base.gameObject.SetActive(false);
				Vector3 vector = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(Singleton<AvatarManager>.Instance.GetLocalAvatar().RootNodePosition);
				base.transform.SetPositionX(vector.x);
			}
		}
	}
}
