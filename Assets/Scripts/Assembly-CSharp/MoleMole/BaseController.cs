namespace MoleMole
{
	public abstract class BaseController
	{
		public uint ControllerType { get; private set; }

		public BaseController(uint controllerType, BaseMonoEntity entity)
		{
			ControllerType = controllerType;
		}

		public abstract void Core();
	}
}
