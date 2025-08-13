using System;
using UnityEngine;

namespace MoleMole
{
	public class Mono_BOSS_010 : BaseMonoBoss
	{
		[Serializable]
		public class ResizeColliderEntry
		{
			public string skillID;

			public Vector3 center;

			public Vector3 size;

			public float normalizedTimeStart;

			public float normalizedTimeStop;
		}

		private enum ResizeState
		{
			Idle = 0,
			InStateWaiting = 1,
			InStateResized = 2
		}

		[Header("Target BoxCollider to scale")]
		public BoxCollider boxCollider;

		[Header("Scale By Skill ID Entries")]
		public ResizeColliderEntry[] scaleBySkillIDs;

		private ResizeState _state;

		private Vector3 _origSize;

		private Vector3 _origCenter;

		private ResizeColliderEntry _curEntry;

		protected override void PostInit()
		{
			base.PostInit();
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(ScaleColliderBySkillID));
			_state = ResizeState.Idle;
			_origCenter = boxCollider.center;
			_origSize = boxCollider.size;
			_curEntry = null;
		}

		private ResizeColliderEntry ScaleBySkillIDsContains(string skillID)
		{
			if (skillID == null)
			{
				return null;
			}
			for (int i = 0; i < scaleBySkillIDs.Length; i++)
			{
				if (scaleBySkillIDs[i].skillID == skillID)
				{
					return scaleBySkillIDs[i];
				}
			}
			return null;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (_state == ResizeState.InStateWaiting)
			{
				if (GetCurrentNormalizedTime() > _curEntry.normalizedTimeStart)
				{
					boxCollider.size = _curEntry.size;
					boxCollider.center = _curEntry.center;
					_state = ResizeState.InStateResized;
				}
			}
			else if (_state == ResizeState.InStateResized && GetCurrentNormalizedTime() > _curEntry.normalizedTimeStop)
			{
				boxCollider.size = _origSize;
				boxCollider.center = _origCenter;
				_state = ResizeState.Idle;
			}
		}

		private void ScaleColliderBySkillID(string from, string to)
		{
			ResizeColliderEntry resizeColliderEntry = ScaleBySkillIDsContains(to);
			if (resizeColliderEntry != null)
			{
				_curEntry = resizeColliderEntry;
				_state = ResizeState.InStateWaiting;
			}
			else if (_state == ResizeState.InStateResized)
			{
				boxCollider.size = _origSize;
				boxCollider.center = _origCenter;
				_curEntry = null;
				_state = ResizeState.Idle;
			}
			else if (_state == ResizeState.InStateWaiting)
			{
				_curEntry = null;
				_state = ResizeState.Idle;
			}
		}
	}
}
