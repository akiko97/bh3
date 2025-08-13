namespace MoleMole
{
	public sealed class MonoInLevelUICamera : BaseMonoCamera
	{
		public void Init(uint runtimeID)
		{
			Init(2u, runtimeID);
		}

		public override void Update()
		{
			base.Update();
		}
	}
}
