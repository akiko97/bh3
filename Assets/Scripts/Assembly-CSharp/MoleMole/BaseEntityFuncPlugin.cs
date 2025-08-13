namespace MoleMole
{
	public abstract class BaseEntityFuncPlugin
	{
		protected BaseMonoEntity _entity;

		public BaseEntityFuncPlugin(BaseMonoEntity entity)
		{
			_entity = entity;
		}

		public abstract void FixedCore();

		public abstract void Core();

		public abstract bool IsActive();
	}
}
