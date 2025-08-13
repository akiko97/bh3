namespace MoleMole
{
	public class MonoDummyDynamicObject : BaseMonoDynamicObject
	{
		private bool _isToBeRemoved;

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}
	}
}
