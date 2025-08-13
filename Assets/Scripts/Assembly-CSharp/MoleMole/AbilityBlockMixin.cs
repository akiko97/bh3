using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityBlockMixin : BaseAbilityMixin
	{
		private BlockMixin config;

		private float _blockChance;

		private EntityTimer _blockTimer;

		private bool _allowBlock = true;

		public AbilityBlockMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BlockMixin)config;
			_blockChance = instancedAbility.Evaluate(this.config.BlockChance);
			_blockChance = Mathf.Clamp(_blockChance, 0f, 1f);
			_blockTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BlockTimer));
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool IsBlockHasCD()
		{
			if (_blockTimer.timespan > 0f)
			{
				return true;
			}
			return false;
		}

		public override void Core()
		{
			if (!_allowBlock && _blockTimer.isActive && IsBlockHasCD())
			{
				_blockTimer.Core(1f);
				if (_blockTimer.isTimeUp)
				{
					_allowBlock = true;
					_blockTimer.Reset(false);
				}
			}
		}

		private bool OnBeingHit(EvtBeingHit evt)
		{
			if (!_allowBlock)
			{
				return false;
			}
			if (config.BlockSkillIDs != null)
			{
				string currentSkillID = actor.entity.CurrentSkillID;
				if (string.IsNullOrEmpty(currentSkillID))
				{
					return false;
				}
				bool flag = false;
				for (int i = 0; i < config.BlockSkillIDs.Length; i++)
				{
					if (currentSkillID == config.BlockSkillIDs[i])
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.AttackerPredicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
			{
				return false;
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.TargetPredicates, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt))
			{
				return false;
			}
			if (Random.value < _blockChance)
			{
				evt.attackData.damage -= instancedAbility.Evaluate(config.DamageReduce);
				evt.attackData.damage *= 1f - instancedAbility.Evaluate(config.DamageReduceRatio);
				evt.attackData.damage = ((!(evt.attackData.damage < 0f)) ? evt.attackData.damage : 0f);
				evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
				if (evt.attackData.damage == 0f)
				{
					evt.attackData.hitLevel = AttackResult.ActorHitLevel.Mute;
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.BlockActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				if (IsBlockHasCD())
				{
					_blockTimer.Reset(true);
					_allowBlock = false;
				}
				return true;
			}
			return false;
		}
	}
}
