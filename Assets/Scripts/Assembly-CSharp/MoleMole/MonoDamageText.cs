using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDamageText : MonoBehaviour
	{
		private DamageType _type;

		private Animation _animation;

		private List<Text> _textList;

		private float _speedX;

		private BaseMonoEntity _attackee;

		private Vector3 _positionOffset = Vector3.zero;

		private float _uiPositionXOffset;

		public void SetupView(DamageType type, float damage, Vector3 pos, BaseMonoEntity attackee)
		{
			_type = type;
			_attackee = attackee;
			_positionOffset = pos - attackee.XZPosition;
			_uiPositionXOffset = 0f;
			if (_textList == null)
			{
				Init();
			}
			base.transform.position = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(pos);
			base.transform.SetLocalPositionZ(0f);
			bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(attackee.GetRuntimeID());
			for (int i = 0; i < _textList.Count; i++)
			{
				if (i == (int)_type)
				{
					_textList[i].gameObject.SetActive(true);
					int num = UIUtil.FloorToIntCustom(damage);
					_textList[i].text = ((!flag) ? num : (-num)).ToString();
				}
				else
				{
					_textList[i].gameObject.SetActive(false);
				}
			}
			if (flag)
			{
				_animation.Play("DisplayAvatarHPDown");
			}
			else if (type == DamageType.Critical)
			{
				_animation.Play("DamageTextCrit");
			}
			else
			{
				_animation.Play("DamageTextMove");
			}
			_speedX = Random.Range(-1f, 1f);
		}

		private void Init()
		{
			_animation = GetComponent<Animation>();
			Text component = base.transform.Find("Text/Critical").GetComponent<Text>();
			Text component2 = base.transform.Find("Text/Normal").GetComponent<Text>();
			Text component3 = base.transform.Find("Text/Restrain").GetComponent<Text>();
			Text component4 = base.transform.Find("Text/ENormal").GetComponent<Text>();
			Text component5 = base.transform.Find("Text/EFire").GetComponent<Text>();
			Text component6 = base.transform.Find("Text/EThunder").GetComponent<Text>();
			Text component7 = base.transform.Find("Text/EIce").GetComponent<Text>();
			Text component8 = base.transform.Find("Text/EAllien").GetComponent<Text>();
			_textList = new List<Text> { component2, component, component4, component5, component6, component7, component8, component3 };
		}

		private void Update()
		{
			if (!_animation.isPlaying || _attackee == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.transform.position = GetUIPositionWithOffset();
			_uiPositionXOffset += Time.deltaTime * _speedX;
			base.transform.SetPositionX(base.transform.position.x + _uiPositionXOffset);
			base.transform.SetLocalPositionZ(0f);
		}

		private Vector3 GetUIPositionWithOffset()
		{
			Vector3 xZPosition = _attackee.XZPosition;
			xZPosition += _positionOffset;
			return Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(xZPosition);
		}
	}
}
