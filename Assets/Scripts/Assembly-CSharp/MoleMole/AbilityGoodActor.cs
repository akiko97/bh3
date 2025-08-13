namespace MoleMole
{
	public class AbilityGoodActor : BaseGoodsActor
	{
		public string abilityName;

		public float abilityArgument;

		public override void DoGoodsLogic(uint avatarRuntimeID)
		{
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(_entity.ownerID);
			actor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), avatarRuntimeID, abilityArgument);
			Kill();
		}
	}
}
