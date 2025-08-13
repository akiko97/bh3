using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class MonoFireTriggerProp : MonoTriggerUnitFieldProp
	{
		private float _defaultCD = 2f;

		private float _defaultEffectDuration = 2f;

		private float _effectDuration;

		private float _CD;

		private bool _inForceDisable;

		private float _attackInterval = 0.5f;

		private ParticleSystem[] _fireEffect;

		private string _animEventIDForAttack = "ATK01";

		private Coroutine _waitTriggerHitCoroutine;

		public override void InitUnitFieldPropRange(int numberX, int numberZ)
		{
			base.InitUnitFieldPropRange(numberX, numberZ);
			Transform child = base.gameObject.transform.GetChild(1);
			float length = config.PropArguments.Length;
			Vector3 forward = Vector3.forward;
			Vector3 right = Vector3.right;
			for (int i = 0; i < numberX; i++)
			{
				for (int j = 0; j < numberZ; j++)
				{
					if (i != 0 || j != 0)
					{
						Transform transform = Object.Instantiate(child);
						transform.SetParent(base.gameObject.transform);
						transform.localPosition = child.localPosition + right * length * i + forward * length * j;
					}
				}
			}
			_fireEffect = GetComponentsInChildren<ParticleSystem>();
			StopEffect();
		}

		private IEnumerator WaitEnableFire(float CD)
		{
			yield return new WaitForSeconds(CD);
			if (!_inForceDisable)
			{
				EnableFire(_effectDuration, _CD);
			}
		}

		public void EnableFire(float effectDuration, float CD)
		{
			if (!_triggerCollider.enabled)
			{
				_inForceDisable = false;
				_effectDuration = ((!(effectDuration > 0f)) ? _defaultEffectDuration : effectDuration);
				_CD = ((!(CD > 0f)) ? _defaultCD : CD);
				ClearInsideColliders();
				_triggerCollider.enabled = true;
				StartEffect();
				_waitTriggerHitCoroutine = StartCoroutine(WaitTriggerFireHit());
				StartCoroutine(WaitDisableFire(_effectDuration));
			}
		}

		private IEnumerator WaitDisableFire(float effectDuration)
		{
			yield return new WaitForSeconds(effectDuration);
			DisableFire();
		}

		private void DisableFire()
		{
			if (_triggerCollider.enabled)
			{
				if (_waitTriggerHitCoroutine != null)
				{
					StopCoroutine(_waitTriggerHitCoroutine);
					_waitTriggerHitCoroutine = null;
				}
				ClearInsideColliders();
				_triggerCollider.enabled = false;
				StopEffect();
				StartCoroutine(WaitEnableFire(_CD));
			}
		}

		public void ForceDisableFire()
		{
			_inForceDisable = true;
			ClearInsideColliders();
			_triggerCollider.enabled = false;
			StopEffect();
			StopAllCoroutines();
		}

		private IEnumerator WaitTriggerFireHit()
		{
			yield return new WaitForSeconds(_attackInterval);
			TriggerFireHit();
			_waitTriggerHitCoroutine = StartCoroutine(WaitTriggerFireHit());
		}

		private void TriggerFireHit()
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtFieldHit(_runtimeID, _animEventIDForAttack));
		}

		private void StartEffect()
		{
			if (_fireEffect.Length != 0)
			{
				for (int i = 0; i < _fireEffect.Length; i++)
				{
					_fireEffect[i].Play();
				}
			}
		}

		private void StopEffect()
		{
			if (_fireEffect.Length != 0)
			{
				for (int i = 0; i < _fireEffect.Length; i++)
				{
					_fireEffect[i].Stop();
				}
			}
		}
	}
}
