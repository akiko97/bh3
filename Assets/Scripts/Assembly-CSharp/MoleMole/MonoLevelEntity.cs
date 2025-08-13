using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoLevelEntity : BaseMonoAbilityEntity
	{
		private FixedStack<float> _timeScaleStack;

		private FixedStack<float> _auxTimeScaleStack;

		private static ConfigCommonEntity LEVEL_ENTITY_COMMON_CONFIG = new ConfigCommonEntity
		{
			CommonArguments = new ConfigEntityCommonArguments(),
			EntityProperties = new Dictionary<string, ConfigAbilityPropertyEntry>()
		};

		public override Vector3 XZPosition
		{
			get
			{
				return Vector3.zero;
			}
		}

		public override float TimeScale
		{
			get
			{
				return _timeScaleStack.value;
			}
		}

		public float AuxTimeScale
		{
			get
			{
				return _auxTimeScaleStack.value;
			}
		}

		public FixedStack<float> timeScaleStack
		{
			get
			{
				return _timeScaleStack;
			}
		}

		public FixedStack<float> auxTimeScaleStack
		{
			get
			{
				return _auxTimeScaleStack;
			}
		}

		public override string CurrentSkillID
		{
			get
			{
				return null;
			}
		}

		protected void Awake()
		{
			_runtimeID = 562036737u;
			_timeScaleStack = new FixedStack<float>(8);
			_timeScaleStack.Push(1f, true);
			_auxTimeScaleStack = new FixedStack<float>(8, OnAuxTimeScaleChanged);
			_auxTimeScaleStack.Push(1f, true);
		}

		private void OnAuxTimeScaleChanged(float oldValue, int oldIx, float newValue, int newIx)
		{
			Singleton<WwiseAudioManager>.Instance.SetParam("TimeScale", newValue);
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override bool IsActive()
		{
			return true;
		}

		public override Transform GetAttachPoint(string name)
		{
			return base.transform;
		}

		public override void Init(uint runtimeID)
		{
			commonConfig = LEVEL_ENTITY_COMMON_CONFIG;
			base.Init(runtimeID);
		}

		public override void FireEffect(string patternName)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, XZPosition, base.transform.forward, Vector3.one, this);
		}

		public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, Vector3.one, this);
		}

		public override void FireEffectTo(string patternName, BaseMonoEntity to)
		{
		}

		public override void PushTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Push(stackIx, timescale);
		}

		public override void SetTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Set(stackIx, timescale);
		}

		public override void PopTimeScale(int stackIx)
		{
			_timeScaleStack.Pop(stackIx);
		}

		public override void SetPersistentAnimatorBool(string key, bool value)
		{
		}

		public override void RemovePersistentAnimatorBool(string key)
		{
		}

		public override void AddAnimEventPredicate(string predicate)
		{
		}

		public override void RemoveAnimEventPredicate(string predicate)
		{
		}

		public override void MaskAnimEvent(string animEventName)
		{
		}

		public override void UnmaskAnimEvent(string animEventName)
		{
		}

		public override void MaskTrigger(string triggerID)
		{
		}

		public override void UnmaskTrigger(string triggerID)
		{
		}

		public override void PushMaterialGroup(string targetGroupname)
		{
		}

		public override void PopMaterialGroup()
		{
		}

		public override void SetDied(KillEffect killEffect)
		{
		}

		public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
		{
		}

		public override void SetOverrideVelocity(Vector3 velocity)
		{
		}

		public override void SetHasAdditiveVelocity(bool hasAdditiveVelocity)
		{
		}

		public override void SetAdditiveVelocity(Vector3 velocity)
		{
		}

		public override int AddAdditiveVelocity(Vector3 velocity)
		{
			return -1;
		}

		public override bool HasAdditiveVelocityOfIndex(int index)
		{
			return false;
		}

		public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
		{
		}

		public override void PushHighspeedMovement()
		{
		}

		public override void PopHighspeedMovement()
		{
		}

		public override BaseMonoEntity GetAttackTarget()
		{
			return null;
		}

		public override void SetAttackTarget(BaseMonoEntity attackTarget)
		{
		}

		public override float GetCurrentNormalizedTime()
		{
			return 0f;
		}

		public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
		{
		}

		public override void SetTrigger(string name)
		{
		}

		public override void ResetTrigger(string name)
		{
		}

		public override void SteerFaceDirectionTo(Vector3 forward)
		{
		}

		public override bool ContainAnimEventPredicate(string predicate)
		{
			return false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			auxTimeScaleStack.onChanged = null;
		}
	}
}
