using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarSaveAlliedMixin : BaseAbilityMixin
	{
		private AvatarSaveAlliedMixin config;

		private List<uint> _alliedIDs;

		private int _saveCountRemains;

		private bool _saveActive;

		public AbilityAvatarSaveAlliedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarSaveAlliedMixin)config;
			_alliedIDs = new List<uint>();
		}

		public override void OnAdded()
		{
			if (Singleton<AvatarManager>.Instance.IsHelperAvatar(actor.runtimeID) || !Singleton<AvatarManager>.Instance.IsPlayerAvatar(actor.runtimeID) || Singleton<LevelManager>.Instance.levelActor.levelMode != LevelActor.Mode.Single)
			{
				_saveCountRemains = 0;
				_saveActive = false;
			}
			else
			{
				_saveCountRemains = instancedAbility.Evaluate(config.SaveCountLimit);
				StartSaving();
			}
		}

		public override void OnRemoved()
		{
			if (_saveCountRemains > 0)
			{
				StopSaving();
			}
		}

		private void StartSaving()
		{
			_saveActive = true;
			AvatarActor[] alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<AvatarActor>(actor);
			for (int i = 0; i < alliedActorsOf.Length; i++)
			{
				if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(actor.runtimeID) && alliedActorsOf[i] != actor)
				{
					alliedActorsOf[i].AddAbilityState(AbilityState.Limbo, true);
					_alliedIDs.Add(alliedActorsOf[i].runtimeID);
				}
			}
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(actor.runtimeID);
		}

		private void StopSaving()
		{
			_saveActive = false;
			for (int i = 0; i < _alliedIDs.Count; i++)
			{
				AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(_alliedIDs[i]);
				if (avatarActor != null && (avatarActor.abilityState & AbilityState.Limbo) != AbilityState.None)
				{
					avatarActor.RemoveAbilityState(AbilityState.Limbo);
				}
			}
			_alliedIDs.Clear();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(actor.runtimeID);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtKilled)
			{
				return OnPostKilled((EvtKilled)evt);
			}
			if (evt is EvtReviveAvatar)
			{
				return OnPostReviveAvatar((EvtReviveAvatar)evt);
			}
			return false;
		}

		private bool OnPostKilled(EvtKilled evt)
		{
			if (_saveActive)
			{
				StopSaving();
			}
			return true;
		}

		private bool OnPostReviveAvatar(EvtReviveAvatar evt)
		{
			if (_saveCountRemains > 0)
			{
				StartSaving();
			}
			return true;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (!_saveActive)
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (_alliedIDs.Contains(evt.targetID))
			{
				AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
				if ((float)avatarActor.HP == 0f && avatarActor.IsOnStage())
				{
					Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(avatarActor.runtimeID, actor.runtimeID, true);
					actor.abilityPlugin.HandleActionTargetDispatch(config.AdditionalActions, instancedAbility, instancedModifier, avatarActor, null);
					_saveCountRemains--;
				}
				if (_saveCountRemains == 0)
				{
					StopSaving();
				}
			}
			return true;
		}
	}
}
