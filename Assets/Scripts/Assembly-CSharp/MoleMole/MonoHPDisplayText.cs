using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoHPDisplayText : MonoBehaviour
	{
		private enum Mode
		{
			Up = 0,
			Down = 1
		}

		public void SetupView(int hpBefore, int hpAfter, int delta)
		{
			Text component = base.transform.Find("DisplayText/DownText").GetComponent<Text>();
			Text component2 = base.transform.Find("DisplayText/UpText").GetComponent<Text>();
			if (delta > 0)
			{
				component2.text = string.Format("+{0}", delta);
				PlayDisplayAnimation(Mode.Up);
			}
			if (delta < 0)
			{
				component.text = string.Format("{0}", delta);
				PlayDisplayAnimation(Mode.Down);
			}
		}

		private void PlayDisplayAnimation(Mode mode)
		{
			Animation component = base.transform.Find("DisplayText").GetComponent<Animation>();
			if (component != null)
			{
				if (component.isPlaying)
				{
					component.Rewind();
					component.Stop();
					ResetImageInvisible();
				}
				switch (mode)
				{
				case Mode.Up:
					component.Play("DisplayHPUp", PlayMode.StopAll);
					break;
				case Mode.Down:
					component.Play("DisplayHPDown", PlayMode.StopAll);
					break;
				}
			}
		}

		private void ResetImageInvisible()
		{
			base.transform.Find("DisplayText/DownText").GetComponent<CanvasGroup>().alpha = 0f;
			base.transform.Find("DisplayText/UpText").GetComponent<CanvasGroup>().alpha = 0f;
		}
	}
}
