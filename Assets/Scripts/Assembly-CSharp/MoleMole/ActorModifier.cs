using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class ActorModifier : BaseActorActionContext
	{
		[InspectorCollapsedFoldout]
		public ActorAbility parentAbility;

		[InspectorCollapsedFoldout]
		public BaseAbilityActor owner;

		[InspectorCollapsedFoldout]
		public ConfigAbilityModifier config;

		public int[] stackIndices;

		public int instancedModifierID;

		public ActorModifier(ActorAbility parentAbility, BaseAbilityActor owner, ConfigAbilityModifier config)
		{
			this.parentAbility = parentAbility;
			this.config = config;
			this.owner = owner;
			List<BaseAbilityMixin> list = new List<BaseAbilityMixin>();
			for (int i = 0; i < config.ModifierMixins.Length; i++)
			{
				BaseAbilityMixin baseAbilityMixin = owner.abilityPlugin.CreateInstancedAbilityMixin(parentAbility, this, config.ModifierMixins[i]);
				if (baseAbilityMixin != null)
				{
					list.Add(baseAbilityMixin);
				}
			}
			instancedMixins = list.ToArray();
			for (int j = 0; j < instancedMixins.Length; j++)
			{
				instancedMixins[j].instancedMixinID = j;
			}
			stackIndices = new int[config.Properties.Count + config.EntityProperties.Count];
		}

		public override string GetDebugContextName()
		{
			return string.Format("{0} -> {1}", parentAbility.config.AbilityName, config.ModifierName);
		}

		public override BaseAbilityActor GetDebugOwner()
		{
			return owner;
		}

		public void Attach()
		{
			for (int i = 0; i < config.Properties.Count; i++)
			{
				string propertyKey = config.Properties.Keys[i];
				float value = parentAbility.Evaluate(config.Properties.Values[i]);
				stackIndices[i] = owner.PushProperty(propertyKey, value);
			}
			int count = config.Properties.Count;
			for (int j = 0; j < config.EntityProperties.Count; j++)
			{
				string propertyKey2 = config.EntityProperties.Keys[j];
				float value2 = parentAbility.Evaluate(config.EntityProperties.Values[j]);
				stackIndices[count + j] = owner.PushProperty(propertyKey2, value2);
			}
			if (config.State != AbilityState.None)
			{
				owner.AddAbilityState(config.State, config.MuteStateDisplayEffect);
			}
			AttachToActor(owner);
		}

		public void Detach()
		{
			for (int i = 0; i < config.Properties.Count; i++)
			{
				string propertyKey = config.Properties.Keys[i];
				owner.PopProperty(propertyKey, stackIndices[i]);
			}
			int count = config.Properties.Count;
			for (int j = 0; j < config.EntityProperties.Count; j++)
			{
				string propertyKey2 = config.EntityProperties.Keys[j];
				owner.PopProperty(propertyKey2, stackIndices[count + j]);
			}
			if (config.State != AbilityState.None)
			{
				owner.RemoveAbilityState(config.State);
			}
			DetachFromActor(owner);
		}
	}
}
