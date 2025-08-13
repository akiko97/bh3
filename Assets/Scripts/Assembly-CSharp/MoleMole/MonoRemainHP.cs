using System;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoRemainHP : MonoBehaviour
	{
		private float _timer;

		private float _checkInterval = 5f;

		private bool _update;

		private EndlessAvatarHp _avatarHPData;

		[SerializeField]
		private Action<bool> avatarDieCallBack;

		public void SetAvatarHPData(EndlessAvatarHp avatarHPData, Action<bool> avatarDie = null)
		{
			_avatarHPData = avatarHPData;
			avatarDieCallBack = avatarDie;
			_update = true;
			CheckAndSetupView();
		}

		private void Update()
		{
			if (_update)
			{
				_timer += Time.deltaTime;
				if (_timer > _checkInterval)
				{
					CheckAndSetupView();
				}
			}
		}

		private void CheckAndSetupView()
		{
			_timer = 0f;
			DateTime dateTime = Singleton<EndlessModule>.Instance.CheckAvatarHPChanged(_avatarHPData);
			if (dateTime == DateTime.MinValue)
			{
				_checkInterval = 300f;
			}
			else if (dateTime > TimeUtil.Now && (dateTime - TimeUtil.Now).TotalSeconds > 60.0)
			{
				_checkInterval = 60f;
			}
			else
			{
				_checkInterval = 5f;
			}
			base.transform.Find("HPSlider/Slider").GetComponent<Image>().fillAmount = (float)Mathf.Clamp((int)_avatarHPData.hp_percent, 0, 100) / 100f;
			bool obj = _avatarHPData.is_dieSpecified && _avatarHPData.is_die;
			if (avatarDieCallBack != null)
			{
				avatarDieCallBack(obj);
			}
			base.transform.Find("HPSlider/HpRecovery").gameObject.SetActive(_avatarHPData.hp_percent < 100);
		}
	}
}
