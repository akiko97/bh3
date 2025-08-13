using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Avatar")]
	public class DebugAvatarButtonHold : BaseAvatarAction
	{
		public string SkillButtonID = "ATK";

		public float HoldTime;

		public string StartSkillID;

		public string EndSkillID;

		private MonoSkillButton _skillButton;

		private float _timer;

		private bool _countBegin;

		public override void OnStart()
		{
			_skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(SkillButtonID);
			_skillButton.OnPointerDown(null);
			_timer = HoldTime;
		}

		public override TaskStatus OnUpdate()
		{
			if (_avatar.CurrentSkillID == StartSkillID)
			{
				_countBegin = true;
			}
			if (_countBegin)
			{
				_timer -= Time.deltaTime;
				if (_timer < 0f)
				{
					_skillButton.OnPointerUp(null);
					_countBegin = false;
				}
			}
			if (_avatar.CurrentSkillID == EndSkillID)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Running;
		}
	}
}
