using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoDAMeiWS : MonoDAMei
	{
		private bool _isInRun;

		private string _attackRunTriggerID = "TriggerRunToAtk";

		private string _attackRunStopSkillID = "ATK02_02_RUN";

		protected override void PostInit()
		{
			base.PostInit();
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			if (to == _attackRunStopSkillID)
			{
				List<CollisionResult> list = CollisionDetectPattern.CircleCollisionDetectBySphere(base.RootNodePosition, 0f, base.FaceDirection, config.CommonArguments.CollisionRadius * 3f, 1 << InLevelData.AVATAR_LAYER);
				if (list.Count > 0)
				{
					SetTrigger(_attackRunTriggerID);
					_isInRun = false;
				}
				else
				{
					_isInRun = true;
				}
			}
			else
			{
				ResetTrigger(_attackRunTriggerID);
				_isInRun = false;
			}
		}

		protected override void OnCollisionEnter(Collision collision)
		{
			base.OnCollisionEnter(collision);
			if (InLevelData.AVATAR_LAYER == collision.gameObject.layer && _isInRun)
			{
				SetTrigger(_attackRunTriggerID);
				_isInRun = false;
			}
		}
	}
}
