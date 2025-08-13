using UnityEngine;

namespace MoleMole
{
	public class MonoLevelDropIconButtonBox : MonoBehaviour
	{
		public enum Type
		{
			DefaultDrop = 0,
			NormalFinishChallengeReward = 1,
			FastFinishChallengeReward = 2,
			SonicFinishChallengeReward = 3
		}

		private Type _type;

		private bool _isOpen;

		public void SetupTypeView(Type type, bool isOpen)
		{
			_type = type;
			_isOpen = isOpen;
			SetupView();
		}

		public void SetOpenStatusView(bool isOpen)
		{
			_isOpen = isOpen;
		}

		private void SetupView()
		{
			bool flag = IsSenior();
			base.transform.Find("Item").gameObject.SetActive(_isOpen);
			base.transform.Find("Senior").gameObject.SetActive(!_isOpen && flag);
			base.transform.Find("Ordinary").gameObject.SetActive(!_isOpen && !flag);
			switch (_type)
			{
			case Type.DefaultDrop:
				base.transform.Find("Ordinary/Box2/Label").gameObject.SetActive(false);
				break;
			case Type.NormalFinishChallengeReward:
				base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
				base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(true);
				base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(false);
				base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(false);
				break;
			case Type.FastFinishChallengeReward:
				base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
				base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(false);
				base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(true);
				base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(false);
				break;
			case Type.SonicFinishChallengeReward:
				base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
				base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(false);
				base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(false);
				base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(true);
				break;
			}
		}

		public bool IsSenior()
		{
			return _type != Type.DefaultDrop;
		}

		public string GetOpenAnimationName()
		{
			return (!IsSenior()) ? "DropItemBoxOpenOrdinary" : "DropItemBoxOpenSenior";
		}

		public void SetItemAfterAnimation()
		{
			base.transform.SetLocalScaleX(0.7f);
			base.transform.SetLocalScaleY(0.7f);
			Transform transform = base.transform.Find("Item");
			transform.SetLocalScaleX(1f);
			transform.SetLocalScaleY(1f);
		}
	}
}
