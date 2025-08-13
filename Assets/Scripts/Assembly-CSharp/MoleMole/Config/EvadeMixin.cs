namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EvadeMixin : ConfigAbilityMixin, IHashable
	{
		private static DynamicFloat DEFAULT_EVADE_EXTEND_INVIN = new DynamicFloat
		{
			fixedValue = 0.4f
		};

		public DynamicFloat EvadeWindow;

		public DynamicFloat EvadeSuccessExtendedInvincibleWindow = DEFAULT_EVADE_EXTEND_INVIN;

		public string EvadeDummyName;

		public ConfigAbilityAction[] EvadeStartActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] EvadeSuccessActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] EvadeFailActions = ConfigAbilityAction.EMPTY;

		public EvadeMixin()
		{
			isUnique = true;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (EvadeWindow != null)
			{
				HashUtils.ContentHashOnto(EvadeWindow.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(EvadeWindow.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(EvadeWindow.dynamicKey, ref lastHash);
			}
			if (EvadeSuccessExtendedInvincibleWindow != null)
			{
				HashUtils.ContentHashOnto(EvadeSuccessExtendedInvincibleWindow.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(EvadeSuccessExtendedInvincibleWindow.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(EvadeSuccessExtendedInvincibleWindow.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(EvadeDummyName, ref lastHash);
			if (EvadeStartActions != null)
			{
				ConfigAbilityAction[] evadeStartActions = EvadeStartActions;
				foreach (ConfigAbilityAction configAbilityAction in evadeStartActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (EvadeSuccessActions != null)
			{
				ConfigAbilityAction[] evadeSuccessActions = EvadeSuccessActions;
				foreach (ConfigAbilityAction configAbilityAction2 in evadeSuccessActions)
				{
					if (configAbilityAction2 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
					}
				}
			}
			if (EvadeFailActions == null)
			{
				return;
			}
			ConfigAbilityAction[] evadeFailActions = EvadeFailActions;
			foreach (ConfigAbilityAction configAbilityAction3 in evadeFailActions)
			{
				if (configAbilityAction3 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityEvadeMixin(instancedAbility, instancedModifier, this);
		}

		public override ConfigAbilityAction[][] GetAllSubActions()
		{
			return new ConfigAbilityAction[3][] { EvadeStartActions, EvadeSuccessActions, EvadeFailActions };
		}

		public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			BaseAbilityActor baseAbilityActor = ((instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner);
			BaseMPIdentity identity = Singleton<MPManager>.Instance.GetIdentity<BaseMPIdentity>(baseAbilityActor.runtimeID);
			if (identity.remoteMode.IsRemoteReceive())
			{
				return new MPAbilityEvadeMixin_RemoteRecveive(instancedAbility, instancedModifier, this);
			}
			return new MPAbilityEvadeMixin_RemoteNoRecveive(instancedAbility, instancedModifier, this);
		}
	}
}
