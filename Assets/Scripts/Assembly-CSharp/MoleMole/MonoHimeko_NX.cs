namespace MoleMole
{
	public class MonoHimeko_NX : MonoHimeko
	{
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
