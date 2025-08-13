namespace MoleMole
{
	public class MonoKiana_C5 : MonoKiana
	{
		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
		}

		protected override void Update()
		{
			base.Update();
		}

		public override void TriggerSkill(int skillNum)
		{
			if (skillNum == 1)
			{
				SetLocomotionBool("EvadeBackward", !GetActiveControlData().hasSteer);
			}
			base.TriggerSkill(skillNum);
		}
	}
}
