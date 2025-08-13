using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class BTreeAvatarAIController : BaseAvatarAIController
	{
		public const string AUTO_BATTLE_AI_RES_PATH = "AI/Avatar/AvatarAutoBattleBehavior_Alt";

		public const string AUTO_MOVE_AI_RES_PATH = "AI/Avatar/AvatarAutoMoveBehavior";

		public const string SUPPORTER_AI_RES_PATH = "AI/Avatar/";

		private BaseMonoAvatar _avatar;

		private BehaviorTree _btree;

		public ExternalBehaviorTree autoBattleBehavior;

		public ExternalBehaviorTree supporterBehavior;

		public ExternalBehaviorTree autoMoveBehvior;

		private bool _hasOverridenBehavior;

		public BTreeAvatarAIController(BaseMonoEntity avatar)
			: base(avatar)
		{
			_avatar = (BaseMonoAvatar)avatar;
			string path = "AI/Avatar/AvatarAutoBattleBehavior_Alt";
			autoBattleBehavior = Miscs.LoadResource<ExternalBehaviorTree>(path);
			autoMoveBehvior = Miscs.LoadResource<ExternalBehaviorTree>("AI/Avatar/AvatarAutoMoveBehavior");
			string text = "AvatarSupporterBehavior_Alt";
			if (_avatar.config.AIArguments.SupporterAI != string.Empty)
			{
				text = _avatar.config.AIArguments.SupporterAI;
			}
			supporterBehavior = Miscs.LoadResource<ExternalBehaviorTree>("AI/Avatar/" + text);
			_btree = _avatar.gameObject.AddComponent<BehaviorTree>();
			_btree.RestartWhenComplete = true;
			_btree.StartWhenEnabled = false;
			_btree.DisableBehavior();
			_btree.UpdateInterval = UpdateIntervalType.EveryFrame;
		}

		public void DisableBehavior()
		{
			_btree.DisableBehavior();
		}

		public void ChangeBehavior(string AIName)
		{
			string path = "AI/Avatar/" + AIName;
			ExternalBehaviorTree externalBehavior = Miscs.LoadResource<ExternalBehaviorTree>(path);
			_btree.ExternalBehavior = externalBehavior;
			_hasOverridenBehavior = true;
		}

		public void ChangeToMoveBehavior(Vector3 pointPos)
		{
			_btree.ExternalBehavior = autoMoveBehvior;
			_hasOverridenBehavior = true;
			_btree.SetVariableValue("TargetPosition", pointPos);
			_btree.EnableBehavior();
		}

		public void ChangeToSupporterBehavior()
		{
			_btree.ExternalBehavior = supporterBehavior;
			_btree.EnableBehavior();
		}

		public void ChangeToAutoBattleBehavior()
		{
			_btree.ExternalBehavior = autoBattleBehavior;
			_btree.EnableBehavior();
		}

		public override void SetActive(bool isActive)
		{
			if (base.active == isActive && !isActive == (_btree.ExecutionStatus == TaskStatus.Inactive))
			{
				return;
			}
			ResetWhenRefesh();
			base.SetActive(isActive);
			if (!_hasOverridenBehavior)
			{
				if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi && !Singleton<AvatarManager>.Instance.IsLocalAvatar(_avatar.GetRuntimeID()) && Singleton<AvatarManager>.Instance.IsPlayerAvatar(_avatar))
				{
					_btree.ExternalBehavior = supporterBehavior;
				}
				else
				{
					_btree.ExternalBehavior = supporterBehavior;
				}
			}
			if (!isActive)
			{
				_btree.UpdateInterval = UpdateIntervalType.Manual;
				base.avatar.SetMuteAnimRetarget(false);
				return;
			}
			SharedFloat sharedFloat = (SharedFloat)_btree.GetVariable("AttackDistance");
			sharedFloat.Value = base.avatar.config.AIArguments.AttackDistance;
			_btree.EnableBehavior();
			_btree.UpdateInterval = UpdateIntervalType.EveryFrame;
		}

		public override void Core()
		{
		}

		private void ResetWhenRefesh()
		{
			if (_avatar.IsActive())
			{
				_avatar.OrderMove = false;
				_avatar.ClearAttackTriggers();
			}
			base.controlData.FrameReset();
		}

		public void SetBehaviorVariable(string variableName, object variableValue)
		{
			List<SharedVariable> allVariables = _btree.GetAllVariables();
			for (int i = 0; i < allVariables.Count; i++)
			{
				if (allVariables[i].Name == variableName)
				{
					allVariables[i].SetValue(variableValue);
					break;
				}
			}
		}
	}
}
