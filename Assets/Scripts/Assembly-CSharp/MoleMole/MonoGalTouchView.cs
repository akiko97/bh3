using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoGalTouchView : MonoBehaviour
	{
		public Text goodFeelLabel;

		public Text todayRemainFeel;

		public Text additionalLabel;

		public GameObject[] heartObjects;

		public GameObject additionalObject;

		public GameObject sliderView;

		public float preAnimateTime;

		public float animateTime;

		public float postAnimateTime;

		public float sliderMin;

		public float sliderMax = 1f;

		public GameObject[] fullLevelHideObjects;

		private Animator _animator;

		private float _inactiveTimer;

		private int _animateStep;

		private float _fromPer;

		private float _toPer;

		private float _animateTimer;

		private float _lastRatio = -1f;

		private int _finalExp;

		public MonoMaskSlider maskSlider;

		public bool shown { get; set; }

		public event Action Upgrade;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_inactiveTimer = -1f;
		}

		public void Show(float sliderFrom, float sliderTo, int finalExp, string additionalText)
		{
			base.gameObject.SetActive(true);
			if (_animator != null)
			{
				_animator.Play("FeelPopUp");
			}
			_animateStep = 0;
			_finalExp = finalExp;
			PlaySliderAnimation(sliderFrom, sliderTo);
			_inactiveTimer = -1f;
			shown = true;
			int i = 0;
			for (int num = heartObjects.Length; i < num; i++)
			{
				foreach (Transform item in heartObjects[i].transform)
				{
					item.gameObject.SetActive(!heartObjects[i].activeSelf);
				}
			}
			if (additionalObject != null)
			{
				additionalObject.SetActive(!string.IsNullOrEmpty(additionalText));
			}
			if (additionalLabel != null)
			{
				additionalLabel.text = additionalText;
			}
		}

		public void Hide()
		{
			if (_animator != null)
			{
				_animator.Play("FeelHide");
			}
			_inactiveTimer = 1f;
			shown = false;
		}

		private void Update()
		{
			if (_inactiveTimer > 0f)
			{
				_inactiveTimer -= Time.deltaTime;
				if (_inactiveTimer <= 0f)
				{
					_inactiveTimer = -1f;
					base.gameObject.SetActive(false);
				}
			}
			UpdateSliderAnimation();
		}

		public void SetGoodFeel(int val)
		{
			if (goodFeelLabel != null)
			{
				string[] array = goodFeelLabel.text.Split('/');
				goodFeelLabel.text = string.Format("{0}/{1}", val.ToString(), array[1]);
			}
		}

		public void SetMaxGoodFeel(int val)
		{
			if (goodFeelLabel != null)
			{
				string[] array = goodFeelLabel.text.Split('/');
				goodFeelLabel.text = string.Format("{0}/{1}", array[0], val.ToString());
			}
		}

		public void SetHeartLevel(int val)
		{
			if (heartObjects == null || heartObjects.Length != 5)
			{
				Debug.LogWarning("[GalTouch] heartObjects of MonoGalTouchView is not set correctly");
				return;
			}
			val = Mathf.Clamp(val, 0, 5);
			for (int i = 0; i < 5; i++)
			{
				heartObjects[i].SetActive(i < val);
			}
			int j = 0;
			for (int num = fullLevelHideObjects.Length; j < num; j++)
			{
				fullLevelHideObjects[j].SetActive(val < 5);
			}
		}

		public void SetHintVisible(bool visible)
		{
			if (!(todayRemainFeel == null))
			{
				todayRemainFeel.enabled = visible;
			}
		}

		public void SetHintContent(string content)
		{
			if (!(todayRemainFeel == null))
			{
				todayRemainFeel.text = content;
			}
		}

		public void PlaySliderAnimation(float from, float to)
		{
			Image component = sliderView.GetComponent<Image>();
			if (!(component == null))
			{
				_animateTimer = preAnimateTime + animateTime + postAnimateTime;
				_animateStep = 1;
				_fromPer = from;
				_toPer = to;
				SetHeartLevel((int)from);
			}
		}

		private void UpdateSliderAnimation()
		{
			if (_animateStep == 1)
			{
				_animateTimer -= Time.deltaTime;
				float num = Mathf.Lerp(_fromPer, _toPer, (postAnimateTime + animateTime - _animateTimer) / animateTime);
				SetGoodFeelText(num);
				if (_lastRatio > 0f && (int)num > (int)_lastRatio)
				{
					UpgradeAction((int)num);
				}
				float sliderRatio = num - (float)(int)num;
				SetSliderRatio(sliderRatio);
				if (_animateTimer <= 0f)
				{
					Hide();
					_animateStep = 2;
				}
				_lastRatio = num;
			}
		}

		private void SetSliderRatio(float ratio)
		{
			ratio = Mathf.Clamp(ratio, 0f, 1f);
			ratio = Mathf.Lerp(sliderMin, sliderMax, ratio);
			maskSlider.UpdateValue(ratio, 1f, 0f);
		}

		private void SetGoodFeelText(float ratio)
		{
			int num = (int)ratio;
			float num2 = ratio - (float)num;
			int num3 = GalTouchData.QueryLevelUpFeelNeed(num);
			int num4 = (int)((float)num3 * num2);
			goodFeelLabel.text = string.Format("{0}/{1}", (ratio != _toPer) ? num4.ToString() : _finalExp.ToString(), num3.ToString());
		}

		private void UpgradeAction(int level)
		{
			SetHeartLevel(level);
			if (this.Upgrade != null)
			{
				this.Upgrade();
			}
		}
	}
}
