using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoBodyPartEntity : BaseMonoAbilityEntity
	{
		public Collider hitbox;

		public bool IsCameraTargetable = true;

		public BaseMonoAnimatorEntity owner { get; set; }

		public override string CurrentSkillID
		{
			get
			{
				return owner.CurrentSkillID;
			}
		}

		public override Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		public override float TimeScale
		{
			get
			{
				return owner.TimeScale;
			}
		}

		public void Init(uint runtimeID, BaseMonoAnimatorEntity owner)
		{
			commonConfig = owner.commonConfig;
			base.Init(runtimeID);
			this.owner = owner;
		}

		public override bool IsToBeRemove()
		{
			return owner.IsToBeRemove();
		}

		public override bool IsActive()
		{
			return owner.IsActive();
		}

		public override Transform GetAttachPoint(string name)
		{
			return base.transform;
		}

		public override void FireEffect(string patternName)
		{
			owner.FireEffect(patternName);
		}

		public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
		{
			owner.FireEffect(patternName, initPos, initDir);
		}

		public override void FireEffectTo(string patternName, BaseMonoEntity to)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, XZPosition, base.transform.forward, Vector3.one, this, to);
		}

		public override void AddAnimEventPredicate(string predicate)
		{
			owner.AddAnimEventPredicate(predicate);
		}

		public override void RemoveAnimEventPredicate(string predicate)
		{
			owner.RemoveAnimEventPredicate(predicate);
		}

		public override bool ContainAnimEventPredicate(string predicate)
		{
			return owner.ContainAnimEventPredicate(predicate);
		}

		public override void MaskAnimEvent(string animEventName)
		{
			owner.MaskAnimEvent(animEventName);
		}

		public override void UnmaskAnimEvent(string animEventName)
		{
			owner.UnmaskAnimEvent(animEventName);
		}

		public override void MaskTrigger(string triggerID)
		{
			owner.MaskTrigger(triggerID);
		}

		public override void UnmaskTrigger(string triggerID)
		{
			owner.UnmaskTrigger(triggerID);
		}

		public override void PushMaterialGroup(string targetGroupname)
		{
			owner.PushMaterialGroup(targetGroupname);
		}

		public override void PopMaterialGroup()
		{
			owner.PopMaterialGroup();
		}

		public override void SetDied(KillEffect killEffect)
		{
			hitbox.enabled = false;
		}

		public override void PushTimeScale(float timescale, int stackIx)
		{
			owner.PushTimeScale(timescale, stackIx);
		}

		public override void SetTimeScale(float timescale, int stackIx)
		{
			owner.SetTimeScale(timescale, stackIx);
		}

		public override void PopTimeScale(int stackIx)
		{
			owner.PopTimeScale(stackIx);
		}

		public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
		{
			owner.SetNeedOverrideVelocity(needOverrideVelocity);
		}

		public override void SetOverrideVelocity(Vector3 velocity)
		{
			owner.SetOverrideVelocity(velocity);
		}

		public override void SetHasAdditiveVelocity(bool hasAdditiveVelocity)
		{
			owner.SetHasAdditiveVelocity(hasAdditiveVelocity);
		}

		public override void SetAdditiveVelocity(Vector3 velocity)
		{
			owner.SetAdditiveVelocity(velocity);
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
			owner.PushHighspeedMovement();
		}

		public override void PopHighspeedMovement()
		{
			owner.PopHighspeedMovement();
		}

		public override BaseMonoEntity GetAttackTarget()
		{
			return owner.GetAttackTarget();
		}

		public override void SetAttackTarget(BaseMonoEntity attackTarget)
		{
			owner.SetAttackTarget(attackTarget);
		}

		public override float GetCurrentNormalizedTime()
		{
			return owner.GetCurrentNormalizedTime();
		}

		public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
		{
			owner.TriggerAttackPattern(animEventID, layerMask);
		}

		public override void SetTrigger(string name)
		{
			owner.SetTrigger(name);
		}

		public override void ResetTrigger(string name)
		{
			owner.ResetTrigger(name);
		}

		public override void SteerFaceDirectionTo(Vector3 forward)
		{
			owner.SteerFaceDirectionTo(forward);
		}

		public override float GetProperty(string propertyKey)
		{
			return 0f;
		}

		public override int PushProperty(string propertyKey, float value)
		{
			return 0;
		}

		public override void PopProperty(string propertyKey, int stackIx)
		{
		}

		public override float GetPropertyByStackIndex(string propertyKey, int stackIx)
		{
			return 0f;
		}

		public override void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
		{
		}
	}
}
