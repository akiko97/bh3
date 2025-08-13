using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityTriggerField : TriggerFieldActor
	{
		public MonoTriggerField triggerField;

		private BaseAbilityActor _owner;

		private bool _isFieldFollowOwner;

		public override void Init(BaseMonoEntity entity)
		{
			triggerField = (MonoTriggerField)entity;
			runtimeID = triggerField.GetRuntimeID();
		}

		public void Setup(BaseAbilityActor owner, float uniformScale, MixinTargetting targetting, bool followOwner = false)
		{
			_owner = owner;
			triggerField.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityTargettingMask(_owner.runtimeID, targetting));
			Vector3 localScale = Vector3.one * uniformScale;
			localScale.y = 1f;
			triggerField.transform.localScale = localScale;
			_isFieldFollowOwner = followOwner;
		}

		public List<uint> GetInsideRuntimeIDs()
		{
			return _insideRuntimes;
		}

		public override void Core()
		{
			base.Core();
			if (_isFieldFollowOwner && triggerField != null && _owner != null)
			{
				triggerField.transform.position = _owner.entity.transform.position;
			}
		}

		public override void Kill()
		{
			base.Kill();
			triggerField.SetDied();
		}
	}
}
